using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Class for finding max values of linear equations using implementation of Goal Seek algorithm
    /// </summary>
    public class Solver
    {
        
        /// <summary>
        /// Struct holding parameters for Solver instance
        /// </summary>
        public struct SolverArguments
        {
            // Maximum allowed budget for all ads and fees
            public decimal MaxBudget {get; set;}
            // Initial ad budget guess
            public decimal StartingGuess {get; set;}
            // Sum of all in-house ad budgets
            public decimal InHouseAdSum {get; set;}
            // Sum of all third-party ad budgets
            public decimal ThirdPartyAdSum  {get; set;}
            // Percentage fee taken by agency
            public float AgencyFeePercent  {get; set;}
            // Percentage fee taken by third parties
            public float ThirdPartyFeePercent  {get; set;}
            // Fixed agency cost for hours
            public decimal HourCost  {get; set;}
            // Whether the new budget is for a third-party or in-house ad
            public bool NewAdIsThirdParty  {get; set;}

            // Whether to print detailed debug information each iteration
            public bool Debug {get; set;}
            
            /// <summary>
            /// Constructor for a solver arguments instance
            /// </summary>
            /// <param name="maxBudget">Maximum allowed budget</param>
            /// <param name="startingGuess">Initial ad value guess</param>
            /// <param name="inHouseAdBudgets">Budgets of other ads that don't use third party tools</param>
            /// <param name="thirdPartyAdBudgets">Budgets of other ads that do use third party tools</param>
            /// <param name="agencyFeePercent">Percentage fee of the agency</param>
            /// <param name="thirdPartyFeePercent">Percentage fee third party tools</param>
            /// <param name="hourCost">Fixed cost for agency hours</param>
            /// <param name="newAdIsThirdParty">Flag specifying if the new ad will incur third party tool fees</param>
            /// <param name="debug">Flag indicating iteration information should be printed</param>
            public SolverArguments(decimal maxBudget, decimal startingGuess, IEnumerable<decimal> inHouseAdBudgets, IEnumerable<decimal> thirdPartyAdBudgets,
                float agencyFeePercent, float thirdPartyFeePercent, decimal hourCost, bool newAdIsThirdParty = false, int maxIterations = 25, bool debug = false)
            {
                // Set all parameters and perform sanity checks
                MaxBudget            = Math.Max(maxBudget, decimal.Zero);
                StartingGuess        = Math.Clamp(startingGuess, 0, MaxBudget);
                AgencyFeePercent     = Math.Clamp(agencyFeePercent, 0, 100);
                ThirdPartyFeePercent = Math.Clamp(thirdPartyFeePercent, 0, 100);
                HourCost             = Math.Max(hourCost, decimal.Zero);
                InHouseAdSum         = inHouseAdBudgets.Select(x => Math.Max(x, decimal.Zero)).Sum();
                ThirdPartyAdSum      = thirdPartyAdBudgets.Select(x => Math.Max(x, decimal.Zero)).Sum();
                NewAdIsThirdParty    = newAdIsThirdParty;
                Debug                = debug;
            }

            public static SolverArguments Empty()
            {
                return new SolverArguments(maxBudget: 0m,
                    startingGuess: 0m,
                    inHouseAdBudgets: [],
                    thirdPartyAdBudgets: [],
                    agencyFeePercent: 0,
                    thirdPartyFeePercent: 0,
                    hourCost: decimal.Zero,
                    newAdIsThirdParty: false,
                    maxIterations: 20,
                    debug: true);
            }
        }

        /// <summary>
        /// Struct holding parameters for Solver result
        /// </summary>
        public struct SolverResult(decimal newAdBudget, decimal totalSpent)
        {
            public decimal NewAdBudget { get; set; } = newAdBudget;
            public decimal TotalSpent { get; set; } = totalSpent;
        }

        public SolverArguments Args { get; set; }
        
        /// <summary>
        /// Constructor for a new solver instance
        /// </summary>
        /// <param name="args"><see cref="SolverArguments"/> instances containing parameters></param>
        public Solver(SolverArguments args)
        {
            Args = args; 
        }

        /// <summary>
        /// Calculates the total spend given a <paramref name="budgetAllocation"/> for a new ad
        /// </summary>
        /// <param name="budgetAllocation">Maximum allowed budget</param>
        /// <returns>Total current ad and fee spending</returns>
        private decimal TotalSpend(decimal budgetAllocation)
        {
            // Include new ad in thirdPartyAdSpend if it will use third party tools, inHouse otherwise
            var thirdPartyAdSpend = Args.NewAdIsThirdParty ? Args.ThirdPartyAdSum + budgetAllocation : Args.ThirdPartyAdSum;
            var inHouseAdSpend = Args.NewAdIsThirdParty ? Args.InHouseAdSum : Args.InHouseAdSum + budgetAllocation;
            var adSpend = inHouseAdSpend + thirdPartyAdSpend;
            // Calculate and return the total spend including fees
            return adSpend + (adSpend * (decimal)(Args.AgencyFeePercent/100)) + (thirdPartyAdSpend * (decimal)(Args.ThirdPartyFeePercent/100)) + Args.HourCost;
        }
        
        
        /// <summary>
        /// Calculates a new guess rounded to cents within a range
        /// </summary>
        /// <param name="minVal">Minimum value</param>
        /// <param name="maxVal">Maximum value</param>
        /// <returns>Rounded guess within range</returns>
        private static decimal GuessFromRange(decimal minVal, decimal maxVal)
        {
            return decimal.Round((minVal + maxVal) / 2m, 2);
        }
        
        /// <summary>
        /// Find maximum ad spend for a new ad without going over <see cref="_maxBudget"/>
        /// </summary>
        /// <returns>Maximum ad spend for a new add, or zero if there is not enough budget</returns>
        public SolverResult GoalSeek()
        {
            var totalSpend = TotalSpend(decimal.Zero);
            // If we are already at or over budget, max budget for a new ad is zero
            if (totalSpend >= Args.MaxBudget)
            {
                return new SolverResult(decimal.Zero, totalSpend);
            }
            
            // Set the initial guessing range
            var maxVal = Args.MaxBudget - totalSpend;
            var minVal = decimal.Zero;
            var budgetAllocation = Math.Clamp(Args.StartingGuess, minVal, maxVal);
            var iteration = 0;

            // Iterate guesses to find optimal value
            while(true)
            {
                // Determine current total spending on ads and fees
                totalSpend = TotalSpend(budgetAllocation);
                
                if(Args.Debug){
                    Log.Information("iter {1} range is {A}-{B}, totalSpend is {C} from budget allocation of {D}",
                    iteration, minVal, maxVal, totalSpend, budgetAllocation);
                }
                
                // If we have converged to the _maxBudget, return our current guess
                if (Args.MaxBudget >= totalSpend && Args.MaxBudget - totalSpend <= 0.01m)
                {
                    return new SolverResult(budgetAllocation, totalSpend);
                }
                
                // Update our current guess range
                if (totalSpend >= Args.MaxBudget)
                {
                    // Decrease the maxVal
                    maxVal = budgetAllocation;
                }
                else if(totalSpend < Args.MaxBudget)
                {
                    // Increase the minVal
                    minVal = budgetAllocation;
                }
                
                // Update our current guess
                budgetAllocation = GuessFromRange(minVal, maxVal);
                iteration++;
            }
            
            return new SolverResult(budgetAllocation, totalSpend);
        }
        
        
        
        
    }
}