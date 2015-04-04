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
    public partial class Items : Form
    {
        private Bench bench = null;

        public Items(Bench bench)
        {
            this.bench = bench;

            InitializeComponent();
        }

        private void Items_Load(object sender, EventArgs e)
        {
            formatListViews();

            if (this.MdiParent != null)
            {
                this.MdiParent.Resize += MdiParent_Resize;
            }

            loadItems();
        }

        private void formatListViews()
        {
            this.itemsListView.Items.Clear();
            this.itemsListView.View = View.Details;
            this.itemsListView.Columns.Add("Item");
            this.itemsListView.Columns[0].Width = this.itemsListView.Width - 4;
            this.itemsListView.HeaderStyle = ColumnHeaderStyle.None;

            this.itemOperationsListView.Items.Clear();
            this.itemOperationsListView.View = View.Details;
            this.itemOperationsListView.Columns.Add("Item");
            this.itemOperationsListView.Columns[0].Width = this.itemOperationsListView.Width - 4;
            this.itemOperationsListView.HeaderStyle = ColumnHeaderStyle.None;
        }

        void MdiParent_Resize(object sender, EventArgs e)
        {
            // NOP
        }

        private void loadItems()
        {
            this.itemsListView.Items.Clear();

            if (this.bench != null)
            {
                if (this.bench.HasItems)
                {

                    foreach (Item item in bench.Items)
                    {
                        ListViewItem listItem = new ListViewItem(item.Name);
                        listItem.Tag = item;

                        this.itemsListView.Items.Add(listItem);
                    }

                    this.itemsListView.Items[0].Selected = true;

                    loadOperations();
                }
            }
        }

        public void reload()
        {
            loadItems();
        }

        private void Items_Resize(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void itemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadOperations();
        }

        private void loadOperations()
        {
            if (this.itemsListView.SelectedIndices.Count > 0)
            {
                ListViewItem listViewItem = this.itemsListView.SelectedItems[0];

                if (listViewItem != null)
                {
                    Item item = (Item)listViewItem.Tag;

                    this.itemOperationsListView.Items.Clear();

                    foreach (Operation operation in item.Operations)
                    {
                        ListViewItem listItem = new ListViewItem(operation.Name);
                        listItem.Tag = operation;

                        this.itemOperationsListView.Items.Add(listItem);
                    }
                }
            }

        }
    }
}
