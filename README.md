# Overview

Repository provides a Command Line Interface that allows perform a fleet structure prediction for a given period. The solution is adapted to the case of retail companies which does not maintain its own fleet of vehicles, and use services of local carriers instead.  Constraints that exists in the problem are:

- The Carrier could rent a vehicle as a Spot or as a Regular vehicle:
    - Regular vehicles are rented on a full planning period 
    - Spot vehicles are rented on one day
    - Regular vehicles are cheaper than Spot vehicles on the day rent basis, but are more beneficial if vehicle will be used often during planning.
- Nodes in problem have demands and predefined time windows
- Duration of vehicle route could not be longer than 8 hours.
- Break between two routes for the same vehicle should be at least 8 hours.

The objective is to minimize fleet.
Refer to Chapter 3 of attached [thesis file](https://github.com/Qwebeck/capacity-planning-solver/blob/master/thesis.pdf) for more details.



# Solvers
Repository contains three Solvers for Fleet Structure Optimization Problem, implemented in different computational paradigms: 

- VRP solver (`VrpBasedFleetStructureSolver.cs`) that reformulates a problem as an instance of Rich VRP spanned on multiple days,
- CP-SAT solver (`CpSatBasedFleetStructureSolver.cs`) that describes a problem with set of logical formulas,
- Linear solver (`LinearSolver.cs`) that describes a problem with a system of linear equations.

VRP solver taks in consideration all constraints that do exists in the problem, while CP-SAT and Linear solvers are working on a relaxed version of problem.

Refer to Chapter 4 of attached [thesis file](https://github.com/Qwebeck/capacity-planning-solver/blob/master/thesis.pdf) for more details.

# Command Line Interface
This repository containts a CLI(Command Line Interface) that allows to make a prediction with  solvers mentioned above, run a VRP simulation with predicted fleet and reproduce tests mentioned in the Chapter 5 of the thesis. Specific commands are outlined below.

## vrp-simulate
Runs a VRP simulation for predicted fleet and returns json containing simulated and predicted fleet costs, and their difference.
Arguments:
```
-f, --fleet-structure-prediction    Required. Path to json with predicted fleet

-c, --characteristic-days           Required. Path to characteristic days model from which day data will be generated

-s, --save-to                       (Default: simulation_results.json) Path where solution will be saved

-t, --time-limit                    (Default: 60) Time limit for every day in model

-m, --metaheuristic                 Required. Metaheuristic to be used by solver. One of: sa (Simulated Annealing), ts (Tabu Seach), gls (Guided Local Search), gd (GreedyDescent).
```

### Example:
```
fleet-structure-solver vrp-simulate -f fleet_structure.json -c characteristic_days.json -m sa
```
<details>
<summary>fleet_structure.json (see class `DayBreaks.Solver.Solution.FleetStructure`)</summary>

```json
{
  "FleetPositions": [
    {
      "Count": 15,
      "RentalType": 0,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 2500,
      "DayUsageCost": 0,
      "CostPerKm": 1,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 10,
      "RentalType": 0,
      "Name": "medium",
      "Capacity": 200,
      "MonthUsageCost": 5000,
      "DayUsageCost": 0,
      "CostPerKm": 2,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 750,
      "DayUsageCost": 0,
      "CostPerKm": 1,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 2250,
      "DayUsageCost": 0,
      "CostPerKm": 1,
      "SourceDepotName": "depot 0",
      "StartDay": 1,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 750,
      "DayUsageCost": 0,
      "CostPerKm": 1,
      "SourceDepotName": "depot 0",
      "StartDay": 2,
      "EndDay": 0
    }
  ],
  "EstimatedCost": 106250
}
```
</details>


<details>
<summary>characteristic_days.json (see class `DayBreaks.Problem.ProblemModel`)</summary>

```json
{
  "Budget": 10000000,
  "MaxDistance": 10000000,
  "Days": [
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 2",
          "Demand": 30,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 10,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 15,
      "Visits": [
        {
          "PointName": "customer 3",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 10,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 50,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 0,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 10,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 0,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 20,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 40,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 10,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 30,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 50,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 10,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 20,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 40,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 0,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 10,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 0,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 30,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 20,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 40,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 10,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 30,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 20,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 1",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 3",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 4",
          "Demand": 30,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 5",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 6",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 12",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 10,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 50,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 10,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 24",
          "Demand": 50,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 20,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 40,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 31",
          "Demand": 10,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 32",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 33",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 30,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 40",
          "Demand": 0,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 0,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 20,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 43",
          "Demand": 40,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 50,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 0,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 10,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 30,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 50,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 0,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 10,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 20,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 40,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 50,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 0,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 55",
          "Demand": 10,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 56",
          "Demand": 30,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 57",
          "Demand": 50,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 0,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 30,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 60",
          "Demand": 40,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 30,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 40,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 63",
          "Demand": 0,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 10,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 20,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 66",
          "Demand": 40,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 0,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 68",
          "Demand": 10,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 69",
          "Demand": 40,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 70",
          "Demand": 50,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 71",
          "Demand": 10,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 72",
          "Demand": 50,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 73",
          "Demand": 40,
          "FromTime": 144,
          "ToTime": 1380,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 0,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 75",
          "Demand": 10,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 77",
          "Demand": 30,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 40,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 0,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 10,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 30,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 0,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 84",
          "Demand": 50,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 85",
          "Demand": 0,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 86",
          "Demand": 30,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 40,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 50,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 0,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 90",
          "Demand": 30,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 40,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 92",
          "Demand": 50,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 93",
          "Demand": 10,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 30,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 95",
          "Demand": 20,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 96",
          "Demand": 40,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 97",
          "Demand": 10,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 98",
          "Demand": 30,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 99",
          "Demand": 40,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 0,
          "FromTime": 158,
          "ToTime": 1394,
          "ServiceTime": 90
        }
      ]
    }
  ],
  "VehicleTypes": [
    {
      "Name": "small",
      "Capacity": 100,
      "CostPerKm": 1,
      "CostAsContractVehicle": 100,
      "CostAsSpotVehicle": 150
    },
    {
      "Name": "medium",
      "Capacity": 200,
      "CostPerKm": 2,
      "CostAsContractVehicle": 200,
      "CostAsSpotVehicle": 300
    },
    {
      "Name": "large",
      "Capacity": 300,
      "CostPerKm": 3,
      "CostAsContractVehicle": 300,
      "CostAsSpotVehicle": 500
    }
  ],
  "Depots": [
    {
      "Vehicles": {
        "small": 15,
        "medium": 15,
        "large": 15
      },
      "Name": "depot 0",
      "Coords": {
        "X": 40.0,
        "Y": 50.0
      }
    }
  ],
  "Clients": [
    {
      "Name": "customer 1",
      "Coords": {
        "X": 45.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 2",
      "Coords": {
        "X": 45.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 3",
      "Coords": {
        "X": 42.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 4",
      "Coords": {
        "X": 42.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 5",
      "Coords": {
        "X": 42.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 6",
      "Coords": {
        "X": 40.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 7",
      "Coords": {
        "X": 40.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 8",
      "Coords": {
        "X": 38.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 9",
      "Coords": {
        "X": 38.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 10",
      "Coords": {
        "X": 35.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 11",
      "Coords": {
        "X": 35.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 12",
      "Coords": {
        "X": 25.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 13",
      "Coords": {
        "X": 22.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 14",
      "Coords": {
        "X": 22.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 15",
      "Coords": {
        "X": 20.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 16",
      "Coords": {
        "X": 20.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 17",
      "Coords": {
        "X": 18.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 18",
      "Coords": {
        "X": 15.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 19",
      "Coords": {
        "X": 15.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 20",
      "Coords": {
        "X": 30.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 21",
      "Coords": {
        "X": 30.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 22",
      "Coords": {
        "X": 28.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 23",
      "Coords": {
        "X": 28.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 24",
      "Coords": {
        "X": 25.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 25",
      "Coords": {
        "X": 25.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 26",
      "Coords": {
        "X": 25.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 27",
      "Coords": {
        "X": 23.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 28",
      "Coords": {
        "X": 23.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 29",
      "Coords": {
        "X": 20.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 30",
      "Coords": {
        "X": 20.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 31",
      "Coords": {
        "X": 10.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 32",
      "Coords": {
        "X": 10.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 33",
      "Coords": {
        "X": 8.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 34",
      "Coords": {
        "X": 8.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 35",
      "Coords": {
        "X": 5.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 36",
      "Coords": {
        "X": 5.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 37",
      "Coords": {
        "X": 2.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 38",
      "Coords": {
        "X": 0.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 39",
      "Coords": {
        "X": 0.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 40",
      "Coords": {
        "X": 35.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 41",
      "Coords": {
        "X": 35.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 42",
      "Coords": {
        "X": 33.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 43",
      "Coords": {
        "X": 33.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 44",
      "Coords": {
        "X": 32.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 45",
      "Coords": {
        "X": 30.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 46",
      "Coords": {
        "X": 30.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 47",
      "Coords": {
        "X": 30.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 48",
      "Coords": {
        "X": 28.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 49",
      "Coords": {
        "X": 28.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 50",
      "Coords": {
        "X": 26.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 51",
      "Coords": {
        "X": 25.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 52",
      "Coords": {
        "X": 25.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 53",
      "Coords": {
        "X": 44.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 54",
      "Coords": {
        "X": 42.0,
        "Y": 10.0
      }
    },
    {
      "Name": "customer 55",
      "Coords": {
        "X": 42.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 56",
      "Coords": {
        "X": 40.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 57",
      "Coords": {
        "X": 40.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 58",
      "Coords": {
        "X": 38.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 59",
      "Coords": {
        "X": 38.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 60",
      "Coords": {
        "X": 35.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 61",
      "Coords": {
        "X": 50.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 62",
      "Coords": {
        "X": 50.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 63",
      "Coords": {
        "X": 50.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 64",
      "Coords": {
        "X": 48.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 65",
      "Coords": {
        "X": 48.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 66",
      "Coords": {
        "X": 47.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 67",
      "Coords": {
        "X": 47.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 68",
      "Coords": {
        "X": 45.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 69",
      "Coords": {
        "X": 45.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 70",
      "Coords": {
        "X": 95.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 71",
      "Coords": {
        "X": 95.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 72",
      "Coords": {
        "X": 53.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 73",
      "Coords": {
        "X": 92.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 74",
      "Coords": {
        "X": 53.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 75",
      "Coords": {
        "X": 45.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 76",
      "Coords": {
        "X": 90.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 77",
      "Coords": {
        "X": 88.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 78",
      "Coords": {
        "X": 88.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 79",
      "Coords": {
        "X": 87.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 80",
      "Coords": {
        "X": 85.0,
        "Y": 25.0
      }
    },
    {
      "Name": "customer 81",
      "Coords": {
        "X": 85.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 82",
      "Coords": {
        "X": 75.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 83",
      "Coords": {
        "X": 72.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 84",
      "Coords": {
        "X": 70.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 85",
      "Coords": {
        "X": 68.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 86",
      "Coords": {
        "X": 66.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 87",
      "Coords": {
        "X": 65.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 88",
      "Coords": {
        "X": 65.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 89",
      "Coords": {
        "X": 63.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 90",
      "Coords": {
        "X": 60.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 91",
      "Coords": {
        "X": 60.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 92",
      "Coords": {
        "X": 67.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 93",
      "Coords": {
        "X": 65.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 94",
      "Coords": {
        "X": 65.0,
        "Y": 82.0
      }
    },
    {
      "Name": "customer 95",
      "Coords": {
        "X": 62.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 96",
      "Coords": {
        "X": 60.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 97",
      "Coords": {
        "X": 60.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 98",
      "Coords": {
        "X": 58.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 99",
      "Coords": {
        "X": 55.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 100",
      "Coords": {
        "X": 55.0,
        "Y": 85.0
      }
    }
  ]
}
```
</details>

<details>
<summary>simulation_results.json (Output. See class `DayBreaks.FleetStructureTesting.Models.FleetEvaluationResult`)</summary>

```json
{
  "PredictedFleetCost": 106250,
  "RealFleetCost": SOME_VALUE,
  "CostOverestimate": 106250 - SOME_VALUE
}
```
</details>

# fs-predict
Predicts fleet structure given characteristic days.

```
-p, --characteristic-days-path    Required. Path to characteristic days for which prediction should be done

-s, --solution-path               (Default: fleet_structure.json) Path to folder to which execution results would be saved

-m, --method                      Required. Method that will be used for prediction. One of: sat, vrp, linear
```

### Example
```
fleet-structure-solver  fs-predict -p characteristic_days.json -m linear
```

<details>
<summary>characteristic_days.json</summary>

```json
{
  "Budget": 10000000,
  "MaxDistance": 10000000,
  "Days": [
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 2",
          "Demand": 30,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 10,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 15,
      "Visits": [
        {
          "PointName": "customer 3",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 10,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 50,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 0,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 10,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 0,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 20,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 40,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 10,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 30,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 50,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 10,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 20,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 40,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 0,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 10,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 0,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 30,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 20,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 40,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 10,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 30,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 20,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 1",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 3",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 4",
          "Demand": 30,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 5",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 6",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 12",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 10,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 50,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 10,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 24",
          "Demand": 50,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 20,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 40,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 31",
          "Demand": 10,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 32",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 33",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 30,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 40",
          "Demand": 0,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 0,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 20,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 43",
          "Demand": 40,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 50,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 0,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 10,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 30,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 50,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 0,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 10,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 20,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 40,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 50,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 0,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 55",
          "Demand": 10,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 56",
          "Demand": 30,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 57",
          "Demand": 50,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 0,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 30,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 60",
          "Demand": 40,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 30,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 40,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 63",
          "Demand": 0,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 10,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 20,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 66",
          "Demand": 40,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 0,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 68",
          "Demand": 10,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 69",
          "Demand": 40,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 70",
          "Demand": 50,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 71",
          "Demand": 10,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 72",
          "Demand": 50,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 73",
          "Demand": 40,
          "FromTime": 144,
          "ToTime": 1380,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 0,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 75",
          "Demand": 10,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 77",
          "Demand": 30,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 40,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 0,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 10,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 30,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 0,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 84",
          "Demand": 50,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 85",
          "Demand": 0,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 86",
          "Demand": 30,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 40,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 50,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 0,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 90",
          "Demand": 30,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 40,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 92",
          "Demand": 50,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 93",
          "Demand": 10,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 30,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 95",
          "Demand": 20,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 96",
          "Demand": 40,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 97",
          "Demand": 10,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 98",
          "Demand": 30,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 99",
          "Demand": 40,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 0,
          "FromTime": 158,
          "ToTime": 1394,
          "ServiceTime": 90
        }
      ]
    }
  ],
  "VehicleTypes": [
    {
      "Name": "small",
      "Capacity": 100,
      "CostPerKm": 1,
      "CostAsContractVehicle": 100,
      "CostAsSpotVehicle": 150
    },
    {
      "Name": "medium",
      "Capacity": 200,
      "CostPerKm": 2,
      "CostAsContractVehicle": 200,
      "CostAsSpotVehicle": 300
    },
    {
      "Name": "large",
      "Capacity": 300,
      "CostPerKm": 3,
      "CostAsContractVehicle": 300,
      "CostAsSpotVehicle": 500
    }
  ],
  "Depots": [
    {
      "Vehicles": {
        "small": 15,
        "medium": 15,
        "large": 15
      },
      "Name": "depot 0",
      "Coords": {
        "X": 40.0,
        "Y": 50.0
      }
    }
  ],
  "Clients": [
    {
      "Name": "customer 1",
      "Coords": {
        "X": 45.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 2",
      "Coords": {
        "X": 45.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 3",
      "Coords": {
        "X": 42.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 4",
      "Coords": {
        "X": 42.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 5",
      "Coords": {
        "X": 42.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 6",
      "Coords": {
        "X": 40.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 7",
      "Coords": {
        "X": 40.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 8",
      "Coords": {
        "X": 38.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 9",
      "Coords": {
        "X": 38.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 10",
      "Coords": {
        "X": 35.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 11",
      "Coords": {
        "X": 35.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 12",
      "Coords": {
        "X": 25.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 13",
      "Coords": {
        "X": 22.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 14",
      "Coords": {
        "X": 22.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 15",
      "Coords": {
        "X": 20.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 16",
      "Coords": {
        "X": 20.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 17",
      "Coords": {
        "X": 18.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 18",
      "Coords": {
        "X": 15.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 19",
      "Coords": {
        "X": 15.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 20",
      "Coords": {
        "X": 30.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 21",
      "Coords": {
        "X": 30.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 22",
      "Coords": {
        "X": 28.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 23",
      "Coords": {
        "X": 28.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 24",
      "Coords": {
        "X": 25.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 25",
      "Coords": {
        "X": 25.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 26",
      "Coords": {
        "X": 25.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 27",
      "Coords": {
        "X": 23.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 28",
      "Coords": {
        "X": 23.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 29",
      "Coords": {
        "X": 20.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 30",
      "Coords": {
        "X": 20.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 31",
      "Coords": {
        "X": 10.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 32",
      "Coords": {
        "X": 10.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 33",
      "Coords": {
        "X": 8.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 34",
      "Coords": {
        "X": 8.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 35",
      "Coords": {
        "X": 5.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 36",
      "Coords": {
        "X": 5.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 37",
      "Coords": {
        "X": 2.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 38",
      "Coords": {
        "X": 0.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 39",
      "Coords": {
        "X": 0.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 40",
      "Coords": {
        "X": 35.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 41",
      "Coords": {
        "X": 35.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 42",
      "Coords": {
        "X": 33.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 43",
      "Coords": {
        "X": 33.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 44",
      "Coords": {
        "X": 32.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 45",
      "Coords": {
        "X": 30.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 46",
      "Coords": {
        "X": 30.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 47",
      "Coords": {
        "X": 30.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 48",
      "Coords": {
        "X": 28.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 49",
      "Coords": {
        "X": 28.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 50",
      "Coords": {
        "X": 26.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 51",
      "Coords": {
        "X": 25.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 52",
      "Coords": {
        "X": 25.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 53",
      "Coords": {
        "X": 44.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 54",
      "Coords": {
        "X": 42.0,
        "Y": 10.0
      }
    },
    {
      "Name": "customer 55",
      "Coords": {
        "X": 42.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 56",
      "Coords": {
        "X": 40.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 57",
      "Coords": {
        "X": 40.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 58",
      "Coords": {
        "X": 38.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 59",
      "Coords": {
        "X": 38.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 60",
      "Coords": {
        "X": 35.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 61",
      "Coords": {
        "X": 50.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 62",
      "Coords": {
        "X": 50.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 63",
      "Coords": {
        "X": 50.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 64",
      "Coords": {
        "X": 48.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 65",
      "Coords": {
        "X": 48.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 66",
      "Coords": {
        "X": 47.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 67",
      "Coords": {
        "X": 47.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 68",
      "Coords": {
        "X": 45.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 69",
      "Coords": {
        "X": 45.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 70",
      "Coords": {
        "X": 95.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 71",
      "Coords": {
        "X": 95.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 72",
      "Coords": {
        "X": 53.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 73",
      "Coords": {
        "X": 92.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 74",
      "Coords": {
        "X": 53.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 75",
      "Coords": {
        "X": 45.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 76",
      "Coords": {
        "X": 90.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 77",
      "Coords": {
        "X": 88.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 78",
      "Coords": {
        "X": 88.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 79",
      "Coords": {
        "X": 87.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 80",
      "Coords": {
        "X": 85.0,
        "Y": 25.0
      }
    },
    {
      "Name": "customer 81",
      "Coords": {
        "X": 85.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 82",
      "Coords": {
        "X": 75.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 83",
      "Coords": {
        "X": 72.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 84",
      "Coords": {
        "X": 70.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 85",
      "Coords": {
        "X": 68.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 86",
      "Coords": {
        "X": 66.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 87",
      "Coords": {
        "X": 65.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 88",
      "Coords": {
        "X": 65.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 89",
      "Coords": {
        "X": 63.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 90",
      "Coords": {
        "X": 60.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 91",
      "Coords": {
        "X": 60.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 92",
      "Coords": {
        "X": 67.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 93",
      "Coords": {
        "X": 65.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 94",
      "Coords": {
        "X": 65.0,
        "Y": 82.0
      }
    },
    {
      "Name": "customer 95",
      "Coords": {
        "X": 62.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 96",
      "Coords": {
        "X": 60.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 97",
      "Coords": {
        "X": 60.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 98",
      "Coords": {
        "X": 58.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 99",
      "Coords": {
        "X": 55.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 100",
      "Coords": {
        "X": 55.0,
        "Y": 85.0
      }
    }
  ]
}
```
</details>

<details>
<summary>fleet_structure.json</summary>

```json
{
  "FleetPositions": [
    {
      "Count": 14,
      "RentalType": 0,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 2500,
      "DayUsageCost": 0,
      "CostPerKm": 0,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 8,
      "RentalType": 0,
      "Name": "medium",
      "Capacity": 200,
      "MonthUsageCost": 5000,
      "DayUsageCost": 0,
      "CostPerKm": 0,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 750,
      "DayUsageCost": 0,
      "CostPerKm": 0,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 2250,
      "DayUsageCost": 0,
      "CostPerKm": 0,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    },
    {
      "Count": 5,
      "RentalType": 1,
      "Name": "small",
      "Capacity": 100,
      "MonthUsageCost": 750,
      "DayUsageCost": 0,
      "CostPerKm": 0,
      "SourceDepotName": "depot 0",
      "StartDay": 0,
      "EndDay": 0
    }
  ],
  "EstimatedCost": 93750
}
```
</details>

# create-characteristic-days
Creates model with three types of characteristic days (assuming `N` - is number of points in model they will look like: `Hard`- `N` number of visits, `Normal` - `0.5 * N` number of visits, `Easy` - `0.3 * N` number of visits)  model from Solomon Benchmark.
```
-b, --benchmark-path    Required. Path to Solomon Benchmark that will be used to generate characteristic days

-s, --save-to           (Default: characteristic_days_model.json) Path where result model will be saved
```
### Example
```
fleet-structure-solver create-characteristic-days -b c101.txt
```

<details>
<summary>c101.txt</summary>

```
C101

VEHICLE
NUMBER     CAPACITY
  25         200

CUSTOMER
CUST NO.  XCOORD.   YCOORD.    DEMAND   READY TIME  DUE DATE   SERVICE   TIME
 
    0      40         50          0          0       1236          0   
    1      45         68         10        912        967         90   
    2      45         70         30        825        870         90   
    3      42         66         10         65        146         90   
    4      42         68         10        727        782         90   
    5      42         65         10         15         67         90   
    6      40         69         20        621        702         90   
    7      40         66         20        170        225         90   
    8      38         68         20        255        324         90   
    9      38         70         10        534        605         90   
   10      35         66         10        357        410         90   
   11      35         69         10        448        505         90   
   12      25         85         20        652        721         90   
   13      22         75         30         30         92         90   
   14      22         85         10        567        620         90   
   15      20         80         40        384        429         90   
   16      20         85         40        475        528         90   
   17      18         75         20         99        148         90   
   18      15         75         20        179        254         90   
   19      15         80         10        278        345         90   
   20      30         50         10         10         73         90   
   21      30         52         20        914        965         90   
   22      28         52         20        812        883         90   
   23      28         55         10        732        777         90   
   24      25         50         10         65        144         90   
   25      25         52         40        169        224         90   
   26      25         55         10        622        701         90   
   27      23         52         10        261        316         90   
   28      23         55         20        546        593         90   
   29      20         50         10        358        405         90   
   30      20         55         10        449        504         90   
   31      10         35         20        200        237         90   
   32      10         40         30         31        100         90   
   33       8         40         40         87        158         90   
   34       8         45         20        751        816         90   
   35       5         35         10        283        344         90   
   36       5         45         10        665        716         90   
   37       2         40         20        383        434         90   
   38       0         40         30        479        522         90   
   39       0         45         20        567        624         90   
   40      35         30         10        264        321         90   
   41      35         32         10        166        235         90   
   42      33         32         20         68        149         90   
   43      33         35         10         16         80         90   
   44      32         30         10        359        412         90   
   45      30         30         10        541        600         90   
   46      30         32         30        448        509         90   
   47      30         35         10       1054       1127         90   
   48      28         30         10        632        693         90   
   49      28         35         10       1001       1066         90   
   50      26         32         10        815        880         90   
   51      25         30         10        725        786         90   
   52      25         35         10        912        969         90   
   53      44          5         20        286        347         90   
   54      42         10         40        186        257         90   
   55      42         15         10         95        158         90   
   56      40          5         30        385        436         90   
   57      40         15         40         35         87         90   
   58      38          5         30        471        534         90   
   59      38         15         10        651        740         90   
   60      35          5         20        562        629         90   
   61      50         30         10        531        610         90   
   62      50         35         20        262        317         90   
   63      50         40         50        171        218         90   
   64      48         30         10        632        693         90   
   65      48         40         10         76        129         90   
   66      47         35         10        826        875         90   
   67      47         40         10         12         77         90   
   68      45         30         10        734        777         90   
   69      45         35         10        916        969         90   
   70      95         30         30        387        456         90   
   71      95         35         20        293        360         90   
   72      53         30         10        450        505         90   
   73      92         30         10        478        551         90   
   74      53         35         50        353        412         90   
   75      45         65         20        997       1068         90   
   76      90         35         10        203        260         90   
   77      88         30         10        574        643         90   
   78      88         35         20        109        170         90   
   79      87         30         10        668        731         90   
   80      85         25         10        769        820         90   
   81      85         35         30         47        124         90   
   82      75         55         20        369        420         90   
   83      72         55         10        265        338         90   
   84      70         58         20        458        523         90   
   85      68         60         30        555        612         90   
   86      66         55         10        173        238         90   
   87      65         55         20         85        144         90   
   88      65         60         30        645        708         90   
   89      63         58         10        737        802         90   
   90      60         55         10         20         84         90   
   91      60         60         10        836        889         90   
   92      67         85         20        368        441         90   
   93      65         85         40        475        518         90   
   94      65         82         10        285        336         90   
   95      62         80         30        196        239         90   
   96      60         80         10         95        156         90   
   97      60         85         30        561        622         90   
   98      58         75         20         30         84         90   
   99      55         80         10        743        820         90   
  100      55         85         20        647        726         90   

```
</details>

<details>
<summary>characteristic_days_model.json (see class `DayBreaks.Problem.ProblemModel`)</summary>

```json
{
  "Budget": 10000000,
  "MaxDistance": 10000000,
  "Depots": [
    {
      "Vehicles": {
        "small": 15,
        "medium": 15,
        "large": 15
      },
      "Name": "depot 0",
      "Coords": {
        "X": 40.0,
        "Y": 50.0
      }
    }
  ]
  "Days": [
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 2",
          "Demand": 30,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 10,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 15,
      "Visits": [
        {
          "PointName": "customer 3",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 10,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 30,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 20,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 50,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 0,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 0,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 10,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 20,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 0,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 10,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 20,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 40,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 10,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 30,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 50,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 0,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 0,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 30,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 10,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 20,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 40,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 0,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 10,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 0,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 30,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 20,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 40,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 10,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 30,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 20,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        }
      ]
    },
    {
      "Occurrences": 5,
      "Visits": [
        {
          "PointName": "customer 1",
          "Demand": 40,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 2",
          "Demand": 50,
          "FromTime": 1236,
          "ToTime": 2472,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 3",
          "Demand": 10,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 4",
          "Demand": 30,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 5",
          "Demand": 20,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 6",
          "Demand": 50,
          "FromTime": 967,
          "ToTime": 2203,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 7",
          "Demand": 0,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 8",
          "Demand": 40,
          "FromTime": 870,
          "ToTime": 2106,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 9",
          "Demand": 30,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 10",
          "Demand": 20,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 11",
          "Demand": 40,
          "FromTime": 146,
          "ToTime": 1382,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 12",
          "Demand": 30,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 13",
          "Demand": 50,
          "FromTime": 782,
          "ToTime": 2018,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 14",
          "Demand": 30,
          "FromTime": 67,
          "ToTime": 1303,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 15",
          "Demand": 0,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 16",
          "Demand": 20,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 17",
          "Demand": 40,
          "FromTime": 702,
          "ToTime": 1938,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 18",
          "Demand": 10,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 19",
          "Demand": 20,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 20",
          "Demand": 40,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 21",
          "Demand": 50,
          "FromTime": 225,
          "ToTime": 1461,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 22",
          "Demand": 10,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 23",
          "Demand": 20,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 24",
          "Demand": 50,
          "FromTime": 324,
          "ToTime": 1560,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 25",
          "Demand": 0,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 26",
          "Demand": 10,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 27",
          "Demand": 20,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 28",
          "Demand": 40,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 29",
          "Demand": 50,
          "FromTime": 605,
          "ToTime": 1841,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 30",
          "Demand": 0,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 31",
          "Demand": 10,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 32",
          "Demand": 20,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 33",
          "Demand": 40,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 34",
          "Demand": 50,
          "FromTime": 410,
          "ToTime": 1646,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 35",
          "Demand": 30,
          "FromTime": 505,
          "ToTime": 1741,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 36",
          "Demand": 30,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 37",
          "Demand": 20,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 38",
          "Demand": 50,
          "FromTime": 721,
          "ToTime": 1957,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 39",
          "Demand": 50,
          "FromTime": 92,
          "ToTime": 1328,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 40",
          "Demand": 0,
          "FromTime": 620,
          "ToTime": 1856,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 41",
          "Demand": 0,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 42",
          "Demand": 20,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 43",
          "Demand": 40,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 44",
          "Demand": 50,
          "FromTime": 429,
          "ToTime": 1665,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 45",
          "Demand": 0,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 46",
          "Demand": 10,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 47",
          "Demand": 30,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 48",
          "Demand": 50,
          "FromTime": 528,
          "ToTime": 1764,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 49",
          "Demand": 0,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 50",
          "Demand": 10,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 51",
          "Demand": 20,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 52",
          "Demand": 40,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 53",
          "Demand": 50,
          "FromTime": 148,
          "ToTime": 1384,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 54",
          "Demand": 0,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 55",
          "Demand": 10,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 56",
          "Demand": 30,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 57",
          "Demand": 50,
          "FromTime": 254,
          "ToTime": 1490,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 58",
          "Demand": 0,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 59",
          "Demand": 30,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 60",
          "Demand": 40,
          "FromTime": 345,
          "ToTime": 1581,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 61",
          "Demand": 30,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 62",
          "Demand": 40,
          "FromTime": 73,
          "ToTime": 1309,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 63",
          "Demand": 0,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 64",
          "Demand": 10,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 65",
          "Demand": 20,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 66",
          "Demand": 40,
          "FromTime": 965,
          "ToTime": 2201,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 67",
          "Demand": 0,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 68",
          "Demand": 10,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 69",
          "Demand": 40,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 70",
          "Demand": 50,
          "FromTime": 883,
          "ToTime": 2119,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 71",
          "Demand": 10,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 72",
          "Demand": 50,
          "FromTime": 777,
          "ToTime": 2013,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 73",
          "Demand": 40,
          "FromTime": 144,
          "ToTime": 1380,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 74",
          "Demand": 0,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 75",
          "Demand": 10,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 76",
          "Demand": 30,
          "FromTime": 224,
          "ToTime": 1460,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 77",
          "Demand": 30,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 78",
          "Demand": 40,
          "FromTime": 701,
          "ToTime": 1937,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 79",
          "Demand": 0,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 80",
          "Demand": 10,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 81",
          "Demand": 30,
          "FromTime": 316,
          "ToTime": 1552,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 82",
          "Demand": 0,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 83",
          "Demand": 40,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 84",
          "Demand": 50,
          "FromTime": 593,
          "ToTime": 1829,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 85",
          "Demand": 0,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 86",
          "Demand": 30,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 87",
          "Demand": 40,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 88",
          "Demand": 50,
          "FromTime": 405,
          "ToTime": 1641,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 89",
          "Demand": 0,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 90",
          "Demand": 30,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 91",
          "Demand": 40,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 92",
          "Demand": 50,
          "FromTime": 504,
          "ToTime": 1740,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 93",
          "Demand": 10,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 94",
          "Demand": 30,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 95",
          "Demand": 20,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 96",
          "Demand": 40,
          "FromTime": 237,
          "ToTime": 1473,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 97",
          "Demand": 10,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 98",
          "Demand": 30,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 99",
          "Demand": 40,
          "FromTime": 100,
          "ToTime": 1336,
          "ServiceTime": 90
        },
        {
          "PointName": "customer 100",
          "Demand": 0,
          "FromTime": 158,
          "ToTime": 1394,
          "ServiceTime": 90
        }
      ]
    }
  ],
  "VehicleTypes": [
    {
      "Name": "small",
      "Capacity": 100,
      "CostPerKm": 1,
      "CostAsContractVehicle": 100,
      "CostAsSpotVehicle": 150
    },
    {
      "Name": "medium",
      "Capacity": 200,
      "CostPerKm": 2,
      "CostAsContractVehicle": 200,
      "CostAsSpotVehicle": 300
    },
    {
      "Name": "large",
      "Capacity": 300,
      "CostPerKm": 3,
      "CostAsContractVehicle": 300,
      "CostAsSpotVehicle": 500
    }
  ], "Clients": [
    {
      "Name": "customer 1",
      "Coords": {
        "X": 45.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 2",
      "Coords": {
        "X": 45.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 3",
      "Coords": {
        "X": 42.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 4",
      "Coords": {
        "X": 42.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 5",
      "Coords": {
        "X": 42.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 6",
      "Coords": {
        "X": 40.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 7",
      "Coords": {
        "X": 40.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 8",
      "Coords": {
        "X": 38.0,
        "Y": 68.0
      }
    },
    {
      "Name": "customer 9",
      "Coords": {
        "X": 38.0,
        "Y": 70.0
      }
    },
    {
      "Name": "customer 10",
      "Coords": {
        "X": 35.0,
        "Y": 66.0
      }
    },
    {
      "Name": "customer 11",
      "Coords": {
        "X": 35.0,
        "Y": 69.0
      }
    },
    {
      "Name": "customer 12",
      "Coords": {
        "X": 25.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 13",
      "Coords": {
        "X": 22.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 14",
      "Coords": {
        "X": 22.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 15",
      "Coords": {
        "X": 20.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 16",
      "Coords": {
        "X": 20.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 17",
      "Coords": {
        "X": 18.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 18",
      "Coords": {
        "X": 15.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 19",
      "Coords": {
        "X": 15.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 20",
      "Coords": {
        "X": 30.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 21",
      "Coords": {
        "X": 30.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 22",
      "Coords": {
        "X": 28.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 23",
      "Coords": {
        "X": 28.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 24",
      "Coords": {
        "X": 25.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 25",
      "Coords": {
        "X": 25.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 26",
      "Coords": {
        "X": 25.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 27",
      "Coords": {
        "X": 23.0,
        "Y": 52.0
      }
    },
    {
      "Name": "customer 28",
      "Coords": {
        "X": 23.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 29",
      "Coords": {
        "X": 20.0,
        "Y": 50.0
      }
    },
    {
      "Name": "customer 30",
      "Coords": {
        "X": 20.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 31",
      "Coords": {
        "X": 10.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 32",
      "Coords": {
        "X": 10.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 33",
      "Coords": {
        "X": 8.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 34",
      "Coords": {
        "X": 8.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 35",
      "Coords": {
        "X": 5.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 36",
      "Coords": {
        "X": 5.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 37",
      "Coords": {
        "X": 2.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 38",
      "Coords": {
        "X": 0.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 39",
      "Coords": {
        "X": 0.0,
        "Y": 45.0
      }
    },
    {
      "Name": "customer 40",
      "Coords": {
        "X": 35.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 41",
      "Coords": {
        "X": 35.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 42",
      "Coords": {
        "X": 33.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 43",
      "Coords": {
        "X": 33.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 44",
      "Coords": {
        "X": 32.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 45",
      "Coords": {
        "X": 30.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 46",
      "Coords": {
        "X": 30.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 47",
      "Coords": {
        "X": 30.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 48",
      "Coords": {
        "X": 28.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 49",
      "Coords": {
        "X": 28.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 50",
      "Coords": {
        "X": 26.0,
        "Y": 32.0
      }
    },
    {
      "Name": "customer 51",
      "Coords": {
        "X": 25.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 52",
      "Coords": {
        "X": 25.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 53",
      "Coords": {
        "X": 44.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 54",
      "Coords": {
        "X": 42.0,
        "Y": 10.0
      }
    },
    {
      "Name": "customer 55",
      "Coords": {
        "X": 42.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 56",
      "Coords": {
        "X": 40.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 57",
      "Coords": {
        "X": 40.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 58",
      "Coords": {
        "X": 38.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 59",
      "Coords": {
        "X": 38.0,
        "Y": 15.0
      }
    },
    {
      "Name": "customer 60",
      "Coords": {
        "X": 35.0,
        "Y": 5.0
      }
    },
    {
      "Name": "customer 61",
      "Coords": {
        "X": 50.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 62",
      "Coords": {
        "X": 50.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 63",
      "Coords": {
        "X": 50.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 64",
      "Coords": {
        "X": 48.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 65",
      "Coords": {
        "X": 48.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 66",
      "Coords": {
        "X": 47.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 67",
      "Coords": {
        "X": 47.0,
        "Y": 40.0
      }
    },
    {
      "Name": "customer 68",
      "Coords": {
        "X": 45.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 69",
      "Coords": {
        "X": 45.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 70",
      "Coords": {
        "X": 95.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 71",
      "Coords": {
        "X": 95.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 72",
      "Coords": {
        "X": 53.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 73",
      "Coords": {
        "X": 92.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 74",
      "Coords": {
        "X": 53.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 75",
      "Coords": {
        "X": 45.0,
        "Y": 65.0
      }
    },
    {
      "Name": "customer 76",
      "Coords": {
        "X": 90.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 77",
      "Coords": {
        "X": 88.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 78",
      "Coords": {
        "X": 88.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 79",
      "Coords": {
        "X": 87.0,
        "Y": 30.0
      }
    },
    {
      "Name": "customer 80",
      "Coords": {
        "X": 85.0,
        "Y": 25.0
      }
    },
    {
      "Name": "customer 81",
      "Coords": {
        "X": 85.0,
        "Y": 35.0
      }
    },
    {
      "Name": "customer 82",
      "Coords": {
        "X": 75.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 83",
      "Coords": {
        "X": 72.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 84",
      "Coords": {
        "X": 70.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 85",
      "Coords": {
        "X": 68.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 86",
      "Coords": {
        "X": 66.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 87",
      "Coords": {
        "X": 65.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 88",
      "Coords": {
        "X": 65.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 89",
      "Coords": {
        "X": 63.0,
        "Y": 58.0
      }
    },
    {
      "Name": "customer 90",
      "Coords": {
        "X": 60.0,
        "Y": 55.0
      }
    },
    {
      "Name": "customer 91",
      "Coords": {
        "X": 60.0,
        "Y": 60.0
      }
    },
    {
      "Name": "customer 92",
      "Coords": {
        "X": 67.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 93",
      "Coords": {
        "X": 65.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 94",
      "Coords": {
        "X": 65.0,
        "Y": 82.0
      }
    },
    {
      "Name": "customer 95",
      "Coords": {
        "X": 62.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 96",
      "Coords": {
        "X": 60.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 97",
      "Coords": {
        "X": 60.0,
        "Y": 85.0
      }
    },
    {
      "Name": "customer 98",
      "Coords": {
        "X": 58.0,
        "Y": 75.0
      }
    },
    {
      "Name": "customer 99",
      "Coords": {
        "X": 55.0,
        "Y": 80.0
      }
    },
    {
      "Name": "customer 100",
      "Coords": {
        "X": 55.0,
        "Y": 85.0
      }
    }
  ]
}
```
</details>



# reproduce-thesis-fleet-structure-predictions   
Runs fleet structure predictions with selected methods for same configurations as one used in thesis.

```
-p, --benchmarks-path    Required. Path to the directory that contains c51 and c101 instances of Solomon Benchmark.

-s, --solution-path      (Default: tests) Path to folder to which execution results would be saved

-m, --methods            Required. Method that will be used for fleet structure prediction. One of: sat, vrp, linear
```

### Example
```
fleet-structure-solver  reproduce-thesis-fleet-structure-predictions -p solomon_benchmarks -m sat linear vrp
```

Output:
Directory `tests` with three folders:

- IsDayAmountAffectsPlanningEfficiency
- IsDayComplexityAffectsPlanningEfficiency
- IsDayOrderAffectsPlanningEfficiency

Each folder will contain subfolders for each of the used prediction methods. Each subfolder will contain fleet predictions and characteristic day models for which predictions were done.


# mh-test
Run tests comparing such local search metaheuristic as: Simulated Annealing, Guided Local Search, Tabu Search and Greedy Descent for an instance of a Solomon Benchmark
 
```
-p, --benchmark-path    Required. Path to the Solomon Benchmark on which metaheuristics will be tested.

-s, --save-to           (Default: metaheuristic_test_results) Path to folder where solutions made by every metaheuristic will be saved

-t, --time-limit        (Default: 180) Time in seconds every metaheuristic can spent on the solving process
```

### Example
```
fleet-structure-solver  mh-test -p solomon_benchmarks/c101.txt
```
Output:
Folder `metaheuristic_test_results` with routes predicted by each of metaheuristic.



# Getting Started

Download archive `fleet-structure-solver.zip` from the latest release on the releases [page](https://github.com/Qwebeck/capacity-planning-solver/releases).
Unzip folder the folder and run

```
.\fleet-structure-solver 
```
to see available commands.


