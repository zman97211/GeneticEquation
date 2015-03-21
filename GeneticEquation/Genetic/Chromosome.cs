using System;
using System.Text;

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


    public class Chromosome
    {
        public double Fitness { get; set; }

        public Gene[] Genes { get; set; }

        public Chromosome(int numGenes)
        {
            Genes = new Gene[numGenes];
        }

        private double _cachedValue = 0;

        public double ChromosomeValue
        {
            get { return _cachedValue; }
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
                            value += (int) g;
                            findingNumber = false;
                            break;
                        case Gene.Subtract:
                            value -= (int) g;
                            findingNumber = false;
                            break;
                        case Gene.Multiply:
                            value *= (int) g;
                            findingNumber = false;
                            break;
                        case Gene.Divide:
                            if (g != Gene.Zero)
                            {
                                findingNumber = false;
                                value /= (int) g;
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

}
