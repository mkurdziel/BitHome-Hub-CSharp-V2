using System.Collections.Generic;
using System.Text;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageCatalogResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override Api Api {
			get {
				return Protocol.Api.CATALOG_RESPONSE;
			}
		}

		public int ActionIndex { get; private set;}

		public int ParameterCount { get; private set;}

		public DataType ReturnType { get; private set;}

		public string Name { get; private set; }

		public byte Options { get; private set; }

		public MessageCatalogResponse (
			Node p_sourceNode,
			int p_entryNumber,
			int p_numParams,
			DataType p_returnType,
			string p_name ) : base (p_sourceNode, null)
		{
			ActionIndex = p_entryNumber;
			ParameterCount = p_numParams;
			ReturnType = p_returnType;
			Name = p_name;
		}

        public MessageCatalogResponse(
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
            base(p_sourceNode, p_destinationNode)
        {
            ActionIndex = p_data[p_dataOffset + 2];
            ReturnType = (DataType)p_data[p_dataOffset + 3];
            ParameterCount = p_data[p_dataOffset + 4];
            Options = p_data[p_dataOffset + 5];
                
			StringBuilder sbname = new StringBuilder ();
			int stringIndex = 6;
			char c;
            while ((c = (char)p_data[p_dataOffset + (stringIndex++)]) != 0x00)
            {
                sbname.Append(c);
                if (stringIndex >= p_data.Length)
                {
                    log.Warn("Name out of bounds");
                }
            }
            Name = sbname.ToString();
        }

		public override string ToString ()
		{
			return string.Format ("[MessageCatalogResponse: ActionIndex={1}, ParameterCount={2}, ReturnType={3}, Name={4}, Options={5}]", 
			                      Api, ActionIndex, ParameterCount, ReturnType, Name, Options);
		}
    }
}
