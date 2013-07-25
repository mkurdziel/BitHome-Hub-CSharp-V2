using System;
using System.Threading;

namespace BitHome.Actions
{
	public class ActionRequest
	{
		private IAction m_action;
		private long m_timeout;
		private ManualResetEvent m_resetEvent;

		public ActionRequestStatus Status { get; private set; }
		public String ErrorString { get; private set; }
		public bool IsExecuted { get; private set; }

		public ActionRequest (IAction action, long timeout)
		{
			m_resetEvent = new ManualResetEvent (false);

			Status = ActionRequestStatus.Unknown;
			IsExecuted = false;
			ErrorString = "";

			m_action = action;
			m_timeout = timeout;
		}

		public void ThreadPoolCallback (object state)
		{
			Status = ActionRequestStatus.Requested;

			m_action.PrepareExecute();

			if (m_action.Execute (m_timeout)) {
				this.Status = ActionRequestStatus.Executed;
			} else {
				this.Status = ActionRequestStatus.Error;
				this.ErrorString = m_action.ExecutionErrorString;
			}

			IsExecuted = true;

			m_action.FinishExecute ();

			// Allow any blockers
			m_resetEvent.Set ();
		}

		public bool WaitForExecution()
		{
			if (!IsExecuted) {
				m_resetEvent.WaitOne (TimeSpan.FromMilliseconds (m_timeout));
			}

			return Status == ActionRequestStatus.Executed;
		}
	}
}
