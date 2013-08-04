using System;

namespace BitHome
{
	[Serializable]
	public enum ActionParameterType
	{
		Unknown,
		// Parameter needs to be set by the user
		Input,
		// Parameter is defined as a constant
		Constant,
		// Parameter is dependent upon another parameters value
		Dependent,
		// Parameter is internally set
		Internal
	}
}

