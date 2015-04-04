namespace BenchProcessor
{
    partial class Items
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
            this.itemsListView = new System.Windows.Forms.ListView();
            this.itemOperationsListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // itemsListView
            // 
            this.itemsListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.itemsListView.Location = new System.Drawing.Point(0, 0);
            this.itemsListView.MultiSelect = false;
            this.itemsListView.Name = "itemsListView";
            this.itemsListView.Size = new System.Drawing.Size(165, 518);
            this.itemsListView.TabIndex = 2;
            this.itemsListView.UseCompatibleStateImageBehavior = false;
            this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemsListBox_SelectedIndexChanged);
            // 
            // itemOperationsListView
            // 
            this.itemOperationsListView.Dock = System.Windows.Forms.DockStyle.Right;
            this.itemOperationsListView.Location = new System.Drawing.Point(171, 0);
            this.itemOperationsListView.MultiSelect = false;
            this.itemOperationsListView.Name = "itemOperationsListView";
            this.itemOperationsListView.Size = new System.Drawing.Size(194, 518);
            this.itemOperationsListView.TabIndex = 3;
            this.itemOperationsListView.UseCompatibleStateImageBehavior = false;
            // 
            // Items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 518);
            this.Controls.Add(this.itemOperationsListView);
            this.Controls.Add(this.itemsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Items";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parçalar";
            this.Load += new System.EventHandler(this.Items_Load);
            this.Resize += new System.EventHandler(this.Items_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView itemsListView;
        private System.Windows.Forms.ListView itemOperationsListView;
    }
}