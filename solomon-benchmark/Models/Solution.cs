using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.ConstraintSolver;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    public class Solution
    {
        public readonly long ObjectiveValue;
        public readonly List<Route> VehicleRoutes;
        public readonly Model Model;
        public readonly Assignment OriginalAssignment;
        public readonly int[] UsedVehicles;
        public int VehicleCount => UsedVehicles.Length;
        public Solution(Assignment assignment, RoutingModel routing,
            Model model,
            RoutingIndexManager manager)
        {
            ObjectiveValue = assignment.ObjectiveValue();
            VehicleRoutes = Enumerable.Range(0, model.VehicleCount).Select(vehIdx =>
            {
                var index = routing.Start(vehIdx);
                var result = new List<int>
                {
                    manager.IndexToNode(index)
                };
                while (!routing.IsEnd(index))
                {
                    index = assignment.Value(routing.NextVar(index));
                    result.Add(manager.IndexToNode(index));
                }

                return new Route(result);
            }).ToList();
            Model = model;
            OriginalAssignment = assignment;
            UsedVehicles =
                model.Vehicles.Where(i => routing.IsVehicleUsed(assignment, i)).ToArray();
        }
    }
}