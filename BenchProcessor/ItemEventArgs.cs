using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Tezgahta üzerinde işlem yapılacak parçaya ait olay argümanlarını içerir
    /// </summary>
    public class ItemEventArgs : EventArgs
    {
        /// <summary>
        /// Parça
        /// </summary>
        private Item item;

        /// <summary>
        /// Eğer parça üzerinde operasyon yapılıyorsa hangi operasyonun yapıldığını gösterir
        /// </summary>
        private Operation operation;

        /// <summary>
        /// Parça olay argümanları
        /// </summary>
        /// <param name="item">Parça</param>
        /// <param name="operation">Operasyon</param>
        public ItemEventArgs(Item item, Operation operation)
        {
            this.item = item;

            this.operation = operation;
        }

        /// <summary>
        /// Parça
        /// </summary>
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Parça üzerinde yapılan operasyon
        /// </summary>
        public Operation Operation
        {
            get
            {
                return this.operation;
            }
        }
    }
}
