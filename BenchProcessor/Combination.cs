using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class Combination
    {
        private List<List<int>> result = new List<List<int>>();

        public Combination()
        {
            this.DigitCount = 0;
            this.Combinations = 0;
        }

        public int DigitCount { get; set; }

        public int Combinations { get; set; }

        public List<List<int>> GetCombinations()
        {
            this.result.Clear();

            if (this.DigitCount > 0 && this.Combinations > 0)
                GetCombinations(new List<int>(), -1);
            else
                throw new Exception("Input set or combination is not defined!");

            return new List<List<int>>(this.result);
        }

        public List<List<int>> GetCombinations(int digitcount, int combinations)
        {
            this.DigitCount = digitcount;
            this.Combinations = combinations;

            return this.GetCombinations();
        }

        private void GetCombinations(List<int> current, int index)
        {
            for (int i = index + 1; i < this.DigitCount; i++)
            {
                List<int> tmpList = new List<int>(current);
                tmpList.Add(i);

                GetCombinations(tmpList, i);
            }

            if (current.Count == this.Combinations)
            {
                this.result.Add(current);
            }
        }
    }
}
