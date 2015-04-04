using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// PML'ler tarafından talep edilen rezervasyon isteklerinin sonuçları bu sınıfta tutulur
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Rezervasyon
        /// </summary>
        /// <param name="reserved">Rezerve işlemi başarılı mı başarısız mı?</param>
        public Reservation(bool reserved) : this(reserved, null, new List<Operation>()) { }

        /// <summary>
        /// Rezervasyon
        /// </summary>
        /// <param name="reserved">Rezerve işlemi başarılı mı başarısız mı?</param>
        /// <param name="item">Rezerve edilen parça</param>
        /// <param name="operations">Rezerve edilen operasyon ya da operasyonlar</param>
        public Reservation(bool reserved, Item item, List<Operation> operations)
        {
            if (item != null && operations != null)
            {
                foreach (Operation operation in operations)
                {
                    if (item.GetOperation(operation) == null)
                    {
                        throw new Exception("Parça belirtilen \""+ operation.Name +"\" adlı operasyonu içermiyor! Rezervasyon talep edilemez!");
                    }
                }
            }

            this.Reserved = reserved;
            this.Item = item;
            this.Operations = operations;
            this.MachineUnitNOs = new List<int>();
        }

        /// <summary>
        /// Rezervasyon işlemi başarılı mı başarısız mı?
        /// </summary>
        public bool Reserved { get; set; }

        /// <summary>
        /// Rezerve edilen parça
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Rezerve edilen operasyon ya da operasyonlar
        /// </summary>
        public List<Operation> Operations { get; set; }

        /// <summary>
        /// Rezervasyon kodu
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Rezerve edilen operasyonlara ait tercih edilen makine üniteleri
        /// </summary>
        public List<int> MachineUnitNOs { get; set; }
    }
}
