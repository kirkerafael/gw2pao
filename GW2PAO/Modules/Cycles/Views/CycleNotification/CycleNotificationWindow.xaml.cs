using GW2PAO.Modules.Cycles.Interfaces;
using GW2PAO.Modules.Cycles.ViewModels.CycleNotification;
using GW2PAO.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GW2PAO.Modules.Cycles.Views.CycleNotification
{
	/// <summary>
	/// Interaction logic for CycleNotificationWindow.xaml
	/// </summary>
	public partial class CycleNotificationWindow : OverlayWindow
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// View model
		/// </summary>
		[Import]
		public CycleNotificationsWindowViewModel ViewModel
		{
			get
			{
				return this.DataContext as CycleNotificationsWindowViewModel;
			}
			set
			{
				this.DataContext = value;
			}
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public CycleNotificationWindow()
		{
			InitializeComponent();
			this.Loaded += (o, e) => this.LoadWindowLocation();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
			e.Handled = true;
			Properties.Settings.Default.CycleNotificationX = this.Left;
			Properties.Settings.Default.CycleNotificationY = this.Top;
			Properties.Settings.Default.Save();
		}

		private void LoadWindowLocation()
		{
			// Set the window location
			if (Properties.Settings.Default.CycleNotificationX == -1
				&& Properties.Settings.Default.CycleNotificationY == -1)
			{
				// Use default location (bottom-right corner)
				this.Left = System.Windows.SystemParameters.WorkArea.Width - 5 - this.ActualWidth;
				this.Top = System.Windows.SystemParameters.WorkArea.Height - 5 - this.ActualHeight;
			} else
			{
				// Use saved location
				this.Left = Properties.Settings.Default.CycleNotificationX;
				this.Top = Properties.Settings.Default.CycleNotificationY;
			}
		}
	}
}