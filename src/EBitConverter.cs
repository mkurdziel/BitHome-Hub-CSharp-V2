using System;
using System.Collections.Generic;

namespace BitHome {
  static class EBitConverter {

    public static UInt16 ToUInt16(List<byte> p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[2];
      // HACK: bounds checking
      if (p_byte.Count > (p_startIndex+1))
      {
        bytes[0] = p_byte[p_startIndex + 1];
        bytes[1] = p_byte[p_startIndex];
      }
      return BitConverter.ToUInt16(bytes, 0);
    }

    public static UInt32 ToUInt32(List<byte> p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[4];
      bytes[0] = p_byte[p_startIndex + 3];
      bytes[1] = p_byte[p_startIndex + 2];
      bytes[2] = p_byte[p_startIndex + 1];
      bytes[3] = p_byte[p_startIndex + 0];
      return BitConverter.ToUInt32(bytes, 0);
    }

    public static UInt64 ToUInt64(List<byte> p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[8];
      bytes[0] = p_byte[p_startIndex + 7];
      bytes[1] = p_byte[p_startIndex + 6];
      bytes[2] = p_byte[p_startIndex + 5];
      bytes[3] = p_byte[p_startIndex + 4];
      bytes[4] = p_byte[p_startIndex + 3];
      bytes[5] = p_byte[p_startIndex + 2];
      bytes[6] = p_byte[p_startIndex + 1];
      bytes[7] = p_byte[p_startIndex + 0];
      return BitConverter.ToUInt64(bytes, 0);
    }

    public static UInt16 ToUInt16(byte[] p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[2];
      bytes[0] = p_byte[p_startIndex + 1];
      bytes[1] = p_byte[p_startIndex];
      return BitConverter.ToUInt16(bytes, 0);
    }

    public static UInt32 ToUInt32(byte[] p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[4];
      bytes[0] = p_byte[p_startIndex + 3];
      bytes[1] = p_byte[p_startIndex + 2];
      bytes[2] = p_byte[p_startIndex + 1];
      bytes[3] = p_byte[p_startIndex + 0];
      return BitConverter.ToUInt32(bytes, 0);
    }

    public static UInt64 ToUInt64(byte[] p_byte, int p_startIndex)
    {
      byte[] bytes = new byte[8];
      bytes[0] = p_byte[p_startIndex + 7];
      bytes[1] = p_byte[p_startIndex + 6];
      bytes[2] = p_byte[p_startIndex + 5];
      bytes[3] = p_byte[p_startIndex + 4];
      bytes[4] = p_byte[p_startIndex + 3];
      bytes[5] = p_byte[p_startIndex + 2];
      bytes[6] = p_byte[p_startIndex + 1];
      bytes[7] = p_byte[p_startIndex + 0];
      return BitConverter.ToUInt64(bytes, 0);
    }

    public static byte[] GetBytes(UInt16 p_val)
    {
      byte[] b1 = BitConverter.GetBytes(p_val);
      byte[] b2 = new byte[2];
      b2[0] = b1[1];
      b2[1] = b1[0];
      return b2;
    }

    public static byte[] GetBytes(UInt32 p_val)
    {
      byte[] b1 = BitConverter.GetBytes(p_val);
      byte[] b2 = new byte[4];
      b2[0] = b1[3];
      b2[1] = b1[2];
      b2[2] = b1[1];
      b2[3] = b1[0];
      return b2;
    }

    public static byte[] GetBytes(UInt64 p_val)
    {
      byte[] b1 = BitConverter.GetBytes(p_val);
      byte[] b2 = new byte[8];
      b2[0] = b1[7];
      b2[1] = b1[6];
      b2[2] = b1[5];
      b2[3] = b1[4];
      b2[4] = b1[3];
      b2[5] = b1[2];
      b2[6] = b1[1];
      b2[7] = b1[0];
      return b2;
    }

    public static byte[] GetBytes(Int64 p_val)
    {
        byte[] b1 = BitConverter.GetBytes(p_val);
        byte[] b2 = new byte[8];
        b2[0] = b1[7];
        b2[1] = b1[6];
        b2[2] = b1[5];
        b2[3] = b1[4];
        b2[4] = b1[3];
        b2[5] = b1[2];
        b2[6] = b1[1];
        b2[7] = b1[0];
        return b2;
    }



    ////////////////////////////////////////////////////////////////////////////
    #region Utility Functions

    public static String ToHex(UInt64 p_val)
    {
      String high = ToHex((UInt32)(p_val >> 32));
      String low = ToHex((UInt32)(p_val));
      return String.Concat(high, low);
    }

    public static String ToHex(UInt32 p_val)
    {
      return Convert.ToString(p_val, 16).PadLeft(8, '0').ToUpper();
    }

    public static String ToHex(UInt16 p_val)
    {
      return Convert.ToString(p_val, 16).PadLeft(4, '0').ToUpper();
    }

    public static String ToHex(byte p_val)
    {
      return Convert.ToString(p_val, 16).PadLeft(2, '0').ToUpper();
    }
    #endregion
    ////////////////////////////////////////////////////////////////////////////

  }
}
