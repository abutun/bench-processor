using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Tezgahlara ait makine ünitelerini (MU) temsil eder
    /// </summary>
    public class MachineUnit: IComparable<MachineUnit>
    {
        /// <summary>
        /// Bu makine ünitesi içerisinde bir işlem yapılacağı zaman işlem Thread-Safe yapılmalı
        /// </summary>
        private Object operationLock = new object();

        /// <summary>
        /// Bu makine ünitesinin durumunun Thread-Safe olarak değiştirilmesi gerekiyor
        /// </summary>
        private Object statusLock = new Object();

        /// <summary>
        /// Makine ünitesinin müsait olup olmadığını gösterir
        /// </summary>
        private bool available = true;

        /// <summary>
        /// Bu makine ünitesi tarafından yapılan en son operasyon
        /// </summary>
        public Operation lastOperation = null;

        /// <summary>
        /// Parça değişim süresi
        /// </summary>
        public const int TOOL_CHANGE_TIME = 15;

        /// <summary>
        /// Makine ünitesi
        /// </summary>
        /// <param name="no">Makine ünitesi numarası</param>
        /// <param name="name">Makine ünitesi adı</param>
        public MachineUnit(int no, string name)
        {
            this.No = no;
            this.Name = name;
            this.Start = 0;
            this.Finish = 0;

            this.Operations = new List<Operation>();
        }

        /// <summary>
        /// Makine ünitesi numarası
        /// </summary>
        public int No { get; internal set; }

        /// <summary>
        /// Makine ünitesi adı
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// MU'nun her operasyon sonucunda kullanıldığı zaman başlangıcı
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// MU'nun her operasyon sonucunda kullanıldığı zaman sonu
        /// </summary>
        public int Finish { get; set; }

        /// <summary>
        /// Makine ünitesinin desteklediği operasyonlar
        /// </summary>
        public List<Operation> Operations { get; internal set; }

        public void AddOperation(Operation operation)
        {
            this.Operations.Add(operation);
        }

        /// <summary>
        /// Makine ünitesinin belirtilen operasyonu o an yapıp yapamayacağını gösterir
        /// </summary>
        /// <param name="operation">Operasyon</param>
        /// <returns>Operasyon o an için yapılabiliyorsa true, yapılamıyorsa false döner</returns>
        public bool CanDoOperation(Operation operation)
        {
            return CanDoOperation(operation.No);
        }

        /// <summary>
        /// Makine ünitesinin belirtilen operasyonu o an yapıp yapamayacağını gösterir
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <returns>Operasyon o an için yapılabiliyorsa true, yapılamıyorsa false döner</returns>
        public bool CanDoOperation(int operationNo)
        {
            if (Available)
            {
                foreach (Operation operation in this.Operations)
                {
                    if (operation.No == operationNo)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Makine ünitesinin belirtilen operasyonu yapıp yapamayacağı bilgisini verir
        /// </summary>
        /// <param name="operation">Operasyon</param>
        /// <returns>Operasyon destekleniyorsa true, desteklenmiyorsa false döner</returns>
        public bool IsOperationSupported(Operation operation)
        {
            return IsOperationSupported(operation.No);
        }

        /// <summary>
        /// Makine ünitesinin belirtilen operasyonu yapıp yapamayacağı bilgisini verir
        /// </summary>
        /// <param name="operation">Operasyon</param>
        /// <returns>Operasyon destekleniyorsa true, desteklenmiyorsa false döner</returns>
        public bool IsOperationSupported(int operationNo)
        {
            foreach (Operation operation in this.Operations)
            {
                if (operation.No == operationNo)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Bu makine ünitesinde desteklenen herhangi bir operasyon olup olmadığını gösterir
        /// </summary>
        public bool HasOperations
        {
            get
            {
                return this.Operations.Count > 0;
            }
        }

        /// <summary>
        /// Bu makine ünitesi tarafından desteklenen operasyonu alır
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <returns>Operasyon makine ünitesi tarafından destekleniyorsa operasyonu, desteklenmiyorsa null döndürür</returns>
        public Operation GetOperation(int operationNo)
        {
            foreach (Operation operation in this.Operations)
            {
                if (operationNo == operation.No)
                {
                    return operation;
                }
            }

            return null;
        }

        /// <summary>
        /// Makine ünitesinin müsait olup olmadığını gösterir
        /// </summary>
        public bool Available
        {
            get
            {
                return this.available;
            }

            set
            {
                lock (statusLock)
                {
                    this.available = value;
                }
            }
        }

        public Operation LastOperation
        {
            get
            {
                return this.lastOperation;
            }
        }

        /// <summary>
        /// Belirtilen görev makine ünitesinde yapılmaya başlanır
        /// </summary>
        /// <param name="operation">Operasyon</param>
        /// <returns>Operasyonun sonucunu döndürür</returns>
        public OperationResult DoOperation(Operation operation)
        {
            return DoOperation(operation.No);
        }

        /// <summary>
        /// Belirtilen görev makine ünitesinde yapılmaya başlanır
        /// </summary>
        /// <param name="operation">Operasyon numarası</param>
        /// <returns>Operasyonun sonucunu döndürür</returns>
        public OperationResult DoOperation(int operationNo)
        {
            OperationResult result = new OperationResult();

            if (IsOperationSupported(operationNo))
            {
                // Herbir PML için ayrı bir Thread oluşturulduğu için bu işlem Thread-Safe yapılmalı
                lock (operationLock)
                {
                    // Operasyonun bu ünitede yapılıp yapılamayacağını kontrol et
                    if (CanDoOperation(operationNo))
                    {
                        // Makine ünitesini kilitle
                        Available = false;

                        // Operasyonu al
                        Operation operation = GetOperation(operationNo);

                        if (operation != null)
                        {
                            result.Result = 0;

                            // Operasyon süresini al
                            result.Time = operation.Time;

                            // Takım değişim süresi (default 0)
                            result.ToolChangeTime = 0;

                            // Takım değişimi gerekiyorsa, takım değiştirme süresini de ekle
                            if (lastOperation != null)
                            {
                                if (lastOperation.ToolNo != operation.ToolNo)
                                {
                                    result.ToolChangeTime = TOOL_CHANGE_TIME;
                                }
                            }

                            // Makine ünitesinin başlangıç zamanı verilmiş olmalı zaten
                            result.Start = this.Start;

                            // Bitiş zamanını hesapla
                            this.Finish = result.Finish = this.Start + result.Time + result.ToolChangeTime;

                            // En son yapılan operasyon olarak işaretle
                            this.lastOperation = operation;

                            // En son makine ünitesini set et (önemli)
                            result.MachineUnitNo = this.No;
                        }
                        else
                        {
                            result.Result = (int)Error.MU_INVALID_OPERATION;
                        }

                        // Makine ünitesini yeniden aç
                        Available = true;
                    }
                    else
                    {
                        result.Result = (int)Error.MU_IS_NOT_AVAILABLE;
                    }
                }
            }
            else
            {
                result.Result = (int)Error.MU_OPERATION_IS_NOT_SUPPORTED;
            }

            return result;
        }

        /// <summary>
        /// Aynı MU'yu kullanmak isteyen operasyonlardan birisi bir diğerini mutlaka beklemek durumundadır
        /// </summary>
        /// <param name="time">MU'yu kullanmak istediğimiz zaman</param>
        /// <returns>Eğer MU bir işlem yapıyorken bu MU'yu kullanmak istiyorsak true, değilse false döner</returns>
        public bool IsInTimeRange(int time)
        {
            if (this.Finish > 0)
            {
                if (this.Start >= time && time < this.Finish)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Locked { get; set; }

        private Object lockObject = new Object();

        public bool CheckAndLockUnit()
        {
            lock (lockObject)
            {
                if (!Locked)
                {
                    Locked = true;

                    return true;
                }

                return false;
            }
        }

        private Object unLockObject = new Object();

        public void Unlock()
        {
            lock (unLockObject)
            {
                Locked = false;
            }
        }

        public int CompareTo(MachineUnit other)
        {
            return this.No.CompareTo(other.No);
        }

        public void Shift(int time)
        {
            this.Start += time;
            this.Finish += time;
        }
    }
}
