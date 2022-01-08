
using System.Collections.Generic;
using CommandLine;
using DayBreaks.CLI.FleetStructurePrediction;

namespace DayBreaks.CLI
{
    public static class  CommandLineInterface
    {
        public static int Run(IEnumerable<string> args) =>  Parser.Default
            .ParseArguments<VrpSimulateOptions, ReproduceFleetStructureTestsOptions, CompareVrpMetaheuristicOptions,CreateCharacteristicDaysModelOptions, PredictFleetStructureOptions>(args)
            .MapResult(
                (VrpSimulateOptions options) => VrpSimulationRunner.Run(options),
                (ReproduceFleetStructureTestsOptions options) => FleetStructurePredictor.MakePredictions(options),
                (PredictFleetStructureOptions options) => FleetStructurePredictor.MakePredictions(options),
                (CompareVrpMetaheuristicOptions options) =>  MetaheuristicTestRunner.Run(options),
                (CreateCharacteristicDaysModelOptions options) => CharacteristicDayCreator.Run(options),
                errs => 1);

    }
}