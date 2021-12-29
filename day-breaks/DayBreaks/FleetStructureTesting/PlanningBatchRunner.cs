using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DayBreaks.Mappers;
using DayBreaks.Problem;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Newtonsoft.Json;
using Tools.Extensions;

namespace DayBreaks.FleetStructureTesting
{


    public record TestCase(string Instance, IEnumerable<DayType> DayTypes);
    public record SolverConfiguration(string Name, Func<ProblemModel, IFleetStructureSolver> SolverFactory);
    
    public static class  PlanningBatchRunner
    {

        private static Random Random => new(42);

        private static string ResultsFolder =>
            @"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\day-breaks\DayBreaks\FleetStructureTestResults"; 

        private static IEnumerable<TestCase> IsDayOrderAffectsPlanningEfficiency => new[]
        {
            new TestCase("c101.txt", new [] {DayType.Hard, DayType.Normal, DayType.Easy}),
            new TestCase("c101.txt", new [] {DayType.Easy, DayType.Normal, DayType.Hard}),
        };

        private static IEnumerable<TestCase> IsDayComplexityAffectsPlanningEfficiency => new[]
        {
            new TestCase("c51.txt", new[] {DayType.Easy, DayType.Normal, DayType.Hard}),
            new TestCase("c101.txt", new[] {DayType.Easy, DayType.Normal, DayType.Hard}),
        };

        private static IEnumerable<TestCase> IsDayAmountAffectsPlanningEfficiency => new[]
        {
            new TestCase("c101.txt",new[] {DayType.Easy, DayType.Normal, DayType.Hard}),
            new TestCase("c101.txt",new[] {DayType.Easy, DayType.Normal, DayType.Hard, DayType.Easy, DayType.Normal, DayType.Hard}),
            new TestCase("c101.txt",new[]
            {
                DayType.Easy, DayType.Normal, DayType.Hard, DayType.Easy, DayType.Normal, DayType.Hard, DayType.Easy,
                DayType.Normal, DayType.Hard
            }),
        };
        public static void Evaluate(IEnumerable<SolverConfiguration> solverConfigurations)
        {
            solverConfigurations = solverConfigurations.ToList();
            RunBatch(nameof(IsDayOrderAffectsPlanningEfficiency), IsDayOrderAffectsPlanningEfficiency, solverConfigurations);
            RunBatch(nameof(IsDayComplexityAffectsPlanningEfficiency), IsDayComplexityAffectsPlanningEfficiency, solverConfigurations);
            RunBatch(nameof(IsDayAmountAffectsPlanningEfficiency),IsDayAmountAffectsPlanningEfficiency, solverConfigurations);
            
        }

        private static void RunBatch(string batchName, IEnumerable<TestCase> testCases, IEnumerable<SolverConfiguration> solverConfigurations)
        {
            testCases.ForEach(testCase =>
            {
                var dayTypes = testCase.DayTypes.ToList();
                var model = CreateProblemModel(testCase);
                solverConfigurations.ForEach(test =>
                {
                    Console.WriteLine(
                        $"Starting test {test.Name} for day day types: {string.Join(' ',dayTypes.Select(t => t.ToString()))}");
                    var solver = test.SolverFactory(model);
                    var fleetStructure = solver.Solve();
                    if (fleetStructure is not null)
                    {
                        SaveSolution($"{ResultsFolder}\\{batchName}", test.Name, testCase, model, fleetStructure);
                    }

                    Console.WriteLine($"Test ended. Solution found: {fleetStructure is not null}");
                });
            });
        }
        
        
        private static ProblemModel CreateProblemModel(TestCase testCase) =>
            SolomonBenchmarkToCustomModelMapper
                .CreateModelFromSolomonBenchmarkFile(
                    $@"C:\Users\Bohdan\Projects\thesis\capacity-planning-solver\solomon-benchmark\data\{testCase.Instance}",
                    testCase.DayTypes,  random: Random);

        private static void SaveSolution(string folder, string testName, TestCase testCase, ProblemModel problemModel, FleetStructure fleetStructure)
        {
            Directory.CreateDirectory($"{folder}\\{testName}");
            var dayTypesPostfix = $"for_instance_{testCase.Instance}_for_day_types_{string.Join('_', testCase.DayTypes.Select(t => t.ToString()))}";
            var pathToSolution = $"{folder}\\{testName}\\solution{dayTypesPostfix}.json";
            var pathToProblemModel = $"{folder}\\{testName}\\problem_model{dayTypesPostfix}.json";
            var solutionJson = JsonConvert.SerializeObject(fleetStructure, Formatting.Indented);
            var modelJson = JsonConvert.SerializeObject(problemModel, Formatting.Indented);
            File.WriteAllText(pathToSolution, solutionJson);
            File.WriteAllText(pathToProblemModel, modelJson);
        }
    }
}