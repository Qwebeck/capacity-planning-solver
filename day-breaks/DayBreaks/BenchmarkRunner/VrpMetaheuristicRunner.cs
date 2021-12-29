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
    enum BenchmarkType {Vrp, FleetStructure}
    public static class VrpMetaheuristicRunner
    {
        private static string ResultsFolder =>
            @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\BenchmarkResults";

        #region VRP
        private static IEnumerable<Metaheuristic> Metaheuristics => new[]
        {
            Metaheuristic.SimulatedAnnealing,
            Metaheuristic.GreedyDescent,
            Metaheuristic.TabuSearch,
            Metaheuristic.GuidedLocalSearch
        };
        
        public static void TestMetaheuristics(IDictionary<string, IVrpSolver> solvers) => solvers.Keys.ForEach(solverName => TestMetaheuristics(solvers[solverName], solverName));

        public static void TestMetaheuristics(IVrpSolver vrpSolver, string name)
        {
            Metaheuristics.ForEach(metaheuristic =>
            {
                Console.WriteLine($"Running benchmark for solver {name} with metaheuristic {metaheuristic}");
                var solution = vrpSolver.Solve(metaheuristic);

                if (solution is null)
                {
                    Console.WriteLine("Solution was not found");
                    return;
                }
                var solutionPath = CreateSolutionFilePath(name, metaheuristic);
                SaveSolution(solution, solutionPath);
            });  
        }
        #endregion

        #region Saving
        private static string CreateSolutionFilePath(string solverName, Metaheuristic metaheuristic) =>
            $"{ResultsFolder}\\{solverName}_{metaheuristic}.json";
        
        private static void SaveSolution(object solution, string path)
        {
            var json = JsonConvert.SerializeObject(solution, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        

        #endregion
        
    }
}