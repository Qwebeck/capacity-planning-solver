using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DayBreaks.FleetStructureTesting.Models;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;
using DayBreaks.Problem;
using DayBreaks.Solver.Solution;
using Newtonsoft.Json;
using Tools.Extensions;

namespace DayBreaks.FleetStructureTesting
{
    public static class  PlanningBatchRunner
    {

        private static Random Random => new(42);

        private static IEnumerable<TestCase> IsDayOrderAffectsPlanningEfficiency
        {
            get
            {
                var dayTypes = new[]
                {
                    new CharacteristicDayDescription(DayType.Hard, Occurrences: 5),
                    new CharacteristicDayDescription(DayType.Normal, Occurrences: 5),
                    new CharacteristicDayDescription(DayType.Easy, Occurrences: 5)
                };
                return new[]
                {
                    new TestCase("c101.txt", dayTypes),
                    new TestCase("c101.txt", dayTypes.Reverse()),
                };
            }
        }

        private static IEnumerable<TestCase> IsDayComplexityAffectsPlanningEfficiency
        {
            get
            {
                var dayTypes = new[]
                {
                    new CharacteristicDayDescription(DayType.Easy, 5), 
                    new CharacteristicDayDescription(DayType.Normal, 5), 
                    new CharacteristicDayDescription(DayType.Hard, 5)
                };
                return new[]
                {
                    new TestCase("c51.txt", dayTypes),
                    new TestCase("c101.txt", dayTypes),
                };
            }
        }

        private static IEnumerable<TestCase> IsDayAmountAffectsPlanningEfficiency => new[]
        {
            new TestCase("c101.txt",new[] 
                {
                    new CharacteristicDayDescription(DayType.Easy, 5), 
                    new CharacteristicDayDescription(DayType.Normal, 5), 
                    new CharacteristicDayDescription(DayType.Hard, 5)
                }),
            new TestCase("c101.txt",new[]
            {
                new CharacteristicDayDescription(DayType.Easy, 3), 
                new CharacteristicDayDescription(DayType.Normal, 3), 
                new CharacteristicDayDescription(DayType.Hard, 3), 
                new CharacteristicDayDescription(DayType.Easy,3), 
                new CharacteristicDayDescription(DayType.Normal,3), 
                new CharacteristicDayDescription(DayType.Hard, 3),
            }),
            new TestCase("c101.txt",new[]
            {
                new CharacteristicDayDescription(DayType.Easy,2), 
                new CharacteristicDayDescription(DayType.Normal,2), 
                new CharacteristicDayDescription(DayType.Hard,2),
                new CharacteristicDayDescription(DayType.Easy,2),
                new CharacteristicDayDescription(DayType.Normal,2), 
                new CharacteristicDayDescription(DayType.Hard,2), 
                new CharacteristicDayDescription(DayType.Easy,2),
                new CharacteristicDayDescription(DayType.Normal,2), 
                new CharacteristicDayDescription(DayType.Hard,2),
            }),
        };


        private static IEnumerable<TestCase> IsDayOccurrencesAffectsFleetStructure => new[]
        {
            // contract fleet should be fitted to day type with more occurences 
            new TestCase("c101.txt", new[]
            {
                new CharacteristicDayDescription(DayType.Easy, 20),
                new CharacteristicDayDescription(DayType.Hard, 5)
            }),
            new TestCase("c101.txt", new[]
            {
                new CharacteristicDayDescription(DayType.Easy, 5),
                new CharacteristicDayDescription(DayType.Hard, 20)
            })
        };

            public static void Evaluate(IEnumerable<SolverConfiguration> solverConfigurations, string resultsFolder, string solomonBenchmarkDirPath)
        {
            solverConfigurations = solverConfigurations.ToList();
            RunBatch(nameof(IsDayOrderAffectsPlanningEfficiency), resultsFolder,solomonBenchmarkDirPath, IsDayOrderAffectsPlanningEfficiency, solverConfigurations);
            RunBatch(nameof(IsDayComplexityAffectsPlanningEfficiency), resultsFolder, solomonBenchmarkDirPath,IsDayComplexityAffectsPlanningEfficiency, solverConfigurations);
            RunBatch(nameof(IsDayAmountAffectsPlanningEfficiency),resultsFolder, solomonBenchmarkDirPath,IsDayAmountAffectsPlanningEfficiency, solverConfigurations);
            RunBatch(nameof(IsDayOccurrencesAffectsFleetStructure), resultsFolder, solomonBenchmarkDirPath,IsDayAmountAffectsPlanningEfficiency,
                solverConfigurations);
        }

        
        private static void RunBatch(string batchName, string resultsFolder, string solomonBenchmarkDirPath, IEnumerable<TestCase> testCases, IEnumerable<SolverConfiguration> solverConfigurations)
        {
            testCases.ForEach(testCase =>
            {
                var dayTypes = testCase.CharacteristicDays.ToList();
                var model = CreateProblemModel(testCase, solomonBenchmarkDirPath);
                solverConfigurations.ForEach(test =>
                {
                    Console.WriteLine(
                        $"Starting test {test.Name} for day day types: {string.Join(' ',dayTypes.Select(t => t.DayType.ToString()))}");
                    var solver = test.SolverFactory(model);
                    var fleetStructure = solver.Solve();
                    if (fleetStructure is not null)
                    {
                        SaveSolution(Path.Join(resultsFolder, batchName), test.Name, testCase, model, fleetStructure);
                    }

                    Console.WriteLine($"Test ended. Solution found: {fleetStructure is not null}");
                });
            });
        }
        
        
        private static ProblemModel CreateProblemModel(TestCase testCase, string solomonBenchmarkDirPath) =>
            SolomonBenchmarkToCustomModelMapper
                .CreateModelFromSolomonBenchmarkFile(
                    Path.Join(solomonBenchmarkDirPath,testCase.Instance),
                    testCase.CharacteristicDays,  random: Random);

        private static void SaveSolution(string folder, string testName, TestCase testCase, ProblemModel problemModel, FleetStructure fleetStructure)
        {
            Directory.CreateDirectory(Path.Join(folder, testName));
            var dayTypesPostfix = $"for_instance_{testCase.Instance}_for_day_types_{string.Join('_', testCase.CharacteristicDays.Select(t => t.DayType.ToString()))}";
            var pathToSolution = Path.Join(folder, testName, $"solution{dayTypesPostfix}.json");
            var pathToProblemModel = Path.Join(folder, testName, $"problem_model{dayTypesPostfix}.json");
            var solutionJson = JsonConvert.SerializeObject(fleetStructure, Formatting.Indented);
            var modelJson = JsonConvert.SerializeObject(problemModel, Formatting.Indented);
            File.WriteAllText(pathToSolution, solutionJson);
            File.WriteAllText(pathToProblemModel, modelJson);
        }
    }
}