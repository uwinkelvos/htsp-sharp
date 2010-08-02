using System;
using System.Collections.Generic;

namespace htsp
{
	public enum TypeID: byte
	{
		MAP  = 1,
		S64  = 2,
		STR  = 3,
		BIN  = 4,
		LIST = 5,
		DBL  = 6,
	}

	public interface IHtspType
	{
	}
	
	public class HtspType<T> : IHtspType
//		where T: long, Message, string, byte[], List<IHtspType>
	{
		private T val;

		public HtspType(T val) {
			this.val = val;
		}

		public T Value {
			get { return val; }
		}
	}

}
