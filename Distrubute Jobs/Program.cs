using BenchProcessor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrubute_Jobs
{
    class Program
    {
        private static Utility utility = new Utility();

        static void Main(string[] args)
        {
            List<BenchPmlExecutionList> result = utility.GetItemExecutionList(Enumerable.Range(0, 3).ToList(), 5);

            result = utility.GetItemExecutionList(Enumerable.Range(0, 5).ToList(), 4);
        }
    }
}
