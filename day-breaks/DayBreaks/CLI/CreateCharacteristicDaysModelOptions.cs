using CommandLine;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;
using Tools.Utils;


namespace DayBreaks.CLI
{
    [Verb("create-characteristic-days", HelpText = "Creates characteristic days model from Solomon Benchmark")]
    public class CreateCharacteristicDaysModelOptions
    {
        [Option('b', "benchmark-path", Required = true, HelpText = "Path to Solomon Benchmark that will be used to generate characteristic days")]
        public string BenchmarkPath { get; set; }
        
        [Option('s', "save-to", Required = false, HelpText = "Path where result model will be saved", Default = "characteristic_days_model.json")]
        public string SaveTo { get; set; }
    }

    public class CharacteristicDayCreator
    {
        public static int Run(CreateCharacteristicDaysModelOptions options)
        {
            var model = SolomonBenchmarkToCustomModelMapper.CreateModelFromSolomonBenchmarkFile(
                options.BenchmarkPath, new[] {
                    new CharacteristicDayDescription(DayType.Easy, 5),
                    new CharacteristicDayDescription(DayType.Normal, 15),
                    new CharacteristicDayDescription(DayType.Hard, 5)
                });
            JsonUtils.SaveJson(model, options.SaveTo);
            return 0;
        }
    }
}
