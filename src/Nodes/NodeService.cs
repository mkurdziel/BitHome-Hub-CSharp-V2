using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Threading;
using ServiceStack.Common;
using ServiceStack.DataAnnotations;
using ServiceStack.Text;
using NLog;
using BitHome.Messaging.Messages;
using BitHome.Messaging.Protocol;

namespace BitHome
{
	public class NodeService : ServiceStack.ServiceInterface.Service
	{
		private const string KEY_NODES = "nodes";
		private const int QUERY_INVERVAL_MS = 1000 * 60; // 2 minutes
		private const int INVESTIGATION_INTERVAL_MS = 100; // 100 ms
		private const int INVESTIGATION_TIMEOUT_MS = 1000 * 2; // 2 seconds
		private const int INVESTIGATION_RETRIES = 3; // Retry 3 times

		private static Logger log = LogManager.GetCurrentClassLogger();

		private int m_threadWaitMs;

		private Dictionary<String, Node> m_nodes;
		private List<Node> m_nodesToInvestigate;
		private object m_investigateLock;
		private Dictionary<String, NodeUpdateStatus> m_updateStatusMap;

		private readonly ManualResetEvent m_mrevMsgWorkNeededEvent;

		private readonly System.Timers.Timer m_timerNodeRefresh;
		private bool m_bIsTimerElapsed = false;
		private Thread m_thread;
		private Boolean m_isRunning = false;

		public Boolean PeriodicCheckEnabled { get; set; }

		public BroadcastNode BroadcastNode { get; set; }
		public bool IsInvestigating { get; set; }

		public NodeService() 
		{
			log.Trace ("()");

			// Initialize everything needed
			BroadcastNode = new BroadcastNode ();
			PeriodicCheckEnabled = true;
			m_nodes = new Dictionary<string, Node> ();
			m_nodesToInvestigate = new List<Node> ();
			m_investigateLock = new object ();
			m_updateStatusMap = new Dictionary<string, NodeUpdateStatus> ();
			m_mrevMsgWorkNeededEvent = new ManualResetEvent(false);
			SetQueryInterval ();
			m_timerNodeRefresh = new System.Timers.Timer ();
			m_timerNodeRefresh.Elapsed += TimerEvent;
			m_timerNodeRefresh.Interval = m_threadWaitMs;
			m_thread = new Thread (ManageNodesThread);
			m_thread.Name = "NodeServiceThread";
			m_thread.IsBackground = true;

			// Load data from the storage service
			if (StorageService.Store<String[]>.Exists(KEY_NODES)) {
				String[] nodeKeys = StorageService.Store<String[]>.Get (KEY_NODES);

				foreach (String key in nodeKeys) 
				{
					if ( StorageService.Store<Node>.Exists(key) )
					{
						m_nodes.Add (key, StorageService.Store<Node>.Get (key));
					}
				}
			}

			// Listen to the MessageDispatcherService for new messages
			ServiceManager.MessageDispatcherService.MessageRecieved += OnMessageRecievedEvent;
		}

		public bool Start() 
		{
			log.Info ("Starting NodeService");

			m_isRunning = true;

			m_thread.Start ();

			return true;
		}

		public void Stop() 
		{
			log.Info ("Stopping NodeService");

			m_isRunning = false;

			m_mrevMsgWorkNeededEvent.Set();

			m_timerNodeRefresh.Stop ();
		}

		public Node AddNode(Node p_node) 
		{
            // Create the new node and give it a unique ID
			p_node.Id = StorageService.GenerateKey ();

			log.Info ("Creating node {0}", p_node.Id);

            // Save it in the lookup table
			m_nodes[p_node.Id] = p_node;

			AddNodeForInvestigation (p_node);

			return p_node;
		}

		public void RemoveNode(Node p_node)
		{
			log.Info ("Removing node {0}", p_node.Identifier);

			if (m_nodes.ContainsKey(p_node.Id))
			{
				m_nodes.Remove (p_node.Id);
			}

			// Remove it from investigation if necessary
			if (m_nodesToInvestigate.Contains (p_node)) {
				m_nodesToInvestigate.Remove (p_node);
			}
		}

