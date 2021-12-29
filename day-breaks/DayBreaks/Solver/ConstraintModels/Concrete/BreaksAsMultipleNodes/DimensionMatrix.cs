using System;
using DayBreaks.Problem;

namespace DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes
{
    public class DimensionMatrix : IDimensionMatrix
    {
        private readonly NodeManager _nodeManager;
        private readonly ProblemModel _problemModel;
        private readonly VehicleManager _vehicleManager;

        public DimensionMatrix(ProblemModel problemModel, NodeManager nodeManager, VehicleManager vehicleManager) =>
            (_problemModel, _nodeManager, _vehicleManager) = (problemModel, nodeManager, vehicleManager);
        
        public long CalculateDistance(int fromNode, int toNode)
        {
            if (_nodeManager.IsEndNode(fromNode) && toNode != _nodeManager.GetStartNodeForEndNode(fromNode))
            {
                return int.MaxValue;
            }

            if (!_nodeManager.IsDepotNode(fromNode) && !_nodeManager.IsDepotNode(toNode) &&
                !_nodeManager.IsFromTheSameDay(fromNode, toNode))
            {
                return int.MaxValue;
            }
            var fromPoint = _nodeManager.GetPointForNode(fromNode);
            var toPoint = _nodeManager.GetPointForNode(toNode);

            return (long) Math.Sqrt(
                Math.Pow(fromPoint.Coords.X - toPoint.Coords.X, 2) +
                Math.Pow(fromPoint.Coords.Y - toPoint.Coords.Y, 2)
            );
        }

        public long CalculateTime(int fromNode, int toNode)
        {
            const int minutesPerKm = 1;
            var transitTime = CalculateDistance(fromNode, toNode) * minutesPerKm;
            var serviceTime = _nodeManager.IsDepotNode(fromNode)
                ? 0
                : _nodeManager.Visits[fromNode].ServiceTime;
            return transitTime + serviceTime;
        }

        public long CalculateDemand(int atNode)
        {
            return _nodeManager.IsDepotNode(atNode)
                ? -1 * _vehicleManager.MaxVehicleCapacity
                : _nodeManager.Visits[atNode].Demand;
        }

        public long CalculateDuration(int fromNode, int toNode)
        {
            var requiredTime = CalculateTime(fromNode, toNode);
            return _nodeManager.IsStartNode(toNode)
                ? -1 * _problemModel.MaxWorkDuration
                : requiredTime;
        }
    }
}