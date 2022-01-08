using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using DayBreaks.FleetStructureTesting;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.Solvers.Concrete;
using Google.OrTools.ConstraintSolver;
using Tools.Extensions;
using Tools.Utils;
using IntervalsDimensionMatrix = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.DimensionMatrix;
using IntervalsNodeManager = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.NodeManager;

namespace DayBreaks.CLI
{
    [Verb("vrp-simulate", HelpText =
        "Runs a VRP simulation for predicted fleet and returns json and returns a json containing" +
        " prediction simulated and predicted fleet costs, and their difference")]
    class VrpSimulateOptions
    {
        [Option('f', "fleet-structure-prediction", Required = true, HelpText = "Path to predicted fleet")]
        public string FleetStructurePredictionPath { get; set; }

        [Option('c', "characteristic-days", Required = true,
            HelpText = "Path to characteristic days, that will be used to generate days for testing")]
        public string CharacteristicDaysPath { get; set; }
        
        [Option('s', "save-to", Required = false,
            HelpText = "Path where solution will be saved", Default = "simulation_results.json")]
        public string SaveTo { get; set; }
        
        [Option('t', "time-limit", Required = false, HelpText = "Time limit for every day in model", Default = 60)]
        public int TimeLimit { get; set; }

        public record MetaheuristicOption(string Name) : MultipleChoiceOptions<LocalSearchMetaheuristic.Types.Value>(
            Name,
            new Dictionary<string, LocalSearchMetaheuristic.Types.Value>
            {
                {
                    "sa", LocalSearchMetaheuristic.Types.Value.SimulatedAnnealing
                },
                {
                    "gls", LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch
                },
                {
                    "ts", LocalSearchMetaheuristic.Types.Value.TabuSearch
                },
                {
                    "gd", LocalSearchMetaheuristic.Types.Value.GreedyDescent
                }
            });

        [Option('m', "metaheuristic", Required = true, HelpText = "Metaheuristic to be used by solver. One of: sa (Simulated Annealing), ts (Tabu Seach), gls (Guided Local Search), gd (GreedyDescent)")]
        public MetaheuristicOption Metaheuristic { get; set; }

    

    }


    public static class VrpSimulationRunner
    {
        internal static int Run(VrpSimulateOptions options)
        {
            var fleetStructureEvaluator = new VrpSimulator(options.CharacteristicDaysPath,
                options.FleetStructurePredictionPath, options.Metaheuristic.Value);
            var result = fleetStructureEvaluator.Evaluate(problemModel =>
            {
                var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel,
                    timeLimit: options.TimeLimit);
            });
            JsonUtils.SaveJson(result, options.SaveTo);
            return 0;
        }
    }

}