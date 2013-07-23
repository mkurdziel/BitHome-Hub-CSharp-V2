using System;
using NUnit.Framework;

namespace BitHomeTests
{
	[TestFixture()]
	public class ParameterBaseTest
	{
		[Test()]
		public void TestSetValueString ()
		{
			TestParameter param = new TestParameter ();

			// Test Strings
			param.DataType = BitHome.Messaging.Protocol.DataType.STRING;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.BOOL;

			// Test that the proper validation type is set
			Assert.AreEqual (param.ValidationType, BitHome.Messaging.Protocol.ParamValidationType.STRING);

			// Make sure this can't be negative
			param.MinimumValue = -1;
			Assert.IsTrue (param.MinimumValue == 0);

			param.MinimumValue = 2;
			param.MaximumValue = 4;

			Assert.IsFalse (param.SetValue ("a")); // Too Short
			Assert.IsTrue (param.SetValue ("aa")); 
			Assert.IsTrue (param.SetValue ("aaa")); 
			Assert.IsTrue (param.SetValue ("aaaa"));
			Assert.IsTrue ( String.Equals(param.Value, "aaaa"), "Value was: "+param.Value);
			Assert.IsFalse (param.SetValue ("aaaaa")); // Too Long
			Assert.IsFalse ( String.Equals(param.Value, "aaaaa"), "Value was: "+param.Value);
		}

		[Test()]
		public void TestSetValueBool ()
		{
			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.BOOL;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.STRING;

			// Test that the proper validation type is set
			Assert.AreEqual (param.ValidationType, BitHome.Messaging.Protocol.ParamValidationType.BOOL);

			param.MinimumValue = 2;
			param.MaximumValue = 4;

			// Test that the proper range is set
			Assert.AreEqual (param.MaximumValue, 1);
			Assert.AreEqual (param.MinimumValue, 0);

			Assert.IsTrue (param.SetValue ("0")); 
			Assert.AreEqual (param.Value, "0");
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.AreEqual ( param.Value, "1");
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueByteSigned ()
		{
			var max = SByte.MaxValue;
			var min = SByte.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.BYTE;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueByteUnsigned ()
		{
			var max = Byte.MaxValue;
			var min = Byte.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.BYTE;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueWordSigned ()
		{
			var max = Int16.MaxValue;
			var min = Int16.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.WORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueWordUnsigned ()
		{
			var max = UInt16.MaxValue;
			var min = UInt16.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.WORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueDWordSigned ()
		{
			var max = Int32.MaxValue;
			var min = Int32.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.DWORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueDWordUnsigned ()
		{
			var max = UInt32.MaxValue;
			var min = UInt32.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.DWORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueQWordSigned ()
		{
			var max = Int64.MaxValue;
			var min = Int64.MinValue;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.QWORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.SIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}

		[Test()]
		public void TestSetValueQWordUnsigned ()
		{
			var max = Int64.MaxValue;
			var min = 0;

			TestParameter param = new TestParameter ();

			param.DataType = BitHome.Messaging.Protocol.DataType.QWORD;
			param.ValidationType = BitHome.Messaging.Protocol.ParamValidationType.UNSIGNED_RANGE;

			// Test ranges
			param.MinimumValue = Int64.MinValue;
			Assert.AreEqual (param.MinimumValue, min);

			param.MaximumValue = Int64.MaxValue;
			Assert.AreEqual (param.MaximumValue, max);

			// Test ends
			Assert.IsTrue (param.SetValue (min.ToString())); 
			Assert.IsTrue (param.SetValue (max.ToString())); 

			// Test too small
			param.MinimumValue = 1;
			Assert.IsFalse (param.SetValue ("0")); 


			// Test too big
			param.MaximumValue = 3;
			Assert.IsFalse (param.SetValue ("4")); 

			// Test just right
			Assert.IsTrue (param.SetValue ("1")); 
			Assert.IsTrue (param.SetValue ("2")); 
			Assert.IsTrue (param.SetValue ("3")); 

			// Test weird
			Assert.IsFalse (param.SetValue ("a")); 
		}
	}
}

