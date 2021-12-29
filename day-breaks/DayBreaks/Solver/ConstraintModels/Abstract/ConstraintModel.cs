using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using Google.OrTools.ConstraintSolver;
using Tools.Extensions;

namespace DayBreaks.Solver.ConstraintModels.Abstract
{
    
    public abstract class ConstraintModel<TDimensionMatrix, TNodeManager> 
        where TDimensionMatrix: IDimensionMatrix
        where TNodeManager: INodeManager
    {
        public string DemandDim => "Demand";
        public string DistanceDim => "Distance";
        public string TimeDim => "Time";
        public string DurationDim => "Duration";
        public string VehicleCost => "VehicleCost";
        
        public readonly ProblemModel ProblemModel;
        public readonly VehicleManager VehicleManager;
        //
        public readonly TNodeManager NodeManager;
        public readonly TDimensionMatrix DimensionMatrix;
        public RoutingModel Routing { get; set; }
        public RoutingIndexManager IndexManager { get; set; }

        public ConstraintModel(ProblemModel problemModel, 
            VehicleManager vehicleManager, TNodeManager nodeManager, TDimensionMatrix dimensionMatrix)
        {
            ProblemModel = problemModel;
            VehicleManager = vehicleManager;
            NodeManager = nodeManager;
            DimensionMatrix = dimensionMatrix;
            IndexManager = CreateIndexManager(problemModel, vehicleManager, nodeManager);
            Routing = CreateRoutingModel(IndexManager);
            CreateDimensions(Routing);
            BindSpotVehiclesToDays();
            AddConstraints();
        }

        protected abstract RoutingIndexManager CreateIndexManager(ProblemModel problemModel, VehicleManager vehicleManager, TNodeManager nodeManager);
        protected abstract RoutingModel CreateRoutingModel(RoutingIndexManager routingIndexManager);
        protected virtual void AddConstraints() { }
        
        #region Dimensions
        public RoutingDimension Time => Routing.GetDimensionOrDie(TimeDim);
        public RoutingDimension Cost => Routing.GetDimensionOrDie(VehicleCost);
        public RoutingDimension Duration => Routing.GetDimensionOrDie(DurationDim);
        public RoutingDimension Demand => Routing.GetDimensionOrDie(DemandDim);
        #endregion
        
        private void CreateDimensions(RoutingModel routing)
        {
            #region Time - dimension to work with time windows for visits
                
            var timeCallback = routing.RegisterTransitCallback(CalculateTime);
            routing.AddDimension(
                timeCallback,
                ProblemModel.CharacteristicDayCount * ProblemModel.DayDuration, // vehicle can for a full day stay at depot
                ProblemModel.CharacteristicDayCount * ProblemModel.DayDuration,
                false, // time start at 0 in our model, but vehicle shouldn't depart from depot at that time,
                       // so cumulative also should not be fixed to zero
                TimeDim
            );
            #endregion
           
            #region Duration
            // Dimension required to control work time.
            // Not using set global span on time dimension, because time dimension 
            // is a time line containing joined time for all days
            var durationCallback = routing.RegisterTransitCallback(CalculateDuration);
            routing.AddDimension(
                durationCallback,
                 ProblemModel.MaxWorkDuration,
                ProblemModel.MaxWorkDuration,
                true,
                DurationDim
            );
            #endregion
            
            #region Demand
            var demandCallback = routing.RegisterUnaryTransitCallback(CalculateDemand);
            routing.AddDimensionWithVehicleCapacity(
                demandCallback,
                VehicleManager.MaxVehicleCapacity, // in order to unload vehicles as pointed in the comment https://github.com/google/or-tools/issues/943#issuecomment-441167043
                VehicleManager.Vehicles.Select(vehicle => vehicle.Capacity).ToArray(),
                true,
                DemandDim
            );
            #endregion

            #region Distance
            var distanceCallback = routing.RegisterTransitCallback(CalculateDistance);
            routing.AddDimension(
                distanceCallback,
                0,
                ProblemModel.MaxDistance,
                true,
                DistanceDim
            );
            #endregion
            
            #region Vehicle Costs
            var vehicleTransits = VehicleManager.Vehicles.Select(CreateVehicleTransitCallback).Select(callback => routing.RegisterTransitCallback(callback)).ToArray();
            routing.AddDimensionWithVehicleTransits(
                vehicleTransits,
                0,
                ProblemModel.Budget,
                true,
                VehicleCost
            );
            #endregion
            
            VehicleManager.Vehicles.ForEach((vehicle, idx) =>
            {
                routing.SetFixedCostOfVehicle(vehicle.MonthUsageCost, idx);
                routing.SetArcCostEvaluatorOfVehicle(vehicleTransits[idx], idx);
            });
            
        }


        #region Callbacks

        private LongLongToLong CreateVehicleTransitCallback(VehicleInstance vehicleInstance)
        {
            return (fromIdx, toIdx) =>
            {
                var distance = CalculateDistance(fromIdx, toIdx);
                return distance * vehicleInstance.CostPerKm;
            };
        }

        private long CalculateDemand(long atIdx)
        {
            var atNode = IndexManager.IndexToNode(atIdx);
            return DimensionMatrix.CalculateDemand(atNode);
        }

        private long CalculateDistance(long fromIdx, long toIdx)
        {
            var fromNode = IndexManager.IndexToNode(fromIdx);
            var toNode = IndexManager.IndexToNode(toIdx);
            return DimensionMatrix.CalculateDistance(fromNode, toNode);
        }

        private long CalculateTime(long fromIdx, long toIdx)
        {
            var fromNode = IndexManager.IndexToNode(fromIdx);
            var toNode = IndexManager.IndexToNode(toIdx);
            return DimensionMatrix.CalculateTime(fromNode, toNode);
        }

        private long CalculateDuration(long fromIdx, long toIdx)
        {
            var fromNode = IndexManager.IndexToNode(fromIdx);
            var toNode = IndexManager.IndexToNode(toIdx);
            return DimensionMatrix.CalculateDuration(fromNode, toNode);
        }
        #endregion

        private void BindSpotVehiclesToDays()
        {
            Enumerable.Range(0, ProblemModel.CharacteristicDayCount).ForEach(day =>
            {
                VehicleManager.SpotVehiclesForDay(day).ForEach(vehicle =>
                {
                    var startIdx = IndexManager.NodeToIndex(NodeManager.GetVehicleStartDepotForDay(vehicle.Index, day));
                    var endIdx = IndexManager.NodeToIndex(NodeManager.GetVehicleEndDepotForDay(vehicle.Index, day));
                    var (dayStart, dayEnd) = (day * ProblemModel.DayDuration, (day + 1) * ProblemModel.DayDuration);
                    if (startIdx == -1 || endIdx == -1)
                    {
                        return;
                    }
                    Time.CumulVar(startIdx).SetRange(dayStart, dayEnd);
                    Time.CumulVar(endIdx).SetRange(dayStart, dayEnd);
                    
                });
            });
        }
    }
}