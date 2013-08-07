using System;

namespace BitHome.Actions
{
	[Serializable]
	public class ActionUnknown : ActionBase
	{
		public ActionUnknown( String id ) : base(id) {

		}

		#region implemented abstract members of ActionBase

		public override bool Execute (long timeoutMilliseconds)
		{
			throw new NotImplementedException ();
		}

		public override ActionType ActionType {
			get {
				return ActionType.Unknown;
			}
		}

		#endregion
	}
}

