using System;

namespace GeneticEquation.Genetic
{
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

    public class EquationChromosome : Chromosome<Gene>
    {

        private double _cachedValue;

        public double ChromosomeValue
        {
            get { return _cachedValue; }
        }

        public EquationChromosome(int numGenes)
            : base(numGenes)
        {
        }

        public void Recalculate()
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

            _cachedValue = value;
        }

        private readonly Random _random = new Random();
        protected override Gene GetRandomGene()
        {
            return (Gene)(_random.Next() % (int)Gene.Divide);
        }
    }
}
