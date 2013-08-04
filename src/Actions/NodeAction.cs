using System;
using BitHome.Messaging.Messages;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;
using ServiceStack.Text;
using NLog;

namespace BitHome.Actions
{
	[Serializable]
	public class NodeAction : ActionBase, INodeAction
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		Dictionary<int, String> m_parameterIds = new Dictionary<int, String>();
		
		public string NodeId { get; set; }
		public int ActionIndex { get; set; }

		public NodeAction (
			string id, 
			string p_nodeId, 
			int p_entryNumber, 
			string p_name, 
			DataType p_returnType, 
			int p_parameterCount)
			: base(id)
		{
			this.NodeId = p_nodeId;
			this.ActionIndex = p_entryNumber;
			this.Name = p_name;
			this.ReturnType = p_returnType;
			this.TotalParameterCount = p_parameterCount;
		}

		public int TotalParameterCount { get; set; }
		public DataType ReturnType { get; set; }

		private NodeAction() : base(null) { }


		public string GetParameterId (int parameterIndex) {
			if (m_parameterIds.ContainsKey (parameterIndex)) {
				return m_parameterIds [parameterIndex];
			}
			return null;
		}

		public void AddNodeParameter (INodeParameter parameter)
		{
			log.Debug ("Adding parameter {0} to action {1} at index {2}", parameter.Id, this.Identifier, parameter.ParameterIndex);

			// Index it by the parameter index on the device
			if (m_parameterIds.ContainsKey(parameter.ParameterIndex)) {
				log.Warn ("Adding duplicate parameter {0}:{1}", parameter.ActionIndex, parameter.ParameterIndex);
				m_parameterIds.Remove (parameter.ParameterIndex);
			} 

			m_parameterIds.Add (parameter.ParameterIndex, parameter.Id);

			base.AddParameter (parameter);
		}

		public int NextUnknownParameter {
			get {
				log.Trace ("Action {0} getting next unknown parameter {1}:{2}", this.Identifier, m_parameterIds.Count, TotalParameterCount);

				for (int i=1; i<=TotalParameterCount; ++i) {
					if (!m_parameterIds.ContainsKey (i)) {
						return i;
					}
				}
				return 0;
			}
		}


		public override ActionType ActionType {
			get {
				return ActionType.Node;
			}
		}

		public override bool Execute(long timeout)
		{
			// TODO optimize this
			List<INodeParameter> nodeParams = new List<INodeParameter>(m_parameterIds.Count);

			for (int i=0; i<TotalParameterCount; ++i) {
				INodeParameter param = (INodeParameter)ServiceManager.ActionService.GetParameter (GetParameterId (i));
				if (param == null)
					return false;

				nodeParams.Add (param);
			}

		    MessageFunctionTransmit msg = new MessageFunctionTransmit(
                this.ActionIndex,
				nodeParams.ToArray(),
                ReturnDataType );

            ServiceManager.MessageDispatcherService.SendMessage(msg, NodeId);
			//
			//			p_msgDispatcher.sendMessage(msg);
			//
			//			// Wait to see if the message actually sent
			//			if (!msg.waitForSend(p_timeoutMilliseconds))
			//			{
			//				if (msg.getIsError())
			//				{
			//					setExecuteErrorString(msg.getErrorMsg());
			//				}
			//				else
			//				{
			//					setExecuteErrorString("timed-out waiting or send notification");
			//				}
			//				return false;
			//			}
			//			// Now wait for a response
			//			else if(msg.getNeedsReturn())
			//			{
			//				if(msg.waitForResponse(p_timeoutMilliseconds))
			//				{
			//					// Set the string return value
			//					setStringReturnValue(msg.getResponseMsg().getStringValue());
			//				}
			//				else
			//				{
			//					setExecuteErrorString("request sent but no response received");
			//					return false;
			//				}
			//			}
			//
			//			return true;
			//		}
			//		else
			//		{
			//			Logger.w(TAG, super.getName() + " accessing invalid node: " + String.format("0x%x", getNodeId()));
			//		}
			//
			//		return false;
			return true;
		}
	}
}
