using System;
using BitHome.Messaging.Protocol;

namespace BitHome.Actions
{
	public interface IAction
	{
		String Id { get; set; }
		String Name { get; set; }
		String Description { get; }
		String ReturnValue { get; set; }
		DataType ReturnDataType { get; set; }
		ActionType ActionType { get; }

		// Parameters
		String[] ParameterIds { get; }
		void AddParameter ( IActionParameter parameter );
		void RemoveAllParameters();
		int ParameterCount { get; }

		// Execution
		void PrepareExecute();
		bool Execute(long p_timeoutMilliseconds);
		void FinishExecute();
		String ExecutionErrorString { get; set; }
	}
}
