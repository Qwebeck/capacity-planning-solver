using System;
using System.Linq;
using Google.OrTools.ConstraintSolver;

namespace SolomonBenchmark
{
    public record AssigmentPrinter(Assignment Assignment, SolomonBenchmark.Benchmark Benchmark,
        RoutingIndexManager Manager, RoutingModel Routing)
    {
        public void PrintForAll() => Enumerable.Range(0, 
            Benchmark.VehicleCount).ForEach(PrintAssigmentForVehicle);

        public void PrintAssigmentForVehicle(int vehicleIndex)
        {
            Console.WriteLine(new String('-', 20));
            Console.WriteLine($"Solution for vehicle: {vehicleIndex}");
            var index = Routing.Start(vehicleIndex);
            while (!Routing.IsEnd(index))
            {
                Console.WriteLine(" ");
                var node = Manager.IndexToNode(index);
                PrintRouteInfo(fromNode: node);
                PrintDistance(fromNode: node);
                PrintTimeWindow(node);
                index = Assignment.Value(Routing.NextVar(index));
            }
            Console.WriteLine(" ");
        }
        
        private void PrintRouteInfo(int fromNode)
        {
            var idx = Manager.NodeToIndex(fromNode);
            var toNode = Assignment.Value(Routing.NextVar(idx)); // TODO: Why next var returns next idx?? How it acts if all nodes end
            Console.Write($"{fromNode} -> {toNode}");
        }

        private void PrintDimensionOnTransit(string dimensionName, int fromNode,
            Action<IntVar> printFunction) => PrintDimensionOnTransit(dimensionName, fromNode,
            (dimension, fromIdx) =>
            {
                var cumul = dimension.CumulVar(fromIdx); 
                printFunction(cumul);
            });

        private void PrintDimensionOnTransit(string dimensionName, int fromNode, Action<RoutingDimension, long> printFunction)
        {
            var fromIdx = Manager.NodeToIndex(fromNode);
            var dimension = Routing.GetDimensionOrDie(dimensionName);
            printFunction(dimension, fromIdx);
        }

        private void PrintDistance(int fromNode) => PrintDimensionOnTransit(CapacityPlanningSolver.DISTANCE, fromNode, (cumul) =>
            {
                // TODO: why we read values with max and min
                var distance = Assignment.Min(cumul) - Assignment.Max(cumul);
                Console.Write($" dist: {distance} ");
            });
        
        

        private void PrintTimeWindow(int fromNode) => PrintDimensionOnTransit(CapacityPlanningSolver.TIME, fromNode, 
            (dimension, fromIdx) =>
            {
                var startTime = dimension.CumulVar(fromIdx);
                var nextFromIdx = Routing.NextVar(fromIdx);
                var endTime = dimension.CumulVar(Assignment.Value(
                    nextFromIdx // TODO: returns closes next point after index?
                    )); // TODO: returns value assigned to this point during solving this problem
                
                
                
                var (from, to) = (
                    Assignment.Value(startTime),
                    Assignment.Value(
                        endTime)); // TODO are [docs](https://developers.google.com/optimization/routing/vrptw?authuser=2) wrong ??
                                    // Difference between MaxValue and MinValue
                                    // Previous version
                // Assigment.Min(startTime), Assigment.Max(startTime); 
                var duration =  to - from;
                // var serviceTime = nextFrom - to; 
                Console.Write($" time: [{from}, {to}], duration: {duration}");
            });
        
    }
}