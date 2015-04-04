namespace BenchProcessor
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.dosyaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.düzenleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tezhagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readBenchFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.benchItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.benchInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadBenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeBenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parçaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBulkItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadBenchItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hesaplamaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalCalculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scatterSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metot1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metot2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simülastonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yardımToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dosyaToolStripMenuItem,
            this.düzenleToolStripMenuItem,
            this.tezhagToolStripMenuItem,
            this.parçaToolStripMenuItem,
            this.hesaplamaToolStripMenuItem,
            this.simülastonToolStripMenuItem,
            this.yardımToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1006, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // dosyaToolStripMenuItem
            // 
            this.dosyaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.dosyaToolStripMenuItem.Name = "dosyaToolStripMenuItem";
            this.dosyaToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.dosyaToolStripMenuItem.Text = "Dosya";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Enabled = false;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.openToolStripMenuItem.Text = "Aç";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveToolStripMenuItem.Text = "Kaydet";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveAsToolStripMenuItem.Text = "Farklı Kaydet";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.closeToolStripMenuItem.Text = "Kapat";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // düzenleToolStripMenuItem
            // 
            this.düzenleToolStripMenuItem.Enabled = false;
            this.düzenleToolStripMenuItem.Name = "düzenleToolStripMenuItem";
            this.düzenleToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.düzenleToolStripMenuItem.Text = "Düzenle";
            // 
            // tezhagToolStripMenuItem
            // 
            this.tezhagToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readBenchFromFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.benchItemsToolStripMenuItem,
            this.benchInformationToolStripMenuItem,
            this.toolStripSeparator2,
            this.reloadBenchToolStripMenuItem,
            this.toolStripSeparator3,
            this.closeBenchToolStripMenuItem});
            this.tezhagToolStripMenuItem.Name = "tezhagToolStripMenuItem";
            this.tezhagToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.tezhagToolStripMenuItem.Text = "Tezgah";
            // 
            // readBenchFromFileToolStripMenuItem
            // 
            this.readBenchFromFileToolStripMenuItem.Name = "readBenchFromFileToolStripMenuItem";
            this.readBenchFromFileToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.readBenchFromFileToolStripMenuItem.Text = "Tezgah Bilgilerini Al";
            this.readBenchFromFileToolStripMenuItem.Click += new System.EventHandler(this.readBenchFromFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
            // 
            // benchItemsToolStripMenuItem
            // 
            this.benchItemsToolStripMenuItem.Name = "benchItemsToolStripMenuItem";
            this.benchItemsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.benchItemsToolStripMenuItem.Text = "Tezgah Parçalarını Göster";
            this.benchItemsToolStripMenuItem.Click += new System.EventHandler(this.itemsToolStripMenuItem_Click);
            // 
            // benchInformationToolStripMenuItem
            // 
            this.benchInformationToolStripMenuItem.Name = "benchInformationToolStripMenuItem";
            this.benchInformationToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.benchInformationToolStripMenuItem.Text = "Tezgah Bilgisini Göster";
            this.benchInformationToolStripMenuItem.Click += new System.EventHandler(this.benchInformationToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // reloadBenchToolStripMenuItem
            // 
            this.reloadBenchToolStripMenuItem.Name = "reloadBenchToolStripMenuItem";
            this.reloadBenchToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.reloadBenchToolStripMenuItem.Text = "Tezgah Bilgilerini Yenile";
            this.reloadBenchToolStripMenuItem.Click += new System.EventHandler(this.reloadBenchToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(204, 6);
            // 
            // closeBenchToolStripMenuItem
            // 
            this.closeBenchToolStripMenuItem.Name = "closeBenchToolStripMenuItem";
            this.closeBenchToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.closeBenchToolStripMenuItem.Text = "Tezgahı Kapat";
            this.closeBenchToolStripMenuItem.Click += new System.EventHandler(this.closeBenchToolStripMenuItem_Click);
            // 
            // parçaToolStripMenuItem
            // 
            this.parçaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addItemToolStripMenuItem,
            this.addBulkItemToolStripMenuItem,
            this.toolStripSeparator4,
            this.reloadBenchItemsToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeItemsToolStripMenuItem});
            this.parçaToolStripMenuItem.Name = "parçaToolStripMenuItem";
            this.parçaToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.parçaToolStripMenuItem.Text = "Parça";
            // 
            // addItemToolStripMenuItem
            // 
            this.addItemToolStripMenuItem.Enabled = false;
            this.addItemToolStripMenuItem.Name = "addItemToolStripMenuItem";
            this.addItemToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addItemToolStripMenuItem.Text = "Yeni Parça Ekle";
            // 
            // addBulkItemToolStripMenuItem
            // 
            this.addBulkItemToolStripMenuItem.Name = "addBulkItemToolStripMenuItem";
            this.addBulkItemToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.addBulkItemToolStripMenuItem.Text = "Parçaları Bilgilerini Al";
            this.addBulkItemToolStripMenuItem.Click += new System.EventHandler(this.addBulkItemToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // reloadBenchItemsToolStripMenuItem
            // 
            this.reloadBenchItemsToolStripMenuItem.Name = "reloadBenchItemsToolStripMenuItem";
            this.reloadBenchItemsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.reloadBenchItemsToolStripMenuItem.Text = "Parça Bilgilerini Yenile";
            this.reloadBenchItemsToolStripMenuItem.Click += new System.EventHandler(this.reloadBenchItemsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(187, 6);
            // 
            // removeItemsToolStripMenuItem
            // 
            this.removeItemsToolStripMenuItem.Enabled = false;
            this.removeItemsToolStripMenuItem.Name = "removeItemsToolStripMenuItem";
            this.removeItemsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.removeItemsToolStripMenuItem.Text = "Parçaları Kaldır";
            this.removeItemsToolStripMenuItem.Click += new System.EventHandler(this.removeItemsToolStripMenuItem_Click);
            // 
            // hesaplamaToolStripMenuItem
            // 
            this.hesaplamaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.normalCalculationToolStripMenuItem,
            this.scatterSearchToolStripMenuItem});
            this.hesaplamaToolStripMenuItem.Name = "hesaplamaToolStripMenuItem";
            this.hesaplamaToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.hesaplamaToolStripMenuItem.Text = "Hesaplama";
            // 
            // normalCalculationToolStripMenuItem
            // 
            this.normalCalculationToolStripMenuItem.Name = "normalCalculationToolStripMenuItem";
            this.normalCalculationToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.normalCalculationToolStripMenuItem.Text = "Normal";
            this.normalCalculationToolStripMenuItem.Click += new System.EventHandler(this.normalCalculationToolStripMenuItem_Click);
            // 
            // scatterSearchToolStripMenuItem
            // 
            this.scatterSearchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.metot1ToolStripMenuItem,
            this.metot2ToolStripMenuItem});
            this.scatterSearchToolStripMenuItem.Name = "scatterSearchToolStripMenuItem";
            this.scatterSearchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.scatterSearchToolStripMenuItem.Text = "Scatter Search";
            // 
            // metot1ToolStripMenuItem
            // 
            this.metot1ToolStripMenuItem.Name = "metot1ToolStripMenuItem";
            this.metot1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.metot1ToolStripMenuItem.Text = "Metot 1";
            this.metot1ToolStripMenuItem.Click += new System.EventHandler(this.metot1ToolStripMenuItem_Click);
            // 
            // metot2ToolStripMenuItem
            // 
            this.metot2ToolStripMenuItem.Name = "metot2ToolStripMenuItem";
            this.metot2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.metot2ToolStripMenuItem.Text = "Metot 2";
            this.metot2ToolStripMenuItem.Click += new System.EventHandler(this.metot2ToolStripMenuItem_Click);
            // 
            // simülastonToolStripMenuItem
            // 
            this.simülastonToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simulateToolStripMenuItem,
            this.showOutputToolStripMenuItem});
            this.simülastonToolStripMenuItem.Enabled = false;
            this.simülastonToolStripMenuItem.Name = "simülastonToolStripMenuItem";
            this.simülastonToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.simülastonToolStripMenuItem.Text = "Simülasyon";
            // 
            // simulateToolStripMenuItem
            // 
            this.simulateToolStripMenuItem.Name = "simulateToolStripMenuItem";
            this.simulateToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.simulateToolStripMenuItem.Text = "Tezgahı Simüle Et";
            this.simulateToolStripMenuItem.Click += new System.EventHandler(this.simulateToolStripMenuItem_Click);
            // 
            // showOutputToolStripMenuItem
            // 
            this.showOutputToolStripMenuItem.Name = "showOutputToolStripMenuItem";
            this.showOutputToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.showOutputToolStripMenuItem.Text = "Durum Penceresini Göster";
            this.showOutputToolStripMenuItem.Click += new System.EventHandler(this.showOutputToolStripMenuItemToolStripMenuItem_Click);
            // 
            // yardımToolStripMenuItem
            // 
            this.yardımToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.yardımToolStripMenuItem.Name = "yardımToolStripMenuItem";
            this.yardımToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.yardımToolStripMenuItem.Text = "Yardım";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.aboutToolStripMenuItem.Text = "Hakkında";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 624);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tezgah Yöneticisi";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem dosyaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem düzenleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yardımToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tezhagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parçaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addBulkItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeBenchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem benchItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem benchInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadBenchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadBenchItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readBenchFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem simülastonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hesaplamaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalCalculationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scatterSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metot1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metot2ToolStripMenuItem;
    }
}

