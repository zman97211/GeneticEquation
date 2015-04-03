using System;
using System.Collections.Generic;
using System.Linq;
using GeneticEquation.Utility;

namespace GeneticEquation.Genetic
{

    public class Population
    {
        public List<EquationChromosome> Chromosomes { get; set; }
        public int GeneCount { get; set; }
        public int TargetValue { get; set; }
        public double MutationRate { get; set; }

        private readonly Random _random = new Random();


        public Population(int target, int count, int geneCount, double mutationRate)
        {
            MutationRate = mutationRate;
            TargetValue = target;
            GeneCount = geneCount;
            Chromosomes = new List<EquationChromosome>();
            for (var i = 0; i < count; i++)
            {
                var c = new EquationChromosome(geneCount);
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
            foreach (var child in children)
                child.Recalculate();
            Chromosomes.Clear();
            Chromosomes.AddRange(parents);
            Chromosomes.AddRange(children);
            ScoreEm();
        }

        private double CalculateFitness(EquationChromosome c, int target)
        {
            return 1 / Math.Abs(target - c.ChromosomeValue);
        }

        public void ScoreEm()
        {
            foreach (var c in Chromosomes)
                c.Fitness = CalculateFitness(c, TargetValue);
        }

        private List<EquationChromosome> SelectBreeders()
        {
            var startingPopulation = new List<EquationChromosome>(Chromosomes);
            var selected = new List<EquationChromosome>();
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

        private List<EquationChromosome> Breed(List<EquationChromosome> parents)
        {
            parents.Shuffle();
            var children = new List<EquationChromosome>();
            for (var i = 0; i < parents.Count / 2; i++)
            {
                EquationChromosome child1;
                EquationChromosome child2;
                
                Breed(parents[i * 2], parents[i * 2 + 1], out child1, out child2);
                children.Add(child1);
                children.Add(child2);
            }
            return children;
        }

        private void Breed(EquationChromosome p1, EquationChromosome p2, out EquationChromosome c1, out EquationChromosome c2)
        {
            c1 = new EquationChromosome(p1.Genes.Count());
            c2 = new EquationChromosome(p1.Genes.Count());

            int crossoverPoint = _random.Next(p1.Genes.Length / 3) + p1.Genes.Length / 3;

            for (var i = 0; i < p1.Genes.Count(); i++)
            {
                if (i < crossoverPoint)
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

        private void Mutate(List<EquationChromosome> population)
        {
            foreach (var c in population)
                for (var i = 0; i < c.Genes.Count(); i++)
                    if (_random.NextDouble() < MutationRate)
                        c.Genes[i] = (Gene)(_random.Next() % (int)Gene.Divide);
        }
    }

}
