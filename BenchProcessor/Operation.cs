using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Parçalar üzerinde yapılacak operasyonlar
    /// </summary>
    public class Operation: IComparable<Operation>
    {
        /// <summary>
        /// Operasyonların durumları Thread-Safe olarak güncellenmeli
        /// </summary>
        private Object statusLock = new Object();

        /// <summary>
        /// Operasyonun durumu
        /// </summary>
        private OperationStatus status = OperationStatus.WAITING;

        /// <summary>
        /// Operasyonun yapılabileceği takım numarası
        /// </summary>
        public int ToolNo { get; set; }

        /// <summary>
        /// Operasyon modu / tipi
        /// </summary>
        public OperationMode Mode { get; set; }

        /// <summary>
        /// Operasyon
        /// </summary>
        /// <param name="no">Operasyon numarası</param>
        /// <param name="name">Operasyon adı</param>
        /// <param name="time">Operasyon süresi</param>
        public Operation(int no, string name, int time) : this(no, name, time, "") { }

        /// <summary>
        /// Operasyon
        /// </summary>
        /// <param name="no">Operasyon numarası</param>
        /// <param name="name">Operasyon adı</param>
        /// <param name="time">Operasyon süresi</param>
        /// <param name="key">Operasyon anahtarı (yalnızca UI işlemlerinde kullanılır)</param>
        public Operation(int no, string name, int time, string key)
        {
            this.No = no;
            this.Name = name;
            this.Time = time;
            this.Key = key;
            this.ToolNo = -1;
            this.Mode = OperationMode.UNKNOWN;
        }

        /// <summary>
        /// Operasyon numarası
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// Operasyon adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Operasyon süresi
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Yalnızca UI amacıyla kullanılan operasyon anahtarı
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Operasyonun durumu (bekliyor, işlemde, tamamlandı, hatalı)
        /// </summary>
        public OperationStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                lock (statusLock)
                {
                    this.status = value;
                }
            }
        }

        /// <summary>
        /// Operasyonun hangi MU 'lar tarafından desteklendiği
        /// </summary>
        public List<int> MachineUnitNOs { get; set; }

        public int CompareTo(Operation other)
        {
            return this.No.CompareTo(other.No);
        }
    }
}
