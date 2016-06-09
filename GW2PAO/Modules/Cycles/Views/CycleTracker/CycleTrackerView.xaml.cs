using GW2PAO.Modules.Cycles.ViewModels.CycleTracker;
using GW2PAO.Views;
using GW2PAO.Views.Cycles.CycleTracker;
using NLog;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GW2PAO.Modules.Cycles.Views.CycleTracker
{
	/// <summary>
	/// Interaction logic for CycleTrackerView.xaml
	/// </summary>
	public partial class CycleTrackerView : OverlayWindow
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Actual height of an event in the list
		/// </summary>
		private double eventHeight;

		/// <summary>
		/// Count used for keeping track of when we need to adjust our
		/// maximum height/width if the number of visible events
		/// changes
		/// </summary>
		private int prevVisibleCyclesCount = 0;

		/// <summary>
		/// Height before collapsing the control
		/// </summary>
		private double beforeCollapseHeight;

		/// <summary>
		/// View model
		/// </summary>
		[Import]
		public CycleTrackerViewModel ViewModel
		{
			get
			{
				return this.DataContext as CycleTrackerViewModel;
			}
			set
			{
				this.DataContext = value;
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public CycleTrackerView()
		{
			logger.Debug("New CycleTrackerView created");
			InitializeComponent();

			this.eventHeight = new CycleView().Height;

			this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);
			this.Loaded += CycleTrackerView_Loaded;
		}

		private void CycleTrackerView_Loaded(object sender, RoutedEventArgs e)
		{
			// Set up resize snapping
			this.ResizeHelper.SnappingHeightOffset = 12;
			this.ResizeHelper.SnappingThresholdHeight = (int)this.TitleBar.ActualHeight;
			this.ResizeHelper.SnappingIncrementHeight = (int)this.eventHeight;

			// Save the height values for use when collapsing the window
			this.RefreshWindowHeights();
			this.Height = GW2PAO.Properties.Settings.Default.CycleTrackerHeight;

			this.CyclesContainer.LayoutUpdated += CyclesContainer_LayoutUpdated;
			this.Closing += CycleTrackerView_Closing;
			this.beforeCollapseHeight = this.Height;
		}

		/// <summary>
		/// Refreshes the MinHeight and MaxHeight of the window
		/// based on collapsed status and number of visible items
		/// </summary>
		private void RefreshWindowHeights()
		{
			var visibleObjsCount = this.ViewModel.Cycles.Count(o => o.IsVisible);
			if (this.CyclesContainer.Visibility == System.Windows.Visibility.Visible)
			{
				// Expanded
				this.MinHeight = eventHeight * 2 + this.TitleBar.ActualHeight; // Minimum of 2 events
				if (visibleObjsCount < 2)
					this.MaxHeight = this.MinHeight;
				else
					this.MaxHeight = (visibleObjsCount * eventHeight) + this.TitleBar.ActualHeight + 2;
			} else
			{
				// Collapsed, don't touch the height
			}
		}

		private void CyclesContainer_LayoutUpdated(object sender, EventArgs e)
		{
			var visibleObjsCount = this.ViewModel.Cycles.Count(o => o.IsVisible);
			if (prevVisibleCyclesCount != visibleObjsCount)
			{
				prevVisibleCyclesCount = visibleObjsCount;
				this.RefreshWindowHeights();
			}
		}

		private void CycleTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Normal)
			{
				if (this.CyclesContainer.Visibility == System.Windows.Visibility.Visible)
				{
					Properties.Settings.Default.CycleTrackerHeight = this.Height;
				} else
				{
					Properties.Settings.Default.CycleTrackerHeight = this.beforeCollapseHeight;
				}
				Properties.Settings.Default.CycleTrackerWidth = this.Width;
				Properties.Settings.Default.CycleTrackerX = this.Left;
				Properties.Settings.Default.CycleTrackerY = this.Top;
				Properties.Settings.Default.Save();
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			logger.Debug("CycleTrackerView closed");
		}

		private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
			e.Handled = true;
		}

		private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.CyclesContainer.Visibility == System.Windows.Visibility.Visible)
			{
				this.beforeCollapseHeight = this.Height;
				this.MinHeight = this.TitleBar.ActualHeight;
				this.MaxHeight = this.TitleBar.ActualHeight;
				this.Height = this.TitleBar.ActualHeight;
				this.CyclesContainer.Visibility = System.Windows.Visibility.Collapsed;
			} else
			{
				this.CyclesContainer.Visibility = System.Windows.Visibility.Visible;
				this.RefreshWindowHeights();
				this.Height = this.beforeCollapseHeight;
			}
		}

		private void TitleImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				Image image = sender as Image;
				ContextMenu contextMenu = image.ContextMenu;
				contextMenu.PlacementTarget = image;
				contextMenu.IsOpen = true;
				e.Handled = true;
			}
		}
	}
}