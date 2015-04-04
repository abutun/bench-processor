using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Parçaların durum bilgileri ve kodları
    /// </summary>
    public enum ItemStatus { 
        AVAILABLE = 0, 
        RESERVED = 2, 
        DONE = 3, 
        ERROR = 4
    }
}
