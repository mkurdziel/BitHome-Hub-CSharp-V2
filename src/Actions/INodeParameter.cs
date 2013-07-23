using System;

namespace BitHome.Actions
{
	public interface INodeParameter : IActionParameter
	{
		int ParameterIndex { get; }
		int ActionIndex { get; }
		String NodeId { get; }
	}
}

