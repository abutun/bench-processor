using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class Permutation
    {
        private List<List<int>> result = new List<List<int>>();

        public Permutation()
        {
            this.DigitCount = 0;
            this.Permutations = 0;
            this.UseRepeatation = false;
        }

        public bool UseRepeatation { get; set; }

        public int DigitCount { get; set; }

        public int Permutations { get; set; }

        public List<List<int>> GetPermutations()
        {
            this.result.Clear();

            if (this.DigitCount > 0 && this.Permutations > 0)
            {
                GetPermutations(new List<int>());
            }

            return new List<List<int>>(this.result);
        }

        public List<List<int>> GetPermutations(int digitcount, int permutations)
        {
            this.DigitCount = digitcount;
            this.Permutations = permutations;

            return this.GetPermutations();
        }

        private void GetPermutations(List<int> current)
        {
            if (current.Count < this.Permutations)
            {
                for (int i = 0; i < this.DigitCount; i++)
                {
                    if (!current.Contains(i) || this.UseRepeatation)
                    {
                        List<int> tmpList = new List<int>(current);
                        tmpList.Add(i);

                        GetPermutations(tmpList);
                    }
                }
            }
            else if (current.Count == this.Permutations)
            {
                this.result.Add(current);
            }
        }
    }
}
