using CommandLine;
using DayBreaks.BenchmarkRunner;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;
using DayBreaks.Problem;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.Solvers.Abstract;
using DayBreaks.Solver.Solvers.Concrete;
using Tools.Utils;
using IntervalsDimensionMatrix = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.DimensionMatrix;
using IntervalsNodeManager = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.NodeManager;

namespace DayBreaks.CLI
{
    [Verb("mh-test", HelpText = "run tests comparing such local search metaheuristic as: Simulated Annealing, Guided Local Search, Tabu Search and Greedy Descent for an instance of a Solomon Benchmark")]
    class CompareVrpMetaheuristicOptions
    {
        [Option('p', "benchmark-path", Required = true, HelpText = "Path to the Solomon Benchmark on which metaheuristics will be tested.")]
        public string BenchmarkPath { get; set; }
        
        [Option('s', "save-to", Required=false, HelpText = "Path to folder where solutions made by every metaheuristic will be saved", 
            Default = "metaheuristic_test_results")]
        public string SaveTo { get; set; }
        
        [Option('t', "time-limit", Required = false, HelpText = "Time in seconds every metaheuristic can spent on the solving process", Default = 3*60)]
        public int TimeLimit { get; set; }
    }


    static class MetaheuristicTestRunner
    {
        public static int Run(CompareVrpMetaheuristicOptions options)
        {
            var problemModel = SolomonBenchmarkToCustomModelMapper
                .CreateModelFromSolomonBenchmarkFile(
                    options.BenchmarkPath,
                    new CharacteristicDayDescription(DayType.Easy, 15));
            var solver = CreateBreaksAsIntervalsSolver(problemModel, options.TimeLimit);
            VrpMetaheuristicRunner.TestMetaheuristics(solver, "vrp_solver", options.SaveTo);
            return 0;
        }
        
        
        private static IVrpSolver CreateBreaksAsIntervalsSolver(ProblemModel problemModel, int timeLimit)
        {
            var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
            return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel, timeLimit: timeLimit);
        }
    }
}