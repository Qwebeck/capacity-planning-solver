using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace DayBreaks.Solver.Solvers.Concrete
{
    internal static class LinearExpressionExtensions
    {
        public static  Google.OrTools.LinearSolver.LinearExpr LinearSolverSum<T>(this IEnumerable<T> source, Func<Google.OrTools.LinearSolver.LinearExpr, T, Google.OrTools.LinearSolver.LinearExpr> sumLogic) => source.Aggregate(new Google.OrTools.LinearSolver.LinearExpr(), sumLogic);
        
    }
}