using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.ConstraintModels.Abstract;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Tools.Extensions;

namespace DayBreaks.Solver.Solvers.Concrete
{
    public class VrpSolver<TDimensionMatrix, TNodeManager>: IVrpSolver
        where TDimensionMatrix: IDimensionMatrix
        where TNodeManager: INodeManager
    {
        private readonly ConstraintModel<TDimensionMatrix, TNodeManager> _constraintModel;
        private readonly string _solverName;
        private readonly int _timeLimit;
        
        public VrpSolver(ConstraintModel<TDimensionMatrix, TNodeManager> constraintModel, string solverName = "VRP solver", int timeLimit = 60 * 3) => (_constraintModel, _solverName, _timeLimit) = (constraintModel, solverName, timeLimit);
        private VehicleManager VehicleManager => _constraintModel.VehicleManager;
        private ProblemModel ProblemModel => _constraintModel.ProblemModel;
        private RoutingModel Routing => _constraintModel.Routing;
        private INodeManager NodeManager => _constraintModel.NodeManager;
        private RoutingIndexManager IndexManager => _constraintModel.IndexManager;

        public VrpSolution Solve(LocalSearchMetaheuristic.Types.Value metaheuristic) =>
            Solve(metaheuristic, _timeLimit);
        public VrpSolution Solve(LocalSearchMetaheuristic.Types.Value metaheuristic, int timeLimitInSeconds)
        {
            var searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.TimeLimit = new Duration
            {
                Seconds =  timeLimitInSeconds
            };
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.AllUnperformed;
            searchParameters.LocalSearchMetaheuristic = metaheuristic;
            searchParameters.LogSearch = true;
            var assignment = _constraintModel.Routing.SolveWithParameters(searchParameters);
            return assignment is null ? null : CreateSolution(assignment);
        }

        public VrpSolution Solve() => Solve(LocalSearchMetaheuristic.Types.Value.SimulatedAnnealing);
        
        private VrpSolution CreateSolution(Assignment assignment) => new ()
            {
                Depots = ProblemModel.Depots,
                Clients = ProblemModel.Clients,
                VehicleRoutes = CreateVehicleRoutes(assignment),
                NotVisitedClientsCount = CountNotVisitedClients(assignment),
                Vehicles = SelectUsedVehicles(assignment),
                UnusedVehicles = SelectUnusedVehicles(assignment),
                ObjectiveValue = assignment.ObjectiveValue()
            };
        
        private IEnumerable<IEnumerable<Point>> CreateVehicleRoutes(Assignment assigment) =>
            SelectUsedVehicles(assigment).Select(vehicle => CreateVehicleRoute(assigment, vehicle.Index));

        private IEnumerable<Point> CreateVehicleRoute(Assignment assignment, int vehicle)
        {
            var route = new List<Point>();
            var index = Routing.Start(vehicle);
            while (!Routing.IsEnd(index))
            {
                var node = _constraintModel.IndexManager.IndexToNode(index);
                route.Add(new Point
                {
                    Name = _constraintModel.NodeManager.GetPointForNode(node).Name,
                    Coords = _constraintModel.NodeManager.GetPointForNode(node).Coords
                });
                index = assignment.Value(Routing.NextVar(index));
            }

            return route;
        }
        
        private int CountNotVisitedClients(Assignment assignment)
        => Enumerable.Range(0, NodeManager.NodesCount)
                .Count(node => !NodeManager.IsDepotNode(node) && IsUnperformed(node, assignment));
        
        /// <summary>
        /// https://github1s.com/google/or-tools/blob/HEAD/ortools/constraint_solver/routing.cc#L4338-L4339
        /// </summary>
        private bool IsUnperformed(int node, Assignment assignment)
        {
            var index = IndexManager.NodeToIndex(node);
            return !Routing.IsStart(index) && !Routing.IsEnd(index) && 
                assignment.Value(Routing.NextVar(index)) == index;
        }

        
        private IEnumerable<IndexedVehicle> SelectUsedVehicles(Assignment assignment) => VehicleManager.Vehicles
            .Where(vehicle => Routing.IsVehicleUsed(assignment, vehicle.Index));
        private IEnumerable<IndexedVehicle> SelectUnusedVehicles(Assignment assignment) => VehicleManager.Vehicles
            .Where(vehicle => !Routing.IsVehicleUsed(assignment, vehicle.Index));
        
        
        // Debug
        private void PrintSolution(Assignment solution)
        {
            if (solution == null)
            {
                Console.WriteLine("Solution was not found");
                return;
            }
            Console.WriteLine($"Objective: {solution.ObjectiveValue()}");
            Console.WriteLine($"Days in model: {ProblemModel.CharacteristicDayCount}");
            Console.WriteLine("---------------------------------");
            PrintHumanReadableSolution(solution);
        }
        


        private void PrintHumanReadableSolution(Assignment solution)
        {
            Enumerable.Range(0, VehicleManager.Vehicles.Count()).ForEach(vehicle =>
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine($"Route for vehicle {vehicle}");
                var index = Routing.Start(vehicle);
                while (!Routing.IsEnd(index))
                {
                    PrintSolutionAtIdx(solution, index);
                    Console.Write("-> ");
                    index = solution.Value(Routing.NextVar(index));
                }

                
                PrintSolutionAtIdx(solution, index);
                Console.WriteLine();
            });
        }
        private void PrintSolutionAtIdx(Assignment solution, long idx)
        {
            
            var time = _constraintModel.Time.CumulVar(idx);
            var duration = _constraintModel.Duration.CumulVar(idx);
            var demand = _constraintModel.Demand.CumulVar(idx);
            var node = _constraintModel.IndexManager.IndexToNode(idx);
            Console.Write(
                $"{_constraintModel.NodeManager.GetPointForNode(node).Name} " +
                $"T:{solution.Value(time) / ProblemModel.DayDuration}-{(solution.Value(time) % ProblemModel.DayDuration) / 60:00}:{solution.Value(time) % 60:00} [dd-hh:mm] " +
                // "");
            //  $"with collected demand({solution.Min(demand)}) " +
            $"D: {solution.Value(duration) / 60 :00}:{solution.Value(time) % 60 :00} [hh:mm]");
        }

        private void DebugSolution(Assignment solution)
        {
            Console.WriteLine("-------------------------------");
            if (solution == null)
            {
                Console.WriteLine("Solution was not found");
                return;
            }
            var s = Routing.DebugOutputAssignment(solution, _constraintModel.TimeDim);
            Console.WriteLine(s);
            s = Routing.DebugOutputAssignment(solution, _constraintModel.DurationDim);
            Console.WriteLine(s);
        }
    }
}