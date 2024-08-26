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
using Serilog;


namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Ad> _inHouseAdBudgetsData;
        private List<Ad> _thirdPartyAdBudgetsData;
        
        /// <summary>
        /// Sets up window from definitions in MainWindow.xaml and populates two ad budget <see cref="DataGrid"/>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            _inHouseAdBudgetsData = new List<Ad>();
            _inHouseAdBudgetsData.Add(new Ad() {Value=1m});
            _inHouseAdBudgetsData.Add(new Ad() {Value=1m});
            InHouseAdBudgetsDataGrid.ItemsSource = _inHouseAdBudgetsData;
            
            _thirdPartyAdBudgetsData = new List<Ad>();
            _thirdPartyAdBudgetsData.Add(new Ad() {Value=1m});
            _thirdPartyAdBudgetsData.Add(new Ad() {Value=1m});
            ThirdPartyAdBudgetsDataGrid.ItemsSource = _thirdPartyAdBudgetsData;
            
        }
        
        /// <summary>
        /// Wrapper around a <see cref="Value"/> for an ad budget
        /// </summary>
        public class Ad
        {
            public decimal Value { get; set; }
        }
    
        /// <summary>
        /// Called when calculate button is clicked, retrieves ad budget and fee parameters from frontend controls
        /// </summary>
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            
            var maxBudget = Convert.ToDecimal(BudgetSliderValue.Value);
            
            var agencyFeePercent = Convert.ToInt32(AgencyFeeSliderValue.Value);
            
            var thirdPartyFeePercent = Convert.ToInt32(ThirdPartyFeeSliderValue.Value);
            
            var hourCost = Convert.ToDecimal(AgencyHourSliderValue.Value);
            
            var inHouseAdBudgets = _inHouseAdBudgetsData.Select(x => x.Value).ToArray();
            var thirdPartyAdBudgets = _thirdPartyAdBudgetsData.Select(x => x.Value).ToArray();
            
            var newAdIsThirdParty = IsThirdPartyCheckbox.IsChecked != null && IsThirdPartyCheckbox.IsChecked.Value;
            
            var maxIterations = 20;
            var debug = false;
            
           
        }
    }
}