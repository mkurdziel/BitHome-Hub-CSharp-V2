using System;

namespace BitHome.Actions
{
	[Serializable]
	public class UnknownParameter : ParameterBase, IActionParameter
	{
		public UnknownParameter (String id) : base(id)
		{
		}

		#region IActionParameter implementation

		public ActionParameterType ParameterType {
			get {
				return ActionParameterType.Unknown;
			}
		}

		#endregion
	}
}

