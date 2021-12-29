
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace DayBreaks.Problem
{
    public class ProblemModel
    {
        [JsonIgnore] public int DaysInMonth => Days.Sum(day => day.Occurrences);
        [JsonIgnore] public long MaxWorkDuration => 8 * 60;
        [JsonIgnore] public long MinBreakDuration => 60 * 8;
        [JsonIgnore] public long DayDuration => 60 * 24;
        [JsonIgnore] public int VisitsCount => Days.Sum(day => day.Visits.Count());
        [JsonIgnore] public int CharacteristicDayCount => Days.Count();
        public long Budget { get; set; }
        public long MaxDistance { get; set; }
        public IEnumerable<Depot> Depots { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Day> Days { get; set; }
        public IEnumerable<VehicleType> VehicleTypes { get; set; }
    }
}