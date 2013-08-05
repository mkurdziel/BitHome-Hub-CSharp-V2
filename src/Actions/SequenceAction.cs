using System;

namespace BitHome.Actions
{
	public class SequenceAction : ActionBase
	{
		#region Constructors

//		private List<ActionItem> m_actions;

		#endregion

		#region implemented abstract members of ActionBase

		public override bool Execute (long p_timeoutMilliseconds)
		{
			throw new NotImplementedException ();
		}

		public override ActionType ActionType {
			get {
				return ActionType.Sequence;
			}
		}

		#endregion
	}
}

