using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BenchProcessor
{
    /// <summary>
    /// Tezgah
    /// </summary>
    public class Bench
    {
        /// <summary>
        /// Herbir PML operasyonu Thread-Safe yapılmalı
        /// </summary>
        private Object operationLock = new object();

        /// <summary>
        /// Tezgahtaki işlem akışını kontrol eden temsilci
        /// </summary>
        /// <param name="message"></param>
        public delegate void ConsoleEventHandler(string message);

        /// <summary>
        /// Tezgahtaki her işlem akışında bu olay tetiklenir
        /// </summary>
        public event ConsoleEventHandler onNewMessage;

        /// <summary>
        /// Bir parça tezgah üzerinde işleme alındığını kontrol eden temsilci
        /// </summary>
        /// <param name="eventArgs">Olay argümanı</param>
        public delegate void OperationBeginProcessEventHandler(ItemEventArgs eventArgs);

        /// <summary>
        /// Tezgaha yeni bir parça işlem için alındığında bu olay tetiklenir
        /// </summary>
        public event OperationBeginProcessEventHandler onOperationProcessBegin;

        /// <summary>
        /// Bir parçanın tezgah üzerindeki işlemleri tamamlandığını kontrol eden temsilci
        /// </summary>
        /// <param name="eventArgs">Olay argümanı</param>
        public delegate void OperationEndProcessEventHandler(ItemEventArgs eventArgs);

        /// <summary>
        /// Bir parçanın tezgah üzerindeki işlemleri tamamlandığında bu olay tetiklenir
        /// </summary>
        public event OperationEndProcessEventHandler onOperationProcessEnd;

        /// <summary>
        /// Simülasyon işleminin başladığını kontrol eden temsilci
        /// </summary>
        public delegate void SimulationBeginProcessEventHandler();

        /// <summary>
        /// Simülasyon işlemi başladığında bu olay tetiklenir
        /// </summary>
        public event SimulationBeginProcessEventHandler onSimulationBegin;

        public delegate void SimulationStepEventHandler(double step);

        public event SimulationStepEventHandler onSimulationStep;

        /// <summary>
        /// Simülasyon işleminin tamamlandığını kontrol eden temsilci
        /// </summary>
        /// <param name="duration">Olay argümanı</param>
        public delegate void SimulationEndProcessEventHandler(SimulationEndEventArgs eventArgs);

        /// <summary>
        /// Simülasyon işlemi tamamlandığında bu olay tetiklenir
        /// </summary>
        public event SimulationEndProcessEventHandler onSimulationEnd;

        public delegate void ScatterBeginProcessEventHandler();

        public event ScatterBeginProcessEventHandler onScatterBegin;

        public delegate void ScatterStepEventHandler(double step);

        public event ScatterStepEventHandler onScatterStep;

        public delegate void ScatterEndProcessEventHandler(ScatterEndEventArgs eventArgs);

        public event ScatterEndProcessEventHandler onScatterEnd;

        /// <summary>
        /// Simülasyon işleminin başladığını kontrol eden temsilci
        /// </summary>
        public delegate void CalculationBeginProcessEventHandler();

        /// <summary>
        /// Simülasyon işlemi başladığında bu olay tetiklenir
        /// </summary>
        public event CalculationBeginProcessEventHandler onCalculationBegin;

        public delegate void CalculationStepEventHandler(double step);

        public event CalculationStepEventHandler onCalculationStep;

        /// <summary>
        /// Simülasyon işleminin tamamlandığını kontrol eden temsilci
        /// </summary>
        /// <param name="duration">Olay argümanı</param>
        public delegate void CalculationEndProcessEventHandler(CalculationEndEventArgs eventArgs);

        /// <summary>
        /// Simülasyon işlemi tamamlandığında bu olay tetiklenir
        /// </summary>
        public event CalculationEndProcessEventHandler onCalculationEnd;


        /// <summary>
        /// Herbir PM için rezervasyon işlemi Thread-Safe yapılmalı
        /// </summary>
        private Object reservationLock = new object();

        /// <summary>
        /// Operasyonların yeniden işleme alınma sayaçlarını saklayan listemiz
        /// </summary>
        private Dictionary<string, int> operationRetryList = new Dictionary<string, int>();

        /// <summary>
        /// Hatalı operasyonların yeniden kaç defa tekrarlanacağı
        /// </summary>
        private const int MAX_OPERATION_RETRY = 3;

        /// <summary>
        /// Parçaları optimize ederken bir parça üzerinde en fazla kaç defa optimizasyon işleminin tekrarlanacağı
        /// </summary>
        private const int MAX_OPTIMIZATION_RETRY = 100000;

        private const int SCATTER_METHOD_1_STEP_MAX_ITERATION = 4;

        private const int SCATTER_METHOD_2_STEP_MAX_ITERATION = 100;

        /// <summary>
        /// Operasyonun yeniden tekrarlanma sayacaı Thread-Safe kontrol edilmeli
        /// </summary>
        private Object retryLock = new Object();

        /// <summary>
        /// Tezgah sınıfı içerisinde kullanılan Thread ler
        /// </summary>
        private List<Thread> threads = new List<Thread>();

        /// <summary>
        /// Random sayı üreticisi
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Kullanılacak seed solutions sayısı
        /// </summary>
        private const int SEED_SOLUTION_COUNT = 7;

        /// <summary>
        /// Random seed seçiminde kullanılacak iterasyon sayısı
        /// </summary>
        private const int RANDOM_SEED_THRESHOLD = 1000;

        private const int MAX_SEARCH_ITERATION = 10;

        /// <summary>
        /// Operasyonların sonuçları
        /// </summary>
        private Dictionary<string, PmlOperationResult> operationResults = new Dictionary<string, PmlOperationResult>();

        /// <summary>
        /// Thread-safety
        /// </summary>
        private Object operationResultLock = new Object();

        private Utility utility = new Utility();

        /// <summary>
        /// Yeni bir tezgah oluşturur
        /// </summary>
        /// <param name="name">Tezgahın adı</param>
        public Bench(string name)
        {
            this.Name = name;

            this.Operations = new List<Operation>();
            this.PartMachiningLocations = new List<PartMachiningLocation>();
            this.Dependencies = new List<Dependency>();
            this.Items = new List<Item>();
        }

        /// <summary>
        /// Tezgahta oluşabilecek hataları gösterir
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Tezgahın adı
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Bu tezgahta yapılabilecek operasyonlar
        /// </summary>
        public List<Operation> Operations { get; internal set; }

        /// <summary>
        /// Bu tezgahtaki makine üniteleri
        /// </summary>
        public static List<MachineUnit> MachineUnits = new List<MachineUnit>();

        /// <summary>
        /// Bu tezgahtaki parça makine yerleri
        /// </summary>
        public List<PartMachiningLocation> PartMachiningLocations { get; internal set; }

        /// <summary>
        /// Bu tezgahtaki operasyonların birbirlerine olan bağımlılıkları
        /// </summary>
        public List<Dependency> Dependencies { get; internal set; }

        private List<Item> itemList = new List<Item>();

        /// <summary>
        ///  Bu tezgahta işlenmesi istenilen parçalar
        /// </summary>
        public List<Item> Items
        {
            get
            {
                return itemList;
            }
            internal set
            {
                this.itemList = value;

                ItemOperationCount = 0;

                foreach (Item item in this.itemList)
                {
                    ItemOperationCount += item.Operations.Count;
                }
            }
        }

        public int ItemOperationCount { get; internal set; }

        /// <summary>
        /// Çalışan bir simülasyon işlemi olup olmadığını gösterir
        /// </summary>
        public bool SimulationRunning { get; internal set; }

        public bool CalculationRunning { get; internal set; }

        /// <summary>
        /// Bu tezgahta yapılabilecek yeni bir operasyon ekler
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <param name="operationName">Operasyon adı</param>
        /// <param name="operationTime">Operasyon süresi</param>
        public void AddOperation(int operationNo, string operationName, int operationTime)
        {
            this.Operations.Add(new Operation(operationNo, operationName, operationTime));
        }

        /// <summary>
        /// Bu tezgahın desteklediği yeni bir makine unitesi ekler
        /// </summary>
        /// <param name="machineUnitNo">Makine ünitesi numarası</param>
        /// <param name="machineUnitName">Makine ünitesi adı</param>
        public void AddMachineUnit(int machineUnitNo, string machineUnitName)
        {
            MachineUnits.Add(new MachineUnit(machineUnitNo, machineUnitName));
        }

        /// <summary>
        /// Bu tezgaha yeni bir parça makine yeri ekler
        /// </summary>
        /// <param name="partMachiningLocationNo">Parça makine yerinin numarası</param>
        /// <param name="partMachiningLocationName">Parça makine yerinin adı</param>
        public void AddPartMachiningLocation(int partMachiningLocationNo, string partMachiningLocationName)
        {
            this.PartMachiningLocations.Add(new PartMachiningLocation(partMachiningLocationNo, partMachiningLocationName));
        }

        /// <summary>
        /// Bu tezgaha ait bir operasyonu alır
        /// </summary>
        /// <param name="operationNo">Operasyon numarası</param>
        /// <returns>Operasyon tezgah tarafından destekleniyorsa operasyonu, desteklenmiyorsa null döndürür</returns>
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
        /// Bu tezgahta yer alan bir makine ünitesine yeni bir operasyon ekler
        /// </summary>
        /// <param name="machineUnit">Makine ünitesi</param>
        /// <param name="operation">Eklenecek olan operasyon</param>
        /// <returns>Operasyon eklenirse true, eklenmezse false döner</returns>
        public bool AddOperationToMachineUnit(MachineUnit machineUnit, Operation operation)
        {
            for (int i=0;i<MachineUnits.Count;i++)
            {
                MachineUnit unit = MachineUnits[i];

                if (machineUnit.No == unit.No)
                {
                    unit.AddOperation(operation);

                    MachineUnits[i] = unit;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parça makine yerine yeni bir makine ünitesi ekler
        /// </summary>
        /// <param name="partMachiningLocation">Parça makine yeri</param>
        /// <param name="machineUnit">Makine ünitesi</param>
        /// <returns>Makine ünitesi eklenirse true, eklenmezse false döner</returns>
        public bool AddMachineUnitToPartMachiningLocation(PartMachiningLocation partMachiningLocation, MachineUnit machineUnit)
        {
            for (int i = 0; i < this.PartMachiningLocations.Count; i++)
            {
                PartMachiningLocation pml = this.PartMachiningLocations[i];

                if (partMachiningLocation.No == pml.No)
                {
                    partMachiningLocation.AddMachineUnit(machineUnit);

                    this.PartMachiningLocations[i] = partMachiningLocation;

                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Tezgaha yer alan makine ünitesini alır
        /// </summary>
        /// <param name="machineUnitNo">Makine ünitesi numarası</param>
        /// <returns>Makine ünitesi tezgahta bulunuyorsa makine ünitesini, bulunmazsa null döndürür</returns>
        public static MachineUnit GetMachineUnit(int machineUnitNo)
        {
            foreach (MachineUnit unit in MachineUnits)
            {
                if (machineUnitNo == unit.No)
                {
                    return unit;
                }
            }

            return null;
        }

        /// <summary>
        /// Parça makine yerine yeni bir makine ünitesi ekler
        /// </summary>
        /// <param name="partMachiningLocation">Parça makine yeri</param>
        /// <param name="machineUnit">Makine ünitesi</param>
        /// <returns>Makine ünitesi eklenirse true, eklenmezse false döner</returns>
        public bool AddMachineUnit(PartMachiningLocation partMachiningLocation, MachineUnit machineUnit)
        {
            for (int i = 0; i < this.PartMachiningLocations.Count; i++)
            {
                PartMachiningLocation pml = this.PartMachiningLocations[i];

                if (partMachiningLocation.No == pml.No)
                {
                    pml.AddMachineUnit(machineUnit);

                    this.PartMachiningLocations[i] = pml;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tezgahta yer alan bir parça makine yerini alır
        /// </summary>
        /// <param name="partMachiningLocationNo">Parça yeri numarası</param>
        /// <returns>Parça yeri varsa parça yeri, yoksa null döner</returns>
        public PartMachiningLocation GetPartMachiningLocation(int partMachiningLocationNo)
        {
            foreach (PartMachiningLocation pml in this.PartMachiningLocations)
            {
                if (partMachiningLocationNo == pml.No)
                {
                    return pml;
                }
            }

            return null;
        }

        /// <summary>
        /// Tezgahta yer alan bir parça makine yerini alır
        /// </summary>
        /// <param name="partMachiningLocationNo">Parça makine yeri</param>
        /// <returns>Parça yeri varsa parça yeri, yoksa null döner</returns>
        public PartMachiningLocation GetPartMachiningLocation(PartMachiningLocation pml)
        {
            return GetPartMachiningLocation(pml.No);
        }

        /// <summary>
        /// Tezgahta yer alan bir parçayı alır
        /// </summary>
        /// <param name="no">Parça numarası</param>
        /// <returns>Parça bulunursa parçayı, bulunamazsa null değer döndürür</returns>
        public Item GetItem(int no)
        {
            foreach (Item item in this.Items)
            {
                if (item.No == no)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Tezgahta yer alan bir parçayı alır
        /// </summary>
        /// <param name="no">Parça</param>
        /// <returns>Parça bulunursa parçayı, bulunamazsa null değer döndürür</returns>
        public Item GetItem(Item item)
        {
            return GetItem(item.No);
        }

        /// <summary>
        /// Tezgahta yer alan operasyonlar arasında yeni bir bağımlılık ekler
        /// </summary>
        /// <param name="sourceOperationNo">Kaynak operasyon numarası</param>
        /// <param name="targetOperationNo">Hedef operasyon numarası</param>
        /// <param name="type">Bağımlılık tipi</param>
        /// <returns>Bağımlılık eklenirse true, eklenmezse false döner</returns>
        public bool AddDependency(int sourceOperationNo, int targetOperationNo, DependencyType type)
        {
            Operation sourceOperation = GetOperation(sourceOperationNo);

            Operation targetOperation = GetOperation(targetOperationNo);

            if (sourceOperation != null && targetOperation != null)
            {
                this.Dependencies.Add(new Dependency(sourceOperation, targetOperation, type));
            }

            return false;
        }

        /// <summary>
        /// Tezgahta tanımlanmış bir bağımlılık bilgisini alır
        /// </summary>
        /// <param name="source">Kaynak operasyon</param>
        /// <param name="target">Hedef operasyon</param>
        /// <returns>Belirtilen iki operasyon arasında bir bağımlılık varsa, bu bağımlılığı; yoksa UNKNOWN bağımlılık döndürür</returns>
        public DependencyType GetDependency(Operation source, Operation target)
        {
            foreach (Dependency dependency in this.Dependencies)
            {
                if (dependency.Source.No == source.No && dependency.Target.No == target.No)
                {
                    return dependency.Type;
                }
            }

            return DependencyType.UNKNOWN;
        }

        /// <summary>
        /// Tezgaha işlenecek olan parçaları dosyadan yükler
        /// </summary>
        /// <param name="path">Dosya yolu</param>
        /// <returns>Parçalar yüklenirse True, yüklenemezse False döndürür</returns>
        public bool LoadItemsFromFile(string path)
        {
            if (path != null)
            {
                // Önce mevcut parçaları kaldır
                this.Items.Clear();

                this.ItemOperationCount = 0;

                System.IO.StreamReader file = new System.IO.StreamReader(path);

                try
                {
                    string line;

                    bool readItem = false;

                    int itemCounter = 0;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.ToLower().Equals("%items"))
                        {
                            readItem = true;

                            continue;
                        }

                        // parçayı oku
                        if (readItem)
                        {
                            string[] tmpStr = line.Split(',');

                            try
                            {
                                Item item = new Item(itemCounter, "Parça " + (itemCounter + 1));

                                int i = 0;
                                foreach (string str in tmpStr)
                                {
                                    int operationNo = int.Parse(str) - 1;

                                    if (operationNo >= 0)
                                    {
                                        Operation benchOperation = GetOperation(operationNo);

                                        // Parçalara ait operasyonlar tamamen birbirlerinden bağımsız olmalılar! 
                                        // Tezgahta tanımlı operasyonlarla bir ilişkileri kesinlikle olmamalı!
                                        // Aksi halde simulasyon yanlış çalışacaktır!!!
                                        Operation newOperation = new Operation(benchOperation.No, benchOperation.Name, benchOperation.Time, i + "-" + item.No + "-" + benchOperation.No);
                                        newOperation.Mode = benchOperation.Mode;
                                        newOperation.ToolNo = benchOperation.ToolNo;

                                        item.Operations.Add(newOperation);

                                        i++;
                                    }
                                    else
                                    {
                                        throw new Exception("Operasyon bulunamadı:" + str);
                                    }
                                }

                                this.Items.Add(item);

                                this.ItemOperationCount += item.Operations.Count;

                                itemCounter++;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Geçersiz satır: " + line + (e.Message.StartsWith("Operasyon")? " " + e.Message : ""));
                            }
                        }
                    }

                    file.Close();

                    ErrorMessage = "OK";

                    return true;
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;

                    return false;
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                throw new Exception("Parça bilgilerini almak için dosya yolu belirtmelisiniz!");
            }
        }

        /// <summary>
        /// Tezgah bilgilerini dosyadan okur
        /// </summary>
        /// <param name="path">Dosya yolu</param>
        /// <returns>Geçerli tezgah bilgileri varsa true, yoksa false döndürür. Başarılı olmayan yükleme işleminin sonucunu ErrorMessage özelliği ile görebilirsiniz</returns>
        public bool LoadFromFile(string path)
        {
            if (path != null)
            {
                // Önce mevcut tanımlamaları kaldır
                this.Operations.Clear();

                MachineUnits.Clear();

                this.PartMachiningLocations.Clear();

                this.Dependencies.Clear();

                System.IO.StreamReader file = new System.IO.StreamReader(path);

                try
                {
                    string line;

                    bool readOperation = false;

                    bool readDependency = false;

                    bool readMUs = false;

                    bool readPMLs = false;

                    bool readToolNo = false;

                    bool readMachiningMode = false;

                    int operationCounter = 0;

                    int muOperationCounter = 0;

                    int pmlMachineUnitCounter = 0;

                    int modeOperationCounter = 0;

                    int toolOperationCounter = 0;

                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.ToLower().Equals("%process_times"))
                        {
                            readOperation = true;

                            readDependency = false;

                            readMUs = false;

                            readPMLs = false;

                            readToolNo = false;

                            readMachiningMode = false;

                            continue;
                        }
                        else if (line.ToLower().Equals("%precedence"))
                        {
                            readDependency = true;

                            readOperation = false;

                            readMUs = false;

                            readPMLs = false;

                            readToolNo = false;

                            readMachiningMode = false;

                            continue;
                        }
                        else if (line.ToLower().Equals("%availability_to_the_mus"))
                        {
                            readMUs = true;

                            readDependency = false;

                            readOperation = false;

                            readPMLs = false;

                            readToolNo = false;

                            readMachiningMode = false;

                            continue;
                        }
                        else if (line.ToLower().Equals("%availability_of_mus_to_the_pmls"))
                        {
                            readMUs = false;

                            readDependency = false;

                            readOperation = false;

                            readPMLs = true;

                            readToolNo = false;

                            readMachiningMode = false;

                            continue;
                        }
                        else if (line.ToLower().Equals("%machining_modes"))
                        {
                            readMUs = false;

                            readDependency = false;

                            readOperation = false;

                            readPMLs = false;

                            readToolNo = false;

                            readMachiningMode = true;

                            continue;
                        }
                        else if (line.ToLower().Equals("%toolno"))
                        {
                            readMUs = false;

                            readDependency = false;

                            readOperation = false;

                            readPMLs = false;

                            readToolNo = true;

                            readMachiningMode = false;

                            continue;
                        }

                        // Operasyon tanımlarını oku
                        if (readOperation)
                        {
                            int time = 0;

                            try
                            {
                                time = int.Parse(line);
                            }
                            catch (Exception)
                            {
                                throw new Exception("Geçersiz satır: " + line);
                            }

                            this.AddOperation(operationCounter, "Operasyon " + (operationCounter + 1), time);

                            operationCounter++;
                        }

                        // Operasyon bağımlılıklarını oku
                        if (readDependency)
                        {
                            try
                            {
                                string[] tmpStr = line.Split(',');

                                int sourceNo = int.Parse(tmpStr[0]) - 1;

                                int targetNo = int.Parse(tmpStr[1]) - 1;

                                if (sourceNo >= 0 && targetNo >= 0)
                                {
                                    int type = int.Parse(tmpStr[2]);

                                    // Bağımlılığı ekle
                                    this.AddDependency(sourceNo, targetNo, (DependencyType)type);
                                }
                                else
                                {
                                    throw new Exception("Operasyon bulunamadı: " + (sourceNo < 0 ? tmpStr[0] : tmpStr[1]));
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Geçersiz satır: " + line + (e.Message.StartsWith("Operasyon") ? " " + e.Message : ""));
                            }
                        }

                        // MU tanımlarını oku
                        if (readMUs)
                        {
                            try
                            {
                                string[] tmpStr = line.Split(',');

                                for (int i = 0; i < tmpStr.Length; i++)
                                {
                                    int flag = int.Parse(tmpStr[i]);

                                    MachineUnit unit = GetMachineUnit(i);

                                    if (unit == null)
                                    {
                                        // Yeni makine ünitesi ekle
                                        MachineUnit newMachineUnit = new MachineUnit(i, "Makine Ünitesi " + (i + 1));

                                        if (flag == 1)
                                        {
                                            // Makine ünitesini yapabileceği operasyonlarla ilişkilendir
                                            newMachineUnit.AddOperation(GetOperation(muOperationCounter));
                                        }

                                        MachineUnits.Add(newMachineUnit);
                                    }
                                    else
                                    {
                                        if (flag == 1)
                                        {
                                            // Makine ünitesini yapabileceği operasyonlarla ilişkilendir
                                            AddOperationToMachineUnit(unit, GetOperation(muOperationCounter));
                                        }
                                    }
                                }

                                muOperationCounter++;
                            }
                            catch (Exception)
                            {
                                throw new Exception("Geçersiz satır: " + line);
                            }
                        }

                        // PML tanımlarını oku
                        if (readPMLs)
                        {
                            try
                            {
                                string[] tmpStr = line.Split(',');

                                for (int i = 0; i < tmpStr.Length; i++)
                                {
                                    int flag = int.Parse(tmpStr[i]);

                                    PartMachiningLocation pml = GetPartMachiningLocation(i);

                                    if (pml == null)
                                    {
                                        // Yeni parça makine yeri ekle
                                        PartMachiningLocation newPartMachiningLocation = new PartMachiningLocation(i, "Parça Makine Yeri " + (i + 1));

                                        if (flag == 1)
                                        {
                                            // Parça makine yeri tarafından kullanılabilecek makine ünitesini PML ile ilişkilendir
                                            newPartMachiningLocation.AddMachineUnit(GetMachineUnit(pmlMachineUnitCounter));
                                        }

                                        this.PartMachiningLocations.Add(newPartMachiningLocation);
                                    }
                                    else
                                    {
                                        if (flag == 1)
                                        {
                                            // Parça makine yeri tarafından kullanılabilecek makine ünitesini PML ile ilişkilendir
                                            AddMachineUnitToPartMachiningLocation(pml, GetMachineUnit(pmlMachineUnitCounter));
                                        }
                                    }
                                }

                                pmlMachineUnitCounter++;
                            }
                            catch (Exception)
                            {
                                throw new Exception("Geçersiz satır: " + line);
                            }
                        }

                        // Takım tanımlarını oku
                        if (readToolNo)
                        {
                            int toolNo = 0;

                            try
                            {
                                toolNo = int.Parse(line);
                            }
                            catch (Exception)
                            {
                                throw new Exception("Geçersiz satır: " + line);
                            }

                            Operation operation = GetOperation(toolOperationCounter);

                            if (operation != null)
                            {
                                operation.ToolNo = toolNo;
                            }

                            toolOperationCounter++;
                        }

                        // Mod tanımlarını oku
                        if (readMachiningMode)
                        {
                            int mode = 0;

                            try
                            {
                                mode = int.Parse(line);

                                Operation operation = GetOperation(modeOperationCounter);

                                if (operation != null)
                                {
                                    operation.Mode = (OperationMode)mode;
                                }
                            }
                            catch (Exception)
                            {
                                throw new Exception("Geçersiz satır: " + line);
                            }

                            modeOperationCounter++;
                        }
                    }

                    file.Close();

                    ErrorMessage = "OK";

                    return true;
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;

                    return false;
                }
                finally
                {
                    file.Close();
                }
            }
            else
            {
                throw new Exception("Tezgah bilgilerini almak için dosya yolu belirtmelisiniz!");
            }
        }

        /// <summary>
        /// Tezgah hakkında bilgi verir
        /// </summary>
        /// <returns>Tezgah bilgilerini geriye döndürür</returns>
        public string GetInformation()
        {
            string result = "";

            result = "TEZGAH ADI: " + Name + Environment.NewLine + Environment.NewLine;

            result += "********************PARÇA MAKİNE ÜNİTESİ YERİ********************";

            result += Environment.NewLine + Environment.NewLine;

            foreach(PartMachiningLocation pml in this.PartMachiningLocations ){
                result += pml.No + " - " + pml.Name;

                result += Environment.NewLine;

                if (pml.HasMachineUnits)
                {
                    foreach (int unitNo in pml.MachineUnitNOs)
                    {
                        result += unitNo + " - " + GetMachineUnit(unitNo).Name + Environment.NewLine;
                    }
                }

                result += Environment.NewLine;
            }

            result += Environment.NewLine;

            result += "********************PARÇA MAKİNE ÜNİTESİ YERİ********************";

            result += Environment.NewLine + Environment.NewLine;

            result += "*************************MAKİNE ÜNİTELERİ************************";

            result += Environment.NewLine + Environment.NewLine;

            foreach (MachineUnit mu in MachineUnits)
            {
                result += mu.No + " - " + mu.Name;

                result += Environment.NewLine;

                if (mu.HasOperations)
                {
                    foreach (Operation operation in mu.Operations)
                    {
                        result += operation.No + " - " + operation.Name + Environment.NewLine;
                    }
                }

                result += Environment.NewLine;
            }

            result += Environment.NewLine;

            result += "*************************MAKİNE ÜNİTELERİ************************";

            result += Environment.NewLine + Environment.NewLine;

            result += "************************OPERASYON TANIMLARI**********************";

            result += Environment.NewLine + Environment.NewLine;

            foreach (Operation operation in this.Operations)
            {
                result += operation.No + " - " + operation.Name + ", Süre: " + operation.Time + " saniye, Takım: " + operation.ToolNo + ", Mod: " + operation.Mode;

                result += Environment.NewLine;
            }

            result += Environment.NewLine;

            result += "************************OPERASYON TANIMLARI**********************";

            result += Environment.NewLine + Environment.NewLine;

            result += "***************************BAĞIMLILIKLAR*************************";

            result += Environment.NewLine + Environment.NewLine;

            foreach (Dependency dependency in this.Dependencies)
            {
                result += dependency.ToString();

                result += Environment.NewLine;
            }
            result += Environment.NewLine;

            result += "***************************BAĞIMLILIKLAR*************************";

            result += Environment.NewLine + Environment.NewLine;

            result += "******************************PARÇALAR***************************";

            result += Environment.NewLine + Environment.NewLine;

            foreach (Item item in this.Items)
            {
                result += item.Name;

                result += Environment.NewLine;

                if (item.HasOperations)
                {
                    foreach (Operation operation in item.Operations)
                    {
                        result += operation.No + " - " + operation.Name + Environment.NewLine;
                    }
                }

                result += Environment.NewLine;
            }

            result += Environment.NewLine;

            result += "******************************PARÇALAR***************************";

            result += Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Tezgah içerisinde işlenecek parça olup olmadığını gösterir
        /// </summary>
        /// <returns>Parça ya da parçalar varsa True, yoksa False döndürür</returns>
        public bool HasItems
        {
            get
            {
                return this.Items.Count > 0;
            }
        }

        /// <summary>
        /// Çalışan bir simülasyonu iptal eder
        /// </summary>
        public void CancelSimulation()
        {
            SimulationRunning = false;

            KillSimulationThreads();
        }

        private void KillSimulationThreads()
        {
            foreach (Thread thread in this.threads)
            {
                try
                {
                    thread.Abort();
                }
                catch (Exception)
                {
                    // NOP
                }
            }

            this.threads.Clear();
        }

        public void Calculate(BenchPmlExecutionList executionList, List<Item> itemList, List<BenchPmlExecutionList> usedExecutionList, int bestTime = Int32.MinValue)
        {
            if (!CalculationRunning)
            {
                CalculationMode = BenchProcessor.CalculationMode.NORMAL;

                Thread calculationThread = new Thread(() => CalculateThreadJob(executionList, itemList, usedExecutionList, bestTime));
                calculationThread.Name = "Bench Calculation Thread";

                calculationThread.Start();
            }
        }

        private PmlOperationResultList CalculateThreadJob(BenchPmlExecutionList executionList, List<Item> itemList, List<BenchPmlExecutionList> usedExecutionList, int bestTime = Int32.MinValue)
        {
            CalculationRunning = true;

            PmlOperationResultList bestSolution = null;

            if (HasItems)
            {
                if (this.onCalculationBegin != null)
                {
                    this.onCalculationBegin();
                }

                Stopwatch stopWatch = Stopwatch.StartNew();

                int possibilities = 0;

                if (HaveValidOperations())
                {
                    List<Item> orginalItems = this.Items;

                    // Önceden tanımlı bir parça listesi verilmişse bunu kullan
                    if (itemList != null)
                    {
                        this.Items = itemList;
                    }

                    OptimizeItems(this.Items);

                    List<BenchPmlExecutionList> benchExecutionList = new List<BenchPmlExecutionList>();

                    if (executionList == null)
                    {
                        benchExecutionList = utility.GetItemExecutionList(Enumerable.Range(0, this.PartMachiningLocations.Count).ToList(), this.Items.Count);
                    }
                    else
                    {
                        benchExecutionList.Add(executionList);
                    }

                    int step = 0;

                    possibilities = benchExecutionList.Count;

                    foreach (BenchPmlExecutionList benchExecution in benchExecutionList)
                    {
                        bool used = false;

                        if (usedExecutionList!=null)
                        {
                            foreach (BenchPmlExecutionList usedList in usedExecutionList)
                            {
                                if (benchExecution.Code.Equals(usedList.Code))
                                {
                                    used = true;

                                    break;
                                }
                            }
                        }

                        if (!used)
                        {
                            // Tezgahı sıfırla
                            Reset();

                            bool error = false;

                            PmlOperationResultList result = new PmlOperationResultList();

                            foreach (PmlItemExecutionList pmlExecution in benchExecution)
                            {
                                PartMachiningLocation pml = this.PartMachiningLocations[pmlExecution.PMLIndex];

                                // Herbir parçayı işle
                                int operationOrderNo = 0;

                                foreach (int itemIndex in pmlExecution.ItemIndices)
                                {
                                    Item item = this.Items[itemIndex];

                                    List<PmlOperationResult> itemOperationResults = new List<PmlOperationResult>();

                                    // Yeni bir rezervasyon talep et
                                    Reservation reservation = ReserveOperations(pml, item);

                                    while (reservation.Reserved && !error)
                                    {
                                        PmlOperationResult pmlOperationResult = new PmlOperationResult();

                                        // Operasyonları yap
                                        List<int> usedMachineUnits = new List<int>();

                                        foreach (Operation operation in reservation.Operations)
                                        {
                                            // Operasyonu yap
                                            bool done = false;

                                            // Burada makine ünitesi seçimi farklı bir algoritma ile de yapılabilir.
                                            foreach (int unitNo in operation.MachineUnitNOs)
                                            {
                                                if (!usedMachineUnits.Contains(unitNo))
                                                {
                                                    // İlgili makine ünitesini al
                                                    MachineUnit unit = GetMachineUnit(unitNo);

                                                    // Operasyonu yap
                                                    OperationResult operationResult = unit.DoOperation(operation);

                                                    // Parçayı ata
                                                    operationResult.Item = item;

                                                    // Operasyonu ata
                                                    operationResult.Operation = operation;

                                                    // Operasyon kodunu ata
                                                    operationResult.Code = pml.No + "-" + unit.No + "-" + item.No + "-" + operation.No;

                                                    // Kullanılan makine ünitelerine ekle
                                                    usedMachineUnits.Add(unitNo);

                                                    // Operasyon sonucunu al
                                                    pmlOperationResult.Add(operationResult);

                                                    // Operasyonu yapıldı olarak işaretle
                                                    done = true;

                                                    // Aynı zamanda parça üzerindeki ilgili operasyonu da yapıldı olarak işaretle
                                                    item.UpdateOperationStatus(operation, OperationStatus.DONE);

                                                    break;
                                                }
                                            }

                                            if (!done)
                                            {
                                                error = true;

                                                break;
                                            }
                                        }

                                        if (!error)
                                        {
                                            // Kodu ata
                                            pmlOperationResult.Code = pml.No + "-" + item.No;

                                            // PML'i ata
                                            pmlOperationResult.PartMachiningLocation = pml;

                                            foreach (Operation operation in reservation.Operations)
                                            {
                                                pmlOperationResult.Code += "-" + operation.No;
                                            }

                                            // Parçayı ata
                                            pmlOperationResult.Item = item;

                                            // Operasyon sırasını belirle
                                            pmlOperationResult.No = operationOrderNo;

                                            // Sonuçlara ekle
                                            itemOperationResults.Add(pmlOperationResult);


                                            // Operasyon sıra numarasını arttır
                                            operationOrderNo++;

                                            // Yeni rezervasyonları al
                                            reservation = ReserveOperations(this.PartMachiningLocations[pmlExecution.PMLIndex], item);
                                        }
                                    }

                                    if (!error)
                                    {
                                        result.Add(pml, itemOperationResults);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!error)
                            {
                                if (result.OperationCount == this.ItemOperationCount)
                                {
                                    result.Optimize();

                                    result.ExecutionList = benchExecution;

                                    if (bestSolution == null)
                                    {
                                        bestSolution = result;
                                    }
                                    else
                                    {
                                        if (result.TotalTime < bestSolution.TotalTime)
                                        {
                                            bestSolution = result;
                                        }
                                    }

                                    if (bestSolution.TotalTime < bestTime)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        step++;

                        if (this.onCalculationStep != null)
                        {
                            this.onCalculationStep((100 * step) / benchExecutionList.Count);
                        }
                    }

                    // Önceden tanımlı bir parça listesi verilmişse, orjinal olanları geri yükle
                    if (itemList != null)
                    {
                        this.Items = orginalItems;
                    }
                }

                if (this.onCalculationEnd != null)
                {
                    this.onCalculationEnd(new CalculationEndEventArgs(bestSolution, stopWatch.ElapsedMilliseconds, possibilities));
                }

                stopWatch.Stop();
            }

            CalculationRunning = false;

            return bestSolution;
        }

        /// <summary>
        /// Tezgahı simüle eder
        /// </summary>
        public void Simulate(BenchPmlExecutionList executionList, List<Item> itemList)
        {
            if (!SimulationRunning)
            {
                Thread simulationThread = new Thread(() => SimulateThreadJob(executionList, itemList));
                simulationThread.Name = "Bench Simulation Thread";

                simulationThread.Start();
            }
        }

        /// <summary>
        /// Simülasyon için PML thread leri tarafından kullanılacak olan metot
        /// </summary>
        private PmlOperationResultList SimulateThreadJob(BenchPmlExecutionList executionList, List<Item> itemList)
        {
            PmlOperationResultList bestSolution = null;

            if (HasItems)
            {
                Reset();

                SimulationRunning = true;

                if (this.onSimulationBegin != null)
                {
                    this.onSimulationBegin();
                }

                Stopwatch stopWatch = Stopwatch.StartNew();

                int possibilities = 0;

                if (HaveValidOperations())
                {
                    double step = 0;

                    PmlOperationResultList result = new PmlOperationResultList();

                    List<BenchPmlExecutionList> benchExecutionList = new List<BenchPmlExecutionList>();

                    if (executionList == null)
                    {
                        benchExecutionList = utility.GetItemExecutionList(Enumerable.Range(0, this.PartMachiningLocations.Count).ToList(), this.Items.Count);
                    }
                    else
                    {
                        benchExecutionList.Add(executionList);
                    }

                    List<Item> orginalItems = this.Items;

                    // Önceden tanımlı bir parça listesi verilmişse bunu kullan
                    if (itemList != null)
                    {
                        this.Items = itemList;
                    }

                    OptimizeItems(this.Items);

                    possibilities = benchExecutionList.Count;

                    foreach (BenchPmlExecutionList benchExecution in benchExecutionList)
                    {
                        for (int k = 0; k < MAX_SEARCH_ITERATION; k++)
                        {
                            // Tezgahı resetle
                            Reset();

                            // PML'lere parçaları ata
                            foreach(PmlItemExecutionList pmlExecutionList in benchExecution)
                            {
                                foreach (int itemIndex in pmlExecutionList.ItemIndices)
                                {
                                    this.PartMachiningLocations[pmlExecutionList.PMLIndex].EnqueueItem(this.Items[itemIndex]);
                                }
                            }

                            // PML'leri atanan parçalarla birlikte simüle et
                            result = SimulatePMLs();

                            if (result.OperationCount == this.ItemOperationCount)
                            {
                                result.ExecutionList = benchExecution;

                                if (bestSolution == null)
                                {
                                    bestSolution = result;
                                }
                                else
                                {
                                    if (result.TotalTime < bestSolution.TotalTime)
                                    {
                                        bestSolution = result;
                                    }
                                }
                            }

                            step++;

                            if (this.onSimulationStep != null)
                            {
                                this.onSimulationStep((100 * step) / (benchExecutionList.Count * MAX_SEARCH_ITERATION));
                            }
                        }
                    }

                    // Önceden tanımlı bir parça listesi verilmişse, orjinal olanları geri yükle
                    if (itemList != null)
                    {
                        this.Items = orginalItems;
                    }
                }

                long duration = stopWatch.ElapsedMilliseconds;

                if (this.onSimulationEnd != null)
                {
                    this.onSimulationEnd(new SimulationEndEventArgs(bestSolution, duration, possibilities));
                }

                stopWatch.Stop();

                Reset();

                KillSimulationThreads();

                SimulationRunning = false;
            }

            return bestSolution;
        }

        private List<List<int>> Split(List<int> source, int size)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private PmlOperationResultList SimulatePMLs()
        {
            PmlOperationResultList result = new PmlOperationResultList();

            // PML operasyonlarını başlat
            foreach (PartMachiningLocation pml in this.PartMachiningLocations)
            {
                if (pml.HasItems)
                {
                    pml.Reset();

                    Thread processThread = new Thread(() => UsePartMachiningLocation(pml));
                    processThread.Name = "Bench PML Main Thread " + pml.No;

                    processThread.Start();

                    this.threads.Add(processThread);
                }
            }

            // Simülasyonun tamamlanıp tamamlanmadığını kontrol et
            while (HasUncompletedOperations)
            {
                Thread.Sleep(50);
            }

            // Sonuçları al
            foreach (PartMachiningLocation pml in this.PartMachiningLocations)
            {
                List<PmlOperationResult> list = new List<PmlOperationResult>(pml.Results);

                result.Add(pml, list);
            }

            return result;
        }

        /// <summary>
        /// Parçaların hatalarını kontrol eder
        /// </summary>
        public bool HaveValidOperations()
        {
            bool modeError = false, toolError = false;

            foreach (Item item in this.Items)
            {
                foreach (Operation operation in item.Operations)
                {
                    if (operation.Mode == OperationMode.UNKNOWN)
                    {
                        modeError = true;
                    }
                    else if (operation.ToolNo <= 0)
                    {
                        toolError = true;
                    }
                }
            }

            return !modeError && !toolError;
        }

        /// <summary>
        /// Parça ve operasyonlar bu metot içerisinde optimize edilirler
        /// </summary>
        public void OptimizeItems(List<Item> items)
        {
            // Parçaları öncelik ilişkilerine göre sırala
            foreach (Item item in items)
            {
                OptimizeItem(item, false);
            }
        }

        /// <summary>
        /// Parçaya ait operasyonları öncelik durumlarına göre uygun hale getirir
        /// </summary>
        /// <param name="item">Parça</param>
        /// <param name="showOutput">Bilgi amaçlı çıktılar gösterilsin mi?</param>
        private void OptimizeItem(Item item, bool showOutput)
        {
            int sourceIndex = 0;

            int retryCount = 0;

            while (sourceIndex < item.Operations.Count && retryCount < MAX_OPTIMIZATION_RETRY)
            {
                Operation source = item.Operations[sourceIndex];

                Operation target = null;

                int targetIndex = 0;

                for (int i = 0; i < item.Operations.Count; i++)
                {
                    if (sourceIndex != i)
                    {
                        Operation tmpTarget = item.Operations[i];

                        DependencyType type = GetDependency(source, tmpTarget);

                        if (type != DependencyType.UNKNOWN && sourceIndex < i)
                        {
                            // Hedef operasyon tamamlanmadan kaynak operasyon başlayamaz ya da hedef operasyon başlamadan kaynak operasyon başlayamaz
                            // gibi bir bağımlılık varsa, her halükarda hedef operasyonun kaynak operasyonunun başına alınması gerekir.
                            target = tmpTarget;

                            targetIndex = i;

                            break;
                        }
                    }
                }

                // Değişiklik (shift işlemi) yapılması gerekiyor.
                if (target != null)
                {
                    Operation tmp = target;

                    if (item.Operations.Remove(target))
                    {
                        // Operasyonları ötele
                        item.Operations.Insert(sourceIndex, tmp);

                        if (showOutput)
                        {
                            FireConsoleEvent("\"" + item.Name + "\" içerisindeki \"" + target.Name + "\" operasyonu, \"" + source.Name + "\" operasyonunun önüne alındı");
                        }

                        // Sayacı yeniden başa al
                        sourceIndex = 0;
                    }
                }
                else
                {
                    sourceIndex++;
                }

                retryCount++;
            }
        }

        /// <summary>
        /// Operasyonları öncelik durumlarına göre uygun hale getirir
        /// </summary>
        /// <param name="operations">Operasyonlar</param>
        /// <returns>Öncelik ilişkilerine göre sıralanmış yeni operasyon listesi</returns>
        private List<Operation> OptimizeOperations(List<Operation> operations)
        {
            Item item = new Item(0, "Dummy");

            item.Operations = operations;

            OptimizeItem(item, false);

            return item.Operations;
        }

        private void Reset()
        {
            this.operationResults.Clear();

            // Parça makine yerlerini sıfırla
            foreach (PartMachiningLocation pml in this.PartMachiningLocations)
            {
                pml.Available = true;

                pml.ClearQueue();

                pml.Reset();

                pml.Time = 0;

                // Makine ünitelerini sıfırla
                foreach (MachineUnit mu in MachineUnits)
                {
                    mu.Available = true;

                    mu.Start = 0;

                    mu.Finish = 0;

                    mu.Unlock();
                }
            }

            // Parça ve operasyonları sıfırla
            foreach (Item item in this.Items)
            {
                item.Status = ItemStatus.AVAILABLE;

                // Operasyonları sıfırla
                foreach (Operation operation in item.Operations)
                {
                    operation.Status = OperationStatus.WAITING;
                }
            }
        }

        public int GetItemIndex(Item item)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                Item target = this.Items[i];

                if (target.No == item.No)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetPMLIndex(PartMachiningLocation partMachiningLocation)
        {
            for (int i=0; i<this.PartMachiningLocations.Count;i++)
            {
                PartMachiningLocation pml = this.PartMachiningLocations[i];

                if (pml.No == partMachiningLocation.No)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Tezgahta bulunan bir PML için görev tanımları burada yer alır
        /// </summary>
        /// <param name="index">Tezgahta bulunan PML için index numarası</param>
        private void UsePartMachiningLocation(PartMachiningLocation pml)
        {
            // Tezgahta tamamlanmamış operasyonun olup olmadığını kontrol et
            while (!pml.JobsCompleted())
            {
                Item item = pml.getCurrentItem();

                if (item != null)
                {
                    while (item.Status != ItemStatus.DONE && item.Status != ItemStatus.ERROR)
                    {
                        Reservation reservation = ReserveOperations(pml, item);

                        // Parça başarılı bir şekilde rezerve edilmiş mi?
                        if (reservation.Reserved)
                        {
                            lock (operationResultLock)
                            {
                                if (operationResults.ContainsKey(reservation.Key))
                                {
                                    operationResults[reservation.Key] = null;
                                }
                                else
                                {
                                    operationResults.Add(reservation.Key, null);
                                }
                            }

                            Thread operationWorkerThread = new Thread(() => DoOperation(pml, item, reservation.Operations, 0));
                            operationWorkerThread.Name = "PML " + pml.No + " Operation Worker";

                            operationWorkerThread.Start();

                            // Operasyonların tamamlanmasını bekle
                            PmlOperationResult result = null;

                            while (result == null)
                            {
                                lock (operationResultLock)
                                {
                                    result = operationResults[reservation.Key];
                                }
                            }
                        }
                    }

                    pml.Dequeue();
                }
            }
        }

        private void DoOperation(PartMachiningLocation pml, Item item, List<Operation> operations, int outputIndex)
        {
            PmlOperationResult result = new PmlOperationResult();

            if (pml != null)
            {
                int index = GetPMLIndex(pml);

                if (index != -1)
                {
                    if (item != null)
                    {
                        if (operations != null)
                        {
                            // Operasyona başla
                            foreach (Operation operation in operations)
                            {
                                operation.Status = OperationStatus.IN_PROCESS;

                                if (this.onOperationProcessBegin != null)
                                {
                                    onOperationProcessBegin(new ItemEventArgs(item, operation));
                                }
                            }

                            // Operasyonu yap
                            result = pml.DoOperation(item, operations);

                            foreach (Operation operation in operations)
                            {
                                if (this.onOperationProcessEnd != null)
                                {
                                    onOperationProcessEnd(new ItemEventArgs(item, operation));
                                }
                            }

                            // Operasyon sonucunu belirle
                            lock (operationResultLock)
                            {
                                operationResults[result.Code] = result;
                            }

                            if (result.Result == 0)
                            {
                                // Operasyon sonucunu belirle
                                lock (operationResultLock)
                                {
                                    operationResults[result.Code] = result;
                                }

                                foreach (Operation operation in operations)
                                {
                                    // Operasyonu bitir
                                    operation.Status = OperationStatus.DONE;
                                }
                            }
                            else
                            {
                                // Sayacı kontrol et, max. operasyon tekrarlanma sayısına erişildiyse operasyonu hatalı olarak işaretle, erişilmediyse bekleme durumuna al
                                int retryCount = 1;

                                lock (retryLock)
                                {
                                    if (this.operationRetryList.ContainsKey(result.Code))
                                    {
                                        retryCount = this.operationRetryList[result.Code];

                                        retryCount++;
                                    }

                                    this.operationRetryList[result.Code] = retryCount;
                                }

                                if (retryCount < MAX_OPERATION_RETRY)
                                {
                                    foreach (Operation operation in operations)
                                    {
                                        // Operasyonu yeniden bekleme durumuna al
                                        operation.Status = OperationStatus.WAITING;
                                    }
                                }
                                else
                                {
                                    foreach (Operation operation in operations)
                                    {
                                        // Çok fazla denedik ama operasyonu yapamadık, operasyonu hatalı olarak işaretle
                                        operation.Status = OperationStatus.HAS_ERROR;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.Result = (int)Error.PML_OPERATION_IS_NOT_FOUND;
                        }
                    }
                    else
                    {
                        result.Result = (int)Error.PML_ITEM_IS_NOT_FOUND;
                    }
                }
                else
                {
                    result.Result = (int)Error.PML_PML_NOT_FOUND;
                }
            }
            else
            {
                result.Result = (int)Error.PML_PML_NOT_FOUND;
            }
        }

        /// <summary>
        /// Bu tezgah içerisindeki belirtilen bir PML için tamamlanmamış bir ya da daha fazla operasyonu bulur ve rezerve eder
        /// </summary>
        /// <param name="pml">Parça makine yeri</param>
        /// <returns>Rezervasyon bilgisini döndürür</returns>
        private Reservation ReserveOperations(PartMachiningLocation pml, Item item)
        {
            Reservation reservation = new Reservation(false);

            // Herbir PML için ayrı Thread oluşturulduğu için bu işlem Thread-Safe yapılmalı
            lock (reservationLock)
            {
                int operationsDone = 0;

                // Parça başka bir PML tarafından rezerve edilmiş mi?
                if (item.Status == ItemStatus.AVAILABLE)
                {
                    for (int i = 0; i < item.Operations.Count; i++)
                    {
                        Operation sourceOperation = item.Operations[i];

                        if (sourceOperation.Status == OperationStatus.WAITING)
                        {
                            List<int> sourceUnitList = pml.GetSupportedMachineUnits(sourceOperation);

                            if (sourceUnitList.Count > 0)
                            {
                                // Rezervasyon koduna yalnızca PML ve parça eklenmeli burada, en son adımda, sıralanmış operasyonlar bu koda dahil edilmeli
                                reservation.Key = pml.No + "-" + item.No;

                                if (reservation.Operations.Count < 1)
                                {
                                    // Makine ünitelerini operasyona ekle
                                    sourceOperation.MachineUnitNOs = sourceUnitList;

                                    // Rezervasyon bilgilerini ekle
                                    reservation.Reserved = true;
                                    reservation.Item = item;
                                    reservation.Operations.Add(sourceOperation);

                                    // Rezervasyon yapılan operasyonların yapılabileceği makine ünitelerini ekle
                                    foreach (int unitNo in sourceUnitList)
                                    {
                                        reservation.MachineUnitNOs.Add(unitNo);
                                    }
                                }

                                // Birlikte yapılabilecek operasyonları ara
                                Operation targetOperation = pml.SearchAvailableSimultaneousOperation(this, reservation.Operations, item);

                                if (targetOperation != null)
                                {
                                    List<int> targetUnitList = pml.GetSupportedMachineUnits(targetOperation);

                                    if (targetUnitList.Count > 0)
                                    {
                                        bool add = true;

                                        // Operasyonların ikisi de tek bir MU tarafından mı yapılıyor?
                                        if ((sourceUnitList.Count == 1 && targetUnitList.Count == 1) && (sourceUnitList[0] == targetUnitList[0]))
                                        {
                                            add = false;
                                        }

                                        if (add)
                                        {
                                            // Makine ünitelerini operasyona ekle
                                            targetOperation.MachineUnitNOs = targetUnitList;

                                            // Rezervasyon bilgilerini ekle
                                            reservation.Operations.Add(targetOperation);

                                            // Rezervasyon yapılan operasyonların yapılabileceği makine ünitelerini ekle
                                            foreach (int unitNo in targetUnitList)
                                            {
                                                reservation.MachineUnitNOs.Add(unitNo);
                                            }
                                        }
                                    }
                                }

                                // En fazla 2 adet operasyon eş zamanlı operasyon yapılabilir
                                if (reservation.Operations.Count == PartMachiningLocation.MAX_SIMULTANEOUS_OPERATION)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (sourceOperation.Status == OperationStatus.DONE || sourceOperation.Status == OperationStatus.HAS_ERROR)
                            {
                                operationsDone++;
                            }
                        }
                    }
                }

                // Tüm operasyonlar tamamlanmış mı?
                if (operationsDone == item.Operations.Count)
                {
                    // Hatalı parçalar da burada kontrol edilebilir
                    item.Status = ItemStatus.DONE;
                }
            }

            // Operasyonları sırala (önemli)
            if (reservation.Operations.Count > 0)
            {
                reservation.Operations = reservation.Operations.OrderBy(o => o.MachineUnitNOs.Count).ToList();
            }

            // Rezervasyon koduna operasyonları da ekle
            foreach (Operation operation in reservation.Operations)
            {
                reservation.Key += "-" + operation.No;
            }

            return reservation;
        }

        /// <summary>
        /// Tezgahta tamamlanmamış parça olup olmadığını gösterir
        /// </summary>
        private bool HasUncompletedOperations
        {
            get
            {
                foreach (PartMachiningLocation pml in this.PartMachiningLocations)
                {
                    if (!pml.JobsCompleted())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Tezgahtaki olayları tetikler ve dışarı aktarır
        /// </summary>
        /// <param name="message">Mesaj</param>
        private void FireConsoleEvent(string message)
        {
            if (this.onNewMessage != null)
            {
                this.onNewMessage(message);
            }
        }

        public CalculationMode CalculationMode { get; internal set; }

        public void ScatterSearch(bool useFirstMethod)
        {
            Thread scatterSearchThread = new Thread(() => ScatterSearchThreadJob(useFirstMethod));
            scatterSearchThread.Name = "Bench Scatter Search Thread";

            scatterSearchThread.Start();
        }

        /// <summary>
        /// Scatter Search kullanılarak tezgahı simüle eder
        /// </summary>
        public void ScatterSearchThreadJob(bool useFirstMethod)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            if (this.onScatterBegin != null)
            {
                onScatterBegin();
            }

            #region Create Seed Solutions

            Dictionary<int, List<Item>> referenceSet = new Dictionary<int, List<Item>>();
                
            // Use LPT (longest processing time) algorithm to create the first seed solution
            referenceSet.Add(0, getLPTSeedResult());

            // Use random method to create other seed solutions
            for (int i = 1; i < SEED_SOLUTION_COUNT - 1; i++)
            {
                referenceSet.Add(i, getRandomSeedResult());
            }

            // Use SPT (shortes processing time) algorithm to create the last seed solution
            referenceSet.Add(SEED_SOLUTION_COUNT - 1, getSPTSeedResult());

            FireConsoleEvent("Seçilen ilk (seed) çözümler: ");

            for (int i = 0; i < SEED_SOLUTION_COUNT; i++)
            {
                List<Item> items = referenceSet[i];

                FireConsoleEvent(Environment.NewLine + "Seed: " + (i + 1) + Environment.NewLine);

                foreach (Item item in items)
                {
                    string tmp = "";

                    foreach (Operation operation in item.Operations)
                    {
                        tmp += "No: " + operation.No + " (Süre: " + operation.Time + "), ";
                    }

                    if (!tmp.Equals(""))
                    {
                        tmp = tmp.Substring(0, tmp.Length - 1);

                        FireConsoleEvent("Item " + item.Name + ": " + tmp);
                    }
                }
            }

            #endregion

            // 3 Parçadan küçük tüm değerlendirmeler için 1. metodu kullan
            if (this.Items.Count < 3)
            {
                useFirstMethod = true;
            }

            //List<BenchPMLExecutionList> usedExecutions = new List<BenchPMLExecutionList>();

            Dictionary<int, PmlOperationResultList> referenceSetResults = new Dictionary<int, PmlOperationResultList>();

            PmlOperationResultList result = null;

            PmlOperationResultList tmpResult = null;

            if (useFirstMethod)
            {
                // Method 1
                CalculationMode = BenchProcessor.CalculationMode.SCATTER_METHOD_1;

                int count = 0;

                while (count < SCATTER_METHOD_1_STEP_MAX_ITERATION)
                {
                    for (int i = 0; i < referenceSet.Count; i++)
                    {
                        int bestTime = Int32.MaxValue;

                        List<Item> itemList = referenceSet[i];

                        if (referenceSetResults.ContainsKey(i))
                        {
                            bestTime = referenceSetResults[i].TotalTime;

                            //tmpResult = CalculateThreadJob(null, itemList, usedExecutions, bestTime);
                            tmpResult = CalculateThreadJob(null, itemList, null, bestTime);

                            referenceSetResults[i] = tmpResult;
                        }
                        else
                        {
                            //tmpResult = CalculateThreadJob(null, itemList, usedExecutions, bestTime);

                            tmpResult = CalculateThreadJob(null, itemList, null, bestTime);

                            referenceSetResults.Add(i, tmpResult);
                        }

                        if (tmpResult != null)
                        {
                            // Kullanılmış olan işlem listesine ekle
                            //usedExecutions.Add(tmpResult.ExecutionList);

                            if (result == null)
                            {
                                result = tmpResult;
                            }
                            else
                            {
                                if (tmpResult.TotalTime < result.TotalTime)
                                {
                                    result = tmpResult;

                                    count = 0;
                                }
                            }
                        }
                    }

                    count++;

                    if (this.onScatterStep != null)
                    {
                        this.onScatterStep((100 * count) / SCATTER_METHOD_1_STEP_MAX_ITERATION);
                    }
                }
            }
            else
            {
                // Method 2
                CalculationMode = BenchProcessor.CalculationMode.SCATTER_METHOD_1;

                int count = 0;

                while (count < SCATTER_METHOD_2_STEP_MAX_ITERATION)
                {
                    // Diversification
                    List<PmlOperationResultList> timeSpanList = new List<PmlOperationResultList>();

                    for (int i = 0; i < referenceSet.Count; i++)
                    {
                        List<Item> itemList = referenceSet[i];

                        //tmpResult = CalculateThreadJob(null, itemList, usedExecutions, Int32.MaxValue);

                        tmpResult = CalculateThreadJob(null, itemList, null, Int32.MaxValue);

                        timeSpanList.Add(tmpResult);

                        if (tmpResult != null)
                        {
                            // Kullanılmış olan işlem listesine ekle
                            //usedExecutions.Add(tmpResult.ExecutionList);

                            if (result == null)
                            {
                                result = tmpResult;
                            }
                            else
                            {
                                if (tmpResult.TotalTime < result.TotalTime)
                                {
                                    result = tmpResult;

                                    count = 0;
                                }
                            }
                        }
                    }

                    // Improvement + subset generation + solution combination
                    for (int i = 0; i < timeSpanList.Count; i++)
                    {
                        PmlOperationResultList pmlResult = timeSpanList[i];

                        PmlOperationResultList subsetResult1 = CalculateThreadJob(null, new List<Item> { pmlResult.Items[0], pmlResult.Items[1] }, null, Int32.MaxValue);

                        PmlOperationResultList subsetResult2 = CalculateThreadJob(null, new List<Item> { pmlResult.Items[1], pmlResult.Items[0] }, null, Int32.MaxValue);

                        List<Item> items = new List<Item>();

                        if (subsetResult1.TotalTime < subsetResult2.TotalTime)
                        {
                            items = subsetResult1.Items;
                        }
                        else
                        {
                            items = subsetResult2.Items;
                        }

                        // Generate and combine subsets
                        for (int k = 2; k < this.Items.Count; k++)
                        {
                            PmlOperationResultList bestResult = null;

                            for (int t = 0; t < items.Count + 1; t++)
                            {
                                List<Item> tmpItems = new List<Item>(items);

                                if (t == items.Count)
                                {
                                    tmpItems.Add(pmlResult.Items[k]);
                                }
                                else
                                {
                                    tmpItems.Insert(t, pmlResult.Items[k]);
                                }

                                PmlOperationResultList subsetResult = CalculateThreadJob(null, tmpItems, null, Int32.MaxValue);

                                if (bestResult == null)
                                {
                                    bestResult = subsetResult;
                                }
                                else
                                {
                                    if (subsetResult.TotalTime < bestResult.TotalTime)
                                    {
                                        bestResult = subsetResult;
                                    }
                                }
                            }

                            items = bestResult.Items;
                        }

                        referenceSet[i] = items;
                    }

                    count++;

                    if (this.onScatterStep != null)
                    {
                        this.onScatterStep((100 * count) / SCATTER_METHOD_2_STEP_MAX_ITERATION);
                    }
                }
            }

            if (this.onScatterEnd != null)
            {
                onScatterEnd(new ScatterEndEventArgs(result, stopWatch.ElapsedMilliseconds));
            }

            stopWatch.Stop();
        }

        /// <summary>
        /// Operasyonların öncelik ve eşzamanlı yapılabilme durumları da göz önünde bulundurularak
        /// LPT algoritması ile yeni bir operasyon schedule listesi alır
        /// </summary>
        /// <returns>LPT listesi</returns>
        private List<Item> getLPTSeedResult()
        {
            // Her zaman mevcut listenin bir kopyasını oluştur ve bu kopya üzerinde işlemleri yap
            List<Item> clone = new List<Item>(this.Items);

            foreach (Item item in clone)
            {
                List<Operation> operationList = item.Operations.OrderByDescending(o => o.Time).ToList();

                item.Operations = operationList;
            }

            clone = clone.OrderByDescending(p => p.TotalTime).ToList();

            return clone;
        }

        /// <summary>
        /// Operasyonların öncelik ve eşzamanlı yapılabilme durumları da göz önünde bulundurularak
        /// SPT algoritması ile yeni bir operasyon schedule listesi alır
        /// </summary>
        /// <returns>SPT listesi</returns>
        private List<Item> getSPTSeedResult()
        {
            // Her zaman mevcut listenin bir kopyasını oluştur ve bu kopya üzerinde işlemleri yap
            List<Item> clone = new List<Item>(this.Items);

            foreach (Item item in clone)
            {
                List<Operation> operationList = item.Operations.OrderBy(o => o.Time).ToList();

                item.Operations = operationList;
            }

            clone = clone.OrderBy(p => p.TotalTime).ToList();

            return clone;
        }

        /// <summary>
        /// Operasyonların öncelik ve eşzamanlı yapılabilme durumları da göz önünde bulundurularak
        /// rastgele yeni bir operasyon schedule listesi alır
        /// </summary>
        /// <returns>LPT listesi</returns>
        private List<Item> getRandomSeedResult()
        {
            // Her zaman mevcut listenin bir kopyasını oluştur ve bu kopya üzerinde işlemleri yap
            List<Item> clone = new List<Item>(this.Items);

            List<Operation> result = new List<Operation>();

            // Mevcut operasyon listesini karıştır
            foreach (Item item in clone)
            {
                for (int i = 0; i < RANDOM_SEED_THRESHOLD; i++)
                {
                    int k = random.Next(0, item.Operations.Count);
                    int t = random.Next(0, item.Operations.Count);

                    // Operasyonları yer değiştir
                    Operation tmp = item.Operations[k];
                    item.Operations[k] = item.Operations[t];
                    item.Operations[t] = tmp;
                }
            }

            // Parçaları karıştır
            for (int i = 0; i < RANDOM_SEED_THRESHOLD; i++)
            {
                int k = random.Next(0, clone.Count);
                int t = random.Next(0, clone.Count);

                // Parçaları yer değiştir
                Item tmp = clone[k];
                clone[k] = clone[t];
                clone[t] = tmp;
            }

            return clone;
        }
    }
}
