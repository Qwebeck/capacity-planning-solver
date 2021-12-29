using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tools.Extensions
{
    public static class EnumerableExtensions
    {

        public static long Sum<T>(this IEnumerable<T> enumerable, Func<T, int, long> sumLogic) => enumerable.Select(sumLogic).Sum(); 
        
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) => enumerable.ToImmutableList().ForEach(action);


        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action) =>
            enumerable.ToImmutableList().ForEach(action);
    }
}