using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using Tools.Extensions;

namespace DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes
{
    public class NodeManager : INodeManager
    {
        /// <summary>
        /// Visits for every day that appeared in problem.
        /// </summary>
        public readonly IList<VisitWithDay> Visits;

        private readonly Dictionary<string, Point> _pointsByName = new();
        private readonly Dictionary<int, string> _depotNodeToDepotName = new();
        private readonly Dictionary<(int vehicleIdx, int day, NodeType nodeType), int> _depotVehicleDayToBreakNode = new();
        

        private IEnumerable<int> GetDepotNodesOfType(NodeType type) => _depotVehicleDayToBreakNode.Keys.Where(key => key.nodeType == type)
            .Select(key => _depotVehicleDayToBreakNode[key]);

        public IEnumerable<int> DepotNodes =>
            GetDepotNodesOfType(NodeType.Start).Concat(GetDepotNodesOfType(NodeType.End));
        public bool IsDepotNode(int node)
            => node >= Visits.Count;
        

        public int NodesCount => Visits.Count + _depotNodeToDepotName.Count;

        /// <summary>
        /// Returns node index for depot that should be visited be vehicle on given day
        /// day start from zero to day count
        /// </summary>
        /// <returns></returns>
        
        public int GetVehicleStartDepotForDay(int vehicleIdx, int day) =>
            _depotVehicleDayToBreakNode[(vehicleIdx, day, NodeType.Start)];

        public int GetVehicleEndDepotForDay(int vehicleIdx, int day) =>
            _depotVehicleDayToBreakNode[(vehicleIdx, day, NodeType.End)];
        
        public NodeManager(ProblemModel problemModel)
        {
            
            Visits = AddDayInformationToVisits(problemModel).ToList();
            CreateDepotNodes(problemModel);
            CreatePointToNameMapping(problemModel);
        }
        
        // Flatten visits and assign correct time to it.
        private IEnumerable<VisitWithDay> AddDayInformationToVisits(ProblemModel problemModel) =>
            from indexedDay in problemModel.Days.Select((day, idx) => new {idx, day})
            let dayIdx = indexedDay.idx
            let day = indexedDay.day
            from visit in day.Visits
            select new VisitWithDay
            {
                FromTime = visit.FromTime + dayIdx * problemModel.DayDuration,
                ToTime = visit.ToTime + dayIdx * problemModel.DayDuration,
                Demand = visit.Demand,
                PointName = visit.PointName,
                ServiceTime = visit.ServiceTime,
                Day = dayIdx
            };

        private void CreateDepotNodes(ProblemModel problemModel)
        { 
            var depotNode = Visits.Count;
            var vehicleManager = new VehicleManager(problemModel); 
            vehicleManager.Vehicles.ForEach((vehicle, vehicleIdx) =>
            {
                Enumerable.Range(vehicle.StartDay, vehicle.EndDay + 1).ForEach(day =>
                {
                    _depotVehicleDayToBreakNode[(vehicleIdx, day, NodeType.Start)] = depotNode;
                    _depotVehicleDayToBreakNode[(vehicleIdx, day, NodeType.End)] = depotNode + 1;
                    _depotNodeToDepotName[depotNode] = vehicle.SourceDepotName;
                    _depotNodeToDepotName[depotNode + 1] = vehicle.SourceDepotName;
                    depotNode += 2; // TODO: fix in some way.
                });
            });
        }

        private void CreatePointToNameMapping(ProblemModel problemModel) => problemModel
            .Clients
            .Concat<Point>(problemModel.Depots)
            .ForEach(point =>
            {
                _pointsByName[point.Name] = point;
            });
        
      
        
        public bool IsEndNode(int node) => GetDepotNodesOfType(NodeType.End).Any(endNode => endNode == node);
        public bool IsStartNode(int node) => GetDepotNodesOfType(NodeType.Start).Any(startNode => startNode == node);
        public int GetStartNodeForEndNode(int fromNode)
        {
            return fromNode + 1;
        }

        public bool IsFromTheSameDay(int nodeA, int nodeB)
        {
            var visitA = Visits[nodeA];
            var visitB = Visits[nodeB];
            return visitA.Day == visitB.Day;
        }
    
        public Point GetPointForNode(int node) => IsDepotNode(node)
            ? _pointsByName[_depotNodeToDepotName[node]]
            : _pointsByName[Visits[node].PointName];
    }
}