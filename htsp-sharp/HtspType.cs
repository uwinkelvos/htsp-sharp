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

	public interface IHtspBaseType: IHtspType
	{
	}

	public interface IHtspType
	{
	}

	
	// we probably need an list type!
	public class HtspListType<T> : List<IHtspBaseType>, IHtspType 
		where T: IHtspBaseType // necessary?
	{
		public HtspListType()
			: base()
		{
		}
		public HtspListType(int capacity)
			: base(capacity)
		{
		}
	}
	
	// you can do really stupid things with lists in lists, etc!
	public class HtspType<T> : IHtspBaseType
		//where T: long, Message, string, byte[]
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
