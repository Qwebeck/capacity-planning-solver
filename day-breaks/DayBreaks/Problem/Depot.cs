using System;
using System.Collections.Generic;


namespace DayBreaks.Problem
{
    public class Depot: Point
    { 
        public Dictionary<string, int> Vehicles { get; set; }
    }
}