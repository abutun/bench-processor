using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class ItemIndexList: List<int>
    {
        public ItemIndexList()
        {
            Code = "";
        }

        public new void Add(int index)
        {
            base.Add(index);

            if (Code.Equals(""))
            {
                Code += index;
            }
            else
            {
                Code += "-" + index;
            }
        }

        public String Code { get; internal set; }
    }
}
