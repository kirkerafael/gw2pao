using GW2PAO.Infrastructure;
using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Infrastructure.ViewModels;
using GW2PAO.Modules.Cycles.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace GW2PAO.Modules.Cycles
{
	[Export(typeof(IMenuItem))]
	[ExportMetadata("Order", 1)]
	public class CyclesMenu : IMenuItem
	{
		/// <summary>
		/// Collection of submenu objects
		/// </summary>
		public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

		/// <summary>
		/// Header text of the menu item
		/// </summary>
		public string Header
		{
			get { return Properties.Resources.Cycles; }
		}

		/// <summary>
		/// True if the menu item is checkable, else false
		/// </summary>
		public bool IsCheckable
		{
			get { return false; }
		}

		/// <summary>
		/// True if the menu item is checked, else false
		/// </summary>
		public bool IsChecked
		{
			get { return false; }
			set { }
		}

		/// <summary>
		/// True if the menu item does not close the menu on click, else false
		/// </summary>
		public bool StaysOpen
		{
			get { return false; }
		}

		/// <summary>
		/// The on-click command
		/// </summary>
		public ICommand OnClickCommand { get; private set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		[ImportingConstructor]
		public CyclesMenu(ICyclesViewController viewFactory, CyclesUserData userData)
		{
			this.SubMenuItems = new ObservableCollection<IMenuItem>();
			this.SubMenuItems.Add(new MenuItem(Properties.Resources.OpenCyclesTracker, viewFactory.DisplayCyclesTracker, viewFactory.CanDisplayCyclesTracker));
			this.SubMenuItems.Add(new CheckableMenuItem(Properties.Resources.CycleNotifications, false, () => userData.AreCycleNotificationsEnabled, userData));
			this.SubMenuItems.Add(new MenuItem(Properties.Resources.Configure, () => Commands.OpenCycleSettingsCommand.Execute(null)));
		}
	}
}