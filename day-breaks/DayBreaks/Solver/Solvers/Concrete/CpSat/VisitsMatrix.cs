using System.Collections.Generic;
using System.Linq;
using DayBreaks.Problem;
using Google.OrTools.Sat;
using Tools.Extensions;

namespace DayBreaks.Solver.Solvers.Concrete.CpSat
{
    internal class VisitsMatrix
    {
        public int VisitCount => _isVehiclesVisitsPoint.GetLength(0);
        public int VehicleCount => _isVehiclesVisitsPoint.GetLength(1);
        
        private readonly VariableWithVisit[,] _isVehiclesVisitsPoint;

        public VisitsMatrix(CpModel cpModel, IEnumerable<VariableWithVehicle> availableVehicles, IEnumerable<Visit> visits, int dayIdx)
        {
            visits = visits.ToList();
            var visitsCount = visits.Count();
            availableVehicles = availableVehicles.ToList();
            _isVehiclesVisitsPoint = new VariableWithVisit[visitsCount, availableVehicles.Count()];
            
            availableVehicles.ForEach((vehicle, vehicleIdx) =>
            {
                visits.ForEach((visit, visitIdx) =>
                {
                    var isVisitActive = cpModel.NewBoolVar(
                        $"is_vehicle_{vehicle.VehicleInstance.Name}_visits_point_{visit.PointName}_on_day_{dayIdx}"); 
                    _isVehiclesVisitsPoint[visitIdx, vehicleIdx] = new VariableWithVisit(visit, isVisitActive);
                });
            });
        }

        public IEnumerable<VariableWithVisit> GetVehiclesDisjunctionForVisit(int visitIdx) => Enumerable
            .Range(0, _isVehiclesVisitsPoint.GetLength(1))
            .Select(vehicleIdx => _isVehiclesVisitsPoint[visitIdx, vehicleIdx]);

        public IEnumerable<VariableWithVisit> GetVisitDisjunctionForVehicle(int vehicleIdx) => Enumerable
            .Range(0, _isVehiclesVisitsPoint.GetLength(0))
            .Select(visitIdx => _isVehiclesVisitsPoint[visitIdx, vehicleIdx]);
        
    }
}