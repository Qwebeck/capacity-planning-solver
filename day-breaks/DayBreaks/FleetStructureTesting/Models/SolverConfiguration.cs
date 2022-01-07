using System;
using DayBreaks.Problem;
using DayBreaks.Solver.Solvers.Abstract;

namespace DayBreaks.FleetStructureTesting.Models
{
    public record SolverConfiguration(string Name, Func<ProblemModel, IFleetStructureSolver> SolverFactory);
}