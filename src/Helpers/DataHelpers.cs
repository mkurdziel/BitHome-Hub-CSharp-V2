﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Helpers
{
    static class DataHelpers
    {
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public const long MAX_JAVSCRIPT_NUMBER = 9007199254740992;
        public static readonly Random Random = new Random();

		public static string RandomString(int numChars) {
			var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
			var result = new string(
				Enumerable.Repeat(chars, numChars)
				.Select(s => s[Random.Next(s.Length)])
				.ToArray());
			return result;
		}

		public static long RandomLongPositive() {
            return (long)(DataHelpers.Random.NextDouble() * MAX_JAVSCRIPT_NUMBER);
		}

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

		/// <summary>
		///   Extracts a value of a given width
		/// </summary>
		/// <param name="p_msgData"></param>
		/// <param name="p_nByteIdx"></param>
		/// <param name="p_nValueWidthInBytes"></param>
		/// <param name="p_nBuiltValue"></param>
		/// <returns></returns>
		public static int LoadValueGivenWidth(IList<byte> p_msgData, int p_nByteIdx, int p_nValueWidthInBytes,
		                                      out Int64 p_nBuiltValue)
		{
			p_nBuiltValue = 0;

			for (int nByteCt = 0; nByteCt < p_nValueWidthInBytes; nByteCt++)
			{
				p_nBuiltValue = (p_nBuiltValue << 8) + p_msgData[p_nByteIdx + nByteCt];
			}

			return p_nValueWidthInBytes;
		}

		public static int LoadValueGivenWidth(IList<byte> p_msgData, int p_nByteIdx, DataType dataType,
		                                      out Int64 p_nBuiltValue)
		{
			p_nBuiltValue = 0;
			int width = 0;

			// Get the minium and maximum values
			switch (dataType)
			{
				case DataType.BOOL:
				case DataType.INT8:
				case DataType.UINT8:
				width = 1;
				break;
				case DataType.INT16:
				case DataType.UINT16:
				width = 2;
				break;
				case DataType.INT32:
				case DataType.UINT32:
				width = 4;
				break;
				case DataType.INT64:
				case DataType.UINT64:
				width = 8;
				break;
				case DataType.VOID:
				width = 0;
				break;
				case DataType.STRING:
				width = 1;
				break;
				default:
				log.Warn("Unrecognized Parameter Data Type: {0}", dataType);
				return 0;
			}


			for (int nByteCt = 0; nByteCt < width; nByteCt++)
			{
				p_nBuiltValue = (p_nBuiltValue << 8) + p_msgData[p_nByteIdx + nByteCt];
			}

			return width;
		}


		public static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (ReferenceEquals(a1,a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Length != a2.Length)
				return false;

			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!comparer.Equals(a1[i], a2[i])) return false;
			}
			return true;
		}
    }
}
