using GW2PAO.Modules.Cycles.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace GW2PAO.Views.Cycles.CycleTracker
{
	/// <summary>
	/// Interaction logic for WorldEventView.xaml
	/// </summary>
	public partial class CycleView : UserControl
	{
		public CycleView()
		{
			InitializeComponent();
		}

		private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				((CycleViewModel)this.DataContext).CopyDataCommand.Execute(null);
			}
		}
	}
}