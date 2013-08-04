using System;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;

namespace BitHome.Actions
{
	public interface IParameter
	{
		String Id { get; set; }
        String Identifier { get; }
		String Name { get; set; }
		String Description { get; set; }
		DataType DataType {get; set;}
		ParamValidationType ValidationType { get; set; }
		Dictionary<int, String> EnumValues { get; set; }
		String Value { get; }
		String DependentParameterId { get; set; }
		Int64 MinimumValue { get; set; }
		Int64 MaximumValue { get; set; }
	    Int64 IntValue { get; }

		bool IsSigned { get; }
		bool IsInteger { get; }
		bool IsString { get; }
		bool SetValue (String value);

		bool EqualsExceptId(IParameter param);
	}
}
