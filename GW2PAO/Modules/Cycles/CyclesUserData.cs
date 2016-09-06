using GW2PAO.Data.UserData;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GW2PAO.Modules.Cycles
{
	/// <summary>
	/// User settings for the Events Tracker and Event Notifications
	/// </summary>
	[Serializable]
	public class CyclesUserData : UserData<CyclesUserData>
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// The default settings filename
		/// </summary>
		public const string Filename = "CyclesUserData.xml";

		private bool areInactiveCyclesVisible;
		private bool areResetCyclesVisible;
		private bool areBuildupCyclesVisible;
		private bool areBossWarmupCyclesVisible;
		private bool areBossCyclesVisible;
		private bool showWaypointCopyButtons;
		private bool areCycleNotificationsEnabled;
		private uint notificationDuration;
		private DateTime lastResetDateTime;
		private ObservableCollection<Guid> hiddenCycles = new ObservableCollection<Guid>();
		private ObservableCollection<CycleNotificationSettings> notificationSettings = new ObservableCollection<CycleNotificationSettings>();

		/// <summary>
		/// True if inactive events are visible, else false
		/// </summary>
		public bool AreInactiveCyclesVisible
		{
			get { return this.areInactiveCyclesVisible; }
			set { SetProperty(ref this.areInactiveCyclesVisible, value); }
		}

		public bool AreResetCyclesVisible
		{
			get { return this.areResetCyclesVisible; }
			set { SetProperty(ref this.areResetCyclesVisible, value); }
		}

		public bool AreBuildupCyclesVisible
		{
			get { return this.areBuildupCyclesVisible; }
			set { SetProperty(ref this.areBuildupCyclesVisible, value); }
		}

		public bool AreBossWarmupCyclesVisible
		{
			get { return this.areBossWarmupCyclesVisible; }
			set { SetProperty(ref this.areBossWarmupCyclesVisible, value); }
		}

		public bool AreBossCyclesVisible
		{
			get { return this.areBossCyclesVisible; }
			set { SetProperty(ref this.areBossCyclesVisible, value); }
		}

		/// <summary>
		/// True if the waypoint-code copy buttons are visible in the events tracker, else false
		/// </summary>
		public bool ShowWaypointCopyButtons
		{
			get { return this.showWaypointCopyButtons; }
			set { SetProperty(ref this.showWaypointCopyButtons, value); }
		}

		/// <summary>
		/// True if event notifications are enabled, else false
		/// </summary>
		public bool AreCycleNotificationsEnabled
		{
			get { return this.areCycleNotificationsEnabled; }
			set { SetProperty(ref this.areCycleNotificationsEnabled, value); }
		}

		/// <summary>
		/// The amount of time to display notifications, in seconds
		/// </summary>
		public uint NotificationDuration
		{
			get { return this.notificationDuration; }
			set { SetProperty(ref this.notificationDuration, value); }
		}

		/// <summary>
		/// The last recorded server-reset date/time
		/// </summary>
		public DateTime LastResetDateTime
		{
			get { return this.lastResetDateTime; }
			set { SetProperty(ref this.lastResetDateTime, value); }
		}

		/// <summary>
		/// Collection of user-configured Hidden Events
		/// </summary>
		public ObservableCollection<Guid> HiddenCycles { get { return this.hiddenCycles; } }

		/// <summary>
		/// Collection of notification settings
		/// </summary>
		public ObservableCollection<CycleNotificationSettings> NotificationSettings { get { return this.notificationSettings; } }

		/// <summary>
		/// Default constructor
		/// </summary>
		public CyclesUserData()
		{
			this.AreInactiveCyclesVisible = true;
			this.AreResetCyclesVisible = true;
			this.AreBuildupCyclesVisible = true;
			this.AreBossWarmupCyclesVisible = true;
			this.AreBossCyclesVisible = true;
			this.ShowWaypointCopyButtons = true;
			this.AreCycleNotificationsEnabled = true;
			this.NotificationDuration = 10;
			this.LastResetDateTime = DateTime.UtcNow;
		}

		/// <summary>
		/// Enables auto-save of settings. If called, whenever a setting is changed, this settings object will be saved to disk
		/// </summary>
		public override void EnableAutoSave()
		{
			logger.Info("Enabling auto save");
			this.PropertyChanged += (o, e) => CyclesUserData.SaveData(this, CyclesUserData.Filename);
			this.HiddenCycles.CollectionChanged += (o, e) => CyclesUserData.SaveData(this, CyclesUserData.Filename);
			this.NotificationSettings.CollectionChanged += (o, e) =>
				{
					CyclesUserData.SaveData(this, CyclesUserData.Filename);
					if (e.Action == NotifyCollectionChangedAction.Add)
					{
						foreach (CycleNotificationSettings ens in e.NewItems)
						{
							ens.PropertyChanged += (obj, arg) => CyclesUserData.SaveData(this, CyclesUserData.Filename);
						}
					}
				};
			foreach (CycleNotificationSettings ens in this.NotificationSettings)
			{
				ens.PropertyChanged += (obj, arg) => CyclesUserData.SaveData(this, CyclesUserData.Filename);
			}
		}
	}
}