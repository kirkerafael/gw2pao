using GW2PAO.Modules.Cycles.Interfaces;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GW2PAO.Infrastructure;

namespace GW2PAO.Modules.Cycles.ViewModels.CycleTracker
{
    /// <summary>
    /// Primary Cycle Tracker view model class
    /// </summary>
    [Export(typeof(CycleTrackerViewModel))]
    public class CycleTrackerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Cycle Tracker controller
        /// </summary>
        private ICyclesController controller;

        /// <summary>
        /// Collection of all World Cycles
        /// </summary>
        public ObservableCollection<CycleViewModel> Cycles
        {
            get { return this.controller.Cycles; }
        }

        /// <summary>
        /// Command to reset all hidden events
        /// </summary>
        public DelegateCommand ResetHiddenCyclesCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to open the settings for the Cycles Tracker
        /// </summary>
        public DelegateCommand SettingsCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Cycle Tracker user data
        /// </summary>
        public CyclesUserData UserData { get { return this.controller.UserData; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventTrackerController">The event tracker controller</param>
        [ImportingConstructor]
        public CycleTrackerViewModel(ICyclesController eventTrackerController)
        {
            this.controller = eventTrackerController;
            this.ResetHiddenCyclesCommand = new DelegateCommand(this.ResetHiddenCycles);
            this.SettingsCommand = new DelegateCommand(() => Commands.OpenCycleSettingsCommand.Execute(null));
        }

        /// <summary>
        /// Resets all hidden events
        /// </summary>
        private void ResetHiddenCycles()
        {
            logger.Debug("Resetting hidden events");
            this.UserData.HiddenCycles.Clear();
        }
    }
}
