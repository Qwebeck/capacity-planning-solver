using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using DayBreaks.Solver.Managers;
using DayBreaks.Solver.Solution;
using DayBreaks.Solver.Solvers.Abstract;
using Google.OrTools.Sat;
using Tools.Extensions;

namespace DayBreaks.Solver.Solvers.Concrete.CpSat
{
    internal record VariableWithVisit(Visit Visit, IntVar IsVisitActive);
    internal record VariableWithVehicle(VehicleInstance VehicleInstance, IntVar IsVehicleActive);


    public record CpSatSolverParams(int MaxTime);
    public class CpSatBasedFleetStructureSolver: IFleetStructureSolver
    {
        private static CpSatSolverParams DefaultSolverParmas = new CpSatSolverParams( 50*60);
        private static int MaxPointsVehicleCouldServeInDay => 8;
        private readonly ProblemModel _problemModel;
        private readonly VehicleManager _vehicleManager;
        
        
        public CpSatBasedFleetStructureSolver(ProblemModel problemModel) => (_problemModel, _vehicleManager) = (problemModel, new VehicleManager(problemModel));


        public FleetStructure Solve() => Solve(DefaultSolverParmas);
        public FleetStructure Solve(CpSatSolverParams solverParams)
        {
            var (model, solver) = (new CpModel(), new CpSolver());
            var vehicleVariablesByDays = CreateConstraints(model).ToList();

            LinearExpr SumForDay(IEnumerable<VariableWithVehicle> day) => LinearExpr.ScalProd(day.Select(v => v.IsVehicleActive), day.Select(v => v.VehicleInstance.DayUsageCost));
            var objective = LinearExpr.Sum(vehicleVariablesByDays.Select(SumForDay));
            model.Minimize(objective);
            solver.StringParameters = $"max_time_in_seconds:{solverParams.MaxTime}";
            var status = solver.Solve(model);
            return IsSolutionFound(status) 
                ? CreateSolution(solver, vehicleVariablesByDays) 
                : null;

        }
        
        private IEnumerable<IEnumerable<VariableWithVehicle>> CreateConstraints(CpModel cpModel)
        {
            var vehicleVariablesByDays = _problemModel.Days.Select((day, idx) => CreateDayModel(cpModel, idx, day)).ToList();
            ConnectContractVehiclesVariables(cpModel, vehicleVariablesByDays);
            return vehicleVariablesByDays;
        }
        
        /// <summary>
        /// Creates a constraint model for characteristic day
        /// </summary>
        /// <returns>Variables indicating if vehicle is used on that day</returns>
        private IEnumerable<VariableWithVehicle> CreateDayModel(CpModel cpModel, int dayIdx, Day characteristicDay)
        {
            var availableVehicles = CreateVehicleVariables(cpModel,_vehicleManager.ContractVehicles.Concat(_vehicleManager.SpotVehiclesForDay(dayIdx))).ToList();
            var visitsMatrix = CreateVisitsMatrix(cpModel, dayIdx, characteristicDay, availableVehicles);
            AddDayConstraints(cpModel, availableVehicles, visitsMatrix);
            return availableVehicles;
        }

        private VisitsMatrix CreateVisitsMatrix(CpModel cpModel, int dayIdx, Day characteristicDay, IEnumerable<VariableWithVehicle> availableVehicles)
            => new(cpModel, availableVehicles, characteristicDay.Visits, dayIdx);
        

        private IEnumerable<VariableWithVehicle> CreateVehicleVariables(CpModel cpModel, IEnumerable<IndexedVehicle> availableVehicles) =>
            availableVehicles
                .Select(vehicle => new VariableWithVehicle(
                    vehicle,
                    cpModel.NewBoolVar($"is_vehicle_{vehicle.Name}_with_rental_type_{vehicle.RentalType}_for_day_{vehicle.StartDay}")
                ));


        private void AddDayConstraints(CpModel cpModel, IEnumerable<VariableWithVehicle> vehicleVariables, VisitsMatrix visitsMatrix)
        {
            // each visit could be visited only one time
            Enumerable.Range(0, visitsMatrix.VisitCount).ForEach(visitIdx =>
            {
                var allowedVehicles = visitsMatrix.GetVehiclesDisjunctionForVisit(visitIdx).ToList();
                var visitsToPoint = LinearExpr.Sum(allowedVehicles.Select(v => v.IsVisitActive));
                
                cpModel.Add(visitsToPoint == 1);
            });
            
            
            vehicleVariables.ForEach((vehicle, idx)=>
            {
                // each vehicle cannot transfer more items that its capacity
                var visitsDisjunction = visitsMatrix.GetVisitDisjunctionForVehicle(idx).ToList();
                var servedClientsDemands = LinearExpr.ScalProd(visitsDisjunction.Select(v => v.IsVisitActive), 
                    visitsDisjunction.Select(v => v.Visit.Demand));
                var (vehicleInstance, isVehicleActive) = vehicle;
                var demandConstraintForVehicle = isVehicleActive * vehicleInstance.Capacity >= servedClientsDemands;
                cpModel.Add(demandConstraintForVehicle);
                // any vehicle cannot have more than MaxPointsVehicleCouldServeInDay visits per day
                var visitedPoints = LinearExpr.Sum(visitsMatrix.GetVisitDisjunctionForVehicle(idx).Select(v => v.IsVisitActive));
                cpModel.Add(visitedPoints <= MaxPointsVehicleCouldServeInDay);
            });
            
        }

