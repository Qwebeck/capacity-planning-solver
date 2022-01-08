using System.Collections.Generic;
using DayBreaks.FleetStructureTesting.Models;
using DayBreaks.Solver.ConstraintModels.Concrete.BreaksAsIntervals;
using DayBreaks.Solver.Solvers.Concrete;
using DayBreaks.Solver.Solvers.Concrete.CpSat;
using Google.OrTools.ConstraintSolver;

namespace DayBreaks.CLI.FleetStructurePrediction
{
    public record PredictionMethodOption(string Name) : MultipleChoiceOptions<SolverConfiguration>(Name,
        new Dictionary<string, SolverConfiguration>
        {
            {
                "sat",
                new SolverConfiguration("CpSatModel",
                    problemModel => new CpSatBasedFleetStructureSolver(problemModel))
                    
            },
            {
                "vrp", new SolverConfiguration("VrpBasedSolver", problemModel =>
                {
                    var constraintModel = BreaksAsIntervalsConstraintModelFactory.FromProblemModel(problemModel);
                    return new VrpBasedFleetStructureSolver<DimensionMatrix, NodeManager>(
                        constraintModel, LocalSearchMetaheuristic.Types.Value.TabuSearch, 60*10);
                })
            },
            {
                "linear", new SolverConfiguration("LinearProgrammingModel", problemModel => new LinearSolver(problemModel))
            }
        });
}