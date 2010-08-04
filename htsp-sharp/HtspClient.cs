using System;
using System.Net;
using System.Net.Sockets;

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
		
		public bool DataAvailable {
			get { return stream.DataAvailable; }
		}
		
		public void Flush() {
			stream.Flush();
		}
	}
}