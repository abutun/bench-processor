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
    public partial class BenchInformation : Form
    {
        private Bench bench = null;

        public BenchInformation(Bench bench)
        {
            this.bench = bench;

            InitializeComponent();
        }

        private void BenchInformation_Load(object sender, EventArgs e)
        {
            if (this.bench != null)
            {
                this.Text += " - " + this.bench.Name;

                this.richTextBox1.Text = this.bench.GetInformation();
            }
            else
            {
                this.richTextBox1.Text = "Tezgah bilgisi bulunamadı!";
            }
        }

        public void reload()
        {
            if (this.bench != null)
            {
                this.richTextBox1.Text = "";

                this.richTextBox1.Text = this.bench.GetInformation();
            }
            else
            {
                this.richTextBox1.Text = "Tezgah bilgisi bulunamadı!";
            }
        }
    }
}
