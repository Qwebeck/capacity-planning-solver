using DayBreaks.Problem;

namespace DayBreaks.Solver
{
    public enum NodeType {Start, End}

    public class VisitWithDay : Visit
    {
        public int Day;
    }
    public interface INodeManager
    {
        public Point GetPointForNode(int node);
        public int NodesCount { get; }

        public bool IsDepotNode(int node);

        public int GetVehicleStartDepotForDay(int vehicle, int day);
        public int GetVehicleEndDepotForDay(int vehicle, int day);
    }
}