using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Bir PML tarafından yapılan bir operasyonun sonucu
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Operasyon sonucu
        /// </summary>
        public OperationResult()
        {
            this.ToolChangeTime = 0;
            this.Time = 0;
            this.WaitTimeForAvailableUnit = 0;
            this.Start = 0;
            this.Finish = 0;
            this.MachineUnitNo = -1;
        }

        /// <summary>
        /// Operasyon yapılan parça
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Yapılan operasyon
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Operasyon sırasında kullanılan makine ünitesi
        /// </summary>
        public int MachineUnitNo { get; set; }

        /// <summary>
        /// Takım değişim süresi
        /// </summary>
        public int ToolChangeTime { get; set; }

        /// <summary>
        /// Operasyonun tamamlanma süresi (Start-Finish)
        /// Makine ünitelerini bekleme süresi ve takım değişim süreleri bu süreye dahildir
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Aynı MU tarafından yapılmak istenen iki operasyon çakışırsa
        /// operasyonlardan birisi MU'yu müsait olana kadar beklemek durumundadır.
        /// Bu süre operasyonun ne kadar süre beklemesi gerektiğini gösterir.
        /// </summary>
        public int WaitTimeForAvailableUnit { get; set; }

        /// <summary>
        /// Aynı PML tarafından yapılan bir parçaya ait operasyonun başlangıç zamanı
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Aynı PML tarafından yapılan bir parçaya ait operasyonun bitiş zamanı,
        /// </summary>
        public int Finish { get; set; }

        /// <summary>
        /// Operasyon sonucu ile ilgili bilgi içeren sonuç
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Bu pml içerisinde yapılan operasyonun kodu (Temel olarak: PML.No - MU.No - Item.No - Operation.No)
        /// </summary>
        public string Code { get; set; }

        public void ShiftTime(int time)
        {
            this.Start += time;
            this.Finish += time;
        }
    }
}
