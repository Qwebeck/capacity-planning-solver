using System;
using System.IO;
using Newtonsoft.Json;
using Tools.Extensions;

namespace DayBreaks.Solver.Solution
{
    public class SummaryWriter
    {
        public static string MakeSummaryFor(FleetStructure fleetStructure)
        {
            var writer = new StringWriter();
            var (rentedVehicleInstances, estimatedCost) = fleetStructure;
            rentedVehicleInstances.ForEach(vehicle =>
            {
                writer.WriteLine("+---------------------------+");
                WriteParam(writer, "Name", vehicle.Name);
                WriteParam(writer, "Count", vehicle.Count);
                WriteParam(writer, "RentalType", vehicle.RentalType);
                WriteParam(writer, "Usage cost", vehicle.MonthUsageCost);
                WriteParam(writer, "SourceDepotName", vehicle.SourceDepotName);
                WriteParam(writer, "Start", vehicle.StartDay);
                writer.WriteLine("+---------------------------+");

            });
            writer.WriteLine($"Estimated cost: {estimatedCost}");
            return writer.ToString();
        }

        private static void WriteParam<T>(TextWriter writer, string name, T value)
        {
            if (value?.Equals(default(T)) ?? true)
            {
                return;
            }
            writer.WriteLine($"{name}: {value}");
        }
        public static void SaveHumanReadableSummaryToFile(FleetStructure fleetStructure, string pathToFile)
        {
            var summary = MakeSummaryFor(fleetStructure);
            File.WriteAllText(pathToFile, summary);
        }

        public static void SaveJsonSummaryToFile(FleetStructure fleetStructure, string pathToFile)
        {
            var json = JsonConvert.SerializeObject(fleetStructure, Formatting.Indented);
            File.WriteAllText(pathToFile, json);
        }

    }
}