using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchProcessor
{
    public class PmlOperationResultList : SortedList<PartMachiningLocation, List<PmlOperationResult>>
    {
        public PmlOperationResultList()
        {
            OperationCount = 0;

            TotalTime = 0;
        }

        private void AddOperationResult(PartMachiningLocation pml, PmlOperationResult item)
        {
            if (this.ContainsKey(pml))
            {
                List<PmlOperationResult> result = this[pml];

                result.Add(item);

                this[pml] = result;
            }
            else
            {
                List<PmlOperationResult> result = new List<PmlOperationResult>();

                result.Add(item);
   
                base.Add(pml, result);
            }

            if (item.Finish > TotalTime)
            {
                TotalTime = item.Finish;
            }

            OperationCount += item.Count();
        }

        public new void Add(PartMachiningLocation pml, List<PmlOperationResult> result)
        {
            foreach (PmlOperationResult p in result)
            {
                AddOperationResult(pml, p);
            }
        }

        public void Sort()
        {
            foreach (PartMachiningLocation pml in this.Keys)
            {
                this[pml].Sort();
            }
        }

        public int TotalTime { get; internal set; }

        public int OperationCount { get; internal set; }

        public BenchPmlExecutionList ExecutionList { get; set; }

        public List<Item> Items
        {
            get
            {
                List<Item> result = new List<Item>();

                foreach (List<PmlOperationResult> pmlResultList in this.Values)
                {
                    foreach (PmlOperationResult pmlResult in pmlResultList)
                    {
                        if (!result.Contains(pmlResult.Item))
                        {
                            result.Add(pmlResult.Item);
                        }
                    }
                }

                return result;
            }
        }

        public String Information
        {
            get
            {
                string info = "";

                foreach (PartMachiningLocation pml in this.Keys)
                {
                    info += pml.Name + Environment.NewLine;

                    List<PmlOperationResult> pmlResultList = this[pml];

                    int i = 1;
                    foreach (PmlOperationResult pmlOperation in pmlResultList)
                    {
                        info += Environment.NewLine + "\t" + "İşlem : " + i + ", Süre: " + pmlOperation.Time + " sn" + ", Başlangıç: " + pmlOperation.Start + ", Bitiş: " + pmlOperation.Finish;

                        if (pmlOperation.Count > 1)
                        {
                            info += " (Eşzamanlı)";
                        }

                        info += Environment.NewLine;

                        foreach (OperationResult operationResult in pmlOperation)
                        {
                            info += "\t\t" + "Operasyon: " + operationResult.Operation.Name + Environment.NewLine;
                            info += "\t\t" + "Makine Ünitesi: " + Bench.GetMachineUnit(operationResult.MachineUnitNo).Name + Environment.NewLine;
                            info += "\t\t" + "MU Bekleme Süresi: " + operationResult.WaitTimeForAvailableUnit + Environment.NewLine;
                            info += "\t\t" + "Takım Değişim Süresi: " + operationResult.ToolChangeTime + Environment.NewLine;
                            info += "\t\t" + "Başlangıç: " + operationResult.Start + Environment.NewLine;
                            info += "\t\t" + "Bitiş: " + operationResult.Finish + Environment.NewLine + Environment.NewLine;
                        }

                        i++;
                    }

                    info += Environment.NewLine;
                }

                return info;
            }
        }

        public void Optimize()
        {
            // Zaten sıralanmış olarak gelmesi gerekiyor ama yine de -just in case-
            Sort();

            // Herbir PML için takım değişim sürelerini, başlangıç ve bitiş sürelerini hesapla
            foreach(PartMachiningLocation pml in this.Keys)
            {
                List<PmlOperationResult> pmlResultList = this[pml];

                Dictionary<int, Operation> unitOperationMap = new Dictionary<int, Operation>();

                int time = 0;

                foreach (PmlOperationResult pmlResult in pmlResultList)
                {
                    OperationResult firstResult = pmlResult[0];

                    bool toolChangeNeeded = false;

                    // Eşzamanlı operasyon yapılmış
                    if (pmlResult.Count > 1)
                    {
                        OperationResult secondResult = pmlResult[1];

                        int operationTime = firstResult.Operation.Time;

                        if (secondResult.Operation.Time > operationTime)
                        {
                            operationTime = secondResult.Operation.Time;
                        }

                        operationTime = (int)(operationTime * PartMachiningLocation.SIMULTANEOUS_OPERATION_TIME_RATIO);

                        // Takım değişimi gerekli olmuş mu?
                        if (unitOperationMap.ContainsKey(firstResult.MachineUnitNo))
                        {
                            Operation operation = unitOperationMap[firstResult.MachineUnitNo];

                            if (operation.ToolNo != firstResult.Operation.ToolNo)
                            {
                                toolChangeNeeded = true;
                            }
                        }

                        // Takım değişimi gerekli olmuş mu?
                        if (unitOperationMap.ContainsKey(secondResult.MachineUnitNo) && !toolChangeNeeded)
                        {
                            Operation operation = unitOperationMap[secondResult.MachineUnitNo];

                            if (operation.ToolNo != secondResult.Operation.ToolNo)
                            {
                                toolChangeNeeded = true;
                            }
                        }

                        ReCalculateResult(pmlResult, firstResult, time, operationTime, toolChangeNeeded);

                        ReCalculateResult(pmlResult, secondResult, time, operationTime, toolChangeNeeded);

                        unitOperationMap.Clear();

                        unitOperationMap.Add(firstResult.MachineUnitNo, firstResult.Operation);

                        unitOperationMap.Add(secondResult.MachineUnitNo, secondResult.Operation);
                    }
                    else
                    {
                        // Takım değişimi gerekli olmuş mu?
                        if (unitOperationMap.ContainsKey(firstResult.MachineUnitNo))
                        {
                            Operation operation = unitOperationMap[firstResult.MachineUnitNo];

                            if (operation.ToolNo != firstResult.Operation.ToolNo)
                            {
                                toolChangeNeeded = true;
                            }
                        }

                        ReCalculateResult(pmlResult, firstResult, time, firstResult.Operation.Time, toolChangeNeeded);

                        unitOperationMap.Clear();

                        unitOperationMap.Add(firstResult.MachineUnitNo, firstResult.Operation);
                    }

                    time = pmlResult.Finish;
                }

                if (time > this.TotalTime)
                {
                    this.TotalTime = time;
                }
            }

            // Eğer eşzamanlı PML'ler kullanılmışsa, aynı MU'ları kullanan operasyonların çakışmalarını kontrol et
            if (Count > 1)
            {
                bool haveConflicts = false;

                do
                {
                    haveConflicts = false;

                    foreach (PartMachiningLocation pml in this.Keys)
                    {
                        if (FindAndFixConflicts(pml))
                        {
                            haveConflicts = true;

                            continue;
                        }
                    }
                }
                while (haveConflicts);
            }
        }

        private bool FindAndFixConflicts(PartMachiningLocation sourcePml)
        {
            List<PmlOperationResult> sourceList = this[sourcePml];

            bool haveConflict = false;

            foreach (PartMachiningLocation pml in this.Keys)
            {
                if (pml.No != sourcePml.No)
                {
                    List<PmlOperationResult> targetList = this[pml];

                    foreach (PmlOperationResult pmlResult in sourceList)
                    {
                        int waitTime = 0;

                        int start = 0;

                        foreach (OperationResult operationResult in pmlResult)
                        {
                            waitTime = GetWaitTime(operationResult, targetList);

                            if (waitTime > 0)
                            {
                                haveConflict = true;

                                start = operationResult.Start;

                                break;
                            }
                        }

                        if (haveConflict)
                        {
                            foreach (OperationResult operationResult in pmlResult)
                            {
                                operationResult.WaitTimeForAvailableUnit = waitTime;
                            }

                            Shift(sourceList, start, waitTime);

                            break;
                        }
                    }

                    if (haveConflict)
                    {
                        break;
                    }
                }
            }

            return haveConflict;
        }

        private void Shift(List<PmlOperationResult> sourceList, int start, int waitTime)
        {
            bool flag = false;

            foreach (PmlOperationResult pmlResult in sourceList)
            {
                bool shifted = false;

                foreach (OperationResult operationResult in pmlResult)
                {
                    if (operationResult.Start >= start)
                    {
                        operationResult.Start += waitTime;
                        operationResult.Finish += waitTime;

                        shifted = true;
                    }
                }

                if (shifted)
                {
                    if (flag)
                    {
                        pmlResult.Start += waitTime;
                    }

                    pmlResult.Finish += waitTime;

                    if (pmlResult.Finish > this.TotalTime)
                    {
                        this.TotalTime = pmlResult.Finish;
                    }

                    flag = true;
                }
            }
        }

        private int GetWaitTime(OperationResult result, List<PmlOperationResult> targetList)
        {
            foreach (PmlOperationResult pmlResult in targetList)
            {
                foreach (OperationResult operationResult in pmlResult)
                {
                    if (result.MachineUnitNo == operationResult.MachineUnitNo)
                    {
                        if (result.Start >= operationResult.Start && result.Start < operationResult.Finish)
                        {
                            return operationResult.Finish - operationResult.Start;
                        }
                    }
                }
            }

            return 0;
        }

        private void ReCalculateResult(PmlOperationResult pmlResult, OperationResult operationResult, int start, int operationTime, bool toolChangeNeeded)
        {
            operationResult.Start = start;
            operationResult.Time = operationTime;
            operationResult.ToolChangeTime = toolChangeNeeded ? MachineUnit.TOOL_CHANGE_TIME : 0;
            operationResult.Finish = operationResult.Start + operationResult.Time + operationResult.ToolChangeTime;

            pmlResult.Start = start;
            pmlResult.Time = operationResult.Finish - operationResult.Start;
            pmlResult.Finish = pmlResult.Start + pmlResult.Time;
        }
    }
}
