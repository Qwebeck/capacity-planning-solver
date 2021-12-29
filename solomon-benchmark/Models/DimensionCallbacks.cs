using System;

namespace SolomonBenchmark
{
    public record Callback<T> where T: Delegate
    {
        public int Idx;
        public T Evaluator;
    }
    public record DimensionCallbacks(
        Callback<Func<long, long, long>> Distance, 
        Callback<Func<long, long, long>> Time,
        Callback<Func<long, long>> Demand); 
}