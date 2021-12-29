using System.Collections.Generic;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.Solver.Solution
{
    public class FleetPosition : VehicleInstance
    {
        public int Count;
    }

    public record FleetStructure(
        IEnumerable<FleetPosition> FleetPositions,
        long EstimatedCost
    );
}
