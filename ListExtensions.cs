using System;
using System.Collections.Generic;
using System.Linq;

namespace SolomonBenchmark
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            _ = list.Select((elem, index) =>
            {
                action(elem, index);
                return 0;
            });
        }
    }
}