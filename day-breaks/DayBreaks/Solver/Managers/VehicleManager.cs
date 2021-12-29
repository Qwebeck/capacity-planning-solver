using System;
using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using Tools.Extensions;

namespace DayBreaks.Solver
{
    public class VehicleManager
    {
        private readonly Dictionary<string, VehicleType> _vehicleNameToVehicleType;
        public readonly long MaxVehicleCapacity;
        private readonly IEnumerable<VehicleInstance> _vehiclesInstances;
        private readonly int _contractVehiclesCount;

        // TODO ADD AUTOMAPPER!!!!!
        public IEnumerable<IndexedVehicle> Vehicles => _vehiclesInstances.Select((veh, idx) => new IndexedVehicle
        {
            RentalType = veh.RentalType,
            Name = veh.Name,
            Capacity = veh.Capacity,
            MonthUsageCost = veh.MonthUsageCost,
            CostPerKm = veh.CostPerKm,
            SourceDepotName = veh.SourceDepotName,
            StartDay = veh.StartDay,
            EndDay = veh.EndDay,
            Index = idx,
            DayUsageCost = veh.DayUsageCost
        });

        private const int SpotVehiclesPerDayCount = 5;

        public IEnumerable<IndexedVehicle> ContractVehicles => Vehicles.Take(_contractVehiclesCount);
        public IEnumerable<IndexedVehicle> SpotVehiclesForDay(int day) =>
            Vehicles.Skip(_contractVehiclesCount + SpotVehiclesPerDayCount * day).Take(SpotVehiclesPerDayCount);
        public VehicleManager(ProblemModel problemModel)
        {
            _vehicleNameToVehicleType = problemModel
                .VehicleTypes
                .ToDictionary(vehicle => vehicle.Name, vehicle => vehicle);
            var contractVehicles = CreateContractVehicles(problemModel).ToList();
            _contractVehiclesCount = contractVehicles.Count;
            _vehiclesInstances = contractVehicles.Concat(CreateSpotVehicles(problemModel));
            MaxVehicleCapacity = Vehicles.Max(vehicle => vehicle.Capacity);
        }
        
        private IEnumerable<VehicleInstance> CreateContractVehicles(ProblemModel problemModel) => 
            from indexedDepot in problemModel.Depots.Select((d,idx) => new {idx, d})
                let depotIdx = indexedDepot.idx
                let depot = indexedDepot.d
                from vehicleTypeName in depot.Vehicles.Keys
                let vehicleType = _vehicleNameToVehicleType[vehicleTypeName]
                from _ in Enumerable.Range(0, depot.Vehicles[vehicleTypeName])
                select new VehicleInstance
                {
                    RentalType = RentalType.Contract,
                    Name = vehicleType.Name,
                    Capacity = vehicleType.Capacity,
                    CostPerKm = vehicleType.CostPerKm,
                    MonthUsageCost = vehicleType.CostAsContractVehicle * problemModel.DaysInMonth,
                    SourceDepotName = depot.Name,
                    DayUsageCost = vehicleType.CostAsContractVehicle,
                    StartDay = 0,
                    EndDay = problemModel.CharacteristicDayCount - 1 // because indexing starts at zero
                };
        

        
        /// <summary>
        /// For each visit in model adds an available spot Vehicle
        /// </summary>
        /// <param name="problemModel"></param>
        /// <returns></returns>
        private static IEnumerable<VehicleInstance> CreateSpotVehicles(ProblemModel problemModel)
        {
            var spotDepot = problemModel.Depots.First();
            var spotVehicleType = problemModel.VehicleTypes.First();
            return (from indexedDay in problemModel.Days.Select((day, idx) => new {day, idx})
                from _ in Enumerable.Range(0, SpotVehiclesPerDayCount)
                 select new VehicleInstance
                 {
                     RentalType = RentalType.Spot,
                     Name = spotVehicleType.Name,
                     Capacity = spotVehicleType.Capacity,
                     MonthUsageCost = spotVehicleType.CostAsSpotVehicle * indexedDay.day.Occurrences,
                     CostPerKm = spotVehicleType.CostPerKm,
                     SourceDepotName = spotDepot.Name,
                     DayUsageCost = spotVehicleType.CostAsSpotVehicle,
                     StartDay = indexedDay.idx, // spot ends in the same day as starts
                     EndDay = indexedDay.idx
                 }).ToList();
        }
    }
}