using System.Collections.Generic;
using System.Linq;
using DayBreaks.Solver.ConstraintModels.Abstract;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.Solver.Solvers.Concrete
{
    public class VrpBasedFleetStructureSolver<TDimensionMatrix, TNodeManager> : IFleetStructureSolver
        where TDimensionMatrix: IDimensionMatrix
        where TNodeManager: INodeManager
    {
        private static string SolverName => "CPBasedFleetStructureSolver";

        private readonly IVrpSolver _vrpSolver;

        private readonly LocalSearchMetaheuristic.Types.Value _metaheuristic;
        
        public VrpBasedFleetStructureSolver(ConstraintModel<TDimensionMatrix, TNodeManager> constraintModel): this(constraintModel, LocalSearchMetaheuristic.Types.Value.SimulatedAnnealing)
        {}
        public VrpBasedFleetStructureSolver(ConstraintModel<TDimensionMatrix, TNodeManager> constraintModel,
            LocalSearchMetaheuristic.Types.Value metaheuristic, int timeLimit=60*10)
        {
            _metaheuristic = metaheuristic;
            _vrpSolver = new VrpSolver<TDimensionMatrix, TNodeManager>(constraintModel, SolverName);
        } 
        
        
        public FleetStructure Solve()
        {
            var solution = _vrpSolver.Solve(_metaheuristic);
            return solution is not null ? CreateFleetStructureSolution(solution) : null;
        }
        
        private FleetStructure CreateFleetStructureSolution(VrpSolution vrpSolution) => new(
            CreateFleetStructure(vrpSolution),
            GetEstimatedCost(vrpSolution)
        );
        
        private static IEnumerable<FleetPosition> CreateFleetStructure(VrpSolution solution)
        {
            return solution.Vehicles.GroupBy(
                vehicle => (vehicle.Name, vehicle.RentalType, vehicle.SourceDepotName, vehicle.StartDay),
                vehicle => vehicle,
                (key, elements) =>
                {
                    var vehicles = elements as VehicleInstance[] ?? elements.ToArray();
                    return new FleetPosition
                    {
                        Name = key.Name,
                        RentalType = key.RentalType,
                        Capacity = vehicles.First().Capacity,
                        CostPerKm = vehicles.First().CostPerKm,
                        SourceDepotName = key.SourceDepotName,
                        MonthUsageCost = vehicles.First().MonthUsageCost,
                        Count = vehicles.Length,
                        StartDay = key.StartDay
                    };
                });
        }

        private static long GetEstimatedCost(VrpSolution vrpSolution) =>
            vrpSolution.Vehicles.Sum(vehicle => vehicle.MonthUsageCost);
    }
}