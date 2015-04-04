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
    public class CalculationEndEventArgs : EventArgs
    {
        /// <summary>
        /// Simülasyon sonucu
        /// </summary>
        private PmlOperationResultList result = new PmlOperationResultList();

        /// <summary>
        /// Simülasyon süresi
        /// </summary>
        private long duration = 0;

        private int possibilities = 0;

        /// <summary>
        /// Parça olay argümanları
        /// </summary>
        /// <param name="item">Parça</param>
        /// <param name="operation">Operasyon</param>
        public CalculationEndEventArgs(PmlOperationResultList result, long duration, int possibilities)
        {
            this.result = result;

            this.duration = duration;

            this.possibilities = possibilities;
        }

        public PmlOperationResultList Result
        {
            get
            {
                return result;
            }
        }

        public long Duration
        {
            get
            {
                return duration;
            }
        }

        public int Possibilities
        {
            get
            {
                return possibilities;
            }
        }
    }
}
