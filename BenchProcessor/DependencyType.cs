using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Operasyonların bağımlılık tipleri
    /// 0=bilinmeyen
    /// 1=Hedef operasyon başlamadan kaynak operasyon başlayamaz
    /// 2=Hedef operasyon tamamlanmadan kaynak operasyon başlayamaz
    /// </summary>
    public enum DependencyType { 
        UNKNOWN = 0, 
        CANNOT_START_TILL_TARGET_STARTED = 1, 
        CANNOT_START_TILL_TARGET_FINISHED = 2 
    }
}
