using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace htsp
{
	public class Message
	{
		#region private members
		private Dictionary<string, IHtspType> fields;
		#endregion
		
		#region ctors
		public Message()
		{
			this.fields = new Dictionary<string, IHtspType>();
		}
		
		public Message(byte[] bin, int offset, int count)
			: this()
		{
			ParseBin(bin, offset, count);
		}
	
		public Message(byte[] bin)
			: this(bin, 0, bin.Length)
		{
		}
		#endregion
		
		#region bin parsers
		public static int ParseValueLength(byte[] bin, int offset) {
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bin, offset));
		}
		
		private static long ParseIntField(byte[] bin, int offset, int valLen) {
			long tmp = 0;
			// little endian!
			for (int i = valLen - 1; i >= 0; i--) {
				tmp = tmp << 8;
				tmp = tmp | (long)bin[offset+i];
			}
			return tmp;
		}
		
		private static IHtspType ParseField(TypeID fType, byte[] bin, int offset, int valLen)
		{
			switch(fType) {
				case TypeID.MAP: {
					Message message = new Message(bin, offset, valLen);
					return new HtspType<Message>(message);
				}
				case TypeID.S64: {
					return new HtspType<long>(ParseIntField(bin, offset, valLen));
				}
				case TypeID.STR: {
					return new HtspType<string>(Encoding.UTF8.GetString(bin,offset,valLen));
				}
				case TypeID.BIN: {
					byte[] bVal = new byte[valLen];
					Buffer.BlockCopy(bin, offset, bVal, 0, valLen);
					return new HtspType<byte[]>(bVal);
				}
				case TypeID.LIST: {
					List<IHtspType> list = new List<IHtspType>(16);
					int lOffset = 0;
					while (lOffset < valLen) {
						TypeID lfType = (TypeID)bin[offset + lOffset++];
						// listfields dont have a name! we should probaly check that.
						int lNameLen = bin[offset + lOffset++];
						int lValLen = ParseValueLength(bin, offset + lOffset);
						lOffset += 4;
						//string name = Encoding.UTF8.GetString(bin,offset + lOffset,nameLen);
						lOffset += lNameLen;
						list.Add(ParseField(lfType, bin, offset + lOffset, lValLen));
						lOffset += lValLen;
					}
					return new HtspType<List<IHtspType> >(list);
				}
				default: {
					Console.WriteLine("MessageType ({0}) not implented yet!", bin[offset-1]);
					byte[] bVal = new byte[valLen];
					Buffer.BlockCopy(bin, offset, bVal, 0, valLen);
					Console.WriteLine(BitConverter.ToString(bVal));
					return null;
				}
			}
		}
		
		public int ParseBin(byte[] bin, int offset, int count)
		{
			while (offset < count) {
				TypeID fType = (TypeID)bin[offset++];
				int nameLen = bin[offset++];
				int valLen = ParseValueLength(bin, offset);
				offset += 4;
				string name = Encoding.UTF8.GetString(bin,offset,nameLen);
				offset += nameLen;

				//ParseField
				fields.Add(name, ParseField(fType, bin, offset, valLen));
				
				offset += valLen;
			}
			return offset;
		}
		#endregion
		
		#region fields
		public long GetIntField(string name) {
			return ((HtspType<long>)fields[name]).Value;
		}
		public void SetIntField(string name, long val) {
			fields[name] = new HtspType<long>(val);
		}
		
		public string GetStringField(string name) {
			return ((HtspType<string>)fields[name]).Value;
		}
		public void SetStringField(string name, string val) {
			fields[name] = new HtspType<string>(val);
		}

		public byte[] GetBinField(string name) {
			return ((HtspType<byte[]>)fields[name]).Value;
		}
		public void SetBinField(string name, byte[] val) {
			fields[name] = new HtspType<byte[]>(val);
		}

		public List<IHtspType> GetListField(string name) {
			return ((HtspType<List<IHtspType> >)fields[name]).Value;
		}
		public void SetListField(string name, List<IHtspType> val) {
			fields[name] = new HtspType<List<IHtspType> >(val);
		}

		public Message GetMessageField(string name) {
			return ((HtspType<Message>)fields[name]).Value;
		}
		public void SetMessageField(string name, Message val) {
			fields[name] = new HtspType<Message>(val);
		}
		
		public int FieldCount {
			get { return fields.Count; }
		}
		#endregion
		
		#region representations

		private static void FieldToBin(IHtspType field, out byte bType, out byte[] bVal) {
			if (false); //just a dummy
			// this is untested yet!
			else if (field is HtspType<Message>) {
				bType = (byte)TypeID.MAP;
				Message val = ((HtspType<Message>)field).Value;
				bVal = val.ToBin();
			}
			else if (field is HtspType<long>) {
				bType = (byte)TypeID.S64;
				long val = ((HtspType<long>)field).Value;
				byte valLen = 0;
				long tmp = val;
				while((tmp)!=0) {
					tmp=tmp>>8;
					valLen++;
				}
				bVal = new byte[valLen];
				Buffer.BlockCopy(BitConverter.GetBytes(val), 0 , bVal, 0, valLen);
			}
			else if (field is HtspType<string>) {
				bType = (byte)TypeID.STR;
				string val = ((HtspType<string>)field).Value;
				bVal=Encoding.UTF8.GetBytes(val);
			}
			else if (field is HtspType<byte[]>) {
				bType = (byte)TypeID.BIN;
				bVal = ((HtspType<byte[]>)field).Value;
			}
			else if (field is HtspType<List<IHtspType> >) {
				bType = (byte)TypeID.LIST;
				MemoryStream mstream = new MemoryStream(1460);
				foreach(IHtspType item in ((HtspType<List<IHtspType> >)field).Value) {
					byte lbType;
					byte[] lbVal;
					FieldToBin(item, out lbType, out lbVal);

					mstream.WriteByte(lbType);
					// listfields dont have a name
					mstream.WriteByte(0);
					mstream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(lbVal.Length)),0,4);
					// listfields dont have a name !!!
					//mstream.Write(bName,0,bName.Length);
					mstream.Write(lbVal,0,lbVal.Length);
				}
				mstream.Close();
				bVal = mstream.ToArray();
			}
			else {
				// irgendwas werfen!
				bType = 0;
				bVal = new byte[0];
				Console.WriteLine("{0}.ToBin() not supported yet!", field.GetType().ToString());
			}
		}
		
		// this method needs a good amount of abstraction to support lists
		public byte[] ToBin() {
			MemoryStream mstream = new MemoryStream(1460);//max tcp müssten reichen, oder?
			foreach (KeyValuePair<string, IHtspType>fieldPair in fields) {
				byte bType;
				byte[] bName=Encoding.UTF8.GetBytes(fieldPair.Key);
				byte[] bVal;
				
				FieldToBin(fieldPair.Value, out bType, out bVal);
	
				mstream.WriteByte(bType);
				mstream.WriteByte((byte)bName.Length);
				mstream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bVal.Length)),0,4);
				mstream.Write(bName,0,bName.Length);
				mstream.Write(bVal,0,bVal.Length);
			}
			mstream.Close();
			return mstream.ToArray();
		}
		
		public override string ToString() {
			return ToString(false);
		}
		
		public string ToString(bool debug) {
			var sb = new StringBuilder();
			
			foreach (KeyValuePair<string, IHtspType>field in fields) {
				sb.AppendFormat("{0}: {1}\n", field.Key, FieldToString(field.Value, debug));
			}
			
			return sb.ToString();
		}

		private static string FieldToString(IHtspType field, bool debug) {
			if (false); //just a dummy
			else if (field is HtspType<Message>) {
				var sb = new StringBuilder();
				sb.AppendFormat("(msg) [{0} fields]", ((HtspType<Message>)field).Value.FieldCount);
				sb.AppendFormat("\n+++ Message +++\n");
				sb.AppendFormat("{0}", ((HtspType<Message>)field).Value.ToString(debug));
				sb.AppendFormat("--- Message ---");
				return sb.ToString();
			}
			else if (field is HtspType<long>) {
				return String.Format("(int) 0x{0}", ((HtspType<long>)field).Value.ToString("x"));
			}
			else if (field is HtspType<string>) {
				return String.Format("(str) \"{0}\"", ((HtspType<string>)field).Value);
			}
			else if (field is HtspType<byte[]>) {
				return String.Format("(bin) [{0} bytes]", ((HtspType<byte[]>)field).Value.Length);
			}
			else if (field is HtspType<List<IHtspType> >) {
				var sb = new StringBuilder();
				sb.AppendFormat("(lst) [{0} items]", ((HtspType<List<IHtspType> >)field).Value.Count);
				if (debug) {
					sb.AppendFormat("\n+++ List +++\n");
					foreach(IHtspType item in ((HtspType<List<IHtspType> >)field).Value) {
						sb.AppendFormat("{0}\n", FieldToString(item, debug));
					}
					sb.AppendFormat("--- List ---");
				}
				return sb.ToString();
			}
			else {
				// irgendwas werfen!
				Console.WriteLine("{0}.ToString() not supported yet!", field.GetType().ToString());
				return "";
			}
		}
		#endregion
	}
}