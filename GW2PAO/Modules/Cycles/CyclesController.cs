using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Cycles.Interfaces;
using GW2PAO.Modules.Cycles.ViewModels;
using GW2PAO.Utility;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Cycles
{
	/// <summary>
	/// The Cycles controller. Handles refresh of cycles, including state and timer values
	/// Also handles notifications and notification states
	/// </summary>
	[Export(typeof(ICyclesController))]
	public class CyclesController : ICyclesController
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Service responsible for Cycle information
		/// </summary>
		private ICyclesService cyclesService;

		/// <summary>
		/// Service responsible for Zone information
		/// </summary>
		private IZoneService zoneService;

		/// <summary>
		/// Service that provides player location information
		/// </summary>
		private IPlayerService playerService;

		/// <summary>
		/// The primary cycle refresh timer object
		/// </summary>
		private Timer cycleRefreshTimer;

		/// <summary>
		/// Locking object for operations performed with the refresh timer
		/// </summary>
		private readonly object refreshTimerLock = new object();

		/// <summary>
		/// True if the controller's timers are no longer running, else false
		/// </summary>
		private bool isStopped;

		/// <summary>
		/// The user data
		/// </summary>
		private CyclesUserData userData;

		/// <summary>
		/// Keeps track of how many times Start() has been called in order
		/// to support reuse of a single object
		/// </summary>
		private int startCallCount;

		/// <summary>
		/// Backing store of the World Cycles collection
		/// </summary>
		private ObservableCollection<CycleViewModel> cycles = new ObservableCollection<CycleViewModel>();

		/// <summary>
		/// The collection of World Cycles
		/// </summary>
		public ObservableCollection<CycleViewModel> Cycles { get { return this.cycles; } }

		/// <summary>
		/// Backing store of the Cycle Notifications collection
		/// </summary>
		private ObservableCollection<CycleViewModel> cycleNotifications = new ObservableCollection<CycleViewModel>();

		/// <summary>
		/// The collection of cycles for cycle notifications
		/// </summary>
		public ObservableCollection<CycleViewModel> CycleNotifications { get { return this.cycleNotifications; } }

		/// <summary>
		/// The interval by which to refresh cycles (in ms)
		/// </summary>
		public int CycleRefreshInterval { get; set; }

		/// <summary>
		/// The cycle tracker user data
		/// </summary>
		public CyclesUserData UserData { get { return this.userData; } }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="cyclesService">The cycles service</param>
		/// <param name="userData">The user settings</param>
		[ImportingConstructor]
		public CyclesController(ICyclesService cyclesService, IZoneService zoneService, IPlayerService playerService, CyclesUserData userData)
		{
			logger.Debug("Initializing Cycle Tracker Controller");
			this.cyclesService = cyclesService;
			this.zoneService = zoneService;
			this.playerService = playerService;
			this.isStopped = false;

			this.userData = userData;

			// Initialize the refresh timer
			this.cycleRefreshTimer = new Timer(this.RefreshCycles);
			this.CycleRefreshInterval = 1000;

			// Initialize the start call count to 0
			this.startCallCount = 0;

			// Set up handling of the cycle settings UseAdjustedTable property changed so that we can load the correct table when it changes
			this.UserData.PropertyChanged += UserData_PropertyChanged;

			// Initialize the WorldCycles collection
			this.InitializeCycles();

			// Do this on a background thread since it takes awhile
			Task.Factory.StartNew(this.InitializeCycleZoneNames);

			logger.Info("Cycle Tracker Controller initialized");
		}

		/// <summary>
		/// Starts the automatic refresh
		/// </summary>
		public void Start()
		{
			logger.Debug("Start called");
			Task.Factory.StartNew(() =>
			{
				// Start the timer if this is the first time that Start() has been called
				if (this.startCallCount == 0)
				{
					this.isStopped = false;
					logger.Debug("Starting refresh timers");
					this.RefreshCycles();
				}

				this.startCallCount++;
				logger.Debug("startCallCount = {0}", this.startCallCount);
			}, TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Stops the automatic refresh
		/// </summary>
		public void Stop()
		{
			this.startCallCount--;
			logger.Debug("Stop called - startCallCount = {0}", this.startCallCount);

			// Stop the refresh timer if all calls to Start() have had a matching call to Stop()
			if (this.startCallCount == 0)
			{
				logger.Debug("Stopping refresh timers");
				lock (refreshTimerLock)
				{
					this.isStopped = true;
					this.cycleRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
				}
			}
		}

		/// <summary>
		/// Forces a shutdown of the controller, including all running timers/threads
		/// </summary>
		public void Shutdown()
		{
			logger.Debug("Shutdown called");
			logger.Debug("Stopping refresh timers");
			lock (this.refreshTimerLock)
			{
				this.isStopped = true;
				this.cycleRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		/// <summary>
		/// Initializes the collection of world cycles
		/// </summary>
		private void InitializeCycles()
		{
			lock (refreshTimerLock)
			{
				logger.Debug("Initializing world events");
				this.cyclesService.LoadTable();

				Threading.InvokeOnUI(() =>
				{
					foreach (var cycle in this.cyclesService.TimeTable.Cycles)
					{
						//logger.Debug("Loading localized name for {0}", cycle.ID);
						//cycle.Name = this.cyclesService.GetLocalizedName(cycle.ID);

						logger.Debug("Initializing view model for {0}", cycle.ID);
						this.Cycles.Add(new CycleViewModel(cycle, this.userData, this.CycleNotifications));

						// If the user data does not contain this cycle, add it to that collection as well
						var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.CycleID == cycle.ID);
						if (ens == null)
						{
							this.UserData.NotificationSettings.Add(new CycleNotificationSettings(cycle.ID)
							{
								CycleName = cycle.Name
							});
						} else
						{
							ens.CycleName = cycle.Name;
						}
					}
				});
			}
		}

		private void InitializeCycleZoneNames()
		{
			this.zoneService.Initialize();
			foreach (var cycle in this.cyclesService.TimeTable.Cycles)
			{
				logger.Debug("Loading localized zone location for {0}", cycle.ID);
				var name = this.zoneService.GetZoneName(cycle.WorldMapID);
				Threading.BeginInvokeOnUI(() =>
				{
					cycle.MapName = name;
				});
			}
		}

		/// <summary>
		/// Refreshes all cycles within the cycles collection
		/// This is the primary function of the CycleTrackerController
		/// </summary>
		private void RefreshCycles(object state = null)
		{
			lock (refreshTimerLock)
			{
				if (this.isStopped)
					return; // Immediately return if we are supposed to be stopped

				// Refresh the state of all world cycles
				foreach (var cycle in this.Cycles)
				{
					var newState = this.cyclesService.GetState(cycle.CycleModel);
					Threading.BeginInvokeOnUI(() => cycle.State = newState);

					var timeUntilActive = this.cyclesService.GetTimeUntilActive(cycle.CycleModel);
					var timeSinceActive = this.cyclesService.GetTimeSinceActive(cycle.CycleModel);

					Threading.BeginInvokeOnUI(() => cycle.TimeSinceActive = timeSinceActive);

					if (newState == API.Data.Enums.EventState.Active)
					{
						Threading.BeginInvokeOnUI(() => cycle.TimerValue = timeSinceActive.Negate());
					} else
					{
						Threading.BeginInvokeOnUI(() => cycle.TimerValue = timeUntilActive);

						// Check to see if we need to display a notification for this cycle
						var ens = this.UserData.NotificationSettings.FirstOrDefault(ns => ns.CycleID == cycle.CycleId);
						if (ens != null)
						{
							if (ens.IsNotificationEnabled
								&& timeUntilActive.CompareTo(ens.NotificationInterval) < 0)
							{
								if (!cycle.IsNotificationShown)
								{
									cycle.IsNotificationShown = true;
									this.DisplayCycleNotification(cycle);
								}
							} else
							{
								// Reset the IsNotificationShown state
								cycle.IsNotificationShown = false;
							}
						}
					}
				}

				this.cycleRefreshTimer.Change(this.CycleRefreshInterval, Timeout.Infinite);
			}
		}

		/// <summary>
		/// Adds an cycle to the cycle notifications collection, and then removes the cycle 10 seconds later
		/// </summary>
		private void DisplayCycleNotification(CycleViewModel cycleData)
		{
			const int SLEEP_TIME = 250;

			if (this.UserData.AreCycleNotificationsEnabled)
			{
				if (!this.CycleNotifications.Contains(cycleData))
				{
					Task.Factory.StartNew(() =>
					{
						logger.Info("Displaying notification for \"{0}\"", cycleData.CycleName);
						Threading.BeginInvokeOnUI(() => this.CycleNotifications.Add(cycleData));

						if (this.UserData.NotificationDuration > 0)
						{
							// For X seconds, loop and sleep, with checks to see if notifications have been disabled
							for (int i = 0; i < (this.UserData.NotificationDuration * 1000 / SLEEP_TIME); i++)
							{
								System.Threading.Thread.Sleep(SLEEP_TIME);
								if (!this.UserData.AreCycleNotificationsEnabled)
								{
									logger.Debug("Removing notification for \"{0}\"", cycleData.CycleName);
									Threading.BeginInvokeOnUI(() => this.CycleNotifications.Remove(cycleData));
								}
							}

							logger.Debug("Removing notification for \"{0}\"", cycleData.CycleName);

							// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
							// This makes it so that the notifications can fade out before they are removed from the notification window
							Threading.BeginInvokeOnUI(() => cycleData.IsRemovingNotification = true);
							System.Threading.Thread.Sleep(SLEEP_TIME);
							Threading.BeginInvokeOnUI(() =>
							{
								this.CycleNotifications.Remove(cycleData);
								cycleData.IsRemovingNotification = false;
							});
						}
					});
				}
			}
		}

		/// <summary>
		/// Handles the PropertyChanged cycle for the CycleSettings
		/// </summary>
		private void UserData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//if (e.PropertyName == "UseAdjustedTimeTable")
			//{
			//	Task.Factory.StartNew(() =>
			//	{
			//		// Load a different table
			//		lock (refreshTimerLock)
			//		{
			//			this.cyclesService.LoadTable();
			//			Threading.InvokeOnUI(() =>
			//			{
			//				foreach (var cycle in this.Cycles)
			//				{
			//					var newData = this.cyclesService.TimeTable.Cycles.FirstOrDefault(evt => evt.ID == cycle.CycleId);
			//					cycle.CycleModel.ActiveTimes = newData.ActiveTimes;
			//					cycle.CycleModel.Duration = newData.Duration;
			//					cycle.CycleModel.WarmupDuration = newData.WarmupDuration;
			//				}
			//			});
			//		}
			//	});
			//}
		}
	}
}