using System.Collections.Generic;

namespace DayBreaks.FleetStructureTesting.Models
{
    public record FleetToTest(long EstimatedCost, IEnumerable<FleetPosition> Fleet);
}