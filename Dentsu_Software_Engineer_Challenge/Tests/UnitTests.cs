using Dentsu_Software_Engineer_Challenge;
using Serilog;

namespace Tests
{
    [TestFixture]
    public class UnitTests
    {
        private Solver.SolverArguments _defaultArgs;
        
        [SetUp]
        public void Setup()
        {
            // Create a test logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            _defaultArgs = new Solver.SolverArguments(
                maxBudget: 25m,
                startingGuess: 25m,
                inHouseAdBudgets: [1m, 1m],
                thirdPartyAdBudgets: [1m, 1m],
                agencyFeePercent: 5,
                thirdPartyFeePercent: 5,
                hourCost: 5m,
                newAdIsThirdParty: false,
                maxIterations: 20,
                debug: true);
        }
        
        [Test]
        public void IsNoBudgetAllocatedWithNegativeBudget()
        {
            var args = _defaultArgs;
            args.MaxBudget = -1000m;

            var solver = new Solver(args);
            Assert.That(solver.GoalSeek().NewAdBudget, Is.EqualTo(decimal.Zero), "With negative maxBudget, ad budget allocation should be zero.");
        }
        
        [Test]
        public void IsNoBudgetAllocatedWhenOverBudget()
        {
            var args = _defaultArgs;
            args.MaxBudget = 1m;
            
            var solver = new Solver(args);
            Assert.That(solver.GoalSeek().NewAdBudget, Is.EqualTo(decimal.Zero),  "When no budget left, ad budget allocation should be zero.");
        }
        
        [TestCase(0)]
        [TestCase(25)]
        [TestCase(1000)]
        public void IsMaxBudgetAllocatedWithNoFees(decimal maxBudget)
        {
            var args = Solver.SolverArguments.Empty();
            args.MaxBudget = maxBudget;

            var solver = new Solver(args);
            Assert.That(solver.GoalSeek().NewAdBudget, Is.InRange(maxBudget - 0.01m, maxBudget),
                "With no fees or existing ads, ad budget allocation should be the entire budget.");
        }
        
        [TestCase(0)]
        [TestCase(25)]
        [TestCase(1000)]
        public void IsMaxBudgetAllocatedWithNoFeesAndOtherAds(decimal maxBudget)
        {
            var args = _defaultArgs;
            var sums = args.InHouseAdSum + args.ThirdPartyAdSum;
            args.HourCost = 0m;
            args.AgencyFeePercent = 0;
            args.ThirdPartyFeePercent = 0;
            
            var solver = new Solver(args);
            Assert.That(solver.GoalSeek().NewAdBudget, Is.EqualTo(Math.Max(args.MaxBudget - sums, decimal.Zero)),
                "With no fees but existing ads, ad budget allocation should be the entire budget minus the cost of other ads.");
        }
        

        [Test]
        public void IsAdBudgetEqualForInHouseOrThirdPartyWithNoThirdPartyFee()
        {
            var argsThirdParty = _defaultArgs;
            argsThirdParty.ThirdPartyFeePercent = 0;
            argsThirdParty.NewAdIsThirdParty = true;
            
            var argsInHouse = _defaultArgs;
            argsInHouse.ThirdPartyFeePercent = 0;
            argsInHouse.NewAdIsThirdParty = false;
            

            var solverThirdParty =  new Solver( argsThirdParty);
            var solverInHouse =  new Solver(argsInHouse);
            Assert.That(solverThirdParty.GoalSeek(), Is.EqualTo(solverInHouse.GoalSeek()),
                "With no third party fee cost should be identical for an ad in house or third party.");
        }
        
        [Test]
        public void IsAdBudgetNotEqualForInHouseOrThirdPartyWithThirdPartyFee()
        {
            var argsThirdParty = _defaultArgs;
            argsThirdParty.NewAdIsThirdParty = true;
            
            var argsInHouse = _defaultArgs;
            argsInHouse.NewAdIsThirdParty = false;
            

            var solverThirdParty =  new Solver( argsThirdParty);
            var solverInHouse =  new Solver(argsInHouse);
            
            Assert.That(solverThirdParty.GoalSeek().NewAdBudget, Is.LessThan(solverInHouse.GoalSeek().NewAdBudget),
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
            var args = _defaultArgs;
            args.InHouseAdSum = Array.ConvertAll(inHouseAdBudgetsInt, Convert.ToDecimal).Sum();
            args.ThirdPartyAdSum = Array.ConvertAll(inHouseAdBudgetsInt, Convert.ToDecimal).Sum();


            var solver = new Solver(args);
            var results = solver.GoalSeek();
            
            // Determine the resulting total cost with this new ad budget
            var adSpend = args.InHouseAdSum + args.ThirdPartyAdSum + results.NewAdBudget;
            var totalSpent = adSpend + (adSpend * (decimal)(args.AgencyFeePercent/100)) +
                             (args.ThirdPartyAdSum * (decimal)(args.ThirdPartyFeePercent/100)) + args.HourCost;
            
            Assert.That(totalSpent, Is.EqualTo(results.TotalSpent),
                "Total spent is not correctly calculated with the reported new ad budget.");
            
            Assert.That(totalSpent, Is.InRange(args.MaxBudget - 0.01m, args.MaxBudget),
                "Total spent should be within a cent of the max budget.");
        }
        
        [Test]
        public void TerminatesWithLargeBudgetAndIterations()
        {
            var args = _defaultArgs;
            args.MaxBudget = decimal.MaxValue/2;
            var solver = new Solver(args);
            
            Assert.DoesNotThrow(() => solver.GoalSeek(), 
                "Large values of budget should still terminate.");
        }
    }
}