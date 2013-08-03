using System;

namespace BitHome.Actions
{
	public interface INodeAction : IAction
	{
		String NodeId { get; set; }
		int ActionIndex { get; set; }

		String GetParameterId (int index);
		void AddNodeParameter (INodeParameter parameter);
		int NextUnknownParameter { get; }
		int TotalParameterCount { get; set; }
	}
}
