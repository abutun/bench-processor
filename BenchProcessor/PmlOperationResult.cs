using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Bir PML tarafından yapılan eşzamanlı operasyonların ya da yalnızca bir operasyonun sonucu
    /// </summary>
    public class PmlOperationResult: List<OperationResult>, IComparable<PmlOperationResult>
    {
        /// <summary>
        /// Operasyon sonucu
        /// </summary>
        public PmlOperationResult()
        {
            this.Time = 0;
            this.Start = 0;
            this.Finish = 0;
        }

        /// <summary>
        /// İşlem sırası
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// Operasyon yapılan parça
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Operasyonu yapan PML
        /// </summary>
        public PartMachiningLocation PartMachiningLocation { get; set; }

        /// <summary>
        /// Operasyonun tamamlanma süresi
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Aynı PML tarafından yapılan bir parçaya ait operasyonun başlangıç zamanı
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Aynı PML tarafından yapılan bir parçaya ait operasyonun bitiş zamanı
        /// </summary>
        public int Finish { get; set; }

        /// <summary>
        /// İşlem sonucu ile ilgili bilgi içeren sonuç
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Kod
        /// </summary>
        public string Code { get; set; }

        public int CompareTo(PmlOperationResult other)
        {
            return this.No.CompareTo(other.No);
        }

        public void setWaitTime(int time)
        {
            if (time > 0)
            {
                foreach (OperationResult result in this)
                {
                    if (result != null)
                    {
                        result.WaitTimeForAvailableUnit = time;

                        result.ShiftTime(time);

                        if (result.MachineUnitNo>0)
                        {
                            Bench.GetMachineUnit(result.MachineUnitNo).Shift(time);
                        }
                    }
                }
            }
        }
    }
}
