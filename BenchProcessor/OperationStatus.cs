using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Operasyonların durum bilgileri ve kodları
    /// </summary>
    public enum OperationStatus { 
        WAITING = 0, 
        IN_PROCESS = 1, 
        DONE = 2, 
        HAS_ERROR = 3 
    }
}
