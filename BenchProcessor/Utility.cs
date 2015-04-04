using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class Utility
    {
        private Permutation permutation = new Permutation();

        private Combination combination = new Combination();

        public Utility()
        {
        }

        public List<BenchPmlExecutionList> GetItemExecutionList(List<int> pmlIndexList, int itemCount)
        {
            List<BenchPmlExecutionList> result = new List<BenchPmlExecutionList>();

            if (pmlIndexList.Count <= itemCount)
            {
                int pmlCount = pmlIndexList.Count;

                // Parça ihtimalleri
                List<List<int>> itemPermutations = permutation.GetPermutations(itemCount, itemCount);

                // PML ihtimalleri
                List<List<int>> pmlPermutations = permutation.GetPermutations(pmlCount, pmlCount);

                foreach (List<int> pmls in pmlPermutations)
                {
                    foreach (List<int> items in itemPermutations)
                    {
                        BenchPmlExecutionList benchExecutionList = new BenchPmlExecutionList();

                        Dictionary<int, PmlItemExecutionList> pmlExecutionLists = new Dictionary<int, PmlItemExecutionList>();

                        for (int i=0; i<items.Count; i++)
                        {
                            int pmlIndex = i % pmls.Count;

                            if (pmlExecutionLists.ContainsKey(pmlIndex))
                            {
                                PmlItemExecutionList list = pmlExecutionLists[pmlIndex];

                                list.ItemIndices.Add(items[i]);
                            }
                            else
                            {
                                PmlItemExecutionList list = new PmlItemExecutionList(pmlIndex);

                                list.ItemIndices.Add(items[i]);

                                pmlExecutionLists.Add(pmlIndex, list);
                            }
                        }

                        benchExecutionList.AddRange(pmlExecutionLists.Values);

                        result.Add(benchExecutionList);
                    }
                }
            }
            else
            {
                // PML kombinasyonlarını al
                List<List<int>> pmlCombinations = combination.GetCombinations(pmlIndexList.Count, itemCount);

                foreach (List<int> pmls in pmlCombinations)
                {
                    List<BenchPmlExecutionList> tmpResult = GetItemExecutionList(pmls, itemCount);

                    result.AddRange(tmpResult);
                }
            }

            return result;
        }
    }
}
