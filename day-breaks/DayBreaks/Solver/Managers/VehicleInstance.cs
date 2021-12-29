using System;

namespace DayBreaks.Solver.Managers
{
    public enum RentalType { Contract, Spot }
    public class VehicleInstance
    {
        public RentalType RentalType;
        public string Name;
        public long Capacity;
        public long MonthUsageCost;
        public long DayUsageCost;
        public long CostPerKm;
        public string SourceDepotName;
        public int StartDay;
        public int EndDay;
    }

    public class IndexedVehicle : VehicleInstance
    {
        public int Index;
    }
}