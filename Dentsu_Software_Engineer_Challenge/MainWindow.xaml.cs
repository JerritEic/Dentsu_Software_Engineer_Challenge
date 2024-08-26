using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    public partial class MainWindow
    {
        // Binding for ad budget data grids
        private readonly List<Ad> _inHouseAdBudgetsData;
        private readonly List<Ad> _thirdPartyAdBudgetsData;
        
        // Binding for total budget text box
        private decimal _totalBudget;
        public decimal TotalBudget
        {
            get => _totalBudget;
            set
            {
                if (value == _totalBudget)
                    return;
                _totalBudget = value;
                OnPropertyChanged(default);
            }
        }
        
        // Binding for agency hour cost text box
        private decimal _agencyHourCost;
        public decimal AgencyHourCost
        {
            get => _agencyHourCost;
            set
            {
                if (value == _agencyHourCost)
                    return;
                _agencyHourCost = value;
                OnPropertyChanged(default);
            }
        }
        
        // Binding for starting ad budget guess text box
        private decimal _startingGuess;
        public decimal StartingGuess
        {
            get => _startingGuess;
            set
            {
                if (value == _startingGuess)
                    return;
                _startingGuess = value;
                OnPropertyChanged(default);
            }
        }
        
        /// <summary>
        /// Sets up window from definitions in MainWindow.xaml and populates two ad budget <see cref="DataGrid"/>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            _inHouseAdBudgetsData = [new Ad() { Value = 1m }, new Ad() { Value = 1m }];
            InHouseAdBudgetsDataGrid.ItemsSource = _inHouseAdBudgetsData;
            
            _thirdPartyAdBudgetsData = [new Ad() { Value = 1m }, new Ad() { Value = 1m }];
            ThirdPartyAdBudgetsDataGrid.ItemsSource = _thirdPartyAdBudgetsData;

            _totalBudget = 25;

            _agencyHourCost = 5;

            _startingGuess = 25;
        }
        
        /// <summary>
        /// Wrapper around a <see cref="Value"/> for an ad budget
        /// </summary>
        public class Ad
        {
            public decimal Value { get; init; }
        }
    
        /// <summary>
        /// Called when calculate button is clicked, retrieves ad budget and fee parameters from frontend controls
        /// </summary>
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            var arguments = new Solver.SolverArguments(
                maxBudget: _totalBudget,
                startingGuess: _startingGuess,
                inHouseAdBudgets: _inHouseAdBudgetsData.Select(x => x.Value).ToArray(),
                thirdPartyAdBudgets: _thirdPartyAdBudgetsData.Select(x => x.Value).ToArray(),
                agencyFeePercent: Convert.ToSingle(AgencyFeeSliderValue.Value),
                thirdPartyFeePercent: Convert.ToSingle(ThirdPartyFeeSliderValue.Value),
                hourCost: _agencyHourCost,
                newAdIsThirdParty: IsThirdPartyCheckbox.IsChecked != null && IsThirdPartyCheckbox.IsChecked.Value,
                maxIterations: 20,
                debug: true);
            
            var worker = new BackgroundWorker();
            worker.DoWork += worker_RunSolver;
            worker.RunWorkerCompleted += worker_SolverCompleted;
            worker.RunWorkerAsync(arguments);
        }

        private static void worker_RunSolver(object? sender, DoWorkEventArgs e)
        {
            var args = (Solver.SolverArguments) (e.Argument ?? throw new InvalidOperationException());
            
            e.Result = new Solver(args).GoalSeek();
        }

        private void worker_SolverCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            var result = (Solver.SolverResult) (e.Result ?? throw new InvalidOperationException());
            NewAdBudgetTextBox.Text = result.NewAdBudget.ToString(CultureInfo.InvariantCulture);
            TotalSpentTextBox.Text  = result.TotalSpent.ToString(CultureInfo.InvariantCulture);
            
        }

    }
}