using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Xml.Serialization;

namespace GW2PAO.Modules.Cycles
{
	/// <summary>
	/// Class containing the configuration for an event's notification settings
	/// </summary>
	[Serializable]
	public class CycleNotificationSettings : BindableBase
	{
		private string cycleName;
		private bool isNotificationEnabled;

		/// <summary>
		/// ID of the event
		/// </summary>
		public Guid CycleID
		{
			get;
			set;
		}

		/// <summary>
		/// Name of the Event
		/// I'd prefer not to duplicate this, but I have no alternative at the moment
		/// </summary>
		public string CycleName
		{
			get { return this.cycleName; }
			set { this.SetProperty(ref this.cycleName, value); }
		}

		/// <summary>
		/// True if the notification is enabled, else false
		/// </summary>
		public bool IsNotificationEnabled
		{
			get { return this.isNotificationEnabled; }
			set { this.SetProperty(ref this.isNotificationEnabled, value); }
		}

		/// <summary>
		/// The amount of time before an event that a notification should be shown
		/// </summary>
		public SerializableTimespan TimeToNotify
		{
			get;
			set;
		}

		[XmlIgnore]
		public TimeSpan MaxNotificationInterval { get { return TimeSpan.FromHours(1); } }

		[XmlIgnore]
		public TimeSpan MinNotificationInterval { get { return TimeSpan.Zero; } }

		[XmlIgnore]
		public TimeSpan NotificationInterval
		{
			get { return this.TimeToNotify.Time; }
			set { this.SetProperty(ref this.TimeToNotify.Time, value); }
		}

		/// <summary>
		/// Parameter-less constructor for serialization purposes
		/// </summary>
		public CycleNotificationSettings()
		{
			this.TimeToNotify = new SerializableTimespan();
		}

		/// <summary>
		/// Default constructor, initializes an CycleNotificationSettings object with defaults
		/// </summary>
		/// <param name="cycleId"></param>
		public CycleNotificationSettings(Guid cycleId)
		{
			this.CycleID = cycleId;
			this.TimeToNotify = new SerializableTimespan();
			this.NotificationInterval = TimeSpan.FromMinutes(1);
			this.IsNotificationEnabled = true;
		}
	}
}