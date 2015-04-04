using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Operasyon modu
    /// TURNING = TORNA
    /// MILLING = FREZE
    /// CONTOURING = KONTÜRLEME
    /// </summary>
    public enum OperationMode { 
        TURNING = 0,
        MILLING = 1, 
        CONTOURING = 2, 
        UNKNOWN = 10 
    }
}
