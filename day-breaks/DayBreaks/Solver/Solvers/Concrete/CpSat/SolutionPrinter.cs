using System;
using Google.OrTools.Sat;

namespace DayBreaks.Solver.Solvers.Concrete.CpSat
{
    class SolutionPrinter : CpSolverSolutionCallback
    {
        private int _solutionCount;
        private readonly int _solutionLimit;
        public SolutionPrinter(int solutionLimit) => _solutionLimit = solutionLimit;

        public override void OnSolutionCallback()
        {
            Console.WriteLine($"Solution #{_solutionCount}: time = {WallTime():F2} s");
            _solutionCount++;
            if (_solutionCount < _solutionLimit) return;
            Console.WriteLine($"Stopping search after {_solutionLimit} solutions");
            StopSearch();
        }
    }
}