		private void AddNodeForInvestigation(Node p_node)
		{
			lock(m_investigateLock) {
				if (!m_nodesToInvestigate.Contains (p_node))
				{
					log.Info ("Adding node {0} for investigation", p_node.Id);

					m_nodesToInvestigate.Add (p_node);

					ResetInvestigationAttempts (p_node);
					
					// Wake up the worker thread
					m_mrevMsgWorkNeededEvent.Set ();
				}
			}

			// Set the interval to the investigation interval
			SetInvestigateInterval ();
		}

		private void RemoveNodeForInvestigation(Node p_node)
		{
			lock(m_investigateLock) {
				if (m_nodesToInvestigate.Contains (p_node)) {
					log.Info ("Removing node {0} from investigation", p_node.Id);

					m_nodesToInvestigate.Remove (p_node);
				}

				if (m_nodesToInvestigate.Count == 0) {
					SetQueryInterval ();
				}
			}
		}

		private bool CheckIfNeedInvestigation(Node p_node)
		{
			return (p_node.InvestigationStatus == NodeInvestigationStatus.Timeout || 
			        p_node.InvestigationStatus == NodeInvestigationStatus.Unknown);
		}

		public void ReinvestigateNode(Node p_node)
		{
			p_node.Reset ();
			AddNodeForInvestigation (p_node);
		}

		private void InvestigateNodes()
		{
//			log.Trace ("Investigating Nodes");
			foreach (Node node in m_nodesToInvestigate.ToArray()) {
				InvestigateNode (node);
			}
		}

		private void InvestigateNode(Node p_node) 
		{
			DateTime nextInvestigation = p_node.TimeNextInvestigation;

			// Check to see if we've already retried too many times
			if (p_node.InvestigationRetries == INVESTIGATION_RETRIES)
			{
				log.Info ("Investigating node {0} has timed out", p_node.Identifier);

				p_node.InvestigationStatus = NodeInvestigationStatus.Timeout;

				RemoveNodeForInvestigation(p_node);
			}
			// First check to see that the current time is
			// beyond the next investigation checkpoint
			else if (nextInvestigation < DateTime.Now)
			{
				log.Debug("Investigating:{0} function:{1} now:{2} next:{3}", p_node.Identifier, p_node.NextUnknownAction, DateTime.Now, p_node.TimeNextInvestigation);

				switch(p_node.InvestigationStatus)
				{
					// If unknown, we need to get some information
					case NodeInvestigationStatus.Unknown:
					case NodeInvestigationStatus.Timeout:
					{
						log.Debug("Investigating INFO for {0}", p_node.Identifier);

						MessageDeviceStatusRequest msg = new MessageDeviceStatusRequest (Messaging.Protocol.DeviceStatusRequest.INFO_REQUEST);

						ServiceManager.MessageDispatcherService.SendMessage (msg, p_node);
					}
					break;
					// If we have the info, we need to move on to get the catalog
					case NodeInvestigationStatus.Info:
					{
						log.Debug("Investigating CATALOG for {0}", p_node.Identifier);

						MessageCatalogRequest msg = new MessageCatalogRequest (0);

						ServiceManager.MessageDispatcherService.SendMessage (msg, p_node);
					}
					break;
					// Query until we have all the functions
					case NodeInvestigationStatus.Function:
					{
						int function = p_node.NextUnknownAction;

						if (function != 0)
						{
							log.Debug("Investigating FUNCTION {0} for {1}", function, p_node.Identifier);

							// TODO avoid conversion
							MessageCatalogRequest msg = new MessageCatalogRequest ((byte)function);

							ServiceManager.MessageDispatcherService.SendMessage (msg, p_node);
						} else {
							log.Debug( "Investigating full catalog for {0}", p_node.Identifier);
						}
					}
					break;
					// Query until we have all the parameters
					case NodeInvestigationStatus.Parameter:
					{
						Tuple<byte, byte> pair = p_node.NextUnknownParameter;

						if (pair != null)
						{
							log.Debug ("Investigating PARAMETER {0} : {1} for {2}", pair.Item1, pair.Item2, p_node.Identifier);

							MessageParameterRequest msg = new MessageParameterRequest (pair.Item1, pair.Item2);
	
							ServiceManager.MessageDispatcherService.SendMessage (msg, p_node);
						}
						else
						{
							log.Debug("Investigating full parameters for {0} ", p_node.Identifier);
						}
					}
					break;
					default:
					{
						log.Warn ("An investigation attept is made in an unimplemented state: {0} for {1}", p_node.InvestigationStatus, p_node.Identifier);
					}
					break;
				}

				if(p_node.InvestigationStatus == NodeInvestigationStatus.Completed)
				{
					RemoveNodeForInvestigation(p_node);

				} else {
					p_node.InvestigationRetries++;
					p_node.TimeNextInvestigation = p_node.TimeNextInvestigation.AddMilliseconds(INVESTIGATION_TIMEOUT_MS);
					log.Debug("Setting next investigation time for {0} funtion:{1} to {2}", p_node.Identifier,  p_node.NextUnknownAction, p_node.TimeNextInvestigation);
				}
			}
		}

