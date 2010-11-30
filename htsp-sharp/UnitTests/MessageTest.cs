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
			Message msg1 = new Message("blah");
			msg1.SetBinField("t_bin", new byte[]{1,2,3,4});
			msg1.SetIntField("t_int", 0xAB);
			msg1.SetStringField("t_string", "hallo");
			Message msg3 = new Message("blub");
			msg3.SetStringField("a", "bbbbbbbbbbbbbbbbbbbbbb");
			msg3.SetIntField("blahahah", 1);
			msg1.SetMessageField("t_message", msg3);
			var list = new HtspListType<IHtspBaseType>();
			list.Add(new HtspType<long>(0x99));
			list.Add(new HtspType<long>(0x77));
			msg1.SetListField("t_list", list);
			string sMsg1 = msg1.ToString(true);
			Message msg2 = new Message(msg1.ToBin());
			string sMsg2 = msg2.ToString(true);

			Console.WriteLine(sMsg1);
			Console.WriteLine(BitConverter.ToString(msg1.ToBin()));
			Console.WriteLine(sMsg2);
			Console.WriteLine(BitConverter.ToString(msg2.ToBin()));
			
			StringAssert.AreEqualIgnoringCase(sMsg1, sMsg2);
		}
	}
}

