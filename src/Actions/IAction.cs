using System;
using BitHome.Messaging.Protocol;

namespace BitHome.Actions
{
	public interface IAction
	{
		String Id { get; }
		String Name { get; set; }
		String Description { get; }
		String ReturnValue { get; set; }
		DataType ReturnDataType { get; set; }
		ActionType ActionType { get; }

		IActionParameter[] Parameters { get; }
		void AddParameter (IActionParameter p_parameter);
		IActionParameter GetParameter(int p_index);
		IActionParameter[] InputParameters { get; }
		void RemoveAllParameters();
		int ParameterCount { get; }

		void PrepareExecute();
		bool Execute(long p_timeoutMilliseconds);
		void FinishExecute();
		String ExecutionErrorString { get; set; }
	}
}
