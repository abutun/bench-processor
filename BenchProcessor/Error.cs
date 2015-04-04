using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Tezgahta olabilecek hatalar ve kodları
    /// </summary>
    public enum Error { 
        PML_OPERATION_IS_NOT_SUPPORTED = -1, 
        PML_NO_AVAILABLE_MU_FOUND = -2, 
        PML_IS_NOT_AVAILABLE = -3, 
        PML_OPERATION_IS_NOT_FOUND = -4,
        PML_ITEM_IS_NOT_FOUND = -5,
        PML_PML_NOT_FOUND = -6,
        PML_NOT_ENOUGH_MU_FOR_OPERATIONS = -7,
        MU_OPERATION_IS_NOT_SUPPORTED = -10, 
        MU_IS_NOT_AVAILABLE = -11, 
        MU_INVALID_OPERATION = -12 
    }
}
