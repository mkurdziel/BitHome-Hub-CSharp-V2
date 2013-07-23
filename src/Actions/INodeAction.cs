using System;

namespace BitHome.Actions
{
	public interface INodeAction : IAction
	{
		String NodeId { get; set; }
		int EntryNumber { get; set; }

		new INodeParameter GetParameter (int p_index);
		void AddParameter (INodeParameter p_parameter);
		int NextUnknownParameter { get; }
		new INodeParameter[] Parameters { get; }
		int TotalParameterCount { get; set; }
	}
}
