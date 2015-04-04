using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Bu sınıf, operasyonlar arasındaki bağımlılık ilişkilerini gösterir
    /// </summary>
    public class Dependency
    {
        /// <summary>
        /// Operasyon bağımlılığı
        /// </summary>
        /// <param name="source">Kaykan operasyon</param>
        /// <param name="target">Hedef operasyon</param>
        /// <param name="type">Bağımlılık tipi</param>
        public Dependency(Operation source, Operation target, DependencyType type)
        {
            this.Source = source;
            this.Target = target;
            this.Type = type;
        }

        /// <summary>
        /// Kaynak operasyon
        /// </summary>
        public Operation Source { get; set; }

        /// <summary>
        /// Hedef operasyon
        /// </summary>
        public Operation Target { get; set; }

        /// <summary>
        /// Bağımlılık tipi
        /// </summary>
        public DependencyType Type { get; set; }

        /// <summary>
        /// Bağımlılığın anlamlı halini verir
        /// </summary>
        /// <returns>Bağımlılığın anlamlı halini geriye döndürür</returns>
        public override string ToString()
        {
            if (Type == DependencyType.CANNOT_START_TILL_TARGET_FINISHED)
            {
                return (Source.No + 1) + ", " + (Target.No + 1) + " bitmeden başlayamaz.";
            }
            else if (Type == DependencyType.CANNOT_START_TILL_TARGET_STARTED)
            {
                return (Source.No + 1) + ", " + (Target.No + 1) + " başlamadan başlayamaz.";
            }
            else
            {
                return "Bilinmiyor.";
            }
        }
    }
}
