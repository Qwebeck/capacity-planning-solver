using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.ConstraintSolver;

namespace SolomonBenchmark
{
    public record Route(IEnumerable<int> RouteNodes)
    {
        public IEnumerable<(int, int)> AsArcs => RouteNodes.Zip(
            RouteNodes.ToArray()[1..].Union(new[] {RouteNodes.First()})
        );

        public long CalculateCost(RoutingIndexManager manager, Func<long, long, long> costEvaluator) => AsArcs.Sum(
            arc => costEvaluator(
                manager.NodeToIndex(arc.Item1),
                manager.NodeToIndex((arc.Item2))
            ));

    }
}