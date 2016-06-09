using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2PAO.Modules.Cycles.ViewModels
{
	/// <summary>
	/// View model for an cycle shown by the cycle tracker
	/// </summary>
	public class CycleViewModel : BindableBase
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private EventState state;
		private TimeSpan timeSinceActive;
		private TimeSpan timerValue;
		private bool isVisible;
		private bool isNotificationShown;
		private bool isRemovingNotification;
		private ICollection<CycleViewModel> displayedNotifications;

		/// <summary>
		/// The general cycles-related user settings/data
		/// </summary>
		public CyclesUserData UserData { get; private set; }

		/// <summary>
		/// The primary model object containing the cycle information
		/// </summary>
		public Cycle CycleModel { get; private set; }

		/// <summary>
		/// The cycle's ID
		/// </summary>
		public Guid CycleId { get { return this.CycleModel.ID; } }

		/// <summary>
		/// The cycle's name
		/// </summary>
		public string CycleName { get { return this.CycleModel.Name; } }

		/// <summary>
		/// Name of the zone in which the cycle occurs
		/// </summary>
		public string ZoneName { get { return this.CycleModel.MapName; } }

		///// <summary>
		///// Current state of the cycle
		///// </summary>
		public EventState State
		{
			get { return this.state; }
			set { if (SetProperty(ref this.state, value)) this.RefreshVisibility(); }
		}

		/// <summary>
		/// Depending on the state of the cycle, contains the
		/// 'Time Until Active' or the 'Time Since Active'
		/// </summary>
		public TimeSpan TimerValue
		{
			get { return this.timerValue; }
			set { SetProperty(ref this.timerValue, value); }
		}

		/// <summary>
		/// Time since the cycle was last active
		/// </summary>
		public TimeSpan TimeSinceActive
		{
			get { return this.timeSinceActive; }
			set { SetProperty(ref this.timeSinceActive, value); }
		}

		/// <summary>
		/// Visibility of the cycle
		/// Visibility is based on multiple properties, including:
		///     - EventState and the user configuration for what states are shown
		///     - IsTreasureObtained and whether or not treasure-obtained cycles are shown
		///     - Whether or not the cycle is user-configured as hidden
		/// </summary>
		public bool IsVisible
		{
			get { return this.isVisible; }
			set { SetProperty(ref this.isVisible, value); }
		}

		/// <summary>
		/// True if the notification for this cycle has already been shown, else false
		/// </summary>
		public bool IsNotificationShown
		{
			get { return this.isNotificationShown; }
			set { SetProperty(ref this.isNotificationShown, value); }
		}

		/// <summary>
		/// True if the notification for this cycle is about to be removed, else false
		/// TODO: I hate having this here, but due to a limitation in WPF, there's no reasonable way around this at this time
		/// </summary>
		public bool IsRemovingNotification
		{
			get { return this.isRemovingNotification; }
			set { SetProperty(ref this.isRemovingNotification, value); }
		}

		/// <summary>
		/// Command to hide the cycle
		/// </summary>
		public DelegateCommand HideCommand { get { return new DelegateCommand(this.AddToHiddenEvents); } }

		/// <summary>
		/// Command to copy the nearest waypoint's chat code to the clipboard
		/// </summary>
		public DelegateCommand CopyWaypointCommand { get { return new DelegateCommand(this.CopyWaypointCode); } }

		/// <summary>
		/// Command to copy the information about the cycle to the clipboard
		/// </summary>
		public DelegateCommand CopyDataCommand { get { return new DelegateCommand(this.CopyCycleData); } }

		/// <summary>
		/// Closes the displayed notification
		/// </summary>
		public DelegateCommand CloseNotificationCommand { get { return new DelegateCommand(this.CloseNotification); } }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="cycleData">The cycle's details/data</param>
		/// <param name="userData">Event tracker user data</param>
		/// <param name="displayedNotificationsCollection">Collection of displayed cycle notifications</param>
		public CycleViewModel(Cycle cycleData, CyclesUserData userData, ICollection<CycleViewModel> displayedNotificationsCollection)
		{
			this.CycleModel = cycleData;
			this.UserData = userData;
			this.displayedNotifications = displayedNotificationsCollection;
			this.IsVisible = true;
			this.IsNotificationShown = false;
			this.IsRemovingNotification = false;
			this.State = EventState.Unknown;
			this.TimerValue = TimeSpan.Zero;
			this.UserData.PropertyChanged += (o, e) => this.RefreshVisibility();
			this.UserData.HiddenCycles.CollectionChanged += (o, e) => this.RefreshVisibility();
		}

		/// <summary>
		/// Adds the cycle to the list of hidden cycles
		/// </summary>
		private void AddToHiddenEvents()
		{
			logger.Debug("Adding \"{0}\" to hidden cycles", this.CycleName);
			this.UserData.HiddenCycles.Add(this.CycleModel.ID);
		}

		/// <summary>
		/// Refreshes the visibility of the cycle
		/// </summary>
		private void RefreshVisibility()
		{
			logger.Trace("Refreshing visibility of \"{0}\"", this.CycleName);
			if (this.UserData.HiddenCycles.Any(id => id == this.CycleId))
			{
				this.IsVisible = false;
			} else if (!this.UserData.AreInactiveCyclesVisible
					  && this.State == EventState.Inactive)
			{
				this.IsVisible = false;
			}
			  //else if (!this.UserData.AreCompletedEventsVisible
			  //        && this.IsTreasureObtained)
			  //{
			  //    this.IsVisible = false;
			  //}
			  else
			{
				this.IsVisible = true;
			}
			logger.Trace("IsVisible = {0}", this.IsVisible);
		}

		/// <summary>
		/// Copies the nearest waypoint's chat code to the clipboard
		/// </summary>
		private void CopyWaypointCode()
		{
			logger.Debug("Copying waypoint code of \"{0}\" as \"{1}\"", this.CycleName, this.CycleModel.WaypointCode);
			System.Windows.Clipboard.SetDataObject(this.CycleModel.WaypointCode);
		}

		/// <summary>
		/// Removes this cycle from the collection of displayed notifications
		/// </summary>
		private void CloseNotification()
		{
			this.displayedNotifications.Remove(this);
		}

		/// <summary>
		/// Places a string of data on the clipboard for pasting into the game
		/// Contains the cycle name, status, time until active, waypoint code, etc
		/// </summary>
		private void CopyCycleData()
		{
			string fullText;
			if (this.State == EventState.Active)
			{
				fullText = string.Format("{0} - {1}",
					this.CycleName,
					this.CycleModel.WaypointCode);
			} else
			{
				fullText = string.Format("{0} - {1} {2} - {3}",
					this.CycleName,
					GW2PAO.Properties.Resources.ActiveIn, this.TimerValue.ToString("hh\\:mm\\:ss"),
					this.CycleModel.WaypointCode);
			}

			logger.Debug("Copying \"{0}\" to clipboard", fullText);
			System.Windows.Clipboard.SetDataObject(fullText);
		}
	}
}