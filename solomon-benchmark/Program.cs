using System;
using SolomonBenchmark.Models;

namespace SolomonBenchmark
{
    class Program
    {
        static void Main()
        {
            var model = Model.fromFile("../../../data/test.txt");
            var solver = new CapacityPlanningSolver(model);
            var solution = solver.Solve();
            Console.WriteLine(solver.DescribeSolution(solution));
            var printer = new SolutionPrinter(solution);
            printer.PrintToFile();
            printer.PrintToStdOut();

        } 
    }

}