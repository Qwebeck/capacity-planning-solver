using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Newtonsoft.Json;
using Tools.Extensions;
using Tools.Utils;

namespace DayBreaks.FleetStructureTesting
{

    public record DepotVehiclesModification(string DepotName, string VehicleName, int Count);
    public record FleetToTest(long EstimatedCost, IEnumerable<DepotVehiclesModification> Modifications);
    internal record DayEvaluationResult(bool IsSolved, int? NotVisitedClientsCount, IEnumerable<VehicleInstance> UsedVehicles);

    public record FleetEvaluationResult(
        long PredictedFleetCost,
        long RealFleetCost)
    {
        public long CostOverestimate => PredictedFleetCost - RealFleetCost;
    }
    public class FleetStructureEvaluator
    {
        private readonly IList<Visit> _possibleVisits;
        private readonly ProblemModel _problemModel;
        private readonly Random _random = new ();
        private readonly FleetToTest _fleetToTest;

        private int DayCount => _problemModel.DaysInMonth;
        

        public FleetStructureEvaluator(string jsonModelPath, string predictedFleetStructurePath) : this(JsonUtils.LoadJson<ProblemModel>(jsonModelPath))
        {
            var fleetStructure = JsonUtils.LoadJson<FleetStructure>(predictedFleetStructurePath);
            var fleetModifications = CreateDepotVehicleModification(fleetStructure.FleetPositions);
            _fleetToTest = new FleetToTest(fleetStructure.EstimatedCost, fleetModifications);
        }

        private static IEnumerable<DepotVehiclesModification> CreateDepotVehicleModification(IEnumerable<FleetPosition> fleetPositions)
        {
            return from position in fleetPositions
                where position.RentalType == RentalType.Contract
                select new DepotVehiclesModification(position.SourceDepotName, position.Name, position.Count);
        }


        public FleetStructureEvaluator(ProblemModel problemModel)
        {
            _possibleVisits = problemModel.Days.SelectMany(day => day.Visits).ToList();
            _problemModel = problemModel;
        }
        
        public FleetEvaluationResult Evaluate(Func<ProblemModel, IVrpSolver> solverFactory)
        {
            var (estimatedCost, vehiclesModifications) = _fleetToTest;
            var dayResults = Enumerable.Range(0, DayCount).Select(_ => EvaluateForDay(vehiclesModifications, solverFactory));
            return new FleetEvaluationResult(
                estimatedCost,
                dayResults.Sum(dayResult => dayResult.UsedVehicles.Sum(vehicle => vehicle.MonthUsageCost))
            );
        }

        private DayEvaluationResult EvaluateForDay(IEnumerable<DepotVehiclesModification> depotModifications, Func<ProblemModel, IVrpSolver> solverFactory)
        {
            depotModifications = depotModifications.ToList();
            var model = CreateOneDayProblemModel(depotModifications);
            var solver = solverFactory(model);
            var solution = solver.Solve();
            return solution is null
                ? new DayEvaluationResult(false, null, null)
                : new DayEvaluationResult(true, solution.NotVisitedClientsCount, solution.Vehicles);
        }
        
        private ProblemModel CreateOneDayProblemModel(IEnumerable<DepotVehiclesModification> depotVehiclesModifications)
        {
            var visitsAmount = _random.Next(0, _possibleVisits.Count);
            var visits = _possibleVisits.ChooseRandomItems(visitsAmount).Distinct(new VisitsComparer());
            var vehicles = CreateContractVehiclesFromFleet(depotVehiclesModifications);

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


        private IEnumerable<VehicleInstance> CreateContractVehiclesFromFleet(IEnumerable<DepotVehiclesModification> depotVehiclesModifications)
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
                    MonthUsageCost = vehicleType.CostAsContractVehicle
                };
        }
    }

    internal class VisitsComparer : IEqualityComparer<Visit>
    {
        public bool Equals(Visit x, Visit y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.PointName == y.PointName && x.FromTime == y.FromTime && x.ToTime == y.ToTime;
        }

        public int GetHashCode(Visit obj)
        {
            return HashCode.Combine(obj.PointName, obj.FromTime, obj.ToTime);
        }
    }
}