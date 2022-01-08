using System.Linq;
using DayBreaks.FleetStructureTesting;
using DayBreaks.Problem;
using Tools.Utils;

namespace DayBreaks.CLI.FleetStructurePrediction
{
    static class FleetStructurePredictor
    {
        public static int MakePredictions(ReproduceFleetStructureTestsOptions testsOptions)
        {
            var predictors = testsOptions.PredictionMethods.Select(m => m.Value);
            PlanningBatchRunner.Evaluate(predictors,
                testsOptions.ResultPath, testsOptions.BenchmarkPath);
            return 0;
        }

        public static int MakePredictions(PredictFleetStructureOptions options)
        {
            var problemModel = JsonUtils.LoadJson<ProblemModel>(options.CharacteristicDaysPath);
            var fleetStructure = options.PredictionMethod.Value.SolverFactory(problemModel).Solve();
            JsonUtils.SaveJson(fleetStructure, options.ResultPath);
            return 0;
        }
    }
}