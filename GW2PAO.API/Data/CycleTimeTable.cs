using GW2PAO.API.Constants;
using GW2PAO.API.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GW2PAO.API.Data
{
	/// <summary>
	/// The Cycle events time table
	/// </summary>
	public class CycleTimeTable
	{
		/// <summary>
		/// File name for the standard time table
		/// </summary>
		public static readonly string FileName = "Cycles.xml";

		/// <summary>
		/// List of world events and their details
		/// </summary>
		public List<Cycle> Cycles { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public CycleTimeTable()
		{
			this.Cycles = new List<Cycle>();
		}

		/// <summary>
		/// Loads the world events time table file
		/// </summary>
		/// <returns>The loaded event time table data</returns>
		public static CycleTimeTable LoadTable()
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(CycleTimeTable));
			TextReader reader = new StreamReader(FileName);
			CycleTimeTable loadedData = null;
			try
			{
				object obj = deserializer.Deserialize(reader);
				loadedData = (CycleTimeTable)obj;
			} finally
			{
				reader.Close();
			}

			return loadedData;
		}

		private static void FillActiveTimes(Cycle cycle)
		{
			var currentTS = new EventTimespan(0, 0, 0);
			var wholeDay = new EventTimespan(24, 0, 0);
			cycle.ActiveTimes = new List<EventTimespan>();
			currentTS += cycle.Delay;
			while (currentTS < wholeDay)
			{
				cycle.ActiveTimes.Add(currentTS);
				currentTS += cycle.Recurrence;
			}
		}

		/// <summary>
		/// Creates the world events time table file
		/// </summary>
		/// <returns></returns>
		public static void CreateTable()
		{
			CycleTimeTable tt = new CycleTimeTable();
			tt.Cycles = new List<Cycle>();

			var vbDay = new Cycle()
			{
				Name = "Verdant Brink Day",
				ID = CycleID.VerdantBrinkDay,
				WorldMapID = 1052,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(1, 15, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 30, 0)
			};

			var vbNight = new Cycle()
			{
				Name = "Verdant Brink Night",
				ID = CycleID.VerdantBrinkNight,
				WorldMapID = 1052,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 25, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(1, 45, 0)
			};

			var vbNightBosses = new Cycle()
			{
				Name = "Verdant Brink Night Bosses",
				ID = CycleID.VerdantBrinkNightBosses,
				WorldMapID = 1052,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 20, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 10, 0)
			};

			var abPillars = new Cycle()
			{
				Name = "Auric Basin Pillars",
				ID = CycleID.AuricBasinPilars,
				WorldMapID = 1043,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(1, 15, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(1, 30, 0)
			};

			var abChallenges = new Cycle()
			{
				Name = "Auric Basin Challenges",
				ID = CycleID.AuricBasinChallenges,
				WorldMapID = 1043,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 15, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 45, 0)
			};
			var abOctovine = new Cycle()
			{
				Name = "Auric Basin Octovine",
				ID = CycleID.AuricBasinOctovine,
				WorldMapID = 1043,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 20, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(1, 0, 0)
			};
			var abReset = new Cycle()
			{
				Name = "Auric Basin Reset",
				ID = CycleID.AuricBasinReset,
				WorldMapID = 1043,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 10, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(1, 20, 0)
			};
			var tdOutposts = new Cycle()
			{
				Name = "Tangled Depths Outposts",
				ID = CycleID.TangledDepthsOutposts,
				WorldMapID = 1045,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(1, 35, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 50, 0)
			};
			var tdPrep = new Cycle()
			{
				Name = "Tangled Depths Prep",
				ID = CycleID.TangledDepthsPrep,
				WorldMapID = 1045,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 5, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 25, 0)
			};
			var tdGerent = new Cycle()
			{
				Name = "Tangled Depths Gerent",
				ID = CycleID.TangledDepthsGerent,
				WorldMapID = 1045,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 20, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(0, 30, 0)
			};
			var dsStart = new Cycle()
			{
				Name = "Dragon's Stand Start",
				ID = CycleID.DragonsStandStart,
				WorldMapID = 1041,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(2, 0, 0),
				Recurrence = new EventTimespan(2, 0, 0),
				Delay = new EventTimespan(1, 30, 0)
			};
			var dtCrash = new Cycle()
			{
				Name = "Dry Top Crash",
				ID = CycleID.DryTopCrash,
				WorldMapID = 988,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 40, 0),
				Recurrence = new EventTimespan(1, 0, 0),
				Delay = new EventTimespan(0, 0, 0)
			};
			var dtSandstorm = new Cycle()
			{
				Name = "Dry Top Sandstorm",
				ID = CycleID.DryTopSandstorm,
				WorldMapID = 988,
				WaypointCode = "[&BM0CAAA=]",
				Length = new EventTimespan(0, 20, 0),
				Recurrence = new EventTimespan(1, 0, 0),
				Delay = new EventTimespan(0, 40, 0)
			};

			FillActiveTimes(vbDay);
			FillActiveTimes(vbNight);
			FillActiveTimes(vbNightBosses);
			FillActiveTimes(abPillars);
			FillActiveTimes(abChallenges);
			FillActiveTimes(abOctovine);
			FillActiveTimes(abReset);
			FillActiveTimes(tdOutposts);
			FillActiveTimes(tdPrep);
			FillActiveTimes(tdGerent);
			FillActiveTimes(dsStart);
			FillActiveTimes(dtCrash);
			FillActiveTimes(dtSandstorm);

			tt.Cycles.Add(vbDay);
			tt.Cycles.Add(vbNight);
			tt.Cycles.Add(vbNightBosses);
			tt.Cycles.Add(abPillars);
			tt.Cycles.Add(abChallenges);
			tt.Cycles.Add(abOctovine);
			tt.Cycles.Add(abReset);
			tt.Cycles.Add(tdOutposts);
			tt.Cycles.Add(tdPrep);
			tt.Cycles.Add(tdGerent);
			tt.Cycles.Add(dsStart);
			tt.Cycles.Add(dtCrash);
			tt.Cycles.Add(dtSandstorm);

			XmlSerializer serializer = new XmlSerializer(typeof(CycleTimeTable));
			TextWriter textWriter = new StreamWriter(FileName);
			serializer.Serialize(textWriter, tt);
			textWriter.Close();
		}
	}
}