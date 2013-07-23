using System;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;
using ServiceStack.Text;
using NLog;

namespace BitHome.Actions
{
	public class NodeAction : ActionBase, INodeAction
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		Dictionary<int, INodeParameter> m_parameters = new Dictionary<int, INodeParameter>();
		
		public string NodeId { get; set; }
		public int EntryNumber { get; set; }

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
			this.EntryNumber = p_entryNumber;
			this.Name = p_name;
			this.ReturnType = p_returnType;
			this.TotalParameterCount = p_parameterCount;
		}

		private NodeAction() : base(null) { }


		public int TotalParameterCount { get; set; }
		public DataType ReturnType { get; set; }

		INodeParameter INodeAction.GetParameter (int p_index)
		{
			if (m_parameters.ContainsKey (p_index)) {
				return m_parameters [p_index];
			}
			return null;
		}

		public void AddParameter (INodeParameter p_parameter)
		{
			log.Debug ("Adding parameter {0} to action {1}", p_parameter.Identifier, this.Identifier);

			// Index it by the parameter index on the device
			if (m_parameters.ContainsKey(p_parameter.ParameterIndex)) {
				log.Warn ("Adding duplicate parameter {0}:{1}", p_parameter.ActionIndex, p_parameter.ParameterIndex);
			} else {
				m_parameters.Add (p_parameter.ParameterIndex, p_parameter);
			}

			base.AddParameter (p_parameter);
		}

		public int NextUnknownParameter {
			get {
				log.Trace ("Action {0} getting next unknown parameter {1}:{2}", this.Identifier, m_parameters.Count, TotalParameterCount);

				for (int i=1; i<=TotalParameterCount; ++i) {
					if (!m_parameters.ContainsKey (i)) {
						return i;
					}
				}
				return 0;
			}
		}

		INodeParameter[] INodeAction.Parameters {
			get {
				return m_parameters.Values.ToArray ();
			}
		}


		public override ActionType ActionType {
			get {
				return ActionType.Node;
			}
		}

		public override bool Execute() {
			
			//
			//		NodeBase destNode = p_nodeManager.getNode(this.getNodeId());
			//
			//		if (destNode != null)
			//		{
			//
			//			Logger.v(TAG, String.format("executing action %s %d:%d]", super.getName(), super.getNumParameters(),getParameters().length));
			//
			//			MsgFunctionTransmit msg = new MsgFunctionTransmit(
			//				destNode, 
			//				getFunctionId(), 
			//				super.getNumParameters(), 
			//				getParameters(),
			//				getReturnType());
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
