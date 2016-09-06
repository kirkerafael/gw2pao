using GW2PAO.Infrastructure.Interfaces;
using GW2PAO.Properties;
using Microsoft.Practices.Prism.Mvvm;
using System.ComponentModel.Composition;

namespace GW2PAO.Modules.Cycles.ViewModels
{
	[Export(typeof(CycleSettingsViewModel))]
	public class CycleSettingsViewModel : BindableBase, ISettingsViewModel
	{
		/// <summary>
		/// Heading for the settings view
		/// </summary>
		public string SettingsHeader
		{
			get { return Resources.Cycles; }
		}

		/// <summary>
		/// The event user data
		/// </summary>
		public CyclesUserData UserData
		{
			get;
			private set;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="userData">The events user data</param>
		[ImportingConstructor]
		public CycleSettingsViewModel(CyclesUserData userData)
		{
			this.UserData = userData;
		}
	}
}