using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace htsp
{
	class HtspClient: IDisposable
	{
		private TcpClient client;
		private NetworkStream stream;
		
		public HtspClient (string host, int port)	{
			this.client = new TcpClient(host, port);
			this.stream = client.GetStream();
		}
		
		~HtspClient() {
			Dispose();
		}
		public void Dispose() {
			// tvheadend server autocloses connection on timeout!?
			stream.Close();         
			client.Close();
		}
		
		public void Send(Message message)
		{
			byte[] buf=message.ToBin();
			stream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(buf.Length)),0,4);
			stream.Write(buf,0,buf.Length);
			//Console.WriteLine("Send: {0}", BitConverter.ToString(buf));
		}
		
		public Message Receive() {		
			byte[] buf = new Byte[4];
			// if no data is on the stream we should throw an error!
			stream.Read(buf, 0, buf.Length);
			int msgLen = Message.ParseValueLength(buf,0);
			//Console.WriteLine("msgLen: {0}", msgLen);

			buf = new Byte[msgLen];
			stream.Read(buf, 0, buf.Length);
			//Console.WriteLine("Received: {0}", BitConverter.ToString(buf));
			return new Message(buf);
		}
	}
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			var client = new HtspClient("holzi", 9982);
			int seq = 1;
			
			Message request = new Message();
			request.SetStringField("method", "hello");
			request.SetStringField("clientname", "htsp-sharp");
			request.SetIntField("htspversion", 5);
			request.SetIntField("seq", seq++);
			
			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			Message reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString());

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
			
			request = new Message();
			request.SetStringField("method", "epgQuery");
			request.SetStringField("query", "sport");
			request.SetIntField("seq", seq++);

			client.Send(request);
			Console.WriteLine("Send:\n" + request.ToString());

			reply = client.Receive();
			Console.WriteLine("Received:\n" + reply.ToString(true));

			Message mTest = new Message(reply.ToBin());
			Console.WriteLine("Received:\n" + mTest.ToString(true));
			
			/*
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
			
			// Close everything.
			client.Dispose();
		}
	}
}

