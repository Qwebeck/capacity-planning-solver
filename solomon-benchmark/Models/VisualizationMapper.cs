using System.Linq;
using Google.OrTools.ConstraintSolver;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    public class VisualizationMapper
    {
        public static string MapBenchmark(Model model)
        {
            var firstLine = $"{model.Customers.Count} {model.VehicleCount} {model.Capacity}\n";
            var locations = model.Customers.Select(customer =>
                $"{customer.Demand} {customer.Coords.X} {customer.Coords.Y}");
            return firstLine + string.Join('\n', locations);
        }

        public static string MapSolution(Solution solution)
        {
            var firstLine = $"{solution.ObjectiveValue} 0\n";
            var lines = solution.VehicleRoutes.Select(route => string.Join(' ', route.RouteNodes));
            return firstLine + string.Join('\n', lines);
        }
    }
}