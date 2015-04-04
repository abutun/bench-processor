using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class BenchPmlExecutionList : List<PmlItemExecutionList>, IComparable<BenchPmlExecutionList>
    {
        public BenchPmlExecutionList()
        {
        }

        public String Code
        {
            get
            {
                string code = "";

                foreach (PmlItemExecutionList list in this)
                {
                    if (code.Equals(""))
                    {
                        code += "{" + list.Code + "}";
                    }
                    else
                    {
                        code += "-{" + list.Code + "}";
                    }
                }

                return code;
            }
        }

        public int CompareTo(BenchPmlExecutionList other)
        {
            return this.Code.CompareTo(other.Code);
        }
    }
}
