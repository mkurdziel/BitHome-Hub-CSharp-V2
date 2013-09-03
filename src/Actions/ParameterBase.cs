using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Actions
{
	[Serializable]
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
			case DataType.STRING:
				if (value > 0) {
					return true;
				}
				break;
			case DataType.INT8:
				if (value >= SByte.MinValue && value <= SByte.MaxValue) {
						return true;
				}				
				break;
			case DataType.UINT8:
				if (value >= Byte.MinValue && value <= Byte.MaxValue) {
					return true;
				}
				break;
			case DataType.INT16:
				if (value >= Int16.MinValue && value <= Int16.MaxValue) {
					return true;
				}				
				break;
			case DataType.UINT16:
				if (value >= UInt16.MinValue && value <= UInt16.MaxValue) {
					return true;
				}
				break;
			case DataType.INT32:
				if (value >= Int32.MinValue && value <= Int32.MaxValue) {
					return true;
				}				
				break;
			case DataType.UINT32:
				if (value >= UInt32.MinValue && value <= UInt32.MaxValue) {
					return true;
				}
				break;
			case DataType.INT64:
				if (value >= Int64.MinValue && value <= Int64.MaxValue) {
					return true;
				}				
				break;
			case DataType.UINT64:
				// TODO uint64
				if (value >= 0 && value <= Int64.MaxValue) {
					return true;
				}
				break;
			default:
				throw new NotImplementedException ();
			}

			return true;
		}

		public String Identifier {
			get { return Id; }
		}

		public bool IsSigned {
			get {
				return DataType == DataType.INT8 || 
					DataType == DataType.INT16 || 
					DataType == DataType.INT32 || 
					DataType == DataType.INT64;
			}
		}

		public bool IsInteger {
			get {
				return DataType != DataType.BOOL && 
					DataType != DataType.STRING && 
					DataType != DataType.ENUM && 
					DataType != DataType.DATETIME;
			}
		}

		public bool IsString {
			get {
				return DataType == DataType.STRING;
			}
		}

		public String RangeString {
			get {
				String retVal = "unknown";

				switch (DataType) {
				case DataType.BOOL:
					retVal = "True or False";
					break;
				case DataType.ENUM:
					retVal = "Enumeration";
					break;
				case DataType.STRING:
					retVal = String.Format("{0}-character string", MaximumValue);
					break;
				case DataType.INT8:
				case DataType.INT16:
				case DataType.INT32:
				case DataType.INT64:
				case DataType.UINT8:
				case DataType.UINT16:
				case DataType.UINT32:
				case DataType.UINT64:
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
            Int64 minValue, 
            Int64 maxValue, 
            Dictionary<string, int> enumValues)
        {
            this.Id = id;
            this.Name = name;
            this.DataType = dataType;
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

			switch (DataType) {

			case DataType.BOOL:
				if (String.Equals (value, "0"))
				{
                    intValue = 0;
				} else if (String.Equals (value, "1")) {
                    intValue = 1;
				} else {
					retVal = false;
				}
				break;
			case DataType.ENUM:
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
			case DataType.INT8:
			case DataType.INT16:
			case DataType.INT32:
			case DataType.INT64:
			case DataType.UINT8:
			case DataType.UINT16:
			case DataType.UINT32:
			case DataType.UINT64:
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

		public bool EqualsExceptId (IParameter obj)
		{
			// If is null return false.
			if (obj == null)
			{
				return false;
			}

			// Return true if the fields match:
			bool same = true;

			same &= Name == obj.Name;
			same &= Description == obj.Description;
			same &= DependentParameterId == obj.DependentParameterId;
			same &= DataType == obj.DataType;
			same &= MinimumValue == obj.MinimumValue;
			same &= MaximumValue == obj.MaximumValue;

			return same;
		}

		public override bool Equals (object obj)
		{
			// If is null return false.
			if (obj == null)
			{
				return false;
			}

			// If cannot be cast to this class return false.
			ParameterBase p = obj as ParameterBase;
			if ((System.Object)p == null)
			{
				return false;
			}

			// Return true if the fields match:
			bool same = true;

			same &= this.Id == p.Id;
			same &= this.EqualsExceptId (p);

			return same;
		}
	}
}

