using GW2PAO.Modules.Cycles.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace GW2PAO.Modules.Cycles.ViewModels.CycleNotification
{
	[Export(typeof(CycleNotificationsWindowViewModel))]
	public class CycleNotificationsWindowViewModel : BindableBase
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// The events controller
		/// </summary>
		private ICyclesController controller;

		/// <summary>
		/// Collection of active event notifications
		/// </summary>
		public ObservableCollection<CycleViewModel> CycleNotifications { get { return this.controller.CycleNotifications; } }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="controller">The events controller</param>
		[ImportingConstructor]
		public CycleNotificationsWindowViewModel(ICyclesController controller)
		{
			this.controller = controller;
		}
	}
}