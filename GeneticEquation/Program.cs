using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public enum Gene
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public class Chromosome
    {
        public double Fitness { get; set; }

        public Gene[] Genes { get; set; }

        public Chromosome(int numGenes)
        {
            Genes = new Gene[numGenes];
        }

        public double ChromosomeValue
        {
            get
            {
                var findingNumber = true;
                double value = 0;
                var currentOperator = Gene.Add;

                foreach (var g in Genes)
                {
                    // If we're looking for a number and found one, apply it to the current value using
                    // the current operator. Finally, stop looking for a number and start looking for an
                    // operator.
                    if (g >= Gene.Zero && g <= Gene.Nine && findingNumber)
                    {
                        switch (currentOperator)
                        {
                            case Gene.Add:
                                value += (int)g;
                                findingNumber = false;
                                break;
                            case Gene.Subtract:
                                value -= (int)g;
                                findingNumber = false;
                                break;
                            case Gene.Multiply:
                                value *= (int)g;
                                findingNumber = false;
                                break;
                            case Gene.Divide:
                                if (g != Gene.Zero)
                                {
                                    findingNumber = false;
                                    value /= (int)g;
                                }
                                break;
                            default:
                                throw new Exception("Unexpected operator when calculating fitness.");
                        }
                    }

                    // If we're looking for an operator and found one, store it as the current operator
                    // and start looking for the next number.
                    if (g >= Gene.Add && !findingNumber)
                    {
                        currentOperator = g;
                        findingNumber = true;
                    }
                }

                return value;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var g in Genes)
                switch (g)
                {
                    case Gene.Zero:
                        sb.Append("0");
                        break;
                    case Gene.One:
                        sb.Append("1");
                        break;
                    case Gene.Two:
                        sb.Append("2");
                        break;
                    case Gene.Three:
                        sb.Append("3");
                        break;
                    case Gene.Four:
                        sb.Append("4");
                        break;
                    case Gene.Five:
                        sb.Append("5");
                        break;
                    case Gene.Six:
                        sb.Append("6");
                        break;
                    case Gene.Seven:
                        sb.Append("7");
                        break;
                    case Gene.Eight:
                        sb.Append("8");
                        break;
                    case Gene.Nine:
                        sb.Append("9");
                        break;
                    case Gene.Add:
                        sb.Append("+");
                        break;
                    case Gene.Subtract:
                        sb.Append("-");
                        break;
                    case Gene.Multiply:
                        sb.Append("*");
                        break;
                    case Gene.Divide:
                        sb.Append("/");
                        break;
                }
            return sb.ToString();
        }
    }

    public class Population
    {
        public List<Chromosome> Chromosomes { get; set; }
        private readonly Random _random = new Random();
        public int GeneCount { get; set; }
        public int TargetValue { get; set; }
        private double MutationRate { get; set; }


        public Population(int target, int count, int geneCount, double mutationRate)
        {
            MutationRate = mutationRate;
            TargetValue = target;
            GeneCount = geneCount;
            Chromosomes = new List<Chromosome>();
            for (var i = 0; i < count; i++)
            {
                var c = new Chromosome(geneCount);
                for (var j = 0; j < geneCount; j++)
                    c.Genes[j] = (Gene)(_random.Next() % (int)Gene.Divide);
                //{
                //if (j%2 == 0)
                //c.Genes[j] = (Gene) (_random.Next()%10);
                //else
                //c.Genes[j] = (Gene) (10 + _random.Next()%4);
                //}
                Chromosomes.Add(c);
            }
            ScoreEm();
        }

        public void NextGen()
        {
            var parents = SelectBreeders();
            var children = Breed(parents);
            Mutate(children);
            Chromosomes.Clear();
            Chromosomes.AddRange(parents);
            Chromosomes.AddRange(children);
            ScoreEm();
        }

        private double CalculateFitness(Chromosome c, int target)
        {
            return 1 / Math.Abs(target - c.ChromosomeValue);
        }

        public void ScoreEm()
        {
            foreach (var c in Chromosomes)
                c.Fitness = CalculateFitness(c, TargetValue);
        }

        private List<Chromosome> SelectBreeders()
        {
            var startingPopulation = new List<Chromosome>(Chromosomes);
            var selected = new List<Chromosome>();
            var numToSelect = startingPopulation.Count / 2;

            startingPopulation.Shuffle();
            while (selected.Count < numToSelect)
            {
                var slice = _random.NextDouble() * startingPopulation.Sum(c => c.Fitness);
                var totalSoFar = 0.0;
                foreach (var c in startingPopulation)
                {
                    totalSoFar += c.Fitness;
                    if (totalSoFar > slice)
                    {
                        selected.Add(c);
                        startingPopulation.Remove(c);
                        break;
                    }
                }
            }

            return selected;
        }

        private List<Chromosome> Breed(List<Chromosome> parents)
        {
            parents.Shuffle();
            var children = new List<Chromosome>();
            for (var i = 0; i < parents.Count / 2; i++)
            {
                Chromosome child1;
                Chromosome child2;

                Breed(parents[i * 2], parents[i * 2 + 1], out child1, out child2);
                children.Add(child1);
                children.Add(child2);
            }
            return children;
        }

        private void Breed(Chromosome p1, Chromosome p2, out Chromosome c1, out Chromosome c2)
        {
            c1 = new Chromosome(p1.Genes.Count());
            c2 = new Chromosome(p1.Genes.Count());

            for (var i = 0; i < p1.Genes.Count(); i++)
            {
                if (i < p1.Genes.Count() * 3 / 4)
                {
                    c1.Genes[i] = p1.Genes[i];
                    c2.Genes[i] = p2.Genes[i];
                }
                else
                {
                    c1.Genes[i] = p2.Genes[i];
                    c2.Genes[i] = p1.Genes[i];
                }
            }
        }

        private void Mutate(List<Chromosome> population)
        {
            foreach (var c in population)
                for (var i = 0; i < c.Genes.Count(); i++)
                    if (_random.NextDouble() < MutationRate)
                        c.Genes[i] = (Gene)(_random.Next() % (int)Gene.Divide);
        }
    }


    internal class Program
    {
        private static void Main()
        {
            var target = 123456;

            var logFilename = string.Format(@"c:\Users\steve\Desktop\logs\GenRun-{0:yyyy-MM-dd_hh-mm-ss-tt}.csv",
                DateTime.Now);

            var p = new Population(target, 140, 13, 0.1);
            var generation = 0;
            while (!p.Chromosomes.Any(c => double.IsInfinity(c.Fitness)))
            {
                var best = p.Chromosomes.MaxBy(c => c.Fitness);
                if (generation % 500 == 0)
                {
                    File.AppendAllText(logFilename, string.Format("{0},{1},{2},{3},{4}\n", generation, best,
                        best.ChromosomeValue, target, best.ChromosomeValue - target));
                    Console.WriteLine("{0}: {1} Value: {2} Target: {3} Error: {4}", generation, best,
                        best.ChromosomeValue, target, best.ChromosomeValue - target);
                }
                p.NextGen();
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
