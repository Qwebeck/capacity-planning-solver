using System;
using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using SolomonBenchmark.Models;
using Tools.Extensions;
using SolomonBenchmarkModel = SolomonBenchmark.Models.Model;
namespace DayBreaks.Mappers
{
    
    public enum DayType { Hard, Normal, Easy }
    internal record VisitMutation(int FromTime, int ToTime, int ServiceTime, int Demand);
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

        public static ProblemModel CreateModelFromSolomonBenchmarkFile(string path, DayType dayType,
            IEnumerable<VehicleType> vehicleTypes = null, Random random = null)
            => CreateModelFromSolomonBenchmarkFile(path, new [] {dayType}, vehicleTypes, random);

        public static ProblemModel CreateModelFromSolomonBenchmarkFile(string path, IEnumerable<DayType> dayTypes, 
            IEnumerable<VehicleType> vehicleTypes=null, Random random=null)
        {
            random ??= DefaultRandom;
            vehicleTypes ??= DefaultVehicleTypes;
            var solomonBenchmarkModel = SolomonBenchmarkModel.fromFile(path);
            var visitIntervals = SelectValidVisitIntervals(solomonBenchmarkModel.Customers).ToList();    
            return new ProblemModel
            {
                Budget = 10000000,
                MaxDistance = 10000000,
                Clients = GetClientsFromModel(solomonBenchmarkModel).ToList(),
                Depots = GetDepotsFromModel(solomonBenchmarkModel, vehicleTypes).ToList(),
                Days = GetVisitsFromModel(solomonBenchmarkModel, visitIntervals, dayTypes, random).ToList(),
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
        
        private static IEnumerable<Day> GetVisitsFromModel(SolomonBenchmarkModel model, IEnumerable<VisitMutation> timeIntervals, IEnumerable<DayType> dayTypes, Random random)
        {
            var dayCount = dayTypes.Count();
            var days = dayTypes.Select(dayType => new Day
            {
                Visits = CreateVisitsForDay(model, timeIntervals, dayType, random).ToList(),
                Occurrences = 15 / dayCount
            });
            return days.ToList();
        }

        
        private static IEnumerable<Visit> CreateVisitsForDay(SolomonBenchmarkModel model, IEnumerable<VisitMutation> mutations, DayType dayType, Random random)
        {
            var visitsCount = GetVisitsCountForDay(dayType, model.Customers.Count);
            var visitMutations = mutations.ToList().ChooseRandomItems(visitsCount, random);
            return (from customerMutationTuple  in model.Customers.ChooseRandomItems(visitsCount, random).Zip(visitMutations, (customer, mutation) => (customer, mutation))
                where !model.IsDepot(customerMutationTuple.customer)
                select new Visit
                {
                    Demand = customerMutationTuple.mutation.Demand,
                    FromTime = customerMutationTuple.mutation.FromTime,
                    ToTime = customerMutationTuple.mutation.ToTime,
                    ServiceTime = customerMutationTuple.mutation.ServiceTime,
                    PointName = $"customer {customerMutationTuple.customer.Id}"
                }).ToList();
        }
        

        private static IEnumerable<VisitMutation> SelectValidVisitIntervals(IEnumerable<Customer> visits)
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

        private static int GetVisitsCountForDay(DayType dayType, int maxVistsCount) =>  Convert.ToInt32(dayType switch
        {
        DayType.Easy => 0.31,
        DayType.Normal => 0.51,
        DayType.Hard => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(dayType), dayType, null)
        } * maxVistsCount);
    }
}