using System;
using BitHome.Messaging.Protocol;
using NLog;
using ServiceStack.Text;
using System.Collections.Generic;
using System.Threading;
using BitHome.Messaging.Messages;

namespace BitHome.Actions
{
	public class ActionService : IBitHomeService
	{
		private const string KEY_ACTIONS = "actions";
		private const string KEY_PARAMETERS = "parameters";

		public const int ACTION_TIMEOUT_MS = 5000;
		public const int MAX_ACTION_THREADS = 10;
		public const int MAX_POOL_SIZE = 20;
		public const int CORE_POOL_SIZE = 10;
		public const int KEEP_ALIVE_TIME = 60;
		public const int THREAD_POOL_SIZE = 10;

		private static Logger log = LogManager.GetCurrentClassLogger();

		// List of all actions by Action ID
		// This allows fast lookup of Action when executing
		private Dictionary<string, IAction> m_actions = new Dictionary<string, IAction> ();

		// List of anonymous actions that were note associated with
		// a node but also are not a user-created action
		// These are created so there is only one instance of them in the system
		// and they can be used all over the place
//		private Dictionary<String, IAction> m_anonActionList = new Dictionary<string, IAction> ();

		// List of actions that were created by the user
		List<IAction> m_userActions = new List<IAction>();

		Dictionary<String, DateTime> m_dataValueUpdates = new Dictionary<string, DateTime> ();

		// Map of actions by node ID
		Dictionary<String, List<INodeAction>> m_nodeActions = new Dictionary<string, List<INodeAction>>();

		// Map of all parameters by parameter ID
		Dictionary<String, IActionParameter> m_parameters = new Dictionary<string, IActionParameter>();

		public IAction[] Actions {
			get {
				return m_actions.Values.ToArray ();
			}
		}

		public IActionParameter[] Parameters {
			get {
				return m_parameters.Values.ToArray ();
			}
		}

		public String[] ActionIds {
			get {
				return m_actions.Keys.ToArray ();
			}
		}

		public String[] ParameterIds {
			get {
				return m_parameters.Keys.ToArray ();
			}
		}

		#region Constructors

		public ActionService ()
		{
			ThreadPool.SetMaxThreads (THREAD_POOL_SIZE, 2);

			LoadData ();

			// Listen to the MessageDispatcherService for new messages
			ServiceManager.MessageDispatcherService.MessageRecieved += OnMessageRecievedEvent;
		}

		#endregion Constructors

		private void LoadData() {
			// Load the actions
			if (StorageService.Store<String[]>.Exists(KEY_ACTIONS)) {
				String[] actionKeys = StorageService.Store<String[]>.Get (KEY_ACTIONS);

				foreach (String key in actionKeys) 
				{
					if (StorageService.Store<IAction>.Exists (key)) {
						IAction action = StorageService.Store<IAction>.Get (key);

						m_actions.Add (key, action);

						if (action.ActionType == ActionType.Node) {
							AddNodeActionToCache ((INodeAction)action);
						}
					} else {
						m_actions.Add (key, new ActionUnknown (key));
					}
				}
			}
			// Load the parameters
			if (StorageService.Store<String[]>.Exists(KEY_PARAMETERS)) {
				String[] paramKeys = StorageService.Store<String[]>.Get (KEY_PARAMETERS);

				foreach (String key in paramKeys) 
				{
					if (StorageService.Store<IActionParameter>.Exists (key)) {
						m_parameters.Add (key, StorageService.Store<IActionParameter>.Get (key));
					} else {
						m_parameters.Add (key, new UnknownParameter (key));
					}
				}
			}
		}

		public bool Start()
		{
			log.Info ("Starting ActionService");
			return true;
		}

		public void Stop()
		{
			log.Info ("Stopping ActionService");
			// TODO stop all executing actions and wait
		}

		public INodeAction AddNodeAction (
			Node p_node, 
			int p_entryNumber, 
			string p_name, 
			DataType p_returnType, 
			int p_parameterCount)
		{
			log.Info ("Adding action to node:{0} index:{1} name:{2} returnType:{3} paramCount:{4}",
			         p_node.Id, p_entryNumber, p_name, p_returnType, p_parameterCount);

			// Get a unique key from the storage service
			String actionId = StorageService.GenerateKey ();	

			NodeAction action = new NodeAction (
				actionId,
				p_node.Id,
				p_entryNumber,
				p_name,
				p_returnType,
				p_parameterCount);

			AddAction (action);

			AddNodeActionToCache(action);

			// Set it to the node
			ServiceManager.NodeService.SetNodeAction(p_node, p_entryNumber, action);

			return action;
		}

		private void AddNodeActionToCache(INodeAction action) {
			// Add it to the node map
			if (m_nodeActions.ContainsKey (action.NodeId)) {
				// TODO check for duplicates
				m_nodeActions [action.NodeId].Add (action);
			} else {
				m_nodeActions.Add (action.NodeId, new List<INodeAction> ());
			}
		}

