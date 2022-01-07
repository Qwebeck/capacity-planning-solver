namespace DayBreaks.FleetStructureTesting.Models
{
    public record FleetEvaluationResult(
        long PredictedFleetCost,
        long RealFleetCost)
    {
        public long CostOverestimate => PredictedFleetCost - RealFleetCost;
    }
}