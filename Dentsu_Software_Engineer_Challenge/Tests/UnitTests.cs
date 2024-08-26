using Dentsu_Software_Engineer_Challenge;
using Serilog;

namespace Tests
{
    [TestFixture]
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
            // Create a test logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
        
        [Test]
        public void IsNoBudgetAllocatedWithNegativeBudget()
        {
            decimal maxBudget = -1000m;
            decimal[] inHouseAdBudgets = Array.Empty<decimal>();
            decimal[] thirdPartyAdBudgets = Array.Empty<decimal>();
            int agencyFeePercent = 0;
            int thirdPartyFeePercent = 0;
            decimal hourCost = 0m;
            bool newAdIsThirdParty = false;
            int maxIterations = 20;
            bool debug = false;

            var solver = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations, debug);
            Assert.That(solver.GoalSeek(), Is.EqualTo(decimal.Zero), "With negative maxBudget, ad budget allocation should be zero.");
        }
        
        [Test]
        public void IsNoBudgetAllocatedWhenOverBudget()
        {
            decimal maxBudget = 10m;
            decimal[] inHouseAdBudgets = {5m};
            decimal[] thirdPartyAdBudgets = {5m};
            int agencyFeePercent = 0;
            int thirdPartyFeePercent = 0;
            decimal hourCost = 0m;
            bool newAdIsThirdParty = false;
            int maxIterations = 20;
            bool debug = false;

            var solver = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations, debug);
            Assert.That(solver.GoalSeek(), Is.EqualTo(decimal.Zero), "When no budget left, ad budget allocation should be zero.");
        }
        
        [TestCase(0)]
        [TestCase(25)]
        [TestCase(1000)]
        public void IsMaxBudgetAllocatedWithNoFees(decimal maxBudget)
        {
            decimal[] inHouseAdBudgets = Array.Empty<decimal>();
            decimal[] thirdPartyAdBudgets = Array.Empty<decimal>();
            int agencyFeePercent = 0;
            int thirdPartyFeePercent = 0;
            decimal hourCost = 0m;
            bool newAdIsThirdParty = false;
            int maxIterations = 20;
            bool debug = false;

            var solver = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations, debug);
            Assert.That(solver.GoalSeek(), Is.EqualTo(maxBudget),
                "With no fees or existing ads, ad budget allocation should be the entire budget.");
        }
        
        [TestCase(0)]
        [TestCase(25)]
        [TestCase(1000)]
        public void IsMaxBudgetAllocatedWithNoFeesAndOtherAds(decimal maxBudget)
        {
            decimal[] inHouseAdBudgets = {1m, 1m};
            decimal[] thirdPartyAdBudgets = {1m, 1m};
            decimal sums = inHouseAdBudgets.Sum() + thirdPartyAdBudgets.Sum();
            
            int agencyFeePercent = 0;
            int thirdPartyFeePercent = 0;
            decimal hourCost = 0m;
            bool newAdIsThirdParty = false;
            int maxIterations = 20;
            bool debug = false;

            var solver = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations, debug);
            Assert.That(solver.GoalSeek(), Is.EqualTo(Math.Max(maxBudget - sums, decimal.Zero)),
                "With no fees but existing ads, ad budget allocation should be the entire budget minus the cost of other ads.");
        }
        

        [Test]
        public void IsAdBudgetEqualForInHouseOrThirdPartyWithNoThirdPartyFee()
        {
            decimal maxBudget = 25m;
            decimal[] inHouseAdBudgets = {1m, 1m};
            decimal[] thirdPartyAdBudgets = {1m, 1m};
            
            int agencyFeePercent = 5;
            int thirdPartyFeePercent = 0;
            decimal hourCost = 5m;
            int maxIterations = 20;
            bool debug = false;

            var solverThirdParty = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, true, maxIterations, debug);
            var solverInHouse = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, false, maxIterations, debug);
            Assert.That(solverThirdParty.GoalSeek(), Is.EqualTo(solverInHouse.GoalSeek()),
                "With no third party fee cost should be identical for an ad in house or third party.");
        }
        
        [Test]
        public void IsAdBudgetNotEqualForInHouseOrThirdPartyWithThirdPartyFee()
        {
            decimal maxBudget = 25m;
            decimal[] inHouseAdBudgets = {1m, 1m};
            decimal[] thirdPartyAdBudgets = {1m, 1m};
            
            int agencyFeePercent = 5;
            int thirdPartyFeePercent = 5;
            decimal hourCost = 5m;
            int maxIterations = 20;
            bool debug = false;

            var solverThirdParty = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, true, maxIterations, debug);
            var solverInHouse = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, false, maxIterations, debug);
            Assert.That(solverThirdParty.GoalSeek(), Is.LessThan(solverInHouse.GoalSeek()),
                "With third party fees ad budget will be smaller for a third party than in house.");
        }
        
        [TestCase(25,new[] { 1, 1, 1, 1 } , new int [] { })]
        [TestCase(25,new[] { 1, 1, 1 } , new [] { 1 })]
        [TestCase(25,new[] { 1, 1 } , new [] { 1, 1 })]
        [TestCase(25,new[] { 1 } , new [] { 1, 1, 1 })]
        [TestCase(25,new int[] { } , new [] { 1, 1, 1, 1 })]
        [TestCase(25000000,new int[] { } , new [] { 10000, 10000, 10000, 10000 })]
        public void IsAdBudgetCorrect(decimal maxBudget, int[] inHouseAdBudgetsInt, int[] thirdPartyAdBudgetsInt )
        {
            decimal[] inHouseAdBudgets = Array.ConvertAll(inHouseAdBudgetsInt, Convert.ToDecimal);
            decimal[] thirdPartyAdBudgets = Array.ConvertAll(inHouseAdBudgetsInt, Convert.ToDecimal);
            
            int agencyFeePercent = 5;
            int thirdPartyFeePercent = 5;
            decimal hourCost = 5m;
            bool newAdIsThirdParty = false;
            int maxIterations = 50;
            bool debug = true;

            var solver = new Solver(maxBudget, inHouseAdBudgets, thirdPartyAdBudgets, agencyFeePercent, thirdPartyFeePercent,
                hourCost, newAdIsThirdParty, maxIterations, debug);
            var newAdBudget = solver.GoalSeek();
            // Determine the resulting total cost with this new ad budget
            var adSpend = inHouseAdBudgets.Sum() + thirdPartyAdBudgets.Sum() + newAdBudget;
            var totalSpent = adSpend + (adSpend * (agencyFeePercent/100m)) +
                             (thirdPartyAdBudgets.Sum() * (thirdPartyFeePercent/100m)) + hourCost;
            
            Assert.That(totalSpent, Is.InRange(maxBudget-0.01m, maxBudget),
                "New ad budget should result in total costs <= 1 cent of the max budget.");
        }
        
    }
}