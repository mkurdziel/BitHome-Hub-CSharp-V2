using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitHome
{
    class DataHelpers
    {
        /// <summary>
        ///   Extracts a zero terminating string
        /// </summary>
        /// <param name="p_msgData"></param>
        /// <param name="p_nStartingOffset"></param>
        /// <param name="p_strName"></param>
        /// <returns></returns>
        public static int GatherZeroTermString(IList<byte> p_msgData, int p_nStartingOffset, out string p_strName)
        {
          StringBuilder sb = new StringBuilder();
          int nDiscoveredLength = 0;
          while (p_msgData[p_nStartingOffset] != 0x00)
          {
            sb.Append((char) p_msgData[p_nStartingOffset++]);
            nDiscoveredLength++;
          }
          p_strName = sb.ToString();
          return nDiscoveredLength + 1;
        }

        /// <summary>
        ///   Extracts a value of a given width
        /// </summary>
        /// <param name="p_msgData"></param>
        /// <param name="p_nByteIdx"></param>
        /// <param name="p_nValueWidthInBytes"></param>
        /// <param name="p_nBuiltValue"></param>
        /// <returns></returns>
        public static int LoadValueGivenWidth(IList<byte> p_msgData, int p_nByteIdx, int p_nValueWidthInBytes,
                                              out int p_nBuiltValue)
        {
          p_nBuiltValue = 0;

          for (int nByteCt = 0; nByteCt < p_nValueWidthInBytes; nByteCt++)
          {
            p_nBuiltValue = (p_nBuiltValue << 8) + p_msgData[p_nByteIdx + nByteCt];
          }

          return p_nValueWidthInBytes;
        }
    }
}
