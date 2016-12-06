using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PetriNetCsharp.Helpers;

namespace PetriNetCsharp
{
    public partial class ZoomMatrixForm : Form
    {
//        private MainForm sourceForm;
       private DataGridView sourceDgv;
        public ZoomMatrixForm(MainForm mainForm)
        {
            InitializeComponent();
           // List<DataGridView> dataGridViews = (List<DataGridView>) mainForm.Controls.OfType<DataGridView>();
        }

        public ZoomMatrixForm(DataGridView sourceDgv,String title)
        {
            InitializeComponent();
            this.sourceDgv = sourceDgv;
  //          sourceForm = sourceDataGridView.Parent.Parent.Parent.Parent as MainForm;

            this.Text = title;
            int rows = sourceDgv.RowCount;
            int cols=sourceDgv.ColumnCount;
            dataGridViewDxxx.RowCount = rows;
            dataGridViewDxxx.ColumnCount = cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    dataGridViewDxxx.Rows[i].Cells[j].Value= sourceDgv.Rows[i].Cells[j].Value ;
                }
            }
            UpdateDataGrid(dataGridViewDxxx);
        }


        private void UpdateDataGrid(DataGridView target, int rows = 0)
        {
            //tu by mozna uzyc delegata moze jakiegos i dace motede z mainwindow bo tam sprawdza czy nie mcurrent
            //i na sztywno liczba wierszy =1 wtedy, ale to musze doczytac:]
           for (int i = 0; i < target.ColumnCount; i++)
            {
                target.Columns[i].HeaderText = (i + 1).ToString();
                target.Columns[i].Width = 25;

            }
            for (int i = 0; i < target.RowCount; i++)
            {
                target.Rows[i].HeaderCell.Value = (i + 1).ToString();
                target.RowHeadersWidth = 48;
            }
        }
        
        private void btnFillWithZeros_Click(object sender, EventArgs e)
        {
            FillWholeDgvWithZeros(dataGridViewDxxx);
        
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            int rows = dataGridViewDxxx.RowCount;
            int cols = dataGridViewDxxx.ColumnCount;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sourceDgv.Rows[i].Cells[j].Value = dataGridViewDxxx.Rows[i].Cells[j].Value;
                }
            }
            MainForm sourceForm = sourceDgv.Parent.Parent.Parent.Parent as MainForm;
            sourceForm.UpdatePetriNet();
            this.Close();
        }
    }
}
