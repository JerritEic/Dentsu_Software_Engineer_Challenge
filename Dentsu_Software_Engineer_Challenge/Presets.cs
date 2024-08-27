using System.Collections.Generic;

namespace Dentsu_Software_Engineer_Challenge
{
    /// <summary>
    /// Predefined <see cref="Solver.SolverArguments"/>
    /// </summary>
    public static class Presets
    {
        
        /// <summary>
        /// Dictionary of named <see cref="Solver.SolverArguments"/> presets
        /// </summary>
        public static readonly Dictionary<string, Solver.SolverArguments> SolverPresets = new()
        {
            { "Default", new Solver.SolverArguments(
                maxBudget: 25m,
                startingGuess: 25m,
                inHouseAdBudgets: [1m, 1m],
                thirdPartyAdBudgets: [1m, 1m],
                agencyFeePercent: 5,
                thirdPartyFeePercent: 5,
                hourCost: 5m,
                newAdIsThirdParty: false,
                debug: true) },
            { "Empty", new Solver.SolverArguments(
                maxBudget: 0m,
                startingGuess: 0m,
                inHouseAdBudgets: [],
                thirdPartyAdBudgets: [],
                agencyFeePercent: 0,
                thirdPartyFeePercent: 0,
                hourCost: decimal.Zero,
                newAdIsThirdParty: false,
                debug: true) },
            { "HighBudgetLowGuess", new Solver.SolverArguments(
                maxBudget: 100000000000m,
                startingGuess: 0m,
                inHouseAdBudgets: [5000m, 5000m, 5000m, 5000m],
                thirdPartyAdBudgets: [5000m, 5000m, 5000m, 5000m],
                agencyFeePercent: 5,
                thirdPartyFeePercent: 5,
                hourCost: 12345m,
                newAdIsThirdParty: false,
                debug: true) },
            { "ThirdParty", new Solver.SolverArguments(
                maxBudget: 25000m,
                startingGuess: 12500m,
                inHouseAdBudgets: [100m, 200m, 400m, 800m],
                thirdPartyAdBudgets: [100m, 200m, 400m, 800m],
                agencyFeePercent: 5,
                thirdPartyFeePercent: 50,
                hourCost: 1000m,
                newAdIsThirdParty: true,
                debug: true) },
            { "NoInHouse", new Solver.SolverArguments(
                maxBudget: 500m,
                startingGuess: 250m,
                inHouseAdBudgets: [],
                thirdPartyAdBudgets: [50m, 70m, 20m, 50m],
                agencyFeePercent: 5,
                thirdPartyFeePercent: 100,
                hourCost: 20m,
                newAdIsThirdParty: false,
                debug: true) }
        };
        
        
    }
}