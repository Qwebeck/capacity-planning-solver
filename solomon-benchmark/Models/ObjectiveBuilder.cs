using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.ConstraintSolver;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    public record CostComponent(string Description, Func<Solution, long> CalculateForSolution);
    public class ObjectiveBuilder
    {
        private readonly Model _model;
        private readonly RoutingIndexManager _manager;
        private readonly RoutingModel _routingModel;
        private readonly CustomCostCalculator _costCalculator;
        private readonly List<CostComponent> _costComponents = new ();
        public ObjectiveBuilder(Model model, RoutingIndexManager manager, RoutingModel routingModel)
        {
            _model = model;
            _manager = manager;
            _routingModel = routingModel;
            _costCalculator = new CustomCostCalculator(_model, _manager, _routingModel);
        }

        public void SetGlobalSpanCostCoefficient(string dimensionName, long costCoefficient)
        {
            var dimension = _routingModel.GetDimensionOrDie(dimensionName);
            dimension.SetGlobalSpanCostCoefficient(costCoefficient);

            long CalculateCost(Solution solution) => _costCalculator.CalculateSetGlobalSpanCostCoefficient(solution, dimension, costCoefficient);
            _costComponents.Add(new CostComponent(
                $"Global span cost for dimension: {dimensionName}",
                CalculateCost
                ));
        }

        public void SetCumulVarSoftUpperBound(string dimensionName, long cumulVarIdx, long upperBound, long coefficient)
        {
            var dimension = _routingModel.GetDimensionOrDie(dimensionName);
            dimension.SetCumulVarSoftUpperBound(cumulVarIdx, upperBound, coefficient);

            long CalculateCost(Solution solution) =>
                _costCalculator.CalculateSetCumulVarSoftUpperBound(solution, new[]
                {
                    dimension.CumulVar(cumulVarIdx)
                }, upperBound, new[] {coefficient});

            _costComponents.Add(
                new CostComponent($"Cumulative var upper bound cost for dimension {dimensionName} and node: {_manager.IndexToNode(cumulVarIdx)}",
                    CalculateCost));
        }
        
        public void SetCumulVarSoftLowerBound(string dimensionName, long cumulVarIdx, long upperBound, long coefficient)
        {
            var dimension = _routingModel.GetDimensionOrDie(dimensionName);
            dimension.SetCumulVarSoftLowerBound(cumulVarIdx, upperBound, coefficient);

            long CalculateCost(Solution solution) =>
                _costCalculator.CalculateSetCumulVarSoftLowerBound(solution, new[]
                {
                    dimension.CumulVar(cumulVarIdx)
                }, upperBound, new[] {coefficient});

            _costComponents.Add(
                new CostComponent($"Cumulative var lower bound cost for dimension {dimensionName} and node: {_manager.IndexToNode(cumulVarIdx)}",
                    CalculateCost));
        }


        public void SetFixedCostOfAllVehicles(long cost)
        {
            _routingModel.SetFixedCostOfAllVehicles(cost);
            long CalculateCost(Solution solution) => _costCalculator.CalculateFixedCostOfAllVehicles(solution, cost);
            _costComponents.Add(new CostComponent("Cost of all vehicles", CalculateCost));
        }

        public void SetArcCostEvaluatorOfAllVehicles(Callback<Func<long, long, long>> costEvaluator)
        {
            _routingModel.SetArcCostEvaluatorOfAllVehicles(costEvaluator.Idx);

            long CalculateCost(Solution solution) =>
                _costCalculator.CalculateArcCostEvaluatorOfAllVehicles(solution, costEvaluator.Evaluator);
            _costComponents.Add(new CostComponent("Arc cost evaluator", CalculateCost));
        }


        public string DescribeSolution(Solution solution)
        {
            var costDescriptions = _costComponents.OrderBy((c) => c.Description)
                .Select(c => $"{c.Description}: {c.CalculateForSolution(solution)}");
            return string.Join('\n', costDescriptions) + $"\n Full cost: {_costComponents.Sum(c => c.CalculateForSolution(solution))}";
        }
    }
}