		public void AddAction ( IAction action) {

			if (action.Id == null) {
				// Get a unique key from the storage service
				String actionId = StorageService.GenerateKey ();	
				// Set that action id
				action.Id = actionId;
			}

			// Add the action to our internal map
			m_actions.Add (action.Id, action);
			// Save the action
			SaveAction (action);
			// Save the action list
			SaveActionList ();
		}

		public void RemoveAction ( IAction action ) {
			if (action != null) {

				// Remove all parameters
				foreach (String parameterId in action.ParameterIds) {
					RemoveParameter (parameterId);
				}

				m_actions.Remove (action.Id);

				UnsaveAction (action.Id);

				SaveActionList ();
				SaveParameterList ();
			}
		}

		public void RemoveAction ( String actionId ) {
			IAction action = GetAction (actionId);

			RemoveAction (action);
		}

		public void AddNodeParameter (
			Node node, 
			int actionIndex, 
			int paramIndex, 
			string name, 
			DataType dataType, 
			long maxValue, 
			long minValue, 
			Dictionary<String, int> enumValues)
		{

			String actionId = node.GetActionId (actionIndex);

			if (actionId == null) {
				log.Warn ("Adding node parameter to null action id {0} of node {1}", actionIndex, node.Identifier);
				return;
			}

			INodeAction nodeAction = (INodeAction)GetAction (actionId);

			if (nodeAction == null) {
				log.Warn ("Adding node parameter to null action {0} of node {1}", actionIndex, node.Identifier);
				return;
			}

			NodeParameter nodeParam = new NodeParameter (
				node.Id,
				actionIndex,
				paramIndex,
				StorageService.GenerateKey (),
				name,
				dataType,
				minValue,
				maxValue,
				enumValues,
				nodeAction.Id);

			AddParameter(nodeParam);

			nodeAction.AddNodeParameter (nodeParam);

			SaveAction (nodeAction);
		}

		public void AddParameter ( IActionParameter parameter) {

			if (parameter.Id == null) {
				// Get a unique key from the storage service
				String parameterId = StorageService.GenerateKey ();	
				parameter.Id = parameterId;
			}

			// Add the action to our internal map
			m_parameters.Add (parameter.Id, parameter);
			// Save the parameter
			SaveParameter (parameter);
			// Save the parameter list
			SaveParameterList ();
		}

		public void RemoveParameter ( String parameterId ) {

			m_parameters.Remove (parameterId);

			UnsaveParameter (parameterId);

			SaveParameterList ();
		}

		public IAction GetAction(String actionId) {
			if (m_actions.ContainsKey (actionId)) {
				return m_actions [actionId];
			}
			return null;
		}

		public IParameter GetParameter(String parameterId) {
			if (m_parameters.ContainsKey (parameterId)) {
				return m_parameters [parameterId];
			}
			return null;
		}

		public INodeAction[] GetNodeActions(String nodeId)
		{
			if (m_nodeActions.ContainsKey (nodeId)) {
				return m_nodeActions [nodeId].ToArray ();
			}
			return new INodeAction[]{};
		}

		public IAction[] GetUserActions()
		{
			return m_userActions.ToArray ();
		}

		public void SendDataRequests (HashSet<INodeParameter> parameterRequests)
		{
			Dictionary<string, HashSet<Tuple<byte, byte>>> dataRequests = 
				new Dictionary<string, HashSet<Tuple<byte, byte>>> ();

			foreach (INodeParameter param in parameterRequests) {
				bool requestData = true;

				// Check to see if we have an update value for this param
				if (m_dataValueUpdates.ContainsKey (param.Id)) {
					// Check that enough time has elapsed to send another update
					if ((DateTime.Now - m_dataValueUpdates [param.Id]) < TimeSpan.FromSeconds (10)) {
						requestData = false;
					}
				} else {
					// Add this because we are updating and may need to throttle
					m_dataValueUpdates.Add (param.Id, DateTime.Now);
				}

				if (requestData) {
					if (!dataRequests.ContainsKey (param.NodeId)) {
						dataRequests.Add (param.NodeId, new HashSet<Tuple<byte, byte>> ());
					}

					dataRequests [param.NodeId].Add (new Tuple<byte, byte> ((byte)param.ActionIndex, (byte)param.ParameterIndex));

					m_dataValueUpdates [param.Id] = DateTime.Now;
				}
			}

			// Send all the requests
			foreach (String nodeId in dataRequests.Keys) {
				Node node = ServiceManager.NodeService.GetNode (nodeId);
				if (node != null) {
					MessageDataRequest msg = new MessageDataRequest (dataRequests [nodeId], 
					                                                 DataRequestType.ON_CHANGE, 
					                                                 (UInt16)TimeSpan.FromSeconds (10).TotalMilliseconds);
					ServiceManager.MessageDispatcherService.SendMessage (msg, node);
				} else {
					log.Warn ("SendDataRequests - null node: {0}", nodeId);
				}
			}
		}

