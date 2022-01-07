using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DayBreaks.FleetStructureTesting.Models;
using DayBreaks.Mappers;
using DayBreaks.Mappers.Models;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.ConstraintSolver;
using Newtonsoft.Json;
using Tools.Extensions;
using Tools.Utils;
using FleetPosition = DayBreaks.FleetStructureTesting.Models.FleetPosition;

namespace DayBreaks.FleetStructureTesting
{
    public class VrpSimulator
    {
        private readonly IList<Visit> _possibleVisits;
        private readonly ProblemModel _problemModel;
        private readonly Random _random = new ();
        private readonly FleetToTest _fleetToTest;
        private readonly LocalSearchMetaheuristic.Types.Value _metaheuristic;
        private int MaxVisitsCount => _problemModel.Days.Max(day => day.Visits.Count());

        public VrpSimulator(string jsonModelPath, string predictedFleetStructurePath, LocalSearchMetaheuristic.Types.Value metaheuristic) : this(JsonUtils.LoadJson<ProblemModel>(jsonModelPath))
        {
            var fleetStructure = JsonUtils.LoadJson<FleetStructure>(predictedFleetStructurePath);
            var fleetModifications = CreateDepotVehicleModification(fleetStructure.FleetPositions);
            _fleetToTest = new FleetToTest(fleetStructure.EstimatedCost, fleetModifications);
            _metaheuristic = metaheuristic;
        }

        private static IEnumerable<FleetPosition> CreateDepotVehicleModification(IEnumerable<Solver.Solution.FleetPosition> fleetPositions)
        {
            return from position in fleetPositions
                where position.RentalType == RentalType.Contract
                select new FleetPosition(position.SourceDepotName, position.Name, position.Count);
        }


        private VrpSimulator(ProblemModel problemModel)
        {
            _possibleVisits = CreatePossibleVisits(problemModel.Days.SelectMany(day => day.Visits));
            _problemModel = problemModel;
        }

        private static IList<Visit> CreatePossibleVisits(IEnumerable<Problem.Visit> visits)
        {
            visits = visits.ToList();
            IEnumerable<T> SelectFromVisits<T>(Func<Problem.Visit, T> selector) =>
                visits.Select(selector).Distinct(); 

            var possibleStarts = SelectFromVisits(visit => visit.FromTime);
            var possibleDurations = SelectFromVisits(visit => visit.ToTime - visit.FromTime);
            var possibleDemands = SelectFromVisits(visit => visit.Demand);
            var possibleServiceTimes = SelectFromVisits(visit => visit.ServiceTime);
            var pointNames = SelectFromVisits(visit => visit.PointName);
            return (from start in possibleStarts
                from duration in possibleDurations
                from demand in possibleDemands
                from serviceTime in possibleServiceTimes
                from pointName in pointNames
                select new Visit
                {
                    PointName = pointName,
                    FromTime = start,
                    ToTime = start + duration,
                    ServiceTime = serviceTime,
                    Demand = demand
                }).ToList();
        }
        public FleetEvaluationResult Evaluate(Func<ProblemModel, IVrpSolver> solverFactory)
        {
            var (estimatedCost, fleet) = _fleetToTest;
            var dayResults = from characteristicDay in _problemModel.Days
                let dayType = DayTypesConverter.DetermineDayType(characteristicDay.Visits.Count(), MaxVisitsCount)
                from _ in Enumerable.Range(0, characteristicDay.Occurrences)
                select EvaluateForDay(dayType, fleet, solverFactory);
            
            return new FleetEvaluationResult(
                estimatedCost,
                dayResults.Sum(dayResult => dayResult.UsedVehicles.Sum(vehicle => vehicle.DayUsageCost))
            );
        }

        private DayEvaluationResult EvaluateForDay(DayType dayType, IEnumerable<FleetPosition> fleet, Func<ProblemModel, IVrpSolver> solverFactory)
        {
            fleet = fleet.ToList();
            var model = CreateOneDayProblemModel(dayType, fleet);
            var solver = solverFactory(model);
            var solution = solver.Solve(_metaheuristic);
            return solution is null
                ? new DayEvaluationResult(false, null, null)
                : new DayEvaluationResult(true, solution.NotVisitedClientsCount, solution.Vehicles);
        }
        
        private ProblemModel CreateOneDayProblemModel(DayType dayType, IEnumerable<FleetPosition> fleet)
        {
            var avgVisitsCount = dayType.CalculateVisitsCount(MaxVisitsCount);
            var visitsCount = _random.Next((int) (0.9 * avgVisitsCount), (int) (1.1*avgVisitsCount));
                
                _random.Next((int)_problemModel.Days.Average(day => day.Visits.Count()),
                _problemModel.Days.Max(day => day.Visits.Count()));
            var visits = _possibleVisits.ChooseRandomItems(visitsCount);
            var vehicles = CreateContractVehiclesFromFleet(fleet);
            return new ProblemModel
            {
                Budget = _problemModel.Budget,
                Clients = _problemModel.Clients,
                Depots = CreateVehiclesToDepotsAssignment(vehicles),
                Days = new[]
                {
                    new Day
                    {
                        Occurrences = 1,
                        Visits = visits
                    }
                },
                MaxDistance = _problemModel.MaxDistance,
                VehicleTypes = _problemModel.VehicleTypes
            };
        }

        private IEnumerable<Depot> CreateVehiclesToDepotsAssignment(IEnumerable<VehicleInstance> vehicleInstances)
        {
            var depotVehicleAssignments = vehicleInstances.GroupBy(
                vehicle => vehicle.SourceDepotName,
                vehicle => vehicle,
                (key, group) =>
                    new {depot = key, vehicles = group.GroupBy(vehicle => vehicle.Name).ToDictionary(vehicleGroup => vehicleGroup.Key,
                        vehicleGroup => vehicleGroup.Count())}
            ).ToDictionary(depotVehicleAssignment => depotVehicleAssignment.depot, depotVehicleAssignment => depotVehicleAssignment.vehicles);
            return _problemModel.Depots.Select(depot =>
                new Depot
                {
                    Vehicles = depotVehicleAssignments.TryGetValue(depot.Name, out var vehicles) ? vehicles : new Dictionary<string, int>(),
                    Coords = depot.Coords,
                    Name = depot.Name
                }).ToList();
        }


        private IEnumerable<VehicleInstance> CreateContractVehiclesFromFleet(IEnumerable<FleetPosition> depotVehiclesModifications)
        {
            return from position in depotVehiclesModifications
                from _ in Enumerable.Range(0, position.Count)
                let vehicleType = _problemModel.VehicleTypes.First(vehicle => vehicle.Name == position.VehicleName)
                select new VehicleInstance
                {
                    Capacity = vehicleType.Capacity,
                    CostPerKm = vehicleType.CostPerKm,
                    EndDay = 1,
                    Name = vehicleType.Name,
                    RentalType = RentalType.Contract,
                    SourceDepotName = position.DepotName,
                    MonthUsageCost = vehicleType.CostAsContractVehicle * _problemModel.DaysInMonth,
                    DayUsageCost = vehicleType.CostAsContractVehicle
                };
        }
    }
}