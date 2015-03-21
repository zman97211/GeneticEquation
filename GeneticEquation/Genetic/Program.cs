using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeneticEquation.Genetic;
using MoreLinq;

/*
 * 
 * Genes: Binary encoded in this order: 0 1 2 3 4 5 6 7 8 9 + - * /
 * 
 * Chromosome: We'll start with 9 genes. It looks like this should always be odd.
 * 
 */

namespace GeneticEquation
{
    internal class Program
    {
        private static void Main()
        {
            var target = 123456;

            var logFilename = string.Format(@"c:\Users\steve\Desktop\logs\GenRun-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv",
                DateTime.Now);

            var p = new Population(target, 140, 15, 0.2);
            var generation = 0;
            Stopwatch sw = new Stopwatch();
            while (!p.Chromosomes.Any(c => double.IsInfinity(c.Fitness)))
            {
                var best = p.Chromosomes.MaxBy(c => c.Fitness);
                if (generation % 500 == 0)
                {
                    File.AppendAllText(logFilename, string.Format("{0},{1},{2},{3},{4}\n", generation, best,
                        best.ChromosomeValue, target, best.ChromosomeValue - target));
                    Console.WriteLine("[{5}] {0}: {1} Value: {2} Target: {3} Error: {4}", generation, best,
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
