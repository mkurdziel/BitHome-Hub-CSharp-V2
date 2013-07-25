using System;

namespace BitHome.Actions
{
	public enum ActionRequestStatus
	{
		Unknown,
		// An error occurred during the execution
		Error,
		// Action has been requested
		Requested,
		// Action has been successfully executed
		Executed
	}
}

