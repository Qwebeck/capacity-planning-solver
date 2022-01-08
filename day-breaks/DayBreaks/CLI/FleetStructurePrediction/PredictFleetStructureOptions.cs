using CommandLine;

namespace DayBreaks.CLI.FleetStructurePrediction
{
    [Verb("fs-predict", HelpText = "Predicts fleet structure given characteristic days")]
    public class PredictFleetStructureOptions
    {
        [Option('p', "characteristic-days-path", Required = true, HelpText = "Path to characteristic days for which prediction should be done")]
        public string CharacteristicDaysPath { get; set; }
        
        [Option('s', "solution-path", Required=false, Default = "fleet_structure.json", HelpText = "Path to folder to which execution results would be saved")]
        public string ResultPath { get; set; }

        [Option('m', "method", Required = true, HelpText = "Method that will be used for prediction. One of: sat, vrp, linear")]
        public PredictionMethodOption PredictionMethod { get; set; }

    }
}