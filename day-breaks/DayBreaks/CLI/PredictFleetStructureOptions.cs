using System.Collections.Generic;
using CommandLine;
using DayBreaks.FleetStructureTesting.Models;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.Solvers.Concrete;
using DayBreaks.Solver.Solvers.Concrete.CpSat;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.CLI
{
    [Verb("fs-predict")]
    public class PredictFleetStructureOptions
    {
        [Option('p', "benchmarks-path", Required = true, HelpText = "Path to the directory that contains c51 and c101 instances of Solomon Benchmark.")]
        public string BenchmarkPath { get; set; }
        
        [Option('s', "solution-path", Required=true, HelpText = "Path solution to be saved.")]
        public string ResultPath { get; set; }

        public record PredictionMethodOption(string Name) : MultipleChoiceOptions<SolverConfiguration>(Name,
            new Dictionary<string, SolverConfiguration>
            {
                {
                    "sat",
                    new SolverConfiguration("CpSatModel",
                        problemModel => new CpSatBasedFleetStructureSolver(problemModel))
                    
                },
                {
                    "vrp", new SolverConfiguration("VrpBasedSolver", problemModel =>
                    {
                        var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                        return new VrpBasedFleetStructureSolver<DimensionMatrix, NodeManager>(
                            constraintModel, LocalSearchMetaheuristic.Types.Value.TabuSearch);
                    })
                },
                {
                    "linear", new SolverConfiguration("LinearProgrammingModel", problemModel => new LinearSolver(problemModel))
                }
            });

        [Option('m', "method", Required = true, HelpText = "Method that will be used for prediction")]
        public IEnumerable<PredictionMethodOption> PredictionMethods { get; set; }
        
        [Option('t', "time-limit", Required = true, HelpText = "Method that will be used for prediction")]
        public int TimeLimit { get; set; }
    }
}