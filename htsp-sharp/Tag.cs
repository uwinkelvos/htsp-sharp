using System;
using System.Collections.Generic;

namespace htsp
{
	public class Tag
	{
		long id;
		string name;
		string icon;
		List<Channel> channels;

		public Tag (int id)
		{
			this.id = id;
			this.channels = new List<Channel>();
		}
		
		public long Id {
			get { return id; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string Icon {
			get { return icon; }
			set { icon = value; }
		}

		public List<Channel>  Channels {
			get { return channels; }
		}
	}
}

