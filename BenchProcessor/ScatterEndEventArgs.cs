using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchProcessor
{
    public class ScatterEndEventArgs
    {
                /// <summary>
        /// Simülasyon sonucu
        /// </summary>
        private PmlOperationResultList result = new PmlOperationResultList();

        /// <summary>
        /// Simülasyon süresi
        /// </summary>
        private long duration = 0;

        /// <summary>
        /// Parça olay argümanları
        /// </summary>
        /// <param name="item">Parça</param>
        /// <param name="operation">Operasyon</param>
        public ScatterEndEventArgs(PmlOperationResultList result, long duration)
        {
            this.result = result;

            this.duration = duration;
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
    }
}
