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
        private decimal _maxBudget;
        private decimal _inHouseAdSum;
        private decimal _thirdPartyAdSum;
        private int _agencyFeePercent;
        private int _thirdPartyFeePercent;
        private decimal _hourCost;
        private bool _newAdIsThirdParty;
        private int _maxIterations;

        /// <summary>
        /// Constructor for a new solver instance
        /// </summary>
        /// <param name="maxBudget">Maximum allowed budget</param>
        /// <param name="inHouseAdBudgets">Budgets of other ads that don't use third party tools</param>
        /// <param name="thirdPartyAdBudgets">Budgets of other ads that do use third party tools</param>
        /// <param name="agencyFeePercent">Percentage fee of the agency</param>
        /// <param name="thirdPartyFeePercent">Percentage fee third party tools</param>
        /// <param name="hourCost">Fixed cost for agency hours</param>
        /// <param name="newAdIsThirdParty">Flag specifying if the new ad will incur third party tool fees</param>
        /// <param name="maxIterations">Max number of GoalSeek iterations</param>
        /// <param name="epsilon">Maximum difference between spend and budget to be considered close enough</param>
        public Solver(decimal maxBudget, IEnumerable<decimal> inHouseAdBudgets, IEnumerable<decimal> thirdPartyAdBudgets,
            int agencyFeePercent, int thirdPartyFeePercent, decimal hourCost, bool newAdIsThirdParty = false, int maxIterations = 25)
        {
            
            // Set all parameters
            _maxBudget            = Math.Max(maxBudget, decimal.Zero);
            _agencyFeePercent     = Math.Max(agencyFeePercent, 0);
            _thirdPartyFeePercent = Math.Max(thirdPartyFeePercent, 0);
            _hourCost             = Math.Max(hourCost, decimal.Zero);
            _inHouseAdSum         = inHouseAdBudgets.Select(x => Math.Max(x, decimal.Zero)).Sum();
            _thirdPartyAdSum      = thirdPartyAdBudgets.Select(x => Math.Max(x, decimal.Zero)).Sum();
            _newAdIsThirdParty    = newAdIsThirdParty;
            _maxIterations        = Math.Max(maxIterations, 0);
        }

        /// <summary>
        /// Calculates the total spend given a <paramref name="budgetAllocation"/> for a new ad
        /// </summary>
        /// <param name="budgetAllocation">Maximum allowed budget</param>
        /// <returns>Total current ad and fee spending</returns>
        private decimal TotalSpend(decimal budgetAllocation)
        {
            // Include new ad in thirdPartyAdSpend if it will use third party tools, inHouse otherwise
            var thirdPartyAdSpend = _newAdIsThirdParty ? _thirdPartyAdSum + budgetAllocation : _thirdPartyAdSum;
            var inHouseAdSpend = _newAdIsThirdParty ? _inHouseAdSum : _inHouseAdSum + budgetAllocation;
            var adSpend = inHouseAdSpend + thirdPartyAdSpend;
            // Calculate and return the total spend including fees
            return adSpend + (adSpend * (_agencyFeePercent/100m)) + (thirdPartyAdSpend * (_thirdPartyFeePercent/100m)) + _hourCost;
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
        public decimal GoalSeek()
        {
            var initialSpend = TotalSpend(decimal.Zero);
            // If we are already at or over budget, max budget for a new ad is zero
            if (initialSpend >= _maxBudget)
            {
                return decimal.Zero;
            }
            
            // Guess an optimistic starting allocation for new ad using all of remaining budget
            var maxVal = (_maxBudget - initialSpend) * 2m;
            var minVal = decimal.Zero;
            var budgetAllocation = GuessFromRange(minVal, maxVal);

            // Iterate guesses to find optimal value
            for(int iteration = 0; iteration < _maxIterations; iteration ++)
            {
                // Determine current total spending on ads and fees
                var totalSpend = TotalSpend(budgetAllocation);
                
                Log.Information("range is {A}-{B}, totalSpend is {C} from budget allocation of {D}",
                    minVal, maxVal, totalSpend, budgetAllocation);
                
                // If we have converged to the _maxBudget, return our current guess
                if (_maxBudget-totalSpend <= 0.01m)
                {
                    return budgetAllocation;
                }
                
                // Update our current guess range
                if (totalSpend >= _maxBudget)
                {
                    // Decrease the maxVal
                    maxVal = budgetAllocation;
                }
                else if(totalSpend < _maxBudget)
                {
                    // Increase the minVal
                    minVal = budgetAllocation;
                }
                
                // Update our current guess
                budgetAllocation = GuessFromRange(minVal, maxVal);
            }
            
            return budgetAllocation;
        }
        
        
        
        
    }
}