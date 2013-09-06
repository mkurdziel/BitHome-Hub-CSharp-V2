using System;
using System.Collections.Generic;
using System.Text;
using BitHome.Messaging.Protocol;
using NLog;
using BitHome.Helpers;

namespace BitHome.Messaging.Messages
{
	public class MessageDataResponse : MessageBase
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override Api Api {
			get {
				return Protocol.Api.DATA_RESPONSE;
			}
		}

		public int ActionIndex { get; private set;}

		public int ParameterIndex { get; private set;}

		public DataType DataType { get; private set;}

		public byte Options { get; private set; }

		public string Value { get; private set; }

		public MessageDataResponse(
			Node p_sourceNode,
			Node p_destinationNode,
			byte[] p_data,
			int p_dataOffset) :
			base(p_sourceNode, p_destinationNode)
		{
			ActionIndex = p_data[p_dataOffset + 2];
			ParameterIndex = p_data[p_dataOffset + 3];
			DataType = (DataType)p_data[p_dataOffset + 4];
			Options = p_data[p_dataOffset + 5];

			// TODO handle strings
			Int64 value;
			DataHelpers.LoadValueGivenWidth (p_data, p_dataOffset+6, DataType, out value);
			Value = value.ToString();
		}
		public override string ToString ()
		{
			return string.Format ("[MessageDataResponse: ActionIndex={1}, ParameterIndex={2}, DataType={3}, Options={4}, Value={5}]", 
			                      Api, ActionIndex, ParameterIndex, DataType, Options, Value);
		}
	}
}
