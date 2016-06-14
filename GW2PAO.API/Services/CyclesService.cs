using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Providers;
using GW2PAO.API.Services.Interfaces;
using NLog;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace GW2PAO.API.Services
{
	/// <summary>
	/// Service class for event information
	/// </summary>
	[Export(typeof(ICyclesService))]
	public class CyclesService : ICyclesService
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// String provider for world event names
		/// </summary>
		//private IStringProvider<Guid> cyclesStringProvider;

		/// <summary>
		/// Helper class for retrieving the current system time
		/// </summary>
		private ITimeProvider timeProvider;

		/// <summary>
		/// Default constructor
		/// </summary>
		public CyclesService()
		{
			//this.cyclesStringProvider = new WorldEventNamesProvider();
			this.timeProvider = new DefaultTimeProvider();
		}

		/// <summary>
		/// Alternate constructor
		/// </summary>
		/// <param name="currentTimeProvider">A time provider for determining the current name. If null, the EventsServer will use the DefaultTimeProvider</param>
		public CyclesService(/*IStringProvider<Guid> worldEventNamesProvider, */ITimeProvider currentTimeProvider)
		{
			//this.worldEventStringProvider = worldEventNamesProvider;
			this.timeProvider = currentTimeProvider;
		}

		/// <summary>
		/// The World Events time table
		/// </summary>
		public CycleTimeTable TimeTable { get; private set; }

		/// <summary>
		/// Loads the events time table and initializes all cached event information
		/// </summary>
		public void LoadTable()
		{
			logger.Info("Loading Event Time Table");
			try
			{
				this.TimeTable = CycleTimeTable.LoadTable();
			} catch (Exception ex)
			{
				logger.Error(ex);
				logger.Info("Error loading Event Time Table, re-creating table");

				CycleTimeTable.CreateTable();
				this.TimeTable = CycleTimeTable.LoadTable();
			}
		}

		/// <summary>
		/// Returns the localized name for the given event
		/// </summary>
		/// <param name="id">ID of the event to return the name of</param>
		/// <returns>The localized name</returns>
		//public string GetLocalizedName(Guid id)
		//{
		//    string evtName = this.worldEventStringProvider.GetString(id);
		//    if (string.IsNullOrEmpty(evtName))
		//    {
		//        var allNames = GW2.V1.EventNames.ForCurrentUICulture().FindAll();
		//        return allNames[id].Name;
		//    }
		//    return evtName;
		//}

		/// <summary>
		/// Retrieves the current state of the given event
		/// </summary>
		/// <param name="id">The ID of the event to retrieve the state of</param>
		/// <returns>The current state of the input event</returns>
		public Data.Enums.EventState GetState(Guid id)
		{
			if (this.TimeTable.Cycles.Any(c => c.ID == id))
			{
				Cycle cycle = this.TimeTable.Cycles.First(c => c.ID == id);
				return this.GetState(cycle);
			} else
			{
				return Data.Enums.EventState.Unknown;
			}
		}

		/// <summary>
		/// Retrieves the current state of the given event
		/// </summary>
		/// <param name="evt">The event to retrieve the state of</param>
		/// <returns>The current state of the input event</returns>
		public Data.Enums.EventState GetState(Cycle c)
		{
			var state = Data.Enums.EventState.Unknown;
			if (c != null)
			{
				var timeUntilActive = this.GetTimeUntilActive(c);
				var timeSinceActive = this.GetTimeSinceActive(c);

				if (timeSinceActive >= TimeSpan.FromTicks(0)
					&& timeSinceActive < c.Length.Time)
				{
					state = Data.Enums.EventState.Active;
				} else if (timeUntilActive <= TimeSpan.FromMinutes(15))
				{
					state = Data.Enums.EventState.Preparation;
				} else
				{
					state = Data.Enums.EventState.Inactive;
				}
			}

			return state;
		}

		/// <summary>
		/// Retrieves the amount of time until the next active time for the given event, using the megaserver timetables
		/// </summary>
		/// <param name="evt">The event to retrieve the time for</param>
		/// <returns>Timespan containing the amount of time until the event is next active</returns>
		public TimeSpan GetTimeUntilActive(Cycle evt)
		{
			TimeSpan timeUntilActive = TimeSpan.MinValue;
			if (evt != null)
			{
				// Find the next time
				var nextTime = evt.ActiveTimes.FirstOrDefault(activeTime => (activeTime.Time - this.timeProvider.CurrentTime.TimeOfDay) >= TimeSpan.FromSeconds(0));

				// If there is no next time, then take the first time
				if (nextTime == null)
				{
					nextTime = evt.ActiveTimes.FirstOrDefault();
					if (nextTime != null)
						timeUntilActive = (nextTime.Time + TimeSpan.FromHours(24) - this.timeProvider.CurrentTime.TimeOfDay);
				} else
				{
					// Calculate the number of seconds until the next time
					timeUntilActive = nextTime.Time - this.timeProvider.CurrentTime.TimeOfDay;
				}
			}
			return timeUntilActive;
		}

		/// <summary>
		/// Retrieves the amount of time since the last active time for the given event, using the megaserver timetables
		/// </summary>
		/// <param name="evt">The event to retrieve the time for</param>
		/// <returns>Timespan containing the amount of time since the event was last active</returns>
		public TimeSpan GetTimeSinceActive(Cycle evt)
		{
			TimeSpan timeSinceActive = TimeSpan.MinValue;
			if (evt != null)
			{
				// Find the next time
				var lastTime = evt.ActiveTimes.LastOrDefault(activeTime => (this.timeProvider.CurrentTime.TimeOfDay - activeTime.Time) >= TimeSpan.FromSeconds(0));

				// If there is no next time, then take the first time
				if (lastTime == null)
				{
					lastTime = evt.ActiveTimes.FirstOrDefault();
					if (lastTime != null)
						timeSinceActive = (this.timeProvider.CurrentTime.TimeOfDay - lastTime.Time) + TimeSpan.FromHours(24);
				} else
				{
					// Calculate the number of seconds until the next time
					timeSinceActive = this.timeProvider.CurrentTime.TimeOfDay - lastTime.Time;
				}
			}
			return timeSinceActive;
		}
	}
}