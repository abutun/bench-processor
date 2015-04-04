using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class PmlItemExecutionList
    {
        public PmlItemExecutionList(int pmlIndex)
        {
            PMLIndex = pmlIndex;

            ItemIndices = new ItemIndexList();
        }

        public int PMLIndex { get; internal set; }

        public ItemIndexList ItemIndices { get; internal set; }

        public String Code
        {
            get
            {
                return PMLIndex + "-" + ItemIndices.Code;
            }
        }
    }
}
