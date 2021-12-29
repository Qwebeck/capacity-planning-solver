using System;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.ConstraintModels.Abstract;
using DayBreaks.Solver.Managers;
using Google.OrTools.ConstraintSolver;
using Tools.Extensions;

namespace DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsMultipleNodes
{
    public static class BreaksAsMultipleNodesConstraintModelFactory
    {
        public static ConstraintModel FromProblemModel(ProblemModel problemModel)
        {
            var vehicleManager = new VehicleManager(problemModel);
            var nodeManager = new NodeManager(problemModel);
            var dimensionMatrix = new DimensionMatrix(problemModel, nodeManager, vehicleManager);
            return new ConstraintModel(problemModel, vehicleManager, nodeManager, dimensionMatrix);
        }
    } 
    public class ConstraintModel: ConstraintModel<DimensionMatrix, NodeManager>
    {
        
        public ConstraintModel(ProblemModel problemModel, VehicleManager vehicleManager, NodeManager nodeManager,
            DimensionMatrix dimensionMatrix): base(problemModel, vehicleManager, nodeManager, dimensionMatrix)
        { }

        protected override void AddConstraints()
        {
            AddVisitsConstraints();
            AddBreaks();
            MakeVisitsOptional();
        }

        private int LastDayIdx => ProblemModel.CharacteristicDayCount - 1;
        protected override RoutingIndexManager CreateIndexManager(ProblemModel problemModel, VehicleManager vehicleManager, NodeManager nodeManager)
        {
            
            var vehicleStarts = vehicleManager
                .Vehicles
                .Select(
                    vehicle =>
                        nodeManager.GetVehicleStartDepotForDay(vehicle.Index, vehicle.StartDay)).ToArray();
            var vehicleEnds = vehicleManager
                .Vehicles
                .Select(
                    vehicle =>
                        nodeManager.GetVehicleEndDepotForDay(vehicle.Index, vehicle.EndDay)).ToArray();
            
            return new RoutingIndexManager(
                nodeManager.NodesCount,
                vehicleManager.Vehicles.Count(),
                vehicleStarts,
                vehicleEnds
            );

        }
        
        protected override RoutingModel CreateRoutingModel(RoutingIndexManager indexManager)
        {  
            var routingParameters = operations_research_constraint_solver.DefaultRoutingModelParameters();
        
            // routingParameters.SolverParameters.TraceSearch = true;
            // routingParameters.SolverParameters.TracePropagation = true;
            return new RoutingModel(indexManager, routingParameters);
            
        }
        
         private void AddVisitsConstraints()
        {
            NodeManager.Visits.ForEach((visit, node) =>
            {
                var idx = IndexManager.NodeToIndex(node);
                Time.CumulVar(idx).SetRange(visit.FromTime, visit.ToTime);
            });
            SetEqualSlacksForTimeAndDurationAtVisits();
        }
        
        private void SetEqualSlacksForTimeAndDurationAtVisits()
        {
            var solver = Routing.solver();
            NodeManager.Visits.ForEach((_, node) =>
            {
                var idx = IndexManager.NodeToIndex(node);
                var timeSlack = Time.SlackVar(idx);
                var durationSlack = Duration.SlackVar(idx);
                solver.Add(timeSlack == durationSlack);
            });
        }
        
        private void AddBreaks()
        {
            if (ProblemModel.CharacteristicDayCount == 1)
            {
                return;
            }
            LoopOverAllVehiclesOverAllDays((vehicle, vehicleIdx, day) =>
            {
                if (vehicle.RentalType != RentalType.Contract) return;
                AddBreakEndDayBreak(vehicleIdx, day); // removes 10 00 branches
                BindVehicleToDepot(vehicleIdx, day); // removes 500 000 branches
            });
        }
        
        private void LoopOverAllVehiclesOverAllDays(Action<IndexedVehicle, int, int> actionForVehicleAtDay)
        {
            VehicleManager.Vehicles.ForEach((vehicle, vehicleIdx) =>
            {
                    Enumerable.Range(0, LastDayIdx).ForEach(day =>
                    {
                        actionForVehicleAtDay(vehicle, vehicleIdx, day);
                    });
            });
            
        }

        private void AddBreakEndDayBreak(int vehicleIdx,int day)
        {
            var solver = Routing.solver();
            var endDepot = NodeManager.GetVehicleEndDepotForDay(vehicleIdx, day);
            var startDepot = NodeManager.GetStartNodeForEndNode(endDepot);
            
            var endDepotIdx = IndexManager.NodeToIndex(endDepot);
            var startDepotIdx = IndexManager.NodeToIndex(startDepot);
            var timeDimension = Time;

            if (startDepotIdx == -1 || endDepotIdx == -1)
            {
                return;
            }
            // Depot should be visited somewhere between day start and day end
            var active = Routing.ActiveVar(startDepotIdx) * Routing.ActiveVar(endDepotIdx);
            var endCumulVar = timeDimension.CumulVar(endDepotIdx); 
            endCumulVar.SetRange( 
                day * ProblemModel.DayDuration,
                (day + 1) * ProblemModel.DayDuration
            );
            // After visiting depot, vehicle should stay there at least min break duration
            
            var startCumlVar = timeDimension.CumulVar(startDepotIdx);
            
            startCumlVar.SetMin((day + 1) * ProblemModel.DayDuration);
            solver.Add(startCumlVar *  active
                       >= active * (endCumulVar + ProblemModel.MinBreakDuration));
           
        }

        private void BindVehicleToDepot(int vehicleIdx, int day)
        {
            var passedDayEndDepot = NodeManager.GetVehicleEndDepotForDay(vehicleIdx, day);
            var nextDayStartDepot = NodeManager.GetStartNodeForEndNode(passedDayEndDepot);
            new []{passedDayEndDepot, nextDayStartDepot}.ForEach(node =>
            {
                var depotIdx = IndexManager.NodeToIndex(node);
                Routing.VehicleVar(depotIdx).SetValues(new long[] {vehicleIdx});
            });
        }

        private void MakeVisitsOptional()
        {
            NodeManager.Visits.ForEach((_, node) =>
            {
                Routing.AddDisjunction(new[] {IndexManager.NodeToIndex(node)}, 1000000);
            });
            NodeManager.DepotNodes.ForEach(node =>
            {
                var depotIdx = IndexManager.NodeToIndex(node);
                var isEndDepot = depotIdx == -1;
                if (!isEndDepot)
                {
                    Routing.AddDisjunction(new[] {depotIdx}, 0);
                }
            });
        }
    }
}