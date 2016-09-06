using GW2PAO.Modules.Cycles.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace GW2PAO.Modules.Cycles.Views
{
	/// <summary>
	/// Interaction logic for CycleSettings.xaml
	/// </summary>
	[Export(typeof(CycleSettingsView))]
	public partial class CycleSettingsView : UserControl
	{
		[Import]
		public CycleSettingsViewModel ViewModel
		{
			get { return this.DataContext as CycleSettingsViewModel; }
			set { this.DataContext = value; }
		}

		public CycleSettingsView()
		{
			InitializeComponent();
		}
	}
}