		private void ResetInvestigationAttempts(Node p_node) 
		{
			log.Debug ("Resetting investigation attempts on node {0}", p_node.Id);

			p_node.TimeNextInvestigation = DateTime.Now;
			p_node.InvestigationRetries = 0;
		}

		private void CheckForInvestigation(Node p_node) {
			if (p_node.InvestigationStatus != NodeInvestigationStatus.Completed &&
			    !m_nodesToInvestigate.Contains(p_node)) {

				log.Trace ("{0} needs investigating", p_node.Id);

				AddNodeForInvestigation (p_node);
			}
		}

		private void CheckNodesForUpdate()
		{
			// See if there is anything in the map to be updated
			if (m_updateStatusMap.IsEmpty() == false) {
				foreach (String key in m_updateStatusMap.Keys) {
					NodeUpdateStatus status = m_updateStatusMap [key];

					// If the status is unknown, it hasn't been started
					// Ininitiate the reboot
					if (status.getStatus () == NodeBootloadStatus.UNKNOWN) {
						SendRebootRequest (status.getNode ());

						status.setStatus (NodeBootloadStatus.RESET);
					} else if (status.getTimeNextUpdate () < DateTime.Now) {
						log.Info ("Update {0} timed out. Resending Update", key);
						SendNextUpdate(status.getNode ());
					}
				}
			}
		}

		private void ResetUpdateAttempts(Node p_node) 
		{
			if (m_updateStatusMap.ContainsKey (p_node.Id)) {
				log.Debug ("Resetting update attempts on node {0}", p_node.Id);

				NodeUpdateStatus status = m_updateStatusMap [p_node.Id];
				status.setTimeNextUpdate(DateTime.Now + TimeSpan.FromMilliseconds(INVESTIGATION_TIMEOUT_MS));
				status.setNumRetries (0);
			}
		}

        public Node[] GetNodes()
        {
            return m_nodes.Values.ToArray();
        }

        public Node GetNode(String p_key)
        {
            return m_nodes[p_key];
        }



		#region Node Management Methods 

		public void UpdateNode(Node p_node, String p_updateFile)
		{
			log.Info("Updating node {0} with file {1}", p_node.Identifier, p_updateFile);

			NodeUpdateFile file = new NodeUpdateFile(p_updateFile);

//			if (file.parseFile())
//			{
//				Logger.i(TAG, "Success parsing node update file");
//				// Create the update status, hash it, and wake up the
//				// node manager to handle this
//				NodeUpdateStatus status = new NodeUpdateStatus(p_node, file);
//
//				// If the node is unknown and has only reported a HW reset,
//				// this may be an initial load. Skip the reset and
//				// go straight to waiting for confirmation
//				if (p_node.getInvestigationStatus()== EsnInvestigationStatusEnum.UNKNOWN)
//				{
//					Logger.i(TAG, "Unknown node. Could be an initial software load. Skipping reset");
//					status.setStatus(BootloadStatusEnum.BOOTLOAD_REQUEST);
//				}
//				// Hash this so the node manage thread can pick it up
//				m_updateStatusMap.put(p_node.getNodeId(), status);
//
//				// Change the query interval
//				setInvestigateInterval();
//
//				m_nodeManagerThread.notifyThread();
//
//			}
//			else
//			{
//			log.Warn("Unable to parse update file {)] for node {1}", p_updateFile, p__node.Identifier);
//			}

		}

