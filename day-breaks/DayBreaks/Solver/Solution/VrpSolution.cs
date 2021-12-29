using System.Collections.Generic;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using Google.OrTools.ConstraintSolver;
using Newtonsoft.Json;

namespace DayBreaks.Solver.Solution
{
    public class VrpSolution 
    {
        public IEnumerable<Depot> Depots;
        public IEnumerable<Client> Clients;
        public IEnumerable<IEnumerable<Point>> VehicleRoutes;
        public IEnumerable<VehicleInstance> Vehicles;
        public IEnumerable<VehicleInstance> UnusedVehicles;
        public int NotVisitedClientsCount;
        public long ObjectiveValue;
        
    }
}