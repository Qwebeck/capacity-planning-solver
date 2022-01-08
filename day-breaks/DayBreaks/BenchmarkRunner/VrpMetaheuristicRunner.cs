using System;
using System.Collections.Generic;
using System.IO;
using DayBreaks.Solver;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.ConstraintSolver;
using Newtonsoft.Json;
using Tools.Extensions;
using Metaheuristic = Google.OrTools.ConstraintSolver.LocalSearchMetaheuristic.Types.Value;
namespace DayBreaks.BenchmarkRunner
{
    public static class VrpMetaheuristicRunner
    {
        
        #region VRP
        private static IEnumerable<Metaheuristic> Metaheuristics => new[]
        {
            Metaheuristic.SimulatedAnnealing,
            Metaheuristic.GreedyDescent,
            Metaheuristic.TabuSearch,
            Metaheuristic.GuidedLocalSearch
        };
        
        public static void TestMetaheuristics(IDictionary<string, IVrpSolver> solvers, string resultsFolder) => solvers.Keys.ForEach(solverName => TestMetaheuristics(solvers[solverName], solverName, resultsFolder));

        public static void TestMetaheuristics(IVrpSolver vrpSolver, string name, string resultsFolder)
        {
            if (!Directory.Exists(resultsFolder))
            {
                Directory.CreateDirectory(resultsFolder);
            }
            Metaheuristics.ForEach(metaheuristic =>
            {
                Console.WriteLine($"Running benchmark for solver {name} with metaheuristic {metaheuristic}");
                var solution = vrpSolver.Solve(metaheuristic);

                if (solution is null)
                {
                    Console.WriteLine("Solution was not found");
                    return;
                }
                var solutionPath = CreateSolutionFilePath(resultsFolder, name,  metaheuristic);
                SaveSolution(solution, solutionPath);
            });  
        }
        #endregion

        #region Saving
        private static string CreateSolutionFilePath(string resultsFolder, string solverName, Metaheuristic metaheuristic) =>
            $"{resultsFolder}\\{solverName}_{metaheuristic}.json";
        
        private static void SaveSolution(object solution, string path)
        {
            
            var json = JsonConvert.SerializeObject(solution, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        

        #endregion
        
    }
}