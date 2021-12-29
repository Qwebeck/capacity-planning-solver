using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using Tools.Extensions;

namespace DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals
{
    public class NodeManager: INodeManager
    {
        
        /// <summary>
        /// Visits for every day that appeared in problem.
        /// </summary>
        public readonly IList<VisitWithDay> Visits;

        private readonly Dictionary<string, Point> _pointsByName = new();
        private readonly Dictionary<int, string> _depotNodeToDepotName = new();
        private readonly Dictionary<(int vehicleIdx, int day), int> _vehicleDepotNodeAtDay = new();
        
        public IEnumerable<int> DepotNodes => _vehicleDepotNodeAtDay.Values;
        public bool IsDepotNode(int node)
            => node >= Visits.Count;

        public int GetVehicleStartDepotForDay(int vehicle, int day) => GetVehicleDepotNodeForDay(vehicle, day);

        public int GetVehicleEndDepotForDay(int vehicle, int day) => GetVehicleDepotNodeForDay(vehicle, day + 1);

        public int NodesCount => Visits.Count + _depotNodeToDepotName.Count;

        /// <summary>
        /// Returns node index for depot that should be visited be vehicle on given day
        /// day start from zero to day count
        /// </summary>
        /// <returns></returns>
        
        protected int GetVehicleDepotNodeForDay(int vehicleIdx, int day) =>
            _vehicleDepotNodeAtDay[(vehicleIdx, day)];

    
        public NodeManager(ProblemModel problemModel)
        {
            
            Visits = AddDayInformationToVisits(problemModel).ToList();
            CreateDepotNodes(problemModel);
            CreatePointToNameMapping(problemModel);
        }
        
        // Flatten visits and assign correct time to it.
        private static IEnumerable<VisitWithDay> AddDayInformationToVisits(ProblemModel problemModel) =>
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
                Enumerable.Range(vehicle.StartDay, vehicle.EndDay + 2).ForEach(day =>
                {
                    _vehicleDepotNodeAtDay[(vehicleIdx, day)] = depotNode;
                    _depotNodeToDepotName[depotNode] = vehicle.SourceDepotName;
                    depotNode += 1; // TODO: fix in some way.
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
        
        
        public Point GetPointForNode(int node) => IsDepotNode(node)
            ? _pointsByName[_depotNodeToDepotName[node]]
            : _pointsByName[Visits[node].PointName];
    }
}