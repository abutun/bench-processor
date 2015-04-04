using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BenchProcessor
{
    public partial class Output : Form
    {
        Bench bench = null;

        private bool closing = false;

        private bool scatter = false;

        public Output(Bench bench)
        {
            this.bench = bench;

            InitializeComponent();
        }

        private void Output_Load(object sender, EventArgs e)
        {
            if (this.bench != null)
            {
                this.bench.onNewMessage += bench_onNewMessage;

                this.bench.onSimulationEnd += bench_onSimulationEnd;
                this.bench.onSimulationBegin += bench_onSimulationBegin;
                this.bench.onSimulationStep += bench_onSimulationStep;

                this.bench.onCalculationBegin += bench_onCalculationBegin;
                this.bench.onCalculationEnd += bench_onCalculationEnd;
                this.bench.onCalculationStep += bench_onCalculationStep;

                this.bench.onScatterBegin += bench_onScatterBegin;
                this.bench.onScatterStep += bench_onScatterStep;
                this.bench.onScatterEnd += bench_onScatterEnd;
            }
        }

        void bench_onScatterEnd(ScatterEndEventArgs eventArgs)
        {
            if (this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_1 || this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_2)
            {
                try
                {
                    var action = new Action(() =>
                    {
                        this.outputTextBox.Text += Environment.NewLine + "Scatter Search başarı ile tamamlandı... Hesaplama süresi: " + eventArgs.Duration + " ms." + Environment.NewLine + Environment.NewLine;

                        if (eventArgs.Result == null || eventArgs.Result.Count == 0)
                        {
                            this.outputTextBox.Text += "Olası bir çözüm bulunamadı!" + Environment.NewLine;
                        }
                        else
                        {
                            this.outputTextBox.Text += "Çözüm: " + eventArgs.Result.TotalTime + " saniye" + Environment.NewLine + Environment.NewLine;
                            this.outputTextBox.Text += eventArgs.Result.Information;

                            /*
                            if(MessageBox.Show("Bulunan çözümü simüle etmek ister misiniz?", "Simülasyon", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                            {
                                this.simulationProgress.Value = 0;

                                this.bench.Simulate(eventArgs.ExecutionList);              
                            }*/
                        }

                        this.outputTextBox.SelectionStart = this.outputTextBox.Text.Length;
                        this.outputTextBox.ScrollToCaret();
                    });

                    if (this.InvokeRequired)
                    {
                        this.Invoke(action);
                    }
                    else
                    {
                        action();
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }
        }

        void bench_onScatterStep(double step)
        {
            if (this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_1 || this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_2)
            {
                try
                {
                    var addAction = new Action<double>((item) =>
                    {
                        this.progress.Value = (int)item;

                        this.progress.Refresh();
                    });

                    if (this.progress.InvokeRequired)
                    {
                        this.progress.Invoke(addAction, new Object[] { step });
                    }
                    else
                    {
                        addAction(step);
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }
        }

        void bench_onScatterBegin()
        {
            if (this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_1 || this.bench.CalculationMode == CalculationMode.SCATTER_METHOD_2)
            {
                var action = new Action(() =>
                {
                    this.outputTextBox.Text += "Scatter Search başladı... Lütfen bekleyiniz..." + Environment.NewLine + Environment.NewLine;
                });

                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        void bench_onCalculationStep(double step)
        {
            if (this.bench.CalculationMode == CalculationMode.NORMAL)
            {
                try
                {
                    var addAction = new Action<double>((item) =>
                    {
                        this.progress.Value = (int)item;

                        this.progress.Refresh();
                    });

                    if (this.progress.InvokeRequired)
                    {
                        this.progress.Invoke(addAction, new Object[] { step });
                    }
                    else
                    {
                        addAction(step);
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }
        }

        void bench_onCalculationEnd(CalculationEndEventArgs eventArgs)
        {
            if (this.bench.CalculationMode == CalculationMode.NORMAL)
            {
                try
                {
                    var action = new Action(() =>
                    {
                        this.outputTextBox.Text += "Hesaplama başarı ile tamamlandı... Hesaplama süresi: " + eventArgs.Duration + " ms." + Environment.NewLine + Environment.NewLine;

                        if (eventArgs.Result == null || eventArgs.Result.Count == 0)
                        {
                            this.outputTextBox.Text += "Olası bir çözüm bulunamadı!" + Environment.NewLine;
                        }
                        else
                        {
                            this.outputTextBox.Text += eventArgs.Possibilities + " ihtimal içerisinden bulunan en iyi çözüm: " + Environment.NewLine + Environment.NewLine;
                            this.outputTextBox.Text += "Çözüm: " + eventArgs.Result.TotalTime + " saniye" + Environment.NewLine + Environment.NewLine;
                            this.outputTextBox.Text += eventArgs.Result.Information;

                            /*
                            if(MessageBox.Show("Bulunan çözümü simüle etmek ister misiniz?", "Simülasyon", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                            {
                                this.simulationProgress.Value = 0;

                                this.bench.Simulate(eventArgs.ExecutionList);              
                            }*/
                        }

                        this.outputTextBox.SelectionStart = this.outputTextBox.Text.Length;
                        this.outputTextBox.ScrollToCaret();
                    });

                    if (this.InvokeRequired)
                    {
                        this.Invoke(action);
                    }
                    else
                    {
                        action();
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }
        }

        void bench_onCalculationBegin()
        {
            if (this.bench.CalculationMode == CalculationMode.NORMAL)
            {
                var action = new Action(() =>
                {
                    this.outputTextBox.Text += "Hesaplama başladı... Lütfen bekleyiniz..." + Environment.NewLine + Environment.NewLine;
                });

                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        void bench_onSimulationStep(double step)
        {
            try
            {
                var addAction = new Action<double>((item) =>
                {
                    this.progress.Value = (int)item;

                    this.progress.Refresh();
                });

                if (this.progress.InvokeRequired)
                {
                    this.progress.Invoke(addAction, new Object[] { step });
                }
                else
                {
                    addAction(step);
                }
            }
            catch (Exception)
            {
                // NOP
            }
        }

        private void bench_onSimulationEnd(SimulationEndEventArgs eventArgs)
        {
            try
            {
                var action = new Action(() =>
                {
                    this.outputTextBox.Text += "Simülasyon başarı ile tamamlandı... Simülasyon süresi: " + eventArgs.Duration + " ms." + Environment.NewLine + Environment.NewLine;

                    if (eventArgs.Result == null || eventArgs.Result.Count == 0)
                    {
                        this.outputTextBox.Text += "Olası bir çözüm bulunamadı!" + Environment.NewLine;
                    }
                    else
                    {
                        this.outputTextBox.Text += eventArgs.Possibilities + " ihtimal içerisinden bulunan en iyi çözüm: " + Environment.NewLine + Environment.NewLine;
                        this.outputTextBox.Text += "Çözüm: " + Environment.NewLine + Environment.NewLine;
                        this.outputTextBox.Text += "Süre: " + eventArgs.Result.TotalTime + " saniye.";
                    }

                    this.outputTextBox.SelectionStart = this.outputTextBox.Text.Length;
                    this.outputTextBox.ScrollToCaret();
                });

                if (this.InvokeRequired)
                {
                    this.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception)
            {
                // NOP
            }
        }

        private void bench_onSimulationBegin()
        {
            var action = new Action(() =>
                {
                    this.outputTextBox.Text += Environment.NewLine + Environment.NewLine + "Simülasyon başladı... Lütfen bekleyiniz..." + Environment.NewLine + Environment.NewLine;
                });

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void bench_onNewMessage(string message)
        {
            if (!closing)
            {
                try
                {
                    var addAction = new Action<String>((item) =>
                    {
                        this.outputTextBox.Text += item + Environment.NewLine;

                        this.outputTextBox.Refresh();
                    });

                    if (this.outputTextBox.InvokeRequired)
                    {
                        this.outputTextBox.Invoke(addAction, new Object[] { message });
                    }
                    else
                    {
                        addAction(message);
                    }
                }
                catch (Exception)
                {
                    // NOP
                }
            }
        }

        public void reload()
        {
            this.outputTextBox.Text = "";
            this.progress.Value = 0;
        }

        private void Output_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.closing = true;
        }
    }
}
