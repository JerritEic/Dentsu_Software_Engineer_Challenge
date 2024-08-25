using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Application-wide initialization and event handlers
    /// </summary>
    public partial class App : Application
    {
        
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