using System;
using BitHome.Actions;
using BitHome;

namespace BitHomeTests
{
	[Serializable]
	public class TestAction : ActionBase {

		public bool Executed {get; set;}

		public TestAction() : base() {
			Executed = false;
		}
		#region implemented abstract members of ActionBase

		public override bool Execute (long timeoutMilliseconds)
		{
			Executed = true;

			return true;
		}

		public override ActionType ActionType {
			get {
				return ActionType.Delay;
			}
		}

		#endregion
	}
}

