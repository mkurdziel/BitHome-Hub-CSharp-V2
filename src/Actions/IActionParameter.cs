using System;

namespace BitHome.Actions
{
	public interface IActionParameter : IParameter
	{
		String Id { get; }

		String Identifier { get; }

		ActionParameterType ParameterType { get; }
	}
}

