using System;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;

namespace BitHome.Actions
{
	[Serializable]
	public class NodeParameter : ActionParameter, INodeParameter
	{
		public int ParameterIndex { get; set; }
		public int ActionIndex { get; set; }
		public String NodeId { get; set; }

		public NodeParameter(
			String nodeId,
			int actionIndex,
			int paramIndex,
			String id,
			String name,
			DataType dataType,
			Int64 minValue,
			Int64 maxValue,
			Dictionary<String, int> enumValues,
			String actionId ) :
			base (id,
			      name,
			      dataType,
			      minValue,
			      maxValue,
			      enumValues,
			      actionId)
		{
			ParameterIndex = paramIndex;
			ActionIndex = actionIndex;
			NodeId = nodeId;
		}
	}
}
