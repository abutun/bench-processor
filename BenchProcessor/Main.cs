using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BenchProcessor
{
    public partial class Main : Form
    {
        private Bench bench = null;

        private Items itemsDialog = null;

        private BenchInformation informationDialog = null;

        private Output outputDialog = null;

        private string benchFilePath = null;

        private string benchItemsFilePath = null;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            changeBenchStatus(false);
        }

        private void bench_onSimulationEnd(SimulationEndEventArgs eventArgs)
        {
            var action = new Action(() =>
            {
                this.simulateToolStripMenuItem.Enabled = true;
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

        private void bench_onSimulationBegin()
        {
            var action = new Action(() =>
            {
                this.simulateToolStripMenuItem.Enabled = false;
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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadBench()
        {
            if (this.benchFilePath == null)
            {
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.benchFilePath = dialog.FileName;

                    this.bench = new Bench(Path.GetFileName(this.benchFilePath));

                    this.bench.onSimulationBegin += bench_onSimulationBegin;

                    this.bench.onSimulationEnd += bench_onSimulationEnd;

                    this.addBulkItemToolStripMenuItem.Enabled = true;
                }
            }

            if (this.benchFilePath != null)
            {
                if (bench.LoadFromFile(this.benchFilePath))
                {
                    showBenchInformationDialog();

                    changeBenchStatus(true);
                }
                else
                {
                    MessageBox.Show("Hata oluştu! Hata sebebi: " + bench.ErrorMessage);

                    changeBenchStatus(false);
                }
            }
        }

        private void closeBenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.bench.SimulationRunning)
            {
                this.bench.CancelSimulation();
            }

            changeBenchStatus(false);

            closeAllOpenForms();
        }

        private void changeBenchStatus(bool open)
        {
            this.readBenchFromFileToolStripMenuItem.Enabled = !open;

            this.benchItemsToolStripMenuItem.Enabled = this.closeBenchToolStripMenuItem.Enabled = this.simulateToolStripMenuItem.Enabled = 
            this.benchInformationToolStripMenuItem.Enabled = this.benchItemsToolStripMenuItem.Enabled = this.reloadBenchItemsToolStripMenuItem.Enabled =
            this.reloadBenchToolStripMenuItem.Enabled = this.normalCalculationToolStripMenuItem.Enabled = this.scatterSearchToolStripMenuItem.Enabled = open;

            if (!open)
            {
                closeBench();
            }
        }

        private void reloadBench()
        {
            loadBench();
        }

        private void reloadBenchItems()
        {
            loadBenchItems();
        }

        private void closeBench()
        {
            this.bench = null;

            this.itemsDialog = null;

            this.informationDialog = null;

            this.benchFilePath = null;

            this.benchItemsFilePath = null;

            this.addBulkItemToolStripMenuItem.Enabled = false;
        }

        private void addBulkItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadBenchItems();
        }

        private void loadBenchItems()
        {
            if (this.bench != null)
            {
                if (this.benchItemsFilePath == null || !this.bench.HasItems)
                {
                    OpenFileDialog dialog = new OpenFileDialog();

                    dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.benchItemsFilePath = dialog.FileName;
                    }
                }

                if (this.benchItemsFilePath != null)
                {
                    if (bench.LoadItemsFromFile(this.benchItemsFilePath))
                    {
                        if (bench.HasItems)
                        {
                            this.addBulkItemToolStripMenuItem.Enabled = false;

                            this.removeItemsToolStripMenuItem.Enabled = true;

                            // first reload bench
                            reloadBench();

                            // then show items dialog
                            showBenchItemsDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Hata oluştu! Hata sebebi: " + bench.ErrorMessage);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir tezgah tanımı yapınız!");
            }
        }

        private void showBenchInformationDialog()
        {
            if (this.bench != null)
            {
                bool focus = false;

                if (this.informationDialog == null)
                {
                    this.informationDialog = new BenchInformation(bench);
                }
                else
                {
                    if (IsFormOpen(this.informationDialog.Text))
                    {
                        focus = true;
                    }
                    else
                    {
                        this.informationDialog = new BenchInformation(bench);
                    }
                }

                this.informationDialog.MdiParent = this;

                this.informationDialog.reload();

                if (!focus)
                {
                    this.informationDialog.WindowState = FormWindowState.Normal;

                    this.informationDialog.Show();
                }

                this.informationDialog.Focus();
            }
            else
            {
                MessageBox.Show("Lütfen önce bir tezgah tanımı yapınız!");
            }
        }

        private void showBenchOutputDialog()
        {
            if (this.bench != null)
            {
                bool focus = false;

                if (this.outputDialog == null)
                {
                    this.outputDialog = new Output(bench);
                }
                else
                {
                    if (IsFormOpen(this.outputDialog.Text))
                    {
                        focus = true;
                    }
                    else
                    {
                        this.outputDialog = new Output(bench);
                    }
                }

                this.outputDialog.MdiParent = this;

                if (!focus)
                {
                    this.outputDialog.WindowState = FormWindowState.Normal;

                    this.outputDialog.Show();
                }

                this.outputDialog.Focus();
            }
            else
            {
                MessageBox.Show("Lütfen önce bir tezgah tanımı yapınız!");
            }
        }

        private void showBenchItemsDialog()
        {
            if (this.bench != null)
            {
                if (this.bench.HasItems)
                {
                    bool focus = false;

                    if (this.itemsDialog == null)
                    {
                        this.itemsDialog = new Items(bench);
                    }
                    else
                    {
                        if (IsFormOpen(this.itemsDialog.Text))
                        {
                            focus = true;
                        }
                        else
                        {
                            this.itemsDialog = new Items(bench);
                        }
                    }

                    this.itemsDialog.MdiParent = this;

                    this.itemsDialog.reload();

                    if (!focus)
                    {
                        this.itemsDialog.WindowState = FormWindowState.Normal;

                        this.itemsDialog.Show();
                    }

                    this.Refresh();

                    this.itemsDialog.Focus();
                }
                else
                {
                    MessageBox.Show("Parça tanımları yapılmamış! Lütfen parça ekleyiniz!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen önce bir tezgah tanımı yapınız!");
            }
        }

        private bool IsFormOpen(string name)
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Text == name)
                {
                    return true;
                }
            }

            return false;
        }

        private void closeAllOpenForms(){

            FormCollection fc = Application.OpenForms;

            for (int i = 0; i < fc.Count; i++)
            {
                Form form = fc[i];

                if (form.Text != this.Text)
                {
                    form.Close();

                    i--;
                }
            }
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBenchItemsDialog();
        }

        private void benchInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBenchInformationDialog();
        }

        private void reloadBenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reloadBench();
        }

        private void reloadBenchItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reloadBenchItems();
        }

        private void readBenchFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadBench();
        }

        private void showOutputToolStripMenuItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBenchOutputDialog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.bench != null)
            {
                if (this.bench.SimulationRunning)
                {
                    this.bench.CancelSimulation();
                }
            }
        }

        /// <summary>
        /// Tezgahı simüle eder
        /// </summary>
        private void SimulateBench()
        {
            if (this.bench != null)
            {
                // Önceden başlatılmış bir simülasyon varsa durdur
                if (this.bench.SimulationRunning)
                {
                    this.bench.CancelSimulation();
                }

                showBenchOutputDialog();

                if (this.outputDialog != null)
                {
                    this.outputDialog.reload();
                }

                showBenchItemsDialog();

                if (this.itemsDialog != null)
                {
                    this.itemsDialog.reload();
                }

                this.bench.Simulate(null, null);
            }
        }

        private void Calculate(bool useScatterSearch, bool useFirstMethod)
        {
            if (this.bench != null)
            {
                // Önceden başlatılmış bir simülasyon varsa durdur
                if (this.bench.SimulationRunning)
                {
                    this.bench.CancelSimulation();
                }

                showBenchOutputDialog();

                if (this.outputDialog != null)
                {
                    this.outputDialog.reload();
                }

                showBenchItemsDialog();

                if (this.itemsDialog != null)
                {
                    this.itemsDialog.reload();
                }

                if (!useScatterSearch)
                {
                    this.bench.Calculate(null, null, null);
                }
                else
                {
                    this.bench.ScatterSearch(useFirstMethod);
                }
            }
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimulateBench();
        }

        private void removeItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.bench.Items.Clear();

            this.removeItemsToolStripMenuItem.Enabled = false;

            this.addBulkItemToolStripMenuItem.Enabled = true;

            if (this.itemsDialog != null)
            {
                this.itemsDialog.Close();
            }

            reloadBench();
        }

        private void normalCalculationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Calculate(false, false);
        }

        private void metot1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Calculate(true, true);
        }

        private void metot2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Calculate(true, false);
        }
    }
}
