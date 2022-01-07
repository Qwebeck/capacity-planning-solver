using System;
using System.IO;
using System.Linq;
using CommandLine;
using DayBreaks.BenchmarkRunner;
using DayBreaks.CLI;
using DayBreaks.FleetStructureTesting;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;
using DayBreaks.Problem;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.Solvers.Abstract;
using DayBreaks.Solver.Solvers.Concrete;
using Google.OrTools.ConstraintSolver;
using Newtonsoft.Json;
using Tools.Extensions;
using BreaksAsMultipleNodesDM = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes.DimensionMatrix;
using BreaksAsMultipleNodesNM = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes.NodeManager;
using IntervalsDimensionMatrix = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.DimensionMatrix;
using IntervalsNodeManager = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.NodeManager;


namespace DayBreaks
{
    public static class Program
    {
        
        public static void Main(string[] args) => Parser.Default
                .ParseArguments<VrpSimulateOptions, PredictFleetStructureOptions, CompareVrpMetaheuristicOptions>(args)
                .MapResult(
                    (VrpSimulateOptions options) => RunFleetStructureSimulation(options),
                    (PredictFleetStructureOptions options) => RunBatchFleetStructurePlanning(options),
                    (CompareVrpMetaheuristicOptions options) =>  RunVrpWithDifferentMetaheuristics(options),
                    errs => 1);

        private static int RunFleetStructureSimulation(VrpSimulateOptions options)
        {
            Directory
                .GetDirectories(options.FleetStructurePredictionsPath)
                .ForEach(dir => RunEvaluationForTestDirectories(dir, options.TimeLimit, options.Metaheuristic.Value));
            return 0;
        }


        private static void RunEvaluationForTestDirectories(string testDirectory, int timeLimit, LocalSearchMetaheuristic.Types.Value metaheuristic)
        {
            Console.WriteLine($"Test started for directory {testDirectory}");
            const string problemPrefix = "problem_model";
            Directory.GetDirectories(testDirectory).ForEach(solverDirectory =>
            {
                Directory.GetFiles(solverDirectory).Where(file => file.Contains(problemPrefix)).ForEach(problemJsonPath =>
                {
                    var fleetStructurePath = problemJsonPath.Replace(problemPrefix, "solution");
                    var fleetStructureEvaluator = new VrpSimulator(problemJsonPath, fleetStructurePath, metaheuristic);
                    var overestimatePath =  problemJsonPath.Replace(problemPrefix, "fleetStructureEvaluationResult");
                    if (Directory.Exists(overestimatePath))
                    {
                        return;
                    }
                    var result = fleetStructureEvaluator.Evaluate(problemModel =>
                    {
                        var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                        return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel, timeLimit: timeLimit);
                    });
                    
                    SaveJson(result, overestimatePath);
                });    
            });
            
        }
        private static int RunBatchFleetStructurePlanning(PredictFleetStructureOptions options)
        {
            PlanningBatchRunner.Evaluate(options.PredictionMethods.Select(m => m.Value),
                options.ResultPath, options.BenchmarkPath);
            return 0;
        }

        private static int RunVrpWithDifferentMetaheuristics(CompareVrpMetaheuristicOptions options)
        {
            var problemModel = SolomonBenchmarkToCustomModelMapper
                .CreateModelFromSolomonBenchmarkFile(
                    options.BenchmarkPath,
                    new CharacteristicDayDescription(DayType.Easy, 15));
            SaveJson(problemModel, options.SolutionPath);
            var solver = CreateBreaksAsIntervalsSolver(problemModel);
            VrpMetaheuristicRunner.TestMetaheuristics(solver, "vrp_solver");
            return 0;
        }
        
        
        private static void SaveJson(object toSave, string path)
        {
            var json = JsonConvert.SerializeObject(toSave, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        
        
        private static IVrpSolver CreateBreaksAsIntervalsSolver(ProblemModel problemModel)
        {
            var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
            return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel, "breaksAsIntervals");
        }

    }
}