using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Application-wide initialization and event handlers
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Run when application is started
        /// </summary> 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/app.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            decimal maxBudget = 25m;
            decimal[] inHouseAdBudgets = {1m};
            decimal[] thirdPartyAdBudgets = {1m,1m,1m};
            int agencyFeePercent = 5;
            int thirdPartyFeePercent = 5;
            decimal hourCost = 1m;
            bool newAdIsThirdParty = false;
            int maxIterations = 20;

            var solver = new Solver(maxBudget,  inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations);
            solver.GoalSeek();

        }
        
        /// <summary>
        /// Fallback handler for any uncaught exceptions
        /// </summary>
        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}