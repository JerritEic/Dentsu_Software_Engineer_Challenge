using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        
        /// <summary>
        /// Allows entering only text that is a valid number.
        /// </summary>
        /// <param name="sender">Element receiving text</param>
        /// <param name="e">Text entry event</param>
        private void NumericTextEntry(object sender, TextCompositionEventArgs e)
        {
            // Assure input is numeric value
            if (!IsTextNumeric(e.Text))
            {
                e.Handled = true; 
            }
        }

        /// <summary>
        /// Checks if <paramref name="text"/> contains numeric values only.
        /// </summary>
        /// <param name="text">Element receiving text</param>
        /// <returns> True if <paramref name="text"/> is numeric, False otherwise.</returns>
        private bool IsTextNumeric(string text)
        {
            // Regex matching only positive integers, no leading sign or decimal.
            Regex regex = new Regex(@"[0-9]+");
            return regex.IsMatch(text);
        }
        
        /// <summary>
        /// Allows pasting only text that is a valid number.
        /// </summary>
        /// <param name="sender">Element receiving text</param>
        /// <param name="e">Text paste event</param>
        private void NumericTextPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextNumeric(text))
                {
                    // Abort paste command if the data is not numeric
                    e.CancelCommand(); 
                }
            }
            else
            {
                // Abort paste command if the data is not a string
                e.CancelCommand(); 
            }
        }
    }
}