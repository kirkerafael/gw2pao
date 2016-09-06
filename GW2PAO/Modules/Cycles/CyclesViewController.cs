using GW2PAO.Infrastructure;
using GW2PAO.Modules.Cycles.Interfaces;
using GW2PAO.Modules.Cycles.Views.CycleNotification;
using GW2PAO.Modules.Cycles.Views.CycleTracker;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Commands;
using NLog;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace GW2PAO.Modules.Cycles
{
	[Export(typeof(ICyclesViewController))]
	public class CyclesViewController : ICyclesViewController
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Composition container of composed parts
		/// </summary>
		[Import]
		private CompositionContainer Container { get; set; }

		/// <summary>
		/// The cycles tracker view
		/// </summary>
		private CycleTrackerView cycleTrackerView;

		/// <summary>
		/// The cycle notifications window containing all cycle notifications
		/// </summary>
		private CycleNotificationWindow cycleNotificationsView;

		/// <summary>
		/// Displays all previously-opened windows and other windows
		/// that must be shown at startup
		/// </summary>
		public void Initialize()
		{
			logger.Debug("Initializing");

			logger.Debug("Registering hotkey commands");
			HotkeyCommands.ToggleCycleTrackerCommand.RegisterCommand(new DelegateCommand(this.ToggleCyclesTracker));

			Threading.BeginInvokeOnUI(() =>
			{
				if (Properties.Settings.Default.IsCycleTrackerOpen && this.CanDisplayCyclesTracker())
					this.DisplayCyclesTracker();

				if (this.CanDisplayCycleNotificationsWindow())
					this.DisplayCycleNotificationsWindow();
			});
		}

		/// <summary>
		/// Closes all windows and saves the "was previously opened" state for those windows.
		/// </summary>
		public void Shutdown()
		{
			logger.Debug("Shutting down");

			if (this.cycleTrackerView != null)
			{
				Properties.Settings.Default.IsCycleTrackerOpen = this.cycleTrackerView.IsVisible;
				Threading.InvokeOnUI(() => this.cycleTrackerView.Close());
			}

			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Displays the Cycle Tracker window, or, if already displayed, sets
		/// focus to the window
		/// </summary>
		public void DisplayCyclesTracker()
		{
			if (this.cycleTrackerView == null || !this.cycleTrackerView.IsVisible)
			{
				this.cycleTrackerView = new CycleTrackerView();
				this.Container.ComposeParts(this.cycleTrackerView);
				this.cycleTrackerView.Show();
			} else
			{
				this.cycleTrackerView.Focus();
			}
		}

		/// <summary>
		/// Determines if the cycle tracker can be displayed
		/// </summary>
		/// <returns>Always true</returns>
		public bool CanDisplayCyclesTracker()
		{
			return true;
		}

		/// <summary>
		/// Displays the Cycle Notifications window
		/// </summary>
		public void DisplayCycleNotificationsWindow()
		{
			this.cycleNotificationsView = new CycleNotificationWindow();
			this.Container.ComposeParts(this.cycleNotificationsView);
			this.cycleNotificationsView.Show();
		}

		/// <summary>
		/// Determines if the Cycle Notifications window can be displayed
		/// </summary>
		/// <returns>Always true</returns>
		public bool CanDisplayCycleNotificationsWindow()
		{
			return true;
		}

		/// <summary>
		/// Toggles whether or not the cycles tracker is visible
		/// </summary>
		private void ToggleCyclesTracker()
		{
			if (this.cycleTrackerView == null || !this.cycleTrackerView.IsVisible)
			{
				this.DisplayCyclesTracker();
			} else
			{
				this.cycleTrackerView.Close();
			}
		}
	}
}