using System;
using System.Collections.Generic;

namespace BitHome.Actions
{
	public class SequenceAction : ActionBase
	{
        private List<ActionItem> m_actionItems = new List<ActionItem>();
//		private List<ActionItem> m_actions;

        public ActionItem[] ActionItems {
            get { return m_actionItems.ToArray(); }
        }

		#region implemented abstract members of ActionBase

		public override bool Execute (long timeoutMilliseconds)
		{
			throw new NotImplementedException ();
		}

		public override ActionType ActionType {
			get {
				return ActionType.Sequence;
			}
		}

		#endregion

	    public void AddAction(IAction nodeAction)
	    {
            m_actionItems.Add(new ActionItem{
                ActionId = nodeAction.Id
            });
	    }
	}
}

