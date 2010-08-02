using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace htsp
{
	[TestFixture()]
	public class MessageTest
	{
		[Test()]
		public void TestCase ()
		{
			Message msg1 = new Message();
			msg1.SetBinField("t_bin", new byte[]{1,2,3,4});
			msg1.SetIntField("t_int", 0xAB);
			msg1.SetStringField("t_string", "hallo");
			msg1.SetMessageField("t_message", new Message());
			var list = new List<int>();
			list.Add(0x99);
			list.Add(0x77);
			string sMsg1 = msg1.ToString(true);
			Message msg2 = new Message(msg1.ToBin());
			string sMsg2 = msg2.ToString(true);
			StringAssert.AreEqualIgnoringCase(sMsg1, sMsg2);
		}
	}
}