		private void RefreshNodesInfos() {
			log.Debug ("Refreshing Node Infos");

			ServiceManager.MessageDispatcherService.BroadcastMessage (
				new MessageDeviceStatusRequest(DeviceStatusRequest.INFO_REQUEST)
			);
		}

		private void RefreshNodes() {
			log.Debug ("Refreshing Nodes");

			ServiceManager.MessageDispatcherService.BroadcastMessage (
				new MessageDeviceStatusRequest(DeviceStatusRequest.STATUS_REQUEST)
			);
		}

		#endregion


		#region Message Processing Methods

		private void ProcessMessageDeviceStatus(MessageDeviceStatusResponse p_msg) 
		{ 	
			// Since we've seen the node, update it's last seen time
			p_msg.SourceNode.LastSeen = DateTime.Now;

			// Handle the types of status messages
			switch (p_msg.DeviceStatus)
			{
			case DeviceStatusValue.ACTIVE:
				ProcessStatusActive(p_msg.SourceNode);
				break;
			case DeviceStatusValue.INFO:
				ProcessStatusInfo(p_msg);
				break;
			case DeviceStatusValue.HW_RESET:
				ProcessStatusHwReset(p_msg.SourceNode);
				break;
			}
		}

		private void ProcessMessageCatalogResponse(MessageCatalogResponse p_msg) 
		{ 	
			Node node = p_msg.SourceNode;

			// If this is zero, it's just the base catalog
			if (p_msg.EntryNumber == 0)
			{
				if (node.InvestigationStatus == NodeInvestigationStatus.Info)
				{
					log.Debug ("Received base catalog for {0} with total entries {1}", node.Identifier, p_msg.TotalEntries);

					node.TotalNumberOfFunctions = p_msg.TotalEntries;

					// Update the investigation status
					node.InvestigationStatus = NodeInvestigationStatus.Function;
				} else {
					log.Warn ("Recieved catalog for node {0} when not being investigated", node.Identifier);
				}
			}
			else
			{
				ServiceManager.ActionService.AddNodeAction (
					node,
					p_msg.EntryNumber,
					p_msg.FunctionName,
					p_msg.ReturnType,
					p_msg.NumberParams);

				log.Trace ("Adding node action {0} to node {1}", p_msg.EntryNumber, node.Identifier);

				// If we have all the functions, increment the investigation status
				if(node.NextUnknownAction == 0)
				{
					log.Trace ("No more unknown actions for {0}", node.Identifier);

					node.InvestigationStatus = NodeInvestigationStatus.Parameter;
				}
				else
				{
					log.Trace ("Next unknown action for {0} is {1}", node.Identifier, node.NextUnknownAction);
				}
			}

			ResetInvestigationAttempts(node);
		}

		private void ProcessMessageParameterResponse(MessageParameterResponse p_msg)
		{
			Node node = p_msg.SourceNode;

			ServiceManager.ActionService.AddNodeParameter (
				node,
				p_msg.FunctionId,
				p_msg.ParamId,
				p_msg.ParamName,
				p_msg.DataType,
				p_msg.ValidationType,
				p_msg.MaxValue,
				p_msg.MinValue,
				p_msg.EnumValues
			);

			log.Trace ("Adding parameter to {0} {1}:{2}", node.Identifier, p_msg.FunctionId, p_msg.ParamId);
//			Logger.v(TAG, parameter.getDescription());

			// If we have all the parameters for this function, send a notification
			//		if(node.getNextUnknownParameter(parameter.getFunctionId()) == 0)
			//		{
			//			fireNodeFunctionAdded(node.getFunction(parameter.getFunctionId()));
			//		}

			// If we have all the functions, increment the investigation status
			if(node.NextUnknownParameter == null)
			{
				node.InvestigationStatus = NodeInvestigationStatus.Completed;
				node.IsUnknown = false;
			}
			else
			{
				Tuple<byte, byte> actionPair = node.NextUnknownParameter;

				log.Trace ("Next unknown parameter for {0} is {1}:{2}", node.Identifier, actionPair.Item1, actionPair.Item2);
			}

			ResetInvestigationAttempts(node);
		}

