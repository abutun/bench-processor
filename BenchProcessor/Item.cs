using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Parça
    /// </summary>
    public class Item: IComparable<Item>
    {
        /// <summary>
        /// Parçanın durumu
        /// </summary>
        private ItemStatus status = ItemStatus.AVAILABLE;

        /// <summary>
        /// Parçanın durumu Thread-Safe olarak güncellenmeli
        /// </summary>
        private Object statusLock = new Object();

        /// <summary>
        /// Yeni bir parça oluşturur
        /// </summary>
        /// <param name="name">Parça adı</param>
        public Item(int no, String name)
        {
            this.No = no;
            this.Name = name;

            this.Operations = new List<Operation>();
        }

        /// <summary>
        /// Parçanın numarası
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// Parçanın adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parçada yapılması istenen operasyonlar
        /// </summary>
        public List<Operation> Operations { get; set; }

        /// <summary>
        /// Bu parçanın herhangi bir operasyonu olup olmadığını gösterir
        /// </summary>
        public bool HasOperations
        {
            get
            {
                return this.Operations.Count > 0;
            }
        }

        /// <summary>
        /// Parça içerisindeki bir operasyonu alır
        /// </summary>
        /// <param name="No">Operasyon</param>
        /// <returns>Operasyon parça içerisinde varsa operasyonu, yoksa null değer döndürür</returns>
        public Operation GetOperation(Operation operation)
        {
            return GetOperation(operation.No);
        }

        /// <summary>
        /// Parça içerisindeki bir operasyonu alır
        /// </summary>
        /// <param name="No">Operasyon numarası</param>
        /// <returns>Operasyon parça içerisinde varsa operasyonu, yoksa null değer döndürür</returns>
        public Operation GetOperation(int No)
        {
            foreach (Operation operation in this.Operations)
            {
                if (operation.No == No)
                {
                    return operation;
                }
            }

            return null;
        }

        /// <summary>
        /// Bu parçanın durumunu gösterir (bekliyor, işlemde, rezerve edilmiş, tamamlandı, hatalı)
        /// </summary>
        public ItemStatus Status
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

        public void UpdateOperationStatus(Operation operation, OperationStatus status)
        {
            foreach (Operation item in Operations)
            {
                if (item.No == operation.No && item.Status != status)
                {
                    item.Status = status;

                    break;
                }
            }
        }

        public int CompareTo(Item other)
        {
            return this.No.CompareTo(other.No);
        }

        public int TotalTime
        {
            get
            {
                int total = 0;

                foreach (Operation operation in this.Operations)
                {
                    total += operation.Time;
                }

                return total;
            }
        }
    }
}
