using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GW2PAO.API.Data.Entities
{
	public class WorldEvent
	{
		public string Name { get; set; }
		public Guid ID { get; set; }
		public int MapID { get; set; }
		public string MapName { get; set; }
		public List<EventTimespan> ActiveTimes { get; set; }
		public EventTimespan Duration { get; set; }
		public EventTimespan WarmupDuration { get; set; }
		public string WaypointCode { get; set; }
		public List<Point> CompletionLocations { get; set; }
		public double CompletionRadius { get; set; }

		public WorldEvent()
		{
			this.ActiveTimes = new List<EventTimespan>();
			this.Duration = new EventTimespan();
			this.WarmupDuration = new EventTimespan();
			this.CompletionLocations = new List<Point>();
		}
	}

	/// <summary>
	/// Helper class due to limitation in serializing timespan objects
	/// </summary>
	[Serializable]
	public class EventTimespan
	{
		// Public Property - XmlIgnore as it doesn't serialize anyway
		[XmlIgnore]
		public TimeSpan Time { get; set; }

		// Pretend property for serialization
		[XmlElement("Time")]
		public string XmlTime
		{
			get { return Time.ToString(); }
			set { Time = TimeSpan.Parse(value); }
		}

		public EventTimespan(int hours, int minutes, int seconds)
		{
			this.Time = new TimeSpan(hours, minutes, seconds);
		}

		public EventTimespan()
		{
			this.Time = new TimeSpan();
		}

		public static EventTimespan operator +(EventTimespan lhs, EventTimespan rhs)
		{
			EventTimespan retVal = new EventTimespan();
			retVal.Time = lhs.Time + rhs.Time;
			return retVal;
		}

		public static bool operator >(EventTimespan lhs, EventTimespan rhs)
		{
			if ((object)lhs == null) return false;
			else if ((object)rhs == null) return true;
			else return lhs.Time > rhs.Time;
		}

		public static bool operator >=(EventTimespan lhs, EventTimespan rhs)
		{
			return (lhs > rhs) || (lhs == rhs);
		}

		public static bool operator <(EventTimespan lhs, EventTimespan rhs)
		{
			return !(lhs > rhs) && (lhs != rhs);
		}

		public static bool operator <=(EventTimespan lhs, EventTimespan rhs)
		{
			return (lhs < rhs) || (rhs == lhs);
		}

		public static bool operator ==(EventTimespan lhs, EventTimespan rhs)
		{
			if ((object)lhs == null) return (object)rhs == null;
			else if ((object)rhs == null) return false;
			else return lhs.Time == rhs.Time;
		}

		public static bool operator !=(EventTimespan lhs, EventTimespan rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			var obj_et = obj as EventTimespan;
			if (obj_et == null) return base.Equals(obj);
			return this == obj_et;
		}

		public override int GetHashCode()
		{
			return this.Time.GetHashCode();
		}
	}
}