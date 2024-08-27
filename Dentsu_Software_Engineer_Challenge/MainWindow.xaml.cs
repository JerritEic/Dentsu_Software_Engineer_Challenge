using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        // Bound lists for ad budget data grids
        private ObservableCollection<Ad> _inHouseAdBudgetsData;
        private ObservableCollection<Ad> _thirdPartyAdBudgetsData;
        
        // Bound value for total budget text box
        public UiDecimal TotalBudget = new UiDecimal(0);
        
        // Bound value for agency hour cost text box
        public UiDecimal AgencyHourCost = new UiDecimal(0);

        // Bound value for starting guess text box
        public UiDecimal StartingGuess = new UiDecimal(0);
        
        
        /// <summary>
        /// Sets up window from definitions in MainWindow.xaml and initializes UI bindings 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            // Bind TotalBudget textbox to TotalBudget UiDecimal
            Binding binding = new Binding("Value")
            {
                Source = TotalBudget,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            TotalBudgetBox.SetBinding(TextBox.TextProperty, binding);
            
            // Bind AgencyHourCost textbox to agency hour cost UiDecimal
            binding = new Binding("Value")
            {
                Source = AgencyHourCost,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            AgencyHourCostTextbox.SetBinding(TextBox.TextProperty, binding);
            
            // Bind starting guess textbox to starting guess UiDecimal
            binding = new Binding("Value")
            {
                Source = StartingGuess,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            StartingGuessTextBox.SetBinding(TextBox.TextProperty, binding);
            
            // Initialize ad budget lists
            _inHouseAdBudgetsData = [];
            InHouseAdBudgetsDataGrid.ItemsSource = _inHouseAdBudgetsData;
            
            _thirdPartyAdBudgetsData = [];
            ThirdPartyAdBudgetsDataGrid.ItemsSource = _thirdPartyAdBudgetsData;
            
            // Initialize preset combobox
            foreach(var item in Presets.SolverPresets.Keys)
            {
                PresetsComboBox.Items.Add(item);
            }
            PresetsComboBox.SelectedIndex = 0;
            
            // Fill in values from default preset
            LoadPreset("Default");
        }
        
        /// <summary>
        /// Wrapper around a <see cref="decimal"/> for an ad budget
        /// </summary>
        public class Ad 
        {
            public decimal Budget { get; init; }
        }
        

        /// <summary>
        /// Updates the UI with values from a predefined set of solver arguments
        /// </summary>
        /// <param name="presetName">Name of a defined solver argument preset in <see cref="Presets.SolverPresets"/></param>
        /// <param name="firstLoad">Set when run the first time</param>
        private void LoadPreset(string presetName, bool firstLoad = false)
        {
            if (!Presets.SolverPresets.TryGetValue(presetName, out var args)) return;
           
            // Numeric textbox values
            TotalBudget.Value = args.MaxBudget;
            AgencyHourCost.Value = args.HourCost;
            StartingGuess.Value = args.StartingGuess;
            
            // Populate sliders
            AgencyFeeSliderValue.Value = args.AgencyFeePercent;
            ThirdPartyFeeSliderValue.Value = args.ThirdPartyFeePercent;
            
            // Populate checkbox
            IsThirdPartyCheckbox.IsChecked = args.NewAdIsThirdParty;
            
            // Populate data grids
            _inHouseAdBudgetsData.Clear();
            foreach(var item in args.InHouseAdBudgets.Select(x => new Ad() { Budget = x } ))
            {
                _inHouseAdBudgetsData.Add(item);
            }
            _thirdPartyAdBudgetsData.Clear();
            foreach(var item in  args.ThirdPartyAdBudgets.Select(x => new Ad() { Budget = x } ))
            {
                _thirdPartyAdBudgetsData .Add(item);
            }
            
            // Clear previous results
            NewAdBudgetTextBox.Text = "";
            TotalSpentTextBox.Text = "";
        }

        /// <summary>
        /// Called when LoadPreset button is clicked
        /// </summary>
        private void LoadPresetButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPreset(PresetsComboBox.Text, false);
        }
    
        /// <summary>
        /// Called when calculate button is clicked, retrieves ad budget and fee parameters from frontend controls
        /// </summary>
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve parameters and set up a SolverArgument 
            var arguments = new Solver.SolverArguments(
                maxBudget: TotalBudget.Value,
                startingGuess: StartingGuess.Value,
                inHouseAdBudgets: _inHouseAdBudgetsData.Select(x => x.Budget).ToArray(),
                thirdPartyAdBudgets: _thirdPartyAdBudgetsData.Select(x => x.Budget).ToArray(),
                agencyFeePercent: Convert.ToSingle(AgencyFeeSliderValue.Value),
                thirdPartyFeePercent: Convert.ToSingle(ThirdPartyFeeSliderValue.Value),
                hourCost: AgencyHourCost.Value,
                newAdIsThirdParty: IsThirdPartyCheckbox.IsChecked != null && IsThirdPartyCheckbox.IsChecked.Value,
                debug: true);
            
            // Run Goal Seek solver on worker thread
            var worker = new BackgroundWorker();
            worker.DoWork += worker_RunSolver;
            worker.RunWorkerCompleted += worker_SolverCompleted;
            worker.RunWorkerAsync(arguments);
        }

        /// <summary>
        /// Worker thread running Goal Seek solver on background thread
        /// </summary>
        private static void worker_RunSolver(object? sender, DoWorkEventArgs e)
        {
            var args = (Solver.SolverArguments) (e.Argument ?? throw new InvalidOperationException());
            
            e.Result = new Solver(args).GoalSeek();
        }
        
        /// <summary>
        /// Called when worker is complete and has returned a <see cref="Solver.SolverResult"/>
        /// </summary>
        private void worker_SolverCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            var result = (Solver.SolverResult) (e.Result ?? throw new InvalidOperationException());
            NewAdBudgetTextBox.Text = result.NewAdBudget.ToString(CultureInfo.InvariantCulture);
            TotalSpentTextBox.Text  = decimal.Round(result.TotalSpent, 2).ToString(CultureInfo.InvariantCulture);
            
        }

    }
}