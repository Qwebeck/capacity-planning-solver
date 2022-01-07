using CommandLine;

namespace DayBreaks.CLI
{
    [Verb("mh-test")]
    class CompareVrpMetaheuristicOptions
    {
        [Option('p', "benchmark-path", Required = true, HelpText = "Path to the Solomon Benchmark on which metaheuristics will be tested.")]
        public string BenchmarkPath { get; set; }
        
        [Option('s', "solution-path", Required=true, HelpText = "Path solution to be saved.")]
        public string SolutionPath { get; set; }
    }
}