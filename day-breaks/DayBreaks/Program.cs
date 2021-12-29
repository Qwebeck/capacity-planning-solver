using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DayBreaks.BenchmarkRunner;
using DayBreaks.FleetStructureTesting;
using DayBreaks.Mappers;
using DayBreaks.Problem;
using Newtonsoft.Json;
using DayBreaks.Solver;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using DayBreaks.Solver.Solvers.Concrete;
using DayBreaks.Solver.Solvers.Concrete.CpSat;
using Google.OrTools.ConstraintSolver;
using Google.OrTools.LinearSolver;
using Tools.Extensions;
using BreaksAsMultipleNodesDM = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes.DimensionMatrix;
using BreaksAsMultipleNodesNM = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes.NodeManager;
using IntervalsDimensionMatrix = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.DimensionMatrix;
using IntervalsNodeManager = DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals.NodeManager;


namespace DayBreaks
{
    public static class Program
    {
        
        public static void Main(string[] args)
        {
            // CreateJsonModelFromSolomonBenchmark(3);
            // var problemModel =
            // LoadJson(
            // @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\models\mapped_model.json");
                // @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\models\mapped_model.json");
            // var modelEvaluator = new ModelEvaluator.ModelEvaluator(problemModel);
            // var evaluationResult = modelEvaluator.Evaluate(FleetStructures(), problemModel =>
            // {
            //     var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
            //     return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel);
            // }).First();
            // Console.WriteLine(evaluationResult.CostOverestimate);

            // var constraintModel = BreaksAsMultipleNodesConstraintModelFactory.FromProblemModel(problemModel);
            // var solver = new VrpSolver<BreaksAsMultipleNodesDM, BreaksAsMultipleNodesNM>(constraintModel);
            // var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
            // // var solver = new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel);
            // var solver =
            //     new VrpBasedFleetStructureSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel);
            // var solution = solver.Solve();
            // Console.WriteLine(solution);
            // var solver = new CpSatBasedFleetStructureSolver(problemModel);
            // var solution = solver.Solve();
            // var summary = SummaryWriter.MakeSummaryFor(solution);
            // Console.WriteLine(summary);


            // ------------------
            // var solvers = CreateSolvers(problemModel);
            // BenchmarkRunner.BenchmarkRunner.RunBenchmark(solvers);
            // var solver = new LinearSolver(problemModel);
            // var solution = solver.Solve();
            // var summary = SummaryWriter.MakeSummaryFor(solution);
            // Console.WriteLine(summary);
         
            
            
            //---------------
            // RunVrpWithDifferentMetaheuristics();
            RunBatchFleetStructurePlanning();
            // RunFleetStructureTest();
        }

        private static void RunFleetStructureTest()
        {
            Directory
                .GetDirectories(
                    @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\FleetStructureTestResults")
                .ForEach(
                    RunEvaluationForTestDirectories);
            // Task.WhenAll(tests);
            Console.WriteLine("Evaluation completed");
        }


        private static void RunEvaluationForTestDirectories(object testDirectory)
        {
            Console.WriteLine($"Test started for directory {testDirectory}");
            const string problemPrefix = "problem_model";
            Directory.GetDirectories(testDirectory as string).ForEach(solverDirectory =>
            {
                Directory.GetFiles(solverDirectory).Where(file => file.Contains(problemPrefix)).ForEach(problemJsonPath =>
                {
                    var fleetStructurePath = problemJsonPath.Replace(problemPrefix, "solution");
                    var fleetStructureEvaluator = new FleetStructureEvaluator(problemJsonPath, fleetStructurePath);
                    var result = fleetStructureEvaluator.Evaluate(problemModel =>
                    {
                        var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                        return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel);
                    });
                    var overestimatePath =  problemJsonPath.Replace(problemPrefix, "fleetStructureEvaluationResult");
                    SaveJson(result, overestimatePath);
                });    
            });
            
        }
        
        private static void RunBatchFleetStructurePlanning()
        {
            PlanningBatchRunner.Evaluate(new []
            {
                new SolverConfiguration("LinearProgrammingModel", (problemModel) => new LinearSolver(problemModel)),
                new SolverConfiguration("CpSatModel", (problemModel) => new CpSatBasedFleetStructureSolver(problemModel)),
                new SolverConfiguration("VrpBasedSolver", (problemModel) =>
                {
                var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                return new VrpBasedFleetStructureSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(
                constraintModel, LocalSearchMetaheuristic.Types.Value.SimulatedAnnealing);
                })
            });
        }

        
        private static void RunVrpWithDifferentMetaheuristics()
        {
            var problemModel = SolomonBenchmarkToCustomModelMapper
                .CreateModelFromSolomonBenchmarkFile(
                    @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\solomon-benchmark\data\c101.txt",
                    DayType.Easy);
            SaveJson(problemModel, @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\BenchmarkResults\problem_model.json");
            var solver = CreateBreaksAsIntervalsSolver(problemModel);
            VrpMetaheuristicRunner.TestMetaheuristics(solver, "vrp_solver");
        }
        
        
        private static void SaveJson(object toSave, string path)
        {
            var json = JsonConvert.SerializeObject(toSave, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        
        

        // private static IEnumerable<FleetToTest> FleetStructures()
        // {
        //     return new[]
        //     {
        //         new FleetToTest(386260, new[]
        //         {
        //             new DepotVehiclesModification(Count: 20, DepotName: "depot 0", VehicleName: "medium")
        //         })
        //         
        //     };
        // }
        // private static Dictionary<string, IVrpSolver> CreateSolvers(ProblemModel problemModel) => new()
        // {
        //     {"intervals", CreateBreaksAsIntervalsSolver(problemModel)},
        //     {"dayBreaks", CreateBreaksAsMultipleNodesSolver(problemModel)}
        // };

        // private static IVrpSolver CreateBreaksAsMultipleNodesSolver(ProblemModel problemModel)
        // {
        //     var constraintModel = BreaksAsMultipleNodesConstraintModelFactory.FromProblemModel(problemModel);
        //     return new VrpSolver<BreaksAsMultipleNodesDM, BreaksAsMultipleNodesNM>(constraintModel,"breaksAsNodes");
        // }

        private static IVrpSolver CreateBreaksAsIntervalsSolver(ProblemModel problemModel)
        {
            var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
            return new VrpSolver<IntervalsDimensionMatrix, IntervalsNodeManager>(constraintModel, "breaksAsIntervals");
        }

    }
}