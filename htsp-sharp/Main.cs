using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace htsp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var client = new HtspClient("holzi", 9982);
			int seq = 1;

			Message request;
			Message reply;

			request = new Message();
			request.SetStringField("method", "hello");
			request.SetStringField("clientname", "htsp-sharp");
			request.SetIntField("htspversion", 5);
			request.SetIntField("seq", seq++);
			
			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			request = new Message();
			request.SetStringField("method", "enableAsyncMetadata");
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			int counter = 0;
			while (true)
			{
				Message msg = client.Receive();
				try {
					if ((msg.GetStringField("method") == "channelAdd")
						&& (msg.GetStringField("channelName") == "Das Erste")
						)
						{
							Console.WriteLine("Received:\n" + msg.ToString(true));
							break;
						}
				} catch (KeyNotFoundException ex) {
					
				}
			}

			request = new Message();
			request.SetStringField("method", "subscribe");
			request.SetIntField("channelId", 0x10);
			request.SetIntField("subscriptionId", 0xabcdef);
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			counter = 0;
			FileStream audio = File.Create("test.mp2");
			FileStream video = File.Create("test.mpeg");
			while (counter++ < 10000)
			{
				Message msg = client.Receive();
				try {
					if ((msg.GetStringField("method") == "muxpkt")
						)
					{
						//Console.WriteLine("Received:\n" + msg.ToString(true));
						long stream = msg.GetIntField("stream");
						byte[] payload = msg.GetBinField("payload");
						if (stream == 0x1) {
							video.Write(payload, 0, payload.Length);
						}
						else if (stream == 0x2) {
							audio.Write(payload, 0, payload.Length);
						}
					}
				} catch (KeyNotFoundException ex) {
					
				}
			}
			audio.Close();
			video.Close();

			request = new Message();
			request.SetStringField("method", "unsubscribe");
			request.SetIntField("subscriptionId", 0xabcdef);
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			/*
			*/
			
			#region stuff
			/*
			request = new Message();
			request.SetStringField("method", "getSysTime");
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());

			long unixdate = reply.GetIntField("time") - 60 * reply.GetIntField("timezone");
			Console.WriteLine(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixdate));
			 */
			
			/*
			request = new Message();
			request.SetStringField("method", "epgQuery");
			request.SetStringField("query", "sport");
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString(true));

			var field = reply.GetListField("eventIds");
			var val = (HtspType<long>)field[0];
			
			request = new Message();
			request.SetStringField("method", "getEvents");
			request.SetIntField("eventId", val.Value);
			request.SetIntField("numFollowing", 1);
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString(true));
			
			foreach(IHtspType item in reply.GetListField("events")) {
				//Console.WriteLine("+++ SubMessage\n{0}--- SubMessage\n",((HtspType<Message>)item).Value.ToString());
			}
			*/
			#endregion
			
			// Close everything.
			client.Dispose();
		}
	}
}