        private void ConnectContractVehiclesVariables(CpModel cpModel, IEnumerable<IEnumerable<VariableWithVehicle>> vehicleVariablesByDays)
        {
            var contractVehiclesVariables = vehicleVariablesByDays
                .SelectMany(dayVehiclesCollection
                    => dayVehiclesCollection.Where(vehicle =>
                        vehicle.VehicleInstance.RentalType == RentalType.Contract));
            contractVehiclesVariables.GroupBy(variable => (variable.VehicleInstance.Name, variable.VehicleInstance.RentalType, variable.VehicleInstance.StartDay, variable.VehicleInstance.SourceDepotName)).ForEach(group =>
            {
                // Contract vehicles are used on multiple days
                var isVehicleActiveVars = group.Select(varWithVehicle => varWithVehicle.IsVehicleActive).ToList();
                
                
                for (var i = 1; i < isVehicleActiveVars.Count; ++i)
                {
                    // All used or all unused 
                    cpModel.AddImplication(isVehicleActiveVars[i - 1], isVehicleActiveVars[i]).OnlyEnforceIf(isVehicleActiveVars[0]);
                    cpModel.AddImplication(isVehicleActiveVars[i - 1].Not(), isVehicleActiveVars[i].Not()).OnlyEnforceIf(isVehicleActiveVars[0].Not());
                }

                cpModel.AddImplication(isVehicleActiveVars[^1], isVehicleActiveVars[0]);
                cpModel.AddImplication(isVehicleActiveVars[^1].Not(), isVehicleActiveVars[0].Not());

                // Handle
                // cpModel.AddBoolOr(new[] {isVehicleActive, isVehicleInActive});

            });
            
        }
        private static FleetStructure CreateSolution(CpSolver solver, IEnumerable<IEnumerable<VariableWithVehicle>> vehicleVariablesByDays)
        {
            var fleetPositions = SelectVehiclePositions(solver, vehicleVariablesByDays).ToList();
            var cost = CalculateCost(fleetPositions); 
            return new FleetStructure(fleetPositions, cost);
        }

        private static IEnumerable<FleetPosition> SelectVehiclePositions(CpSolver solver,
            IEnumerable<IEnumerable<VariableWithVehicle>> vehicleVariablesByDays)
        {
            vehicleVariablesByDays = vehicleVariablesByDays.ToList();
            
            var usedContractVehicles = vehicleVariablesByDays.First().Where(dayVehicleCollection 
                => solver.BooleanValue(dayVehicleCollection.IsVehicleActive) && dayVehicleCollection.VehicleInstance.RentalType == RentalType.Contract);
            var usedSpotVehicles = vehicleVariablesByDays.SelectMany(
                dayVehicleCollection => dayVehicleCollection
                    .Where(v => solver.BooleanValue(v.IsVehicleActive) &&
                                v.VehicleInstance.RentalType == RentalType.Spot)
            );

            return usedContractVehicles.Concat(usedSpotVehicles)
                .GroupBy(
                    variableWithVehicle => new
                    {
                        variableWithVehicle.VehicleInstance.Name,
                        variableWithVehicle.VehicleInstance.RentalType,
                        variableWithVehicle.VehicleInstance.StartDay,
                        variableWithVehicle.VehicleInstance.SourceDepotName
                    },
                    variableWithVehicle => variableWithVehicle.VehicleInstance,
                    (_, group) =>
                    {
                        var typicalVehicle = group.First();
                        return new FleetPosition
                        {
                            Capacity = typicalVehicle.Capacity,
                            CostPerKm = typicalVehicle.CostPerKm,
                            MonthUsageCost = typicalVehicle.MonthUsageCost * group.Count(),
                            Count = group.Count(),
                            Name = typicalVehicle.Name,
                            RentalType = typicalVehicle.RentalType,
                            SourceDepotName = typicalVehicle.SourceDepotName
                        };
                    });
        }

        private static long CalculateCost(IEnumerable<FleetPosition> fleet) =>
            fleet.Sum(position => position.Count * position.MonthUsageCost);

        private static bool IsSolutionFound(CpSolverStatus status) => status switch
        {
            CpSolverStatus.Feasible => true,
            CpSolverStatus.Optimal => true,
            _ => false
        };

    }
}