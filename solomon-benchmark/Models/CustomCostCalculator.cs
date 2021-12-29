using System;
using System.Linq;
using Google.OrTools.ConstraintSolver;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    public class CustomCostCalculator
    {
        private readonly Model _model;
        private readonly RoutingIndexManager _manager;
        private readonly RoutingModel _routing;

        public CustomCostCalculator(Model model, RoutingIndexManager manager, RoutingModel routing)
            => (_model, _manager, _routing) = (model, manager, routing); 
        
        public long CalculateArcCostEvaluatorOfAllVehicles(Solution solution, Func<long, long, long> costEvaluator)
        {
          
            var cost = solution.VehicleRoutes.Sum(route => 
                route.CalculateCost(_manager, costEvaluator));
            return cost;
        }

        public long CalculateFixedCostOfAllVehicles(Solution solution, long vehicleCost)
        {
            return solution.VehicleCount * vehicleCost;
        }

        public long CalculateDisjunction(Solution solution, long[] optionalIndices, long penalty, long maxCardinality)
        {
            // Test in few different scenarios
            var allVisitedCustomers = solution.VehicleRoutes.SelectMany(r => _manager.NodesToIndices(r.RouteNodes.ToArray())).ToArray();
            var optionalVisitedCount = optionalIndices.Intersect(allVisitedCustomers).Count();
            return penalty * (maxCardinality - optionalVisitedCount);
        }

        public long CalculateSetCumulVarSoftUpperBound(Solution solution, IntVar[] variables, long upperBound, long[] coefficients)
        {
            return variables.Zip(coefficients).Sum(((IntVar var, long coeff) pair) => 
                Math.Max(0, (solution.OriginalAssignment.Value(pair.var) - upperBound) * pair.coeff));
        }
        
        
        public long CalculateSetCumulVarSoftLowerBound(Solution solution, IntVar[] variables, long lowerBound, long[] coefficients)
        {
            return variables.Zip(coefficients).Sum(((IntVar var, long coeff) pair) => 
                Math.Max(0, (lowerBound - solution.OriginalAssignment.Value(pair.var)) * pair.coeff));
        }


        public long SetAmortizedCostFactorsOfAllVehicles(Solution solution, long[] linearCostFactors, long[] quadraticCostFactors, Func<long, long, long> costEvaluator)
        {
            return _model.Vehicles.Sum(vehIdx => 
                linearCostFactors[vehIdx] - quadraticCostFactors[vehIdx] 
                            * solution.VehicleRoutes[vehIdx].CalculateCost(_manager, costEvaluator)
            );
        }

        public long CalculateSetGlobalSpanCostCoefficient(Solution solution, RoutingDimension dimension, long coefficient)
        {
            
            var startValues = solution.UsedVehicles.Select(vehIdx =>
            {  
                var end = dimension.CumulVar(_routing.Start(vehIdx));
                return solution.OriginalAssignment.Value(end);
                
            }).ToArray();
            var endValues = solution.UsedVehicles.Select(vehIdx =>
            {
                var end = dimension.CumulVar(_routing.End(vehIdx));
                return solution.OriginalAssignment.Value(end);
            }).ToArray();
            
            return coefficient * (endValues.Max() - startValues.Min());
        }
    }
}