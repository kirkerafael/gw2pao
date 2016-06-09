using GW2PAO.Modules.Cycles.ViewModels;
using System.Collections.ObjectModel;

namespace GW2PAO.Modules.Cycles.Interfaces
{
	public interface ICyclesController
	{
		/// <summary>
		/// The collection of World Cycles
		/// </summary>
		ObservableCollection<CycleViewModel> Cycles { get; }

		/// <summary>
		/// The collection of events for event notifications
		/// </summary>
		ObservableCollection<CycleViewModel> CycleNotifications { get; }

		/// <summary>
		/// The interval by which to refresh events (in ms)
		/// </summary>
		int CycleRefreshInterval { get; set; }

		/// <summary>
		/// The event tracker user data
		/// </summary>
		CyclesUserData UserData { get; }

		/// <summary>
		/// Starts the controller
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the controller
		/// </summary>
		void Stop();

		/// <summary>
		/// Forces a shutdown of the controller, including all running timers/threads
		/// </summary>
		void Shutdown();
	}
}