using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Parça makine yeri
    /// </summary>
    public class PartMachiningLocation: IComparable<PartMachiningLocation>
    {
        /// <summary>
        /// PML tarafından yeni bir operasyon sonucu bulunmasını kontrol eden temsilci
        /// </summary>
        ///  <param name="result">Operasyon sonucu</param>
        public delegate void OperationResultFoundEventHandler(PmlOperationResult result);

        /// <summary>
        /// PML tarafından yeni bir operasyon sonucu bulunduğunda tetiklenir
        /// </summary>
        public event OperationResultFoundEventHandler onOperationResultFound;

        /// <summary>
        /// En fazla kaç adet operasyon aynı anda yapılabilir?
        /// </summary>
        public const int MAX_SIMULTANEOUS_OPERATION = 2;

        /// <summary>
        /// Bu makine yeri içerisinde bir işlem yapılacağı zaman işlem Thread-Safe yapılmalı
        /// </summary>
        private Object operationLock = new object();

        /// <summary>
        /// Bu parça makine yerinin durumunun Thread-Safe olarak değiştirilmesi gerekiyor
        /// </summary>
        private Object statusLock = new Object();

        /// <summary>
        /// Bu parça makine yerinin müsait olup olmadığını gösterir
        /// </summary>
        private bool available = true;

        /// <summary>
        /// Birlikte yapılan operasyonlardan, en uzun süren operasyonunun %70'ini al
        /// </summary>
        public const double SIMULTANEOUS_OPERATION_TIME_RATIO = 0.7;

        /// <summary>
        /// Bu PML üzerinde yapılması planlanan Parçalar
        /// </summary>
        private Queue<Item> ItemQueue = new Queue<Item>();

        /// <summary>
        /// Parçalar üzerinde yapılan operasyon sıraları
        /// </summary>
        private int operationOrderNo = 0;

        /// <summary>
        /// Parça makine yeri
        /// </summary>
        /// <param name="no">Parça makine yeri numarası</param>
        /// <param name="name">Parça makine yeri adı</param>
        public PartMachiningLocation(int no, string name)
        {
            this.No = no;
            this.Name = name;
            this.Time = 0;
 
            this.MachineUnitNOs = new List<int>();
            this.Results = new List<PmlOperationResult>();
        }

        /// <summary>
        /// Parça yeri numarası
        /// </summary>
        public int No { get; internal set; }

        /// <summary>
        /// Parça yeri adı
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// PML'in her operasyon sonucunda ilerlediği zamanı gösterir
        /// </summary>
        public int Time { get; internal set; }

        /// <summary>
        /// Parça yerine yerleştirilebilecek makine üniteleri
        /// </summary>
        public List<int> MachineUnitNOs { get; internal set; }

        /// <summary>
        /// Yapılan operasyonlara ait sonuçlar
        /// </summary>
        public List<PmlOperationResult> Results { get; internal set; }

        /// <summary>
        /// Parça yerine yerleştirilebilecek yeni bir makine ünitesi ekler
        /// </summary>
        /// <param name="machineUnit">Makine ünitesi</param>
        public void AddMachineUnit(MachineUnit machineUnit)
        {
            this.MachineUnitNOs.Add(machineUnit.No);
        }

        /// <summary>
        /// Belirtilen makine ünitesinin bu parça yerine yerleştirilip yerleştirilemeyeceğini gösterir
        /// </summary>
        /// <param name="unit">Makine ünitesi</param>
        /// <returns>Makine ünitesi kullanıliliyorsa true, kullanılamıyorsa false döner</returns>
        public bool CanUseMachineUnit(MachineUnit unit)
        {
            return CanUseMachineUnit(unit.No);
        }

        /// <summary>
        /// Belirtilen makine ünitesinin bu parça yerine yerleştirilip yerleştirilemeyeceğini gösterir
        /// </summary>
        /// <param name="machineUnitNo">Makine ünitesi numarası</param>
        /// <returns>Makine ünitesi kullanılabiliyorsa true, kullanılamıyorsa false döner</returns>
        public bool CanUseMachineUnit(int machineUnitNo)
        {
            return this.MachineUnitNOs.Contains(machineUnitNo);
        }

        /// <summary>
        /// Bu parça makine ünitesi yerini kullanabilecek makine ünitesi olup olmadığını gösterir
        /// </summary>
        public bool HasMachineUnits
        {
            get
            {
                return this.MachineUnitNOs.Count > 0;
            }
        }

        /// <summary>
        /// Bu PML'in müsait olup olmadığını gösterir
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

        /// <summary>
        /// PML'in parça sırasına yeni bir parça ekler
        /// </summary>
        /// <param name="item"></param>
        public void EnqueueItem(Item item)
        {
            if (!this.ItemQueue.Contains(item))
            {
                this.ItemQueue.Enqueue(item);
            }
        }

        public void ClearQueue()
        {
            this.ItemQueue.Clear();
        }

        public Item getCurrentItem()
        {
            if (this.ItemQueue.Count > 0)
            {
                return this.ItemQueue.ElementAt(0);
            }

            return null;
        }

        /// <summary>
        /// PML'e atanmış parça sırasından bir sonraki Parçayı alır
        /// </summary>
        /// <returns>Bir sonraki parça</returns>
        public Item Dequeue()
        {
            return this.ItemQueue.Dequeue();
        }

        /// <summary>
        /// PML'e atanmış tüm parçaların process edilip edilmediğini gösterir
        /// </summary>
        /// <returns></returns>
        public bool JobsCompleted()
        {
            return this.ItemQueue.Count == 0;
        }

        /// <summary>
        /// Bu PML içerisinde belirtilen operasyonu yapabilecek makine ünitelerini verir
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <returns>Operasyonu yapabilecek Makine Ünitesi listesini döndürür<returns>
        public List<int> GetSupportedMachineUnits(int operationNo)
        {
            List<int> result = new List<int>();

            foreach (int no in this.MachineUnitNOs)
            {
                if (Bench.GetMachineUnit(no).IsOperationSupported(operationNo))
                {
                    result.Add(no);
                }
            }

            return result;
        }

        /// <summary>
        /// Operasyonların farklı MU'lar ile eşzamanlı yapılıp yapılamayacağını gösterir
        /// </summary>
        /// <param name="operations">Operasyonlar</param>
        /// <returns>True ise eş zamanlı yapılabilir, false ise eşzamanlı yapılamaz</returns>
        public bool CanDoOperationsSimultaneously(List<Operation> operations)
        {
            if (Available)
            {
                if (operations.Count > 1 && operations.Count <= MAX_SIMULTANEOUS_OPERATION)
                {
                    List<int> unitsOf1 = GetSupportedMachineUnits(operations[0]);

                    List<int> unitsOf2 = GetSupportedMachineUnits(operations[0]);

                    if (unitsOf1.Count == 1 && unitsOf2.Count == 1 && unitsOf1[0] == unitsOf2[0])
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Bu PML içerisinde belirtilen operasyonun yapılıp yapılmayacağını gösterir
        /// </summary>
        /// <param name="operationNo">Operasyon</param>
        /// <returns>Operasyonu yapabilecek Makine Ünitesi listesini döndürür<returns>
        public List<int> GetSupportedMachineUnits(Operation operation)
        {
            return GetSupportedMachineUnits(operation.No);
        }

        /// <summary>
        /// PML'in belirtilen operasyonu yapıp yapamayacağı bilgisini verir
        /// </summary>
        /// <param name="operation">Operasyon</param>
        /// <returns>Operasyon destekleniyorsa true, desteklenmiyorsa false döner</returns>
        public bool IsOperationSupported(Operation operation)
        {
            return IsOperationSupported(operation.No);
        }

        /// <summary>
        /// PML'in belirtilen operasyonu yapıp yapamayacağı bilgisini verir
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <returns>Operasyon destekleniyorsa true, desteklenmiyorsa false döner</returns>
        public bool IsOperationSupported(int operationNo)
        {
            foreach (int no in this.MachineUnitNOs)
            {
                if (Bench.GetMachineUnit(no).IsOperationSupported(operationNo))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// PML'in belirtilen operasyonu yapıp yapamayacağı bilgisini verir, aynı anda yapılmak istenen operasyonlar için faydalıdır
        /// </summary>
        /// <param name="operationNOs">Operasyon numaraları</param>
        /// <returns>Operasyonlar destekleniyorsa true, desteklenmiyorsa false döner</returns>
        public bool IsOperationSupported(List<int> operationNOs)
        {
            foreach (int operationNo in operationNOs)
            {
                if (!IsOperationSupported(operationNo))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Belirtilen görev bu PML içerisinde yapılmaya başlanır
        /// </summary>
        /// <param name="item">Hangi parça üzerinde çalışıyoruz</param>
        /// <param name="operations">Yapılmak istenen operasyonlar</param>
        /// <returns>Operasyon ya da operasyonların sonucunu döndürür</returns>
        public PmlOperationResult DoOperation(Item item, List<Operation> operations)
        {
            PmlOperationResult result = new PmlOperationResult();

            // Parçayı sonuca ata
            result.Item = item;

            // Yapılmak istenen operasyon bu PML'de yapılabilir mi?
            if (IsOperationSupported(operations.Select(o => o.No).ToList()))
            {
                if (IsValidOperations(operations))
                {
                    // Herbir PML için ayrı bir Thread oluşturduğumuz için bu operasyon Thread-safe yapılmalı
                    lock (operationLock)
                    {
                        if (Available)
                        {
                            // PML'i diğer operasyonlar için kilitle
                            Available = false;

                            // Aynı anda yapılan operasyonlar aynı MU'yu kullanmazlar
                            List<int> simultaneousUnitNos = new List<int>();

                            int waitTime = 0;

                            // Eşzamanlı operasyonlar sıra ile yapılmalı
                            for (int i = 0; i < operations.Count; i++)
                            {
                                Operation operation = operations[i];

                                OperationResult operationResult = null;

                                // Operasyon yapılana kadar devam et
                                while (operationResult == null)
                                {
                                    foreach (int operationUnitNo in operation.MachineUnitNOs)
                                    {
                                        MachineUnit unit = Bench.GetMachineUnit(operationUnitNo);

                                        if (operations.Count > 1 && i == 0)
                                        {
                                            if (operations[1].MachineUnitNOs.Count == 1)
                                            {
                                                if (unit.No == operations[1].MachineUnitNOs[0])
                                                {
                                                    continue;
                                                }
                                            }
                                        }

                                        if (!simultaneousUnitNos.Contains(unit.No) && unit.Available && unit.CanDoOperation(operation.No))
                                        {
                                            if (unit.CheckAndLockUnit())
                                            {
                                                if (unit.IsInTimeRange(this.Time))
                                                {
                                                    // Makine ünitesinin müsait olmasını bekleme süresini de dahil et
                                                    int t = unit.Finish - this.Time;

                                                    if (t > waitTime)
                                                    {
                                                        waitTime = t;
                                                    }
                                                }

                                                // Makine ünitesinin işe başlama zamanını belirle/ Operasyon sonucuna ait başlangıç zamanını belirle
                                                unit.Start = this.Time;

                                                // Operasyonu yap
                                                operationResult = unit.DoOperation(operation.No);

                                                // Operasyonu sonuca ata
                                                operationResult.Operation = operation;

                                                // Operasyon kodunu ata
                                                operationResult.Code = this.No + "-" + unit.No + "-" + item.No + "-" + operation.No;

                                                // Aynı anda yapılan operasyonlar aynı MU'yu kullanmazlar
                                                simultaneousUnitNos.Add(unit.No);

                                                break;
                                            }
                                        }
                                    }
                                }

                                result.Add(operationResult);
                            }

                            if (operations.Count == 1)
                            {
                                OperationResult operationResult = result[0];

                                result.Result = operationResult.Result;

                                result.Time = operationResult.Time + operationResult.ToolChangeTime;

                                if (waitTime > 0)
                                {
                                    // Bekleme süresini operasyon süresine ekle
                                    result.Time += waitTime;

                                    // Sonuca ait bekleme süresini set et (makine üniteleri ve operasyon sonuçlarına ait ötelemeleri otomatik yapar)
                                    result.setWaitTime(waitTime);
                                }

                                // Operasyon sonucu başlangıç süresi
                                result.Start = this.Time;

                                // PML'in işlem süresini set et
                                this.Time += result.Time;

                                // Operasyon sonucu bitiş süresi
                                result.Finish = this.Time;
                            }
                            else
                            {
                                result.Result = result[0].Result;

                                // Her iki operasyon da başarılı yapılmış olmalı
                                if (result[0].Result != 0)
                                {
                                    result.Result = result[1].Result;
                                }

                                if (result.Result == 0)
                                {
                                    // Eşzamanlı operasyon yapılmış
                                    int time = result[0].Time;

                                    // Eşzamanlı aynı anda yalnızca 2 operasyonun yapılabildiğini biliyoruz
                                    if (result[1].Time > time)
                                    {
                                        time = result[1].Time;
                                    }

                                    time = (int)(time * SIMULTANEOUS_OPERATION_TIME_RATIO);

                                    if (result[0].ToolChangeTime > 0)
                                    {
                                        time += result[0].ToolChangeTime;
                                    }
                                    else if (result[1].ToolChangeTime > 0)
                                    {
                                        time += result[1].ToolChangeTime;
                                    }

                                    // Eş zamanlı operasyonlardan en uzun süreninin %XX'i kadarı alındığı için, herbir operasyon sonucunun bitişlerini yeniden set et
                                    result[0].Finish = result[0].Start + time;
                                    result[1].Finish = result[1].Start + time;

                                    // Eş zamanlı operasyonlardan en uzun süreninin %XX'i kadarı alındığı için, herbir makine ünitesinin bitişlerini yeniden set et
                                    Bench.GetMachineUnit(result[0].MachineUnitNo).Finish = Bench.GetMachineUnit(result[0].MachineUnitNo).Start + time;
                                    Bench.GetMachineUnit(result[1].MachineUnitNo).Finish = Bench.GetMachineUnit(result[1].MachineUnitNo).Start + time;

                                    result.Time = time;

                                    if (waitTime > 0)
                                    {
                                        // Bekleme süresini operasyon süresine ekle
                                        result.Time += waitTime;

                                        // Sonuca ait bekleme süresini set et (makine üniteleri ve operasyon sonuçlarına ait ötelemeleri otomatik yapar)
                                        result.setWaitTime(time);
                                    }

                                    // Operasyon sonucu başlangıç süresi
                                    result.Start = this.Time;

                                    // PML'in işlem süresini set et
                                    this.Time += result.Time;

                                    // Operasyon sonucu bitiş süresi
                                    result.Finish = this.Time;
                                }
                            }

                            // Operasyon sırasını belirle
                            operationOrderNo++;

                            result.No = operationOrderNo;

                            foreach (OperationResult operationResult in result)
                            {
                                Bench.GetMachineUnit(operationResult.MachineUnitNo).Unlock();
                            }

                            // PML'i diğer operasyonlar için yeniden aç
                            Available = true;
                        }
                        else
                        {
                            result.Result = (int)Error.PML_IS_NOT_AVAILABLE;
                        }
                    }
                }
                else
                {
                    result.Result = (int)Error.PML_NOT_ENOUGH_MU_FOR_OPERATIONS;
                }
            }
            else
            {
                result.Result = (int)Error.PML_OPERATION_IS_NOT_SUPPORTED;
            }

            // Kodu ata
            result.Code = this.No + "-" + item.No;

            // PML'i ata
            result.PartMachiningLocation = this;

            foreach (Operation operation in operations)
            {
                result.Code += "-" + operation.No;
            }

            if (this.onOperationResultFound != null)
            {
                this.onOperationResultFound(result);
            }

            Results.Add(result);

            return result;
        }

        /// <summary>
        /// Operasyonların yapılmaya başlanmadan önce geçerlilikleri kontrol edilir
        /// </summary>
        /// <param name="operations">Operasyon</param>
        /// <returns>Geçerli ise true, geçersiz ise false döndürür</returns>
        private bool IsValidOperations(List<Operation> operations)
        {
            if (operations != null)
            {
                if (operations.Count > 1)
                {
                    if (operations[0].MachineUnitNOs.Count == 1 && operations[1].MachineUnitNOs.Count == 1 && operations[0].MachineUnitNOs[0] == operations[1].MachineUnitNOs[0])
                    {
                        return false;
                    }

                    return operations[0].MachineUnitNOs.Count > 0 && operations[1].MachineUnitNOs.Count > 0;
                }
                else
                {
                    return operations[0].MachineUnitNOs.Count > 0;
                }
            }

            return true;
        }

        /// <summary>
        /// Birlikte yapılabilecek operasyonları bulur
        /// 
        /// 1) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş bir öncelik ilişkisi (precedence kısıtı) yoksa:
        /// 
        /// a) eğer her iki operasyonun modu da turning (machining_mode=1) veya her iki operasyonun modu da milling (machining_mode=2) veya her iki operasyonun modu da 
        ///    contouring (machining_mode=3) ise operasyonlar eş zamanlı yapılabilir
        ///    
        /// b) eğer operasyonlardan birinin modu milling ise ve diğer operasyonun modu turning veya contouring ise eş zamanlı operasyon yapılamaz.
        /// 
        /// c) eğer operasyonlardan birinin modu turning diğerinin modu contouring ise operasyonlar eş zamanlı yapılabilir.
        /// 
        /// 2) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş 2 kodlu öncelik ilişkisi (precedence kısıtı) varsa (1. operasyon tamamlanmadan ikincisine başlanamaz) 
        ///    --> machining_mode'dan bağımsız olarak iki operasyonun eş zamanlı yapılmazsına izin verilmez.
        ///    
        /// 3) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş 1 kodlu öncelik ilişkisi (precedence kısıtı) varsa (1. operasyon başlamadan ikincisine başlanamaz) 
        ///    --> 1. maddede ki a), b) ve c) maddeleri uygulanır.
        /// </summary>
        /// <param name="bench">Tezgah</param>
        /// <param name="reservation">Yapılan reservasyon</param>
        /// <param name="item">Rezervasyonun hangi parçaya ait olduğu</param>
        /// <returns>Birlikte yapılabilecek yeni bir operasyon bulunursa bu operasyonu, yoksa null döndürür</returns>
        public Operation SearchAvailableSimultaneousOperation(Bench bench, List<Operation> reservation, Item item)
        {
            if (item != null && reservation != null)
            {
                if (item.HasOperations && reservation.Count > 0 && reservation.Count < PartMachiningLocation.MAX_SIMULTANEOUS_OPERATION)
                {
                    Operation source = reservation[0];

                    for (int i = 0; i < item.Operations.Count;i++ )
                    {
                        Operation target = item.Operations[i];

                        if (source.No != target.No && target.Status == OperationStatus.WAITING)
                        {
                            // Seçilen operasyon eşzamanlı yapılabilir mi?
                            if (CanDoOperationsSimultaneously(bench, source, target))
                            {
                                // Hedef operasyonun bağımlılıklarını kontrol et
                                Operation prevOperation = null;

                                if (i > 0)
                                {
                                    prevOperation = item.Operations[i - 1];
                                }

                                bool flag = true;

                                // Eş zamanlı yapılabilecek operasyonlar için, kendinden önceki operasyonlarla bağımlılıkları var mı?
                                if (prevOperation != null)
                                {
                                    DependencyType type = bench.GetDependency(target, prevOperation);

                                    if (type == DependencyType.CANNOT_START_TILL_TARGET_FINISHED && prevOperation.Status != OperationStatus.DONE)
                                    {
                                        flag = false;
                                    }
                                    else if (type == DependencyType.CANNOT_START_TILL_TARGET_STARTED && (prevOperation.Status != OperationStatus.IN_PROCESS && prevOperation.Status != OperationStatus.DONE))
                                    {
                                        flag = false;
                                    }
                                }

                                if (flag)
                                {
                                    return target;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// İki operasyonu birlikte yapılıp yapılamayacağını bulur
        /// 
        /// 1) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş bir öncelik ilişkisi (precedence kısıtı) yoksa:
        /// 
        /// a) eğer her iki operasyonun modu da turning (machining_mode=1) veya her iki operasyonun modu da milling (machining_mode=2) veya her iki operasyonun modu da 
        ///    contouring (machining_mode=3) ise operasyonlar eş zamanlı yapılabilir
        ///    
        /// b) eğer operasyonlardan birinin modu milling ise ve diğer operasyonun modu turning veya contouring ise eş zamanlı operasyon yapılamaz.
        /// 
        /// c) eğer operasyonlardan birinin modu turning diğerinin modu contouring ise operasyonlar eş zamanlı yapılabilir.
        /// 
        /// 2) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş 2 kodlu öncelik ilişkisi (precedence kısıtı) varsa (1. operasyon tamamlanmadan ikincisine başlanamaz) 
        ///    --> machining_mode'dan bağımsız olarak iki operasyonun eş zamanlı yapılmazsına izin verilmez.
        ///    
        /// 3) Eş zamanlı yapılmak istenen iki operasyon arasında verilmiş 1 kodlu öncelik ilişkisi (precedence kısıtı) varsa (1. operasyon başlamadan ikincisine başlanamaz) 
        ///    --> 1. maddede ki a), b) ve c) maddeleri uygulanır.
        /// </summary>
        /// <param name="bench">Tezgah</param>
        /// <param name="source">Kaynak operasyon</param>
        /// <param name="target">Hedef operasyon</param>
        /// <returns>Operasyonlar bir arada yapılabilirse true, yapılamazsa false döndürür</returns>
        public bool CanDoOperationsSimultaneously(Bench bench, Operation source, Operation target)
        {
            if (source != null && target != null)
            {
                if (source.No != target.No)
                {
                    if (source.Status == OperationStatus.WAITING && target.Status == OperationStatus.WAITING)
                    {
                        List<Operation> operations = new List<Operation>();
                        operations.Add(source);
                        operations.Add(target);

                        // PML bu operasyonları yapabilir durumda mı?
                        if (CanDoOperationsSimultaneously(operations))
                        {
                            DependencyType type = bench.GetDependency(source, target);

                            // Yukarıdaki açıklamaların 1. ve 3. maddesi (2. maddeye zaten izin vermiyoruz)
                            if (type == DependencyType.CANNOT_START_TILL_TARGET_STARTED || type == DependencyType.UNKNOWN)
                            {
                                // a maddesi
                                if (source.Mode == target.Mode)
                                {
                                    return true;
                                }
                                 // c maddesi
                                else if ((source.Mode == OperationMode.TURNING && target.Mode == OperationMode.CONTOURING) ||
                                    ((target.Mode == OperationMode.TURNING && source.Mode == OperationMode.CONTOURING)))
                                {
                                    return true;
                                }
                                // b maddesi
                                else
                                {
                                    if ((source.Mode == OperationMode.MILLING && (target.Mode == OperationMode.CONTOURING || target.Mode == OperationMode.TURNING))
                                  || (target.Mode == OperationMode.MILLING && (source.Mode == OperationMode.CONTOURING || source.Mode == OperationMode.TURNING)))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public int CompareTo(PartMachiningLocation other)
        {
            return this.No.CompareTo(other.No);
        }

        public void Reset()
        {
            operationOrderNo = 0;

            Time = 0;

            Results.Clear();
        }

        public bool HasItems
        {
            get
            {
                return this.ItemQueue.Count > 0;
            }
        }
    }
}