		void SendDataRequest (INodeAction action, INodeParameter parameter, DataRequestType type, UInt16 milliseconds)
		{
			Node node = ServiceManager.NodeService.GetNode(action.NodeId);
			if (node != null) {
				// Send a data request to update the parameter value
				MessageDataRequest msg = new MessageDataRequest ((byte)action.ActionIndex, (byte)parameter.ParameterIndex, type, milliseconds);
				ServiceManager.MessageDispatcherService.SendMessage (msg, node);
			} else {
				log.Warn ("Sending data request to unknown node {0}", action.NodeId);
			}
		}

		public string GetParameterValue (IAction action, IParameter param)
		{
			if (param != null) {
				if (param is INodeParameter && action is INodeAction) {
					bool requestData = true;

					// Check to see if we have an update value for this param
					if (m_dataValueUpdates.ContainsKey (param.Id)) {
						// Check that enough time has elapsed to send another update
						if ((DateTime.Now - m_dataValueUpdates [param.Id]) < TimeSpan.FromSeconds (10)) {
							requestData = false;
						}
					} else {
						// Add this because we are updating and may need to throttle
						m_dataValueUpdates.Add (param.Id, DateTime.Now);
					}

					if (requestData) {
						SendDataRequest ((INodeAction)action, (INodeParameter)param, DataRequestType.ON_CHANGE, (UInt16)TimeSpan.FromSeconds(10).TotalMilliseconds);
						m_dataValueUpdates [param.Id] = DateTime.Now;
					}
				}
				return param.Value;
			}
			return null;
		}

		public ActionRequest ExecuteAction(String actionId)
		{
			return ExecuteAction (actionId, ACTION_TIMEOUT_MS);
		}

		public ActionRequest ExecuteAction(String actionId, long timeout)
		{
			if (m_actions.ContainsKey (actionId)) {
				IAction action = m_actions [actionId];

				return ExecuteAction (action, timeout);
			}

			return null;
		}

		public ActionRequest ExecuteAction(IAction action, long timeout)
		{
			// TODO handle null action
			ActionRequest request = new ActionRequest (action, timeout);

			Execute (request);

			return request;
		}

		private void Execute(ActionRequest ActionRequest)
		{
			ThreadPool.QueueUserWorkItem (ActionRequest.ThreadPoolCallback);
		}

		#region Persistance Methods

		private void SaveActionList() 
		{
			StorageService.Store<String[]>.Insert (KEY_ACTIONS, this.ActionIds);
		}

		private void SaveParameterList() 
		{
			StorageService.Store<String[]>.Insert (KEY_PARAMETERS, this.ParameterIds);
		}

		private void SaveAction(IAction action) 
		{
			StorageService.Store<IAction>.Insert (action.Id, action);
		}
		
		private void UnsaveAction (String actionId) 
		{
			StorageService.Store<IAction>.Remove (actionId);
		}

		private void SaveParameter(IParameter parameter) 
		{
			StorageService.Store<IParameter>.Insert (parameter.Id, parameter);
		}

		private void UnsaveParameter(String parameterId)
		{
			StorageService.Store<IParameter>.Remove (parameterId);
		}

		public void WaitFinishSaving ()
		{
			StorageService.Store<String[]>.WaitForCompletion ();
			StorageService.Store<IAction>.WaitForCompletion ();
		}
		#endregion

		void ProcessMessageDataResponse (MessageDataResponse msg)
		{
			log.Trace ("Processing {0}", msg.ToString ());
			Node node = msg.SourceNode;

			// TODO: Optimize
			String actionId = node.GetActionId (msg.ActionIndex);
			if (actionId != null) {
				INodeAction action = (INodeAction)GetAction (actionId);

				if (action != null) {
					String paramId = action.GetParameterId (msg.ParameterIndex);
					if (paramId != null) {
						IParameter param = GetParameter (paramId);
						if (param != null) {
							param.SetValue (msg.Value);
						} else {
							log.Warn ("ProcessMessageDataResponse null parameter");
						}
					} else {
						log.Warn ("ProcessMessageDataResponse null paramId");
					}
				} else {
					log.Warn ("ProcessMessageDataResponse null action");
				}
			} else {
				log.Warn ("ProcessMessageDataResponse null actionId");
			}
		}

		void OnMessageRecievedEvent (object sender, Messaging.MessageRecievedEventArgs e)
		{
			switch (e.Message.Api) {
				case Messaging.Protocol.Api.DATA_RESPONSE:
				ProcessMessageDataResponse ((MessageDataResponse)e.Message);
				break;
			}
		}
	}
}
