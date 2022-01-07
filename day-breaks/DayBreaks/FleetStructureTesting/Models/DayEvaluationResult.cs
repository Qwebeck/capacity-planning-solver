using System.Collections.Generic;
using DayBreaks.Solver.Managers;

namespace DayBreaks.FleetStructureTesting.Models
{
    internal record DayEvaluationResult(bool IsSolved, int? NotVisitedClientsCount, IEnumerable<VehicleInstance> UsedVehicles);
}