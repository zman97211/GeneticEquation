using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MoreLinq;

namespace GeneticEquation.Genetic
{
    internal class Program
    {
        private static void Main()
        {
            const int target = 4100;

            var logFilename = string.Format(@"c:\Users\steve\Desktop\logs\GenRun-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv",
                DateTime.Now);

            var p = new Population(target, 160, 9, 0.3);
            var generation = 0;
            Stopwatch sw = new Stopwatch();
            while (!p.Chromosomes.Any(c => double.IsInfinity(c.Fitness)))
            {
                var best = p.Chromosomes.MaxBy(c => c.Fitness);
                if (generation % 500 == 0)
                {
                    File.AppendAllText(logFilename, string.Format("{0},{1},{2},{3},{4}\n", generation, best,
                        best.ChromosomeValue, target, best.ChromosomeValue - target));
                    Console.WriteLine("[{5}] {0}: Value: {2} Target: {3} Error: {4}", generation, best,
                        best.ChromosomeValue, target, best.ChromosomeValue - target, sw.Elapsed);
                }
                sw.Reset();
                sw.Start();
                p.NextGen();
                sw.Stop();
                generation++;
            }

            var solution = p.Chromosomes.First(c => double.IsInfinity(c.Fitness));
            Console.WriteLine();
            File.AppendAllText(logFilename,
                string.Format("{0},{1},{2},{3},{4}\n", generation, solution, solution.ChromosomeValue, target,
                    solution.ChromosomeValue - target));
            Console.WriteLine("*** SOLUTION: Generation: {0} Best fitness: {1} {2} = {3}", generation, solution.Fitness,
                solution, solution.ChromosomeValue);
            Console.Write("Press enter to quit...");
            Console.ReadLine();
        }
    }
}
