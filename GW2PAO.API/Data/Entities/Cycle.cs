using System;
using System.Collections.Generic;

namespace GW2PAO.API.Data.Entities
{
	public class Cycle
	{
		public string Name { get; set; }
		public Guid ID { get; set; }
		public int WorldMapID { get; set; }
		public string MapName { get; set; }
		public string WaypointCode { get; set; }
		public EventTimespan Length { get; set; }
		public EventTimespan Recurrence { get; set; }
		public EventTimespan Delay { get; set; }
		public List<EventTimespan> ActiveTimes { get; set; }

		//public EventTimespan Duration { get; set; }
		public EventTimespan WarmupDuration { get; set; }
	}
}