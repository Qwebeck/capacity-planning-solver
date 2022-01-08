using System.Collections.Generic;
using CommandLine;

namespace DayBreaks.CLI.FleetStructurePrediction
{
    [Verb("reproduce-thesis-fleet-structure-predictions", HelpText = "Runs fleet structure predictions with selected methods for same configurations as one used in thesis")]
    public class ReproduceFleetStructureTestsOptions
    {
        [Option('p', "benchmarks-path", Required = true, HelpText = "Path to the directory that contains c51 and c101 instances of Solomon Benchmark.")]
        public string BenchmarkPath { get; set; }
        
        [Option('s', "solution-path", Required=false, Default = "tests", HelpText = "Path to folder to which execution results would be saved")]
        public string ResultPath { get; set; }
        

        [Option('m', "methods", Required = true, HelpText = "Method that will be used for fleet structure prediction. One of: sat, vrp, linear")]
        public IEnumerable<PredictionMethodOption> PredictionMethods { get; set; }
      
    }
}