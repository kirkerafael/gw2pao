using NLog;
using System.Windows.Controls;

namespace GW2PAO.Modules.Cycles.Views.CycleNotification
{
	/// <summary>
	/// Interaction logic for CycleNotificationView.xaml
	/// </summary>
	public partial class CycleNotificationView : UserControl
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Default constructor
		/// </summary>
		public CycleNotificationView()
		{
			InitializeComponent();
		}
	}
}