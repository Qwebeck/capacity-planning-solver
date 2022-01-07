using System;
using System.Collections.Generic;
using System.Linq;
using DayBreaks.Mappers.Models;
using DayBreaks.Problem;
using SolomonBenchmark.Models;
using Tools.Extensions;
using SolomonBenchmarkModel = SolomonBenchmark.Models.Model;

namespace DayBreaks.Mappers
{
    public static class SolomonBenchmarkToCustomModelMapper
    {

        private static readonly Random DefaultRandom = new(42);
        private static int VehicleCountForEachType => 15; 
        private static readonly IEnumerable<VehicleType> DefaultVehicleTypes = new[]
        {
            new VehicleType
            {
                Capacity = 100,
                CostAsContractVehicle = 100,
                CostAsSpotVehicle = 150,
                CostPerKm = 1,
                Name = "small"
            },
            new VehicleType
            {
                Capacity = 200,
                CostAsContractVehicle = 200,
                CostAsSpotVehicle = 300,
                CostPerKm = 2,
                Name = "medium"
            },
            new VehicleType
            {
                Capacity = 300,
                CostAsContractVehicle = 300,
                CostAsSpotVehicle = 500,
                CostPerKm = 3,
                Name = "large"
            }
        };

        public static ProblemModel CreateModelFromSolomonBenchmarkFile(string path, CharacteristicDayDescription description,
            IEnumerable<VehicleType> vehicleTypes = null, Random random = null)
            => CreateModelFromSolomonBenchmarkFile(path, new [] {description}, vehicleTypes, random);

        public static ProblemModel CreateModelFromSolomonBenchmarkFile(string path, IEnumerable<CharacteristicDayDescription> descriptions, 
            IEnumerable<VehicleType> vehicleTypes=null, Random random=null)
        {
            random ??= DefaultRandom;
            vehicleTypes ??= DefaultVehicleTypes;
            var solomonBenchmarkModel = SolomonBenchmarkModel.fromFile(path);
            var visitIntervals = CreateVisitMutations(solomonBenchmarkModel.Customers).ToList();    
            return new ProblemModel
            {
                Budget = 10000000,
                MaxDistance = 10000000,
                Clients = GetClientsFromModel(solomonBenchmarkModel).ToList(),
                Depots = GetDepotsFromModel(solomonBenchmarkModel, vehicleTypes).ToList(),
                Days = GetVisitsFromModel(solomonBenchmarkModel, visitIntervals, descriptions, random).ToList(),
                VehicleTypes = vehicleTypes.ToList()
            };
        }

        private static IEnumerable<Client> GetClientsFromModel(SolomonBenchmarkModel model)
        {
            return model.Customers.Where(c => !model.IsDepot(c)).Select(customer =>
                new Client
                {
                    Coords = customer.Coords,
                    Name = $"customer {customer.Id}"
                }).ToList();
        }
        
        private static IEnumerable<Depot> GetDepotsFromModel(SolomonBenchmarkModel model, IEnumerable<VehicleType> vehicleTypes)
        {
            return model.Customers.Where(c => model.IsDepot(c)).Select(depot => new Depot
            {
                Coords = depot.Coords,
                Name = $"depot {depot.Id}",
                Vehicles = vehicleTypes.ToDictionary(vehType => vehType.Name,  _ => VehicleCountForEachType)
            }).ToList();
        }
        
        private static IEnumerable<Day> GetVisitsFromModel(SolomonBenchmarkModel model, IEnumerable<VisitMutation> timeIntervals, IEnumerable<CharacteristicDayDescription> descriptions, Random random)
        {
            var dayCount = descriptions.Count();
            var days = descriptions.Select(description => new Day
            {
                Visits = CreateVisitsForDay(model, timeIntervals, description.DayType, random).ToList(),
                Occurrences = description.Occurrences
            });
            return days.ToList();
        }

        
        private static IEnumerable<Problem.Visit> CreateVisitsForDay(SolomonBenchmarkModel model, IEnumerable<VisitMutation> mutations, DayType dayType, Random random)
        {
            var visitsCount = dayType.CalculateVisitsCount(model.Customers.Count);
            var visitMutations = mutations.ToList().ChooseRandomItems(visitsCount, random);
            return (from customerMutationTuple  in model.Customers.ChooseRandomItems(visitsCount, random).Zip(visitMutations, (customer, mutation) => (customer, mutation))
                where !model.IsDepot(customerMutationTuple.customer)
                select new Problem.Visit
                {
                    Demand = customerMutationTuple.mutation.Demand,
                    FromTime = customerMutationTuple.mutation.FromTime,
                    ToTime = customerMutationTuple.mutation.ToTime,
                    ServiceTime = customerMutationTuple.mutation.ServiceTime,
                    PointName = $"customer {customerMutationTuple.customer.Id}"
                }).ToList();
        }
        

        private static IEnumerable<VisitMutation> CreateVisitMutations(IEnumerable<Customer> visits)
        {
            visits = visits.ToList();
            var validWindowsLengths = visits.Select(visit => visit.DueTime - visit.ReadyTime).Distinct();
            var validStarts = visits.Select(visit => visit.DueTime).Distinct();
            var validServiceTime = visits.Where(visit => visit.ServiceTime > 0).Select(visit => visit.ServiceTime).Distinct();
            var validDemands = visits.Select(visit => visit.Demand).Distinct();
            return (from timeWindowLen in validWindowsLengths
                from startTime in validStarts
                from serviceTime in validServiceTime
                from demand in validDemands
                select new VisitMutation(startTime, startTime + timeWindowLen, serviceTime, demand)).ToList();
        }
    }
}