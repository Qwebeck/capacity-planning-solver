using System;
using System.Collections;
using System.Collections.Generic;

namespace DayBreaks.Problem
{
    public class Visit
    {
        public string PointName { get; set; }
        public int Demand { get; set; }
        /// <summary>
        /// Minutes from day start to day end
        /// </summary>
        public long FromTime { get; set; }
        /// <summary>
        /// Minutes from day start to day end
        /// </summary>
        public long ToTime { get; set; }
        public long ServiceTime { get; set; }
    }

    public class Day
    {
        public int Occurrences { get; set; }
        public IEnumerable<Visit> Visits { get; set; }
    }
}