using System;

namespace BitHome.Actions
{
	public interface IActionParameter : IParameter
	{
	
		ActionParameterType ParameterType { get; }
	}
}

