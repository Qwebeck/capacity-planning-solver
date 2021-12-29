using System;
using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.LinearSolver;
using Tools.Extensions;
using OrToolsLinearSolver = Google.OrTools.LinearSolver.Solver;

namespace DayBreaks.Solver.Solvers.Concrete
{

    record VehicleParams(string Name, RentalType RentalType, long Capacity, long UsageCost);
    public class LinearSolver: IFleetStructureSolver
    {
        private readonly OrToolsLinearSolver _solver;
        private readonly Dictionary<Variable, VehicleParams> _variableToVehicleParams = new ();

        public LinearSolver(ProblemModel problemModel)
        {
            _solver = OrToolsLinearSolver.CreateSolver("SCIP");
            var vehicleAmountsVars = CreateVehicleAmountVariables(problemModel).ToList();
            
            var availableCapacitiesExpression = vehicleAmountsVars.LinearSolverSum((acc, variable) =>
                acc + variable * _variableToVehicleParams[variable].Capacity);
            long totalDemand = problemModel.Days.Sum(day => day.Visits.Sum(visit => visit.Demand));

            var totalCost = vehicleAmountsVars.LinearSolverSum((acc, variable) =>
                acc + variable * _variableToVehicleParams[variable].UsageCost);
            
            _solver.Add(availableCapacitiesExpression >= totalDemand);
            _solver.Minimize(totalCost);
        }

        private IEnumerable<Variable> CreateVehicleAmountVariables(ProblemModel problemModel)
        {
            var vehicleManager = new VehicleManager(problemModel);
            var groupedVehicles = vehicleManager.Vehicles.GroupBy(
                vehicle => new {vehicle.RentalType, vehicle.StartDay, vehicle.Name},
                vehicle => vehicle);

            var amountVariables = new List<Variable>();
            groupedVehicles.ForEach(group =>
            {
                var key = group.Key;
                var maxAmount = group.Count();
                var variable = _solver.MakeIntVar(0.0, maxAmount,
                    $"Amount of vehicle {key.Name} of rental type {key.RentalType} for day {key.StartDay}");
                amountVariables.Add(variable);
                var typicalVehicle = group.First();
                var vehicleParams = new VehicleParams(typicalVehicle.Name, typicalVehicle.RentalType, typicalVehicle.Capacity, typicalVehicle.MonthUsageCost);
                _variableToVehicleParams[variable] = vehicleParams;
            });
            return amountVariables;
        }
        
        public FleetStructure Solve()
        {
            var status = _solver.Solve();
            return ShouldAcceptSolution(status) ? CreateSolution() : null;
        }

        private static bool ShouldAcceptSolution(OrToolsLinearSolver.ResultStatus status) => status switch
        {
            OrToolsLinearSolver.ResultStatus.OPTIMAL => true,
            OrToolsLinearSolver.ResultStatus.FEASIBLE => true,
            _ => false
        };

        private Solution.FleetStructure CreateSolution()
        {
            return new Solution.FleetStructure(CreateFleetStructure(), Convert.ToInt64(_solver.Objective().Value()));
        }

        private IEnumerable<FleetPosition> CreateFleetStructure() =>  _variableToVehicleParams.Keys
            .Where(variable => variable.SolutionValue() != 0)
            .Select(variable =>
            {
                var vehicle = _variableToVehicleParams[variable];
                return new FleetPosition
                {
                    Name = vehicle.Name,
                    Capacity = vehicle.Capacity,
                    Count = Convert.ToInt32(variable.SolutionValue()),
                    MonthUsageCost = vehicle.UsageCost,
                    RentalType = vehicle.RentalType
                };
            });
        
    }
}