		private void ProcessStatusHwReset(Node p_node)
		{
			log.Debug ("Node {0} hardware reset", p_node.Identifier);

			// First we check to see if this is a HW reset to start
			// a software update
			if (m_updateStatusMap.ContainsKey(p_node.Id))
			{
				NodeUpdateStatus status = m_updateStatusMap[p_node.Id];

				// If this device is in the reset state, then we are on track
				if (status.getStatus() == NodeBootloadStatus.RESET)
				{
					// We have the proper restart, now we will send the request
					status.setStatus(NodeBootloadStatus.BOOTLOAD_REQUEST);

					SendNextUpdate(p_node);
				}
				// If this device is in the reset again, keep sending bootload requests
				else if (status.getStatus() == NodeBootloadStatus.BOOTLOAD_REQUEST)
				{
					log.Debug ("Node {0} restarted unexpectidly. Sending another bootlaod request", p_node.Identifier);

					SendNextUpdate(p_node);
				}
				else
				{
					log.Warn ("Node {0} rebooted without an update complete confirmation", p_node.Identifier);

					m_updateStatusMap.Remove(p_node.Id);
				}
			}
			else
			{
				// let's change the investigation status to unknown since the firmware
				// and catalog may have changed during the reboot;
				p_node.InvestigationStatus = NodeInvestigationStatus.Unknown;
			}
		}

		private void ProcessStatusActive(Node p_node)
		{
			log.Debug ("Node {0} is ACTIVE", p_node.Identifier);

			CheckForInvestigation(p_node);
		}

		private void ProcessStatusInfo(MessageDeviceStatusResponse p_msg)
		{
			// Populate the node information
			Node node = p_msg.SourceNode;

			log.Debug ("Processing status info for node {0}", node.Identifier);

			// Check to see if anything has changed or if this is unknown
//			if (node.IsUknown || node.Revision != p_msg.getRevision()))
			if (node.IsUnknown)
			{
				log.Debug ("Setting status info for node:{0} revision:{1} to {2} ", node.Identifier, node.Revision, p_msg.Revision);
//
//				node.setManufacturerId(p_msg.getManufacturerID());
//				node.setRevision(p_msg.getRevision());
//				node.setSynetID(p_msg.getSynetID());
//				node.setProfile(p_msg.getProfile());

				// Since it is unknown or has changed, we need the catalog
				node.InvestigationStatus = NodeInvestigationStatus.Info;
			}
			// Use this to prevent from going from unknown to info to completed prematurely 
			else if (node.InvestigationStatus == NodeInvestigationStatus.Unknown)
			{
				//Logger.d(TAG, "Node Revision: " + node.getRevision() + " Msg Revision: " + p_msg.getRevision());
				// Otherwise the node is known and the revision is the same
				node.InvestigationStatus = NodeInvestigationStatus.Completed;
			}
			else if (node.InvestigationStatus == NodeInvestigationStatus.Timeout)
			{
				node.InvestigationStatus = NodeInvestigationStatus.Info;
			}

			CheckForInvestigation(node);
		}

		#endregion

		#region Message Sending Methods

		public void SendUpdateRequest(Node p_node)
		{
			log.Info ("Requesting update for node {0}", p_node.Identifier);

//			MsgBootloadTransmit msg = new MsgBootloadTransmit(p_node, EsnAPIBootloadTransmit.BOOTLOAD_REQUEST);
//			m_msgDispatcher.sendMessage(msg);            
		}

		public void SendRebootRequest(Node p_node)
		{
			log.Info("Rebooting node {0}", p_node.Identifier);
			//			MsgBootloadTransmit msg = new MsgBootloadTransmit(p_node, EsnAPIBootloadTransmit.REBOOT_DEVICE);
			//			m_msgDispatcher.sendMessage(msg);         
		}

		
		private void SendNextUpdate(Node p_node)
		{
		}


