using System;

namespace BitHome
{
	public class NodeUpdateFile
	{
		private byte[] m_dataBytes;
		private String m_fileName;
		private int m_minAddress;
		private int m_maxAddress;
		private long m_lengthInBytes = 0;


		public NodeUpdateFile(String p_fileName)
		{
			m_fileName = p_fileName; 
		}

//		public Boolean parseFile()
//		{
//			File dataFile = new File(m_strFileName);
//			Boolean retVal = false;
//			int byteCount;
//			int lineNumber = 0;
//			int memoryAddressHigh;
//			int memoryAddressLow;
//			int memoryAddress;
//
//				// Make sure the file exists
//				if (dataFile.exists())
//				{
//					m_lengthInBytes = dataFile.length();
//					// Create the byte buffer of the length of the file size
//					// This will be a tad bigger than needed but that's okay
//					// It won't be larger than an int so the cast should be safe
//					m_dataBytes = new byte[(int)m_lengthInBytes];
//
//					Logger.v(TAG, "update file is length " + m_lengthInBytes);
//
//					try
//					{
//						BufferedReader br = new BufferedReader(new FileReader(dataFile));
//
//						String line;
//						// Read through all of the lines
//						while((line = br.readLine()) != null)
//						{
//							//Logger.v(TAG, "reading update file line: " + line);
//							line = line.trim();
//							lineNumber++;
//							// Skip any blank lines
//							if (line.length() == 0) continue;
//
//							// This is for parsing the Intel file format
//							if (line.startsWith(":")) {
//								String subStr = line.substring(7, 9);
//								//Logger.v(TAG, "parsing record type:" + subStr);
//								//  02 - extended segment address record
//								//  04 - extended linear address record
//								if (subStr.equals("02") || subStr.equals("04"))          
//								{
//									Logger.w(TAG, "parsing unsupported data file");
//									return false;
//								} 
//								else if (subStr.equals("03"))
//								{
//									continue;
//								}
//								else if (subStr.equals("01")) // 01 - end-of-file record
//								{
//									//Logger.w(TAG, "found end of file");
//									retVal = true;
//									br.close();
//									return true;
//								}
//								else if (subStr.equals("00"))
//								{
//								}
//								else
//								{
//									Logger.w(TAG, "parsing unknown record type");
//									return false;
//								}
//
//							}
//
//							// Parse the byte count
//							byteCount = Integer.parseInt(line.substring(1,3), 16);
//
//							//Logger.v(TAG, "byte count is " + byteCount);
//
//							if (byteCount == 0) continue;
//
//							// Get the memory address of this line
//							memoryAddressHigh = Integer.parseInt(line.substring(3, 5), 16);
//							memoryAddressLow = Integer.parseInt(line.substring(5, 7), 16);
//
//							//Logger.v(TAG, "memory address high:" + memoryAddressHigh + " low:" + memoryAddressLow); 
//
//							memoryAddress = (memoryAddressHigh * 256) + memoryAddressLow;
//
//							for (int idx = 0; idx < byteCount; idx++) {
//								int address = memoryAddress + idx;
//								if (address >= m_lengthInBytes) {
//									Logger.w(TAG, String.format("Error in line %d of %d Address %02x out of buffer (max. %03x)", lineNumber, m_strFileName, address, m_lengthInBytes));
//									br.close();
//									return false;
//								}
//								if (m_maxAddress < address)
//									m_maxAddress = address;
//								if (m_minAddress > address)
//									m_minAddress = address;
//								int dataIndex = 9 + idx * 2;
//								m_dataBytes[address] = (byte)Integer.parseInt(line.substring(dataIndex, dataIndex+2), 16);
//								//Logger.v(TAG, String.format("%d:%02x", address,m_dataBytes[address]));
//							}
//						}
//						Logger.v(TAG, "done reading data file");
//						br.close();
//					} 
//					catch (FileNotFoundException e)
//					{
//						Logger.w(TAG, "file not found: " + m_strFileName);
//					} 
//					catch (IOException e)
//					{
//						Logger.w(TAG, "IO Error reading file", e);
//					}
//					catch (NumberFormatException e)
//					{
//						Logger.w(TAG, "Number format exception parsing file", e);
//					}
//				}
//				else
//				{
//					Logger.w(TAG, "file does not exist: " + m_strFileName);
//				}
//
//				return retVal;
//			}
//
//			/**
//     * @return the length of the data file in bytes
//     */
//			public long getLengthInBytes()
//			{
//				return m_lengthInBytes;
//			}
//
//			/**
//     * @return the name of the file
//     */
//			public String getFileName()
//			{
//				return m_strFileName;
//			}
//
//			/**
//     * @return the lowest address in the file
//     */
//			public int getMinAddress()
//			{
//				return m_minAddress;
//			}
//
//			/**
//     * @return the highest address in the file
//     */
//			public int getMaxAddress()
//			{
//				return m_maxAddress;
//			}
//
//			/**
//     * Get a data byte 
//     * 
//     * @param p_address
//     * @return
//     */
//			public byte getDataByte(int p_address)
//			{
//				if (p_address < m_dataBytes.length)
//				{
//					return m_dataBytes[p_address];
//				}
//				Logger.w(TAG, "requesting invalid address: " + p_address);
//				return 0;
//			}
//
//		}

	}
}

