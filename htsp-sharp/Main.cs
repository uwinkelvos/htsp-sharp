using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Net;

namespace htsp
{
	class MainClass
	{
		private static Dictionary<long, Channel> channels = new Dictionary<long, Channel>();
		private static Dictionary<long, Tag> tags = new Dictionary<long, Tag>();
		
		private static void MessageDispatch(Message msg)
		{
			string method = msg.Method;
			
			if (method.Contains("channel"))
			{
				long id = msg.GetIntField("channelId");
				if (method.Contains("Delete"))
				{
					channels.Remove(id);
				}
				else
				{
					if (method.Contains("Add"))
					{
						if (!channels.ContainsKey(id)) channels[id] = new Channel(id);
					}
					Channel chan = channels[id];
					// -> (method.Contains("Update"))
					try
					{
						chan.Name = msg.GetStringField("channelName");
					}
					// was kommt da eigentlich argument out of range?
					catch (Exception ex) {}
					try
					{
						chan.Number = msg.GetIntField("channelNumber");
					}
					// was kommt da eigentlich argument out of range?
					catch (Exception ex) {}
					try
					{
						chan.Icon = msg.GetStringField("channelIcon");
					}
					// was kommt da eigentlich argument out of range?
					catch (Exception ex) {}
					try
					{
						chan.CurrentEvent = msg.GetIntField("eventId");
					}
					// was kommt da eigentlich argument out of range?
					catch (Exception ex) {}
				}
			}
			else if (method.Contains("tag"))
			{
				long id = msg.GetIntField("tagId");
			}
			else 
			{
				Console.WriteLine("received: {0}", method);
			}
		}
		
		public static void Main (string[] args)
		{
			
			var client = new HtspClient("holzi", 9982);
			int seq = 1;

			Message request;
			Message reply;

			request = new Message("hello");
			request.SetStringField("clientname", "htsp-sharp");
			request.SetIntField("htspversion", 5);
			request.SetIntField("seq", seq++);
			
			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());
			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			request = new Message("enableAsyncMetadata");
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			int counter = 0;
			while (counter++ < 2000)
			{
				reply = client.Receive();
				if (reply.GetStringField("method").Contains("channel")) Console.WriteLine("Received:\n" + reply.ToString());
				if (reply.GetStringField("method") == "initialSyncCompleted") break;
			}

			//getEvents 0x973
			/*
			request = new Message("getEvents");
			request.SetIntField("eventId", 0x1adf);
			request.SetIntField("numFollowing", 3);

			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString(true));

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString(true));
			*/
			/*
			request = new Message("subscribe");
			request.SetIntField("channelId", 0x15);
			request.SetIntField("subscriptionId", 0xabcdef);
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());

			counter = 0;
			while (counter++ < 2000)
			{
				Message msg = client.Receive();
				if ((msg.Method == "subscriptionStart"))
				{
					Console.WriteLine("Received:\n" + msg.ToString(true));
					//Console.WriteLine("Received: {0}", BitConverter.ToString(msg.ToBin()));

					break;
				}
			}

			request = new Message("unsubscribe");
			request.SetIntField("subscriptionId", 0xabcdef);
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());
			
			*/
			
			#region stuff
			/*
			byte[] bin = request.ToBin();
			FileStream hello = File.Create("hello.bin");
			hello.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bin.Length)),0,4);
			hello.Write(bin,0,bin.Length);
			*/
			/*
			long vduration = 0;
			long aduration = 0;
			FileStream audio = File.Create("test.mp2");
			FileStream video = File.Create("test.mpeg");
						long stream = msg.GetIntField("stream");
						byte[] payload = msg.GetBinField("payload");
						if (stream == 0x1) {
							video.Write(payload, 0, payload.Length);
							vduration += msg.GetIntField("duration");
						}
						else if (stream == 0x2) {
							audio.Write(payload, 0, payload.Length);
							aduration += msg.GetIntField("duration");
						}
			audio.Close();
			video.Close();
			
			Console.WriteLine("aduration: {0} msecs", aduration/1000);
			Console.WriteLine("vduration: {0} msecs", vduration/1000);
			*/
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

