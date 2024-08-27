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
        /// Configure application when first started
        /// </summary> 
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Configure logging to console and file
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
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