		#endregion

//		public void AddMsgBootloadResponse(MsgBootloadResponse p_msg)
//		{
//			NodeBase node = p_msg.getSourceNode();
//
//			// First lets get the update status for this node
//			if (m_updateStatusMap.containsKey(node.getNodeId()))
//			{
//				NodeUpdateStatus status = m_updateStatusMap.get(node.getNodeId());
//
//				switch(p_msg.getBootloadResponse())
//				{
//					case BOOTLOAD_READY:
//				{
//					Logger.v(TAG, "node is ready to update. Starting update.");
//					status.setStatus(BootloadStatusEnum.DATA_TRANSMIT);
//
//				}
//					break;
//					case DATA_SUCCESS:
//				{
//					Logger.v(TAG, String.format("Node update data success. %s Address: %x", node.getDescString(), p_msg.getMemoryAddress()));
//					status.markAsSent(p_msg.getMemoryAddress());
//				}
//					break;
//					case BOOTLOAD_COMPLETE:
//				{
//					Logger.v(TAG, "Node update complete: " + node.getDescString());
//					m_updateStatusMap.remove(node.getNodeId());
//					setQueryInterval();
//				}
//					break;
//					case ERROR_ADDRESS:
//					case ERROR_API:
//					case ERROR_BOOTLOADAPI:
//					case ERROR_BOOTLOADSTART:
//					case ERROR_CHECKSUM:
//					case ERROR_MY16_ADDR:
//					case ERROR_PAGELENGTH:
//					case ERROR_SIZE:
//					case ERROR_SNAPI:
//					case ERROR_SNSTART:
//					case ERROR_START_BIT:
//				{
//					Logger.w(TAG, "Received error from updating device " + node.getDescString() + " " + p_msg.getBootloadResponse()); 
//				}
//					break;
//				}
//
//				// We received an update message to reset the update attempts
//				resetUpdateAttempts(node);
//
//				// Send the next update if not complete
//				if(p_msg.getBootloadResponse() != EsnAPIBootloadResponse.BOOTLOAD_COMPLETE)
//				{
//
//					sendNextUpdate(node, status);
//				}
//			}
//			else
//			{
//				Logger.e(TAG, "received a bootload message when there is no bootload status");
//				// Just reboot the node
//				rebootNode(node);
//			}
//		}

//		/**
//     * Send the next update to the node
//     * 
//     * @param p_node
//     * @param p_status
//     */
//		private void sendNextUpdate(NodeBase p_node, NodeUpdateStatus p_status)
//		{
//			switch(p_status.getStatus())
//			{
//				case RESET:
//				case BOOTLOAD_REQUEST:
//			{
//				Logger.i(TAG, p_node.getDescString() + "Sending bootload request");
//				sendUpdateRequest(p_node);
//			}
//				break;
//				case DATA_TRANSMIT:
//			{
//				if (p_status.isComplete())
//				{
//					sendUpdateDataComplete(p_node);
//				}
//				else
//				{
//					sendUpdateDataNext(p_node, p_status);
//				}
//			}
//				break;
//				case UNKNOWN:
//			{
//				Logger.w(TAG, "send next update for unknown status state");
//			}
//				break;
//			}
//
//			p_status.setNumRetries(p_status.getNumRetries()+1);
//			p_status.setTimeNextUpdate(p_status.getTimeNextUpdate().plus(C_INVESTIGATE_TIMEOUT));
//			Logger.v(TAG, "setting next update time for " + p_node.getDescString() + " to " + p_status.getTimeNextUpdate());
//		}
//
//		/**
//     * Tell the node that the update has completed
//     * 
//     * @param p_node
//     */
//		private void sendUpdateDataComplete(NodeBase p_node)
//		{
//			Logger.v(TAG, String.format("Sending update complete to %s", p_node.getDescString()));
//			MsgBootloadTransmit msg = new MsgBootloadTransmit(p_node, EsnAPIBootloadTransmit.DATA_COMPLETE);
//			m_msgDispatcher.sendMessage(msg); 
//		}
//
//		/**
//     * Send the next chunk of update data
//     * @param p_status
//     */
//		private void sendUpdateDataNext(NodeBase p_node, NodeUpdateStatus p_status)
//		{
//			int nextAddress = p_status.getNextAddress();
//
//			byte[] dataBytes = new byte[p_node.getCodeUpdatePageSize()];
//
//			int checksum = p_node.getCodeUpdatePageSize();
//			checksum += (nextAddress>>8);
//			checksum += (nextAddress & 0xff);
//			for (int i=0; i<p_node.getCodeUpdatePageSize(); ++i)
//			{
//				dataBytes[i] = p_status.getDataByte(nextAddress + i);
//				checksum += dataBytes[i];
//			}
//
//			// Now reduce check_sum to 8 bits
//			while (checksum > 256)
//				checksum -= 256;
//
//			// now take the two's compliment
//			checksum = 256 - checksum;
//
//			Logger.v(TAG, String.format("Sending update address %x to %s", nextAddress, p_node.getDescString()));
//			MsgBootloadTransmit msg = new MsgBootloadTransmit(
//				p_node, 
//				EsnAPIBootloadTransmit.DATA_TRANSMIT,
//				dataBytes,
//				nextAddress,
//				checksum,
//				p_node.getCodeUpdatePageSize()
//				);
//			m_msgDispatcher.sendMessage(msg); 
//		}
//
		

