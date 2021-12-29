using System;
using System.Linq;
using System.Numerics;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    // TODO
    // [] Add function that calculates custom costs
    // https://groups.google.com/g/or-tools-discuss/c/YCUNTmWr8Us/m/IJhb3g0JAQAJ?utm_medium=email&utm_source=footer&pli=1
    // -- next week
    // [] SetAmortizedCost <-
    // [] SetFixedCostPerVehicle
    // [] Add dim distanceFromDepot and Add SoftUpperBound for veh
    // -----
    // [] SetArcCostEvaluatorOfVehicle
    // [] SetFixedCostOfVehicle <- check if only used vehicles are in cost
    // [x] AddDisjunction <- TODO test more precicely    
    // [x] SetCumulVarSoftUpperBound
    // [x] SetCumulVarSoftLowerBound
    
    // [] AddSoftSameVehicleConstraint !
    // --
    // [] Wiki
    // [X] issue
    // --
    // [] SetBreakIntervalsOfVehicle <- simulate multiple days in problem.
    // 
    // [x] Add middleware, that allows to add custom costs.
    // --- 
    // [x] Add objective builder.
    // Objective builder will expose an interface that will allow to add objective parts.
    // Thanks to that is also would be easier to me to calculate a custom objective cost: when particular cost coefficient will be added, cost caluclator will be added to Dictionary.
    // Each cost calculator should have description.
    // --- 
    // 1. Set cost evaluator of all vehicles and set cost evaluator of one vehicle ?
    // How do they work together. Does one removes another.
    // 2. SetAmortizedCost.
    // 3. AddDimensionDependentDimensionWithVehicleCapacity
    // https://developers.google.com/optimization/reference/constraint_solver/routing/RoutingModel?hl=en
    // https://groups.google.com/g/or-tools-discuss/c/rNILvzUT21U
 
    public class CapacityPlanningSolver
    {
        #region CONSTS
        static string TIME => "TIME";
        static string DISTANCE => "DISTANCE";
        static string DEMAND => "DEMAND";
        static long FIXED_VEHICLE_COST => 100;
        static long TIME_COEFFICIENT => 100;
        #endregion

        #region members
        private readonly RoutingModel _routingModel;
        private readonly RoutingIndexManager _manager;
        private readonly Model _model;
        private readonly ObjectiveBuilder _objectiveBuilder;

        public string DescribeSolution(Solution solution) => _objectiveBuilder.DescribeSolution(solution);
        public CapacityPlanningSolver(Model model)
        {
            _model = model;
            _manager = new RoutingIndexManager(
                model.NodesCount,
                model.VehicleCount,
                model.DepotNode);
            _routingModel = new RoutingModel(_manager);
            _objectiveBuilder = new ObjectiveBuilder(_model, _manager, _routingModel);
        }
        #endregion
        public Solution Solve()
        {
            var callbacks = new DimensionCallbacks(
                Distance: AddDistanceDimension(),
                Time: AddTimeDimension(),
                Demand: AddDemandDimension()
            );
            AddObjective(callbacks);
            var assigment = RunSearch();
            return new Solution(assigment, _routingModel, _model, _manager);
        }

        #region Dimension functions
        private Callback<Func<long, long>> AddDemandDimension()
        {
            long GetDemand(long atIdx)
            {
                var node = _manager.IndexToNode(atIdx);
                return _model.Customers[node].Demand;
            }
            var demandDimensionCallbackIdx = _routingModel.RegisterUnaryTransitCallback(GetDemand);
            _routingModel.AddDimension(demandDimensionCallbackIdx, 0, _model.Capacity, false, DEMAND);
            return new Callback<Func<long, long>>
            {
                Idx = demandDimensionCallbackIdx,
                Evaluator = GetDemand
            };
        }

        
        private Callback<Func<long, long, long>> AddDistanceDimension()
        {
            long CalculateDistance(long fromIdx, long toIdx)
            {
                var fromNode = _manager.IndexToNode(fromIdx);
                var toNode = _manager.IndexToNode(toIdx);
                return (long) Vector2.Distance(
                    _model.Customers[fromNode].Coords,
                    _model.Customers[toNode].Coords);

            }

            var distanceTransitCallback = _routingModel.RegisterTransitCallback(CalculateDistance);
            _routingModel.AddDimension(
                distanceTransitCallback,
                _model.MaxDistance,
                _model.MaxDistance,
                true,
                DISTANCE
            );
            return new Callback<Func<long, long, long>>
            {
                Idx = distanceTransitCallback,
                Evaluator = CalculateDistance
            };
        }

        private Callback<Func<long, long, long>> AddTimeDimension()
        {
            long CalculateTransitDuration(long fromIdx, long toIdx)
            {
                var fromNode = _manager.IndexToNode(fromIdx);
                var toNode = _manager.IndexToNode(toIdx);
                var distance = (long) Vector2.Distance(
                    _model.Customers[fromNode].Coords,
                    _model.Customers[toNode].Coords
                ) + _model.Customers[fromNode].ServiceTime; 
                return distance;
            }
            var timeTransitCallbackIdx = _routingModel.RegisterTransitCallback(CalculateTransitDuration);

            _routingModel.AddDimension(
                timeTransitCallbackIdx,
                0,
                2400,
                false,
                TIME);

            
            var timeDimension = _routingModel.GetDimensionOrDie(TIME);
            _objectiveBuilder.SetGlobalSpanCostCoefficient(TIME, TIME_COEFFICIENT);
            _model.Customers.ForEach(customer =>
            {
                var idx = _manager.NodeToIndex(customer.Id);
                timeDimension.CumulVar(idx).SetRange(customer.ReadyTime, customer.DueTime);
                _objectiveBuilder.SetCumulVarSoftUpperBound(TIME, idx, customer.ReadyTime ,1);
            });
            
            _model.Customers.ForEach(customer =>
            {
                var idx = _manager.NodeToIndex(customer.Id);
                _routingModel.AddVariableMinimizedByFinalizer(timeDimension.CumulVar(idx));
            });

            return new Callback<Func<long, long, long>>
            {
                Idx = timeTransitCallbackIdx,
                Evaluator = CalculateTransitDuration
            };
            
        }
        
        #endregion
        
        private void AddObjective(DimensionCallbacks callbacks)
        {
            _objectiveBuilder.SetFixedCostOfAllVehicles(FIXED_VEHICLE_COST); // Sets penalize for each vehicle, so now solver will be carefully using new vehicle in the solution.
            _objectiveBuilder.SetArcCostEvaluatorOfAllVehicles(callbacks.Distance);
            
        }


        private Assignment RunSearch()
        {
            var searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration
            {
                Seconds = 1
            };

            return _routingModel.SolveWithParameters(searchParameters);
        }
    }
}
