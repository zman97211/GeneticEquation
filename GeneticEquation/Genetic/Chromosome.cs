using System.Text;

namespace GeneticEquation.Genetic
{



    public abstract class Chromosome<T>
    {
        public double Fitness { get; set; }

        public T[] Genes { get; set; }

        protected Chromosome(int numGenes)
        {
            Genes = new T[numGenes];
            for (int i = 0; i < numGenes; i++)
                Genes[i] = GetRandomGene();
        }

        protected abstract T GetRandomGene();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var g in Genes)
                sb.Append(g.ToString());
            return sb.ToString();
        }
    }

}
