using System;
using System.Linq;
using System.Numerics;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;

namespace SolomonBenchmark
{
    public static class CapacityPlanningSolver
    {
        public static string TIME => "TIME";
        public static string DISTANCE => "DISTANCE";
        public static string DEPOT_DEPARUTRE => "DEPORT DEPARTURE";

        public static void Solve()
        {
            // TODO: Process exited with 139
            var benchmark = Benchmark.fromFile("../../../data/c101.txt");
            var manager = new RoutingIndexManager(
                benchmark.NodesCount,
                benchmark.VehicleCount,
                benchmark.DepotNode
            );
            var routing = new RoutingModel(manager);
            
            var callbacks = new DimensionCallbacks(
                Distance: AddDistanceDimension(benchmark, manager, routing), 
                Time: AddTimeDimension(benchmark, manager, routing),
DepotDeparture: AddDepotDeparture(benchmark, manager, routing)
                );
            AddObjective(benchmark, manager, routing, callbacks);
            var assigment = RunSearch(benchmark, manager, routing);
            PrintSolution(assigment, benchmark, manager, routing);
        }

        private static int AddDistanceDimension(Benchmark benchmark, RoutingIndexManager manager, RoutingModel routing)
        {
            var distanceTransitCallback = routing.RegisterTransitCallback((fromIdx, toIdx) =>
            {
                var fromNode = manager.IndexToNode(fromIdx);
                var toNode = manager.IndexToNode(toIdx);
                return (long)Vector2.Distance(
                    benchmark.Customers[fromNode].Coords,
                    benchmark.Customers[toNode].Coords) + benchmark.Customers[fromNode].ServiceTime;
            });
            // TODO
            // Add descriptive global cost
            // Add custom function for cost calculation
            routing.AddDimension(
                distanceTransitCallback,
                0,
                benchmark.MaxDistance,
                true,
                DISTANCE
            );
            return distanceTransitCallback;
        }

        private static int AddTimeDimension(Benchmark benchmark, RoutingIndexManager manager, RoutingModel routing)
        {
            var timeTransitCallbackIdx = routing.RegisterTransitCallback((fromIdx, toIdx) =>
            {
                var fromNode = manager.IndexToNode(fromIdx);
                var toNode = manager.IndexToNode(toIdx);
                var distance =  (long)Vector2.Distance(
                    benchmark.Customers[fromNode].Coords,
                    benchmark.Customers[toNode].Coords
                );
                // As pointed in description 
                // https://www.sintef.no/projectweb/top/vrptw/solomon-benchmark/
                // Distance is Euclidean, and the value of travel time is equal to the value of distance between two nodes.
                return distance;
            });
            
            routing.AddDimension(
                timeTransitCallbackIdx,
                0,
                240,
                false,
                TIME);
            
            // TODO: GetMutableDimension ?? 
            var timeDimension = routing.GetDimensionOrDie(TIME);
            timeDimension.SetGlobalSpanCostCoefficient(100);
            benchmark.Customers.ForEach((customer, index) =>
            {
                var cumulVar = timeDimension.CumulVar(index);
              
                cumulVar.SetRange(customer.ReadyTime, customer.DueTime);
                // TODO: Set minimal time to stay
            });
            
            // Adding service time at points
            var solver = routing.solver();
            // TODO: model
            benchmark.Customers.ForEach(customer =>
            {
                // TODO: solver.Branches ??
                // TODO: solver.AddLocalSearchMonitor
                
                // Am using nodeToIndex correctly ??
                var customerIdx = manager.NodeToIndex(customer.Id);
                var timeCumul = timeDimension.CumulVar(customerIdx);
                solver.MakeFixedDurationIntervalVar(timeCumul, customer.ServiceTime, $"{customer.Id} service");
                
            });
            
            //solver.MakeCumulative() Popular 
            
            
            
            return timeTransitCallbackIdx;
        }

     

        // TODO: Looks like a very stupid way
        // Use IsActiveVarInstead
        private static int AddDepotDeparture(Benchmark benchmark, RoutingIndexManager manager, RoutingModel routing)
        {
            var depotDepartureCallback = routing.RegisterUnaryTransitCallback(idx =>
                manager.IndexToNode(idx) == benchmark.DepotNode ? 1 : 0);
            routing.AddDimension(depotDepartureCallback,
                0,
                benchmark.VehicleCount,
                true,
                DEPOT_DEPARUTRE);
            return depotDepartureCallback;
        }
        private static void AddObjective(Benchmark benchmark, RoutingIndexManager manager, RoutingModel routing, DimensionCallbacks callbacks)
        {
            
            // 1) Minimize number of vehicles
            // 2) Minimize total distance.
            // 3) Why some points are visited more than ones 
            // What is visit type policy
            // routing.GetVisitType()
            // routing.GetVisitTypePolicy()
            
            // ---
            // What is DisjunctionIndices - disjunction is something that tells solver that it can drop this index,
            // but in case of drop, it will receive some penalty. 
            // routing.GetDisjunctionIndices()
            var solver = routing.solver();
            
            var departureDim = routing.GetDimensionOrDie(DEPOT_DEPARUTRE);

            IntExpr GetCumulForVehicle(int vehicleIdx)
            {
                var vehicleStart = routing.Start(vehicleIdx);
                var vehicleCumul = departureDim.CumulVar(vehicleStart);
                return vehicleCumul;
            }
            var departuredVehicleSum = Enumerable.Range(1, benchmark.VehicleCount).Aggregate(
                GetCumulForVehicle(0),
                (acc, vehicleIdx) =>
                {
                    var cumul = GetCumulForVehicle(vehicleIdx);
                    return acc + cumul;
                });
            // routing.SetFixedCostOfVehicle(); // Another way to minimize vehicle count
            
            solver.MakeMin(departuredVehicleSum, 1); // TODO: step param?
            routing.SetArcCostEvaluatorOfAllVehicles(callbacks.Distance);
        }

        private static Assignment RunSearch(Benchmark benchmark, RoutingIndexManager manager, RoutingModel routing)
        {
            var searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 1 };
            
            return routing.SolveWithParameters(searchParameters);
        }
        

        
        private static void PrintSolution(Assignment assignment, Benchmark benchmark,
            RoutingIndexManager manager, RoutingModel routing) => 
            new AssigmentPrinter(assignment, benchmark, manager, routing).PrintForAll();
     
    }
}