		private void SetQueryInterval()
		{
			log.Trace ("Switching to query interval");

			IsInvestigating = false;

			m_threadWaitMs = QUERY_INVERVAL_MS;

			m_mrevMsgWorkNeededEvent.Set();
		}

		private void SetInvestigateInterval()
		{
			if (!IsInvestigating)
			{
				log.Trace ("Switching to investigation interval");

				IsInvestigating = true;
				m_threadWaitMs = INVESTIGATION_INTERVAL_MS;

				// Since the investigate interval is smaller than the query interval,
				// We need to notify the thread to start spinning at the new interval
				m_mrevMsgWorkNeededEvent.Set();
			}
		}

		public int GetQueryInterval()
		{
			return m_threadWaitMs;
		}

		
		private void TimerEvent(object p_objSource, ElapsedEventArgs p_evElapsedEventArgs)
		{
			m_bIsTimerElapsed = true;
			m_mrevMsgWorkNeededEvent.Set();
		}	

		private void ManageNodesThread() {
			// First time, we want to refresh the info to make sure
			// that the version numbers match up
			RefreshNodesInfos ();

			// Start the timer
			m_timerNodeRefresh.Start();

			while (m_isRunning)
			{
				// See if there are any unknown nodes to investigate
				// Don't send out the pings if we are investigating
				if (IsInvestigating) {

					InvestigateNodes ();

				} else if (PeriodicCheckEnabled) {

					// Send out the periodic if the timer has elapsed
					if (m_bIsTimerElapsed)
					{
						RefreshNodes();
					}
				}

				// See if there is anything to be updated
				CheckNodesForUpdate ();


				// Wait for the next one
				if (m_isRunning)
				{
					// reset our expired indicator
					m_bIsTimerElapsed = false;
					// Set to the current interval
					m_timerNodeRefresh.Interval = m_threadWaitMs;
					// Wait the interval
					m_timerNodeRefresh.Stop();
					m_timerNodeRefresh.Start();

					// Wait if necessary
					m_mrevMsgWorkNeededEvent.Reset ();
					m_mrevMsgWorkNeededEvent.WaitOne();

					// Catch this in case we were woken up to exit
					if (!m_isRunning) {
						return;
					}
				}
			}
		}

		void OnMessageRecievedEvent (object sender, Messaging.MessageRecievedEventArgs e)
		{
			switch (e.Message.Api) {
			case Messaging.Protocol.Api.DEVICE_STATUS_RESPONSE:
				ProcessMessageDeviceStatus ((MessageDeviceStatusResponse)e.Message);
				break;
			case Messaging.Protocol.Api.CATALOG_RESPONSE:
				ProcessMessageCatalogResponse ((MessageCatalogResponse)e.Message);
				break;
			case Messaging.Protocol.Api.PARAMETER_RESPONSE:
				ProcessMessageParameterResponse ((MessageParameterResponse)e.Message);
				break;
			}
		}
	}
}

