using System.Collections.Generic;
using CommandLine;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.CLI
{
    [Verb("vrp-simulate")]
    class VrpSimulateOptions
    {
        [Option('f', "fleet-structure-prediction", Required = true, HelpText = "Path to characteristic days from which simulation data will be generated")]
        public string FleetStructurePredictionsPath { get; set; }
        
        
        public record MetaheuristicOption(string Name) : MultipleChoiceOptions<LocalSearchMetaheuristic.Types.Value>(Name,
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
        
        [Option('m', "metaheuristic", Required = false, HelpText = "Metaheuristic that will be used for test")]
        public MetaheuristicOption Metaheuristic { get; set; }
        
        [Option('t', "time-limit", Required = true, HelpText = "Method that will be used for prediction")]
        public int TimeLimit { get; set; }
        
    }
}