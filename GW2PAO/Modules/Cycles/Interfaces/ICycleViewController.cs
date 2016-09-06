namespace GW2PAO.Modules.Cycles.Interfaces
{
	public interface ICyclesViewController
	{
		void Initialize();

		void Shutdown();

		void DisplayCyclesTracker();

		bool CanDisplayCyclesTracker();

		void DisplayCycleNotificationsWindow();

		bool CanDisplayCycleNotificationsWindow();
	}
}