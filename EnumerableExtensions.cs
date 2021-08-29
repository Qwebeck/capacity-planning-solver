using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SolomonBenchmark
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) => enumerable.ToImmutableList().ForEach(action);


        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action) =>
            enumerable.ToImmutableList().ForEach(action);
    }
}