using GW2PAO.Infrastructure;
using GW2PAO.Modules.Cycles.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using NLog;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace GW2PAO.Modules.Cycles
{
	[ModuleExport(typeof(CyclesModule))]
	public class CyclesModule : IModule
	{
		/// <summary>
		/// Default logger
		/// </summary>
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Composition container of composed parts
		/// </summary>
		[Import]
		private CompositionContainer container { get; set; }

		/// <summary>
		/// Cycles controller
		/// </summary>
		private ICyclesController cyclesController;

		/// <summary>
		/// Factory object responsible for displaying views
		/// </summary>
		private ICyclesViewController viewController;

		/// <summary>
		/// The dungeons user settings and data
		/// </summary>
		private CyclesUserData userData;

		/// <summary>
		/// The dungeons user settings and data
		/// </summary>
		[Export(typeof(CyclesUserData))]
		public CyclesUserData UserData
		{
			get
			{
				if (this.userData == null)
				{
					logger.Debug("Loading cycles user data");
					this.userData = CyclesUserData.LoadData(CyclesUserData.Filename);
					if (this.userData == null)
						this.userData = new CyclesUserData();
					this.userData.EnableAutoSave();
				}

				return this.userData;
			}
		}

		/// <summary>
		/// Notifies the module that it has be initialized.
		/// </summary>
		public void Initialize()
		{
			logger.Debug("Initializing Cycles Module");

			this.cyclesController = this.container.GetExportedValue<ICyclesController>();
			this.viewController = this.container.GetExportedValue<ICyclesViewController>();

			// Register for shutdown
			Commands.ApplicationShutdownCommand.RegisterCommand(new DelegateCommand(this.Shutdown));

			// Get the cycles controller started
			this.cyclesController.Start();

			// Initialize the view controller
			this.viewController.Initialize();

			logger.Debug("Cycles Module initialized");
		}

		/// <summary>
		/// Performs all neccesary shutdown activities for this module
		/// </summary>
		private void Shutdown()
		{
			logger.Debug("Shutting down Cycles Module");

			// Shut down the commerce controller
			this.cyclesController.Shutdown();

			// Shutdown the view controller
			this.viewController.Shutdown();

			// Make sure we have saved all user data
			// Note that this is a little redundant given the AutoSave feature,
			// but it does help to make sure the user's data is really saved
			CyclesUserData.SaveData(this.UserData, CyclesUserData.Filename);
		}
	}
}