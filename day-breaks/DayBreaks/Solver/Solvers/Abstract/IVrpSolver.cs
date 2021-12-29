using DayBreaks.Solver.Solution;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.Solver.Solvers.Abstract
{
    public interface IVrpSolver
    {
        VrpSolution Solve(LocalSearchMetaheuristic.Types.Value metaheuristic);
        VrpSolution Solve();
    }
}