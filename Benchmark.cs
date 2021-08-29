using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;


namespace SolomonBenchmark
{
    public class Benchmark
    {
        public string Name;
        public int VehicleCount;
        public int Capacity;
        public List<Customer> Customers;
        public long MaxDistance => 1000;
        public int NodesCount => Customers.Count();
        public int DepotNode => Customers.FindIndex(x => x.Id == 0);
        public static Benchmark fromFile(string filePath)
        {
            var enumerator = System.IO.File.ReadAllLines(filePath).GetEnumerator();
            enumerator.MoveNext();
            var name = enumerator.Current.ToString();
            Enumerable.Range(0, 4).Select(_ => enumerator.MoveNext()).ToList();
            var stats = enumerator.Current.ToString().Split(' ').Where(s => s != string.Empty).Select(num => int.Parse(num)).ToList();
            var (vehicleNumber, capacity) = (stats[0], stats[1]);
            Enumerable.Range(0, 4).Select(_ => enumerator.MoveNext()).ToList();
            var customers = new List<Customer>();

            while (enumerator.MoveNext())
            {
                var line = enumerator.Current.ToString().Split(' ').Where(s => s != string.Empty).Select(num => int.Parse(num)).ToList();
                customers.Add(new Customer
                {
                    Id = line[0],
                    Coords = new Vector2(line[1], line[2]),
                    Demand = line[3],
                    ReadyTime = line[4],
                    DueTime = line[5],
                    ServiceTime = line[6]
                });
            }

            return new Benchmark
            {
                Name = name,
                VehicleCount = vehicleNumber,
                Capacity = capacity,
                Customers = customers
            };
        }

    }
    
    public class Customer
    {
        public int Id;
        public Vector2 Coords;
        public int Demand;
        public int ReadyTime; // earliest time when the vehicle can start from this customer
        public int DueTime; // latest time when the vehicle can start from this customer
        public int ServiceTime;
    }
}