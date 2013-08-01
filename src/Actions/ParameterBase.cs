using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Actions
{
	public class ParameterBase : IParameter
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public String Id { get; set; }
		public String Name { get; set; }
		public String Value { get; private set;}
		public String Description { get; set; }

		public String DependentParameterId { get; set; }
		public Dictionary<int, String> EnumValues { get; set; }

		private DataType m_dataType;
		public DataType DataType {
			get {
				return m_dataType;
			}
			set {
				m_dataType = value;
				// Set a default validation type
				switch (m_dataType) {
					case DataType.BOOL:
						ValidationType = ParamValidationType.BOOL;
						break;
					case DataType.STRING:
						ValidationType = ParamValidationType.STRING;
						break;
					case DataType.VOID:
						ValidationType = ParamValidationType.UNKNOWN;
						break;
					case DataType.BYTE:
					case DataType.WORD:
					case DataType.DWORD:
					case DataType.QWORD:
						ValidationType = ParamValidationType.UNSIGNED_RANGE;
						break;
					default:
						throw new NotImplementedException ();
				}
			}
		}

		ParamValidationType m_validationType;

		public ParamValidationType ValidationType {
			get {
				return m_validationType;
			}
			set {
				switch (DataType) {
				case DataType.BOOL:
					m_validationType = ParamValidationType.BOOL;
					m_minimumValue = 0;
					m_maximumValue = 1;
					break;
				case DataType.STRING:
					m_validationType = ParamValidationType.STRING;
					MinimumValue = 0;
					m_maximumValue = Int32.MaxValue;
					break;
				case DataType.VOID:
					m_validationType = ParamValidationType.UNKNOWN;
					m_minimumValue = 0;
					m_maximumValue = 0;
					break;
				case DataType.BYTE:
					if (value == ParamValidationType.SIGNED_RANGE) {
						m_validationType = ParamValidationType.SIGNED_RANGE;
						m_minimumValue = SByte.MinValue;
						m_maximumValue = SByte.MaxValue;
					} else {
						m_validationType = ParamValidationType.UNSIGNED_RANGE;
						m_minimumValue = Byte.MinValue;
						m_maximumValue = Byte.MaxValue;
					}
					break;
				case DataType.WORD:
					if (value == ParamValidationType.SIGNED_RANGE) {
						m_validationType = ParamValidationType.SIGNED_RANGE;
						m_minimumValue = Int16.MinValue;
						m_maximumValue = Int16.MaxValue;
					} else {
						m_validationType = ParamValidationType.UNSIGNED_RANGE;
						m_minimumValue = UInt16.MinValue;
						m_maximumValue = UInt16.MaxValue;
					}
					break;
				case DataType.DWORD:
					if (value == ParamValidationType.SIGNED_RANGE) {
						m_validationType = ParamValidationType.SIGNED_RANGE;
						m_minimumValue = Int32.MinValue;
						m_maximumValue = Int32.MaxValue;
					} else {
						m_validationType = ParamValidationType.UNSIGNED_RANGE;
						m_minimumValue = UInt32.MinValue;
						m_maximumValue = UInt32.MaxValue;
					}
					break;
				case DataType.QWORD:
					if (value == ParamValidationType.SIGNED_RANGE) {
						m_validationType = ParamValidationType.SIGNED_RANGE;
						m_minimumValue = Int64.MinValue;
						m_maximumValue = Int64.MaxValue;
					} else {
						m_validationType = ParamValidationType.UNSIGNED_RANGE;
						// TODO unsigned qword
						m_minimumValue = 0;
						m_maximumValue = Int64.MaxValue;
					}
					break;
					default:
						throw new NotImplementedException ();
				}
			}
		}

		private Int64 m_minimumValue;
		public Int64 MinimumValue {
			get {
				return m_minimumValue;
			}
			set {
				if (ValidateRangeValue (value)) {
					m_minimumValue = value;
				}
			}
		}

		private Int64 m_maximumValue;
		public Int64 MaximumValue {
			get {
				return m_maximumValue;
			}
			set {
				if (ValidateRangeValue (value)) {
					m_maximumValue = value;
				}
			}
		}

        private Dictionary<string, int> enumValues;

		private bool ValidateRangeValue(Int64 value) {
			switch (DataType) {
			case DataType.BOOL:
				return false;
				break;
			case DataType.STRING:
				if (value > 0) {
					return true;
				}
				break;
			case DataType.BYTE:
				if (ValidationType == ParamValidationType.SIGNED_RANGE) {
					if (value >= SByte.MinValue && value <= SByte.MaxValue) {
						return true;
					}				
				} else {
					if (value >= Byte.MinValue && value <= Byte.MaxValue) {
						return true;
					}
				}
				break;
			case DataType.WORD:
				if (ValidationType == ParamValidationType.SIGNED_RANGE) {
					if (value >= Int16.MinValue && value <= Int16.MaxValue) {
						return true;
					}				
				} else {
					if (value >= UInt16.MinValue && value <= UInt16.MaxValue) {
						return true;
					}
				}
				break;
			case DataType.DWORD:
				if (ValidationType == ParamValidationType.SIGNED_RANGE) {
					if (value >= Int32.MinValue && value <= Int32.MaxValue) {
						return true;
					}				
				} else {
					if (value >= UInt32.MinValue && value <= UInt32.MaxValue) {
						return true;
					}
				}
				break;
			case DataType.QWORD:
				if (ValidationType == ParamValidationType.SIGNED_RANGE) {
					if (value >= Int64.MinValue && value <= Int64.MaxValue) {
						return true;
					}				
				} else {
					// TODO uint64
					if (value >= 0 && value <= Int64.MaxValue) {
						return true;
					}
				}
				break;
			default:
				throw new NotImplementedException ();
			}

			return false;
		}

		public String Identifier {
			get { return Id; }
		}

		public bool IsSigned {
			get {
				return ValidationType == ParamValidationType.SIGNED_RANGE;
			}
		}

		public bool IsInteger {
			get {
				return ValidationType != ParamValidationType.DATE_TIME && 
					ValidationType != ParamValidationType.STRING &&
					ValidationType != ParamValidationType.UNKNOWN;
			}
		}

		public bool IsString {
			get {
				return ValidationType == ParamValidationType.STRING;
			}
		}

		public String RangeString {
			get {
				String retVal = "unknown";

				switch (ValidationType) {
				case ParamValidationType.BOOL:
					retVal = "True or False";
					break;
				case ParamValidationType.ENUMERATED:
					retVal = "Enumeration";
					break;
				case ParamValidationType.STRING:
					retVal = String.Format("{0}-character string", MaximumValue);
					break;
				case ParamValidationType.UNKNOWN:
					retVal = "Unknown";
					break;
				case ParamValidationType.SIGNED_RANGE:
				case ParamValidationType.UNSIGNED_RANGE:
					retVal = String.Format("{0} - {1}", MinimumValue, MaximumValue);
					break;
				}

				return retVal;
			}
		}

        // TODO test this
        public Int64 IntValue { get; private set; }

		private ParameterBase ()
		{
			log.Trace ("()");
		}

		public ParameterBase (String id) {
			this.Id = id;
		}

        public ParameterBase(
            string id, 
            string name, 
            Messaging.Protocol.DataType dataType, 
            ParamValidationType validationType, 
            Int64 minValue, 
            Int64 maxValue, 
            Dictionary<string, int> enumValues)
        {
            this.Id = id;
            this.Name = name;
            this.DataType = dataType;
            this.ValidationType = validationType;
            this.MinimumValue = minValue;
            this.MaximumValue = maxValue;
            this.enumValues = enumValues;
        }


        private bool ValidateValue(String value, out Int64 intVal)
        {
            if (IsString)
            {
                intVal = 0;
                return ValidateString(value);
            } else {
                return ValidateInteger(value, out intVal);
            }
        }

		private bool ValidateValue (String value)
		{
		    Int64 intVal;
			return IsString ? 
				ValidateString (value) :
					ValidateInteger (value, out intVal);
		}

		private bool ValidateString (String value)
		{
			return value.Length >= MinimumValue && 
				value.Length <= MaximumValue;
		}

        private bool ValidateInteger(String value, out Int64 intValue)
		{
			bool retVal = true;
            intValue = 0;

			switch (ValidationType) {

			case ParamValidationType.BOOL:
				if (String.Equals (value, "0"))
				{
                    intValue = 0;
				} else if (String.Equals (value, "1")) {
                    intValue = 1;
				} else {
					retVal = false;
				}
				break;
			case ParamValidationType.ENUMERATED:
				throw new NotImplementedException ();
				// TODO enumerated types
//						try {
//							sq = Integer.parseInt(p_value);
//							retVal = (m_enumByValueMap.get(Integer.parseInt(p_value)) != null);
//						}
//						catch (NumberFormatException e)
//						{
//							retVal = false;
//						}
			case ParamValidationType.SIGNED_RANGE:
			case ParamValidationType.UNSIGNED_RANGE:
				bool parseResult = Int64.TryParse (value, out intValue);

				if (parseResult == true) {
					retVal = intValue >= MinimumValue &&
						intValue <= MaximumValue;
				} else {
					retVal = false;
				}
				break;
			default:
				throw new NotImplementedException ();
			}
			return retVal;
		}

		public bool SetValue (String value)
		{
		    Int64 intValue;
			if (ValidateValue (value, out intValue)) {
				this.Value = value;
			    this.IntValue = intValue;
				return true;
			} else {
				Value = "";
				log.Trace ("Parameter: {0} Invalid value:{1}", Identifier, value);
				return false;
			}
		}
	}
}

