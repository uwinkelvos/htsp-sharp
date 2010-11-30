using System;
using System.Collections.Generic;

namespace htsp
{
	public class Channel
	{
		long id;
		string name;
		long number;
		string icon;
		long? currentEvent;
		List<Tag> tags;
	
		public Channel (long id)
		{
			this.id = id;
			this.tags = new List<Tag>();
		}
		
		public long Id {
			get { return id; }
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public long Number {
			get { return number; }
			set { number = value; }
		}
		
		public string Icon {
			get { return icon; }
			set { icon = value; }
		}
		
		public long? CurrentEvent {
			get { return currentEvent; }
			set { currentEvent = value; }
		}
		
		public List<Tag> Tags {
			get { return tags; }
		}
	}
}

