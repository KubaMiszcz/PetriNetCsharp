using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using static PetriNetCsharp.Helpers;

namespace PetriNetCsharp
{
   
    public partial class MainForm : Form
    {
        private delegate void UpdateDataGridAsVector(List<int> vector);
        private int _numberOfPlacesDgvCols;
        private int _numberOfTransitionsDgvRows;
        private List<List<int>> ListDin;
        private List<List<int>> ListDout;
        private List<List<int>> ListMo;
        private PetriNet _petriNet;
        private string _currentSavedFileName;
        private string _titleBar;
        private int MhistoryEntries = 7;
        public MainForm()
        {
            InitializeComponent();
            
            //List<int> Mcurrent = new List<int>(new int[] { 1, 0, 0, 0 });
            _numberOfPlacesDgvCols = int.Parse(textBoxNumPlaces.Text);
            _numberOfTransitionsDgvRows = int.Parse(textBoxNumTransitions.Text);
            UpdateDataGrid(dgvDin);
            UpdateDataGrid(dgvDout);
            UpdateDataGrid(dgvMbegin, 1);
            UpdateDataGrid(dgvIncidenceMatrix);
            UpdateDataGrid(dgvMhistory);
            dgvMhistory.RowCount = MhistoryEntries;


            //DRUGI TAB
            UpdateDataGrid(dgvTcond);
            UpdateDataGrid(dgvTready, 2);

            //#################################################testowa siec
            List<List<int>> dinn = new List<List<int>>();
            dinn.Add(new List<int>(new int[] { 0, 1, 1, 0 }));
            dinn.Add(new List<int>(new int[] { 0, 0, 0, 1 }));
            dinn.Add(new List<int>(new int[] { 1, 0, 0, 0 }));
            ImportMatrix2DToDataGridView(dinn, dgvDin);

            List<List<int>> doutt = new List<List<int>>();
            doutt.Add(new List<int>(new int[] { 1, 0, 0, 0 }));
            doutt.Add(new List<int>(new int[] { 0, 1, 1, 0 }));
            doutt.Add(new List<int>(new int[] { 0, 0, 0, 1 }));
            ImportMatrix2DToDataGridView(doutt, dgvDout);

            List<int> mo = new List<int>(new int[] { 1, 0, 0, 0 });
            ImportVectorToDataGridView(mo, dgvMbegin);
            //#################################################koneic testowej sieci

            UpdatePetriNet();

        }

        public void UpdateDataGrid(DataGridView target, int param = 0)
        {

            target.RowCount = _numberOfTransitionsDgvRows;
            target.ColumnCount = _numberOfPlacesDgvCols;

            if (param == 1)
            {
                target.RowCount = 1; //czyli zmieniam mcurrent - tylko jeden wiersz
                if (_numberOfPlacesDgvCols > (int)target.Width / 25)  //szer dgv/szer col== 325/25=13
                {
                    target.Height = 46 + SystemInformation.HorizontalScrollBarHeight;
                }
                else
                {
                    target.Height = 46;
                }
            }
            if (param == 2)//ten dgv kolumnowy bool z tready
            {
                target.ColumnCount = 1;
                target.Columns[0].Width = 35;
                target.Columns[0].HeaderText = "OK?";
                target.RowHeadersWidth = 48;
                for (int i = 0; i < target.RowCount; i++)
                {
                    //target.Rows[i].Cells[0].Value = false;
                    target.Rows[i].HeaderCell.Value = (i + 1).ToString();
                    target.RowHeadersWidth = 48;
                }
                return;
            }

            FillNullsInDgvWithZeros(target);
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

        private void UpdateAllDataGridViews()
        {
            UpdateDataGrid(dgvDin);
            UpdateDataGrid(dgvDout);
            UpdateDataGrid(dgvMbegin, 1);
            UpdateDataGrid(dgvIncidenceMatrix);
            UpdateDataGrid(dgvTcond);
            UpdateDataGrid(dgvTready, 2);
            UpdateDataGrid(dgvMhistory);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _titleBar = this.Text;
        }

        private void textBoxNumPlaces_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _numberOfPlacesDgvCols = int.Parse(textBoxNumPlaces.Text);
                UpdateAllDataGridViews();
                UpdatePetriNet();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("podaj prawidlowo liczbe miejsc!\n\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void textBoxNumTransitions_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _numberOfTransitionsDgvRows = int.Parse(textBoxNumTransitions.Text);
                UpdateAllDataGridViews();
                UpdatePetriNet();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("podaj prawidlowo liczbe tranzycji!\n\n" + ex.StackTrace);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dataGridViewDinDout_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdatePetriNet();
        }

        public void UpdatePetriNet()
        {

            try
            {
                UpdateAllDataGridViews();
                dgvMhistory.RowCount = MhistoryEntries;
                ListDin = ImportDataGridToMatrix2D(dgvDin);
                ListDout = ImportDataGridToMatrix2D(dgvDout);
                ListMo = ImportDataGridToMatrix2D(dgvMbegin);

                _petriNet = new PetriNet(ListDin, ListDout, ListMo[0]);

                ImportMatrix2DToDataGridView(_petriNet.Dmatrix, dgvIncidenceMatrix);
                ImportMatrix2DToDataGridView(_petriNet.Tcond, dgvTcond);
                ImportVectorToDataGridView(_petriNet.TReady, dgvTready);
                tbCurrentStep.Text = _petriNet.CurrentStep.ToString();
            }
            catch (Exception ex)
            {
                Report(ex);
            }
        }

        private void btnNumPlacesDown_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBoxNumPlaces.Text);
            a--;
            textBoxNumPlaces.Text = a.ToString();
            textBoxNumPlaces_TextChanged(sender, e);
        }

        private void btnNumPlacesUp_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBoxNumPlaces.Text);
            a++;
            textBoxNumPlaces.Text = a.ToString();
            textBoxNumPlaces_TextChanged(sender, e);
        }

        private void btnNumTransitionsDown_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBoxNumTransitions.Text);
            a--;
            textBoxNumTransitions.Text = a.ToString();
            textBoxNumTransitions_TextChanged(sender, e);
        }

        private void btnNumTransitionsUp_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBoxNumTransitions.Text);
            a++;
            textBoxNumTransitions.Text = a.ToString();
            textBoxNumTransitions_TextChanged(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FillWholeDgvWithZeros(dgvDout);
            UpdatePetriNet();
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void wyjscieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                saveFileDialog1.Filter = "PetriNet files (*.pn)|*.pn|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.DefaultExt = ".pn";
                saveFileDialog1.FileName = "Projekt1";
                Stream myStream;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (StreamWriter writer = new StreamWriter(myStream, System.Text.Encoding.UTF8))
                        {
                            // Add some text to the file.
                            //writer.Write("This is the ");
                            //writer.WriteLine("header for the file.");
                            //writer.WriteLine("-------------------");
                            // Arbitrary objects can also be written to the file.
                            //var json = new JavaScriptSerializer().Serialize(_petriNet);
                            string json = JsonConvert.SerializeObject(_petriNet, Formatting.Indented);
                            writer.WriteLine(json);
                        }
                        myStream.Close();

                    }
                }
                //String AktualnyPlikDoZapisu_nazwa = System.IO.Path.GetFileNameWithoutExtension(AktualnyPlikDoZapisu);
                _currentSavedFileName = saveFileDialog1.FileName;
                this.Text = _currentSavedFileName + " - " + _titleBar;
            }

        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                if (_currentSavedFileName == null)
                {
                    zapiszJakoToolStripMenuItem_Click(sender, e);
                }
                ;
                FileStream file = new FileStream(_currentSavedFileName, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(file, System.Text.Encoding.UTF8))
                {
                    // Add some text to the file.
                    //writer.Write("This is the ");
                    //writer.WriteLine("header for the file.");
                    //writer.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.
                    // var json = new JavaScriptSerializer().Serialize(_petriNet);
                    string json = JsonConvert.SerializeObject(_petriNet, Formatting.Indented);
                    writer.Write(json);
                }
                file.Close();
            }
        }

        private void otworzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                Stream myStream = null;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                //openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "PetriNet files (*.pn)|*.pn|txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            using (StreamReader reader = new StreamReader(myStream, System.Text.Encoding.UTF8))
                            {
                                // Insert code to read the stream here.
                                while (!reader.EndOfStream)
                                {
                                    string json = reader.ReadToEnd();
                                    _petriNet = JsonConvert.DeserializeObject<PetriNet>(json);
                                    ImportMatrix2DToDataGridView(_petriNet.DinMatrix, dgvDin);
                                    ImportMatrix2DToDataGridView(_petriNet.DoutMatrix, dgvDout);
                                    ImportVectorToDataGridView(_petriNet.Mbegin, dgvMbegin);
                                    UpdatePetriNet();

                                    // _petriNet = new JavaScriptSerializer().Deserialize<PetriNet>(json);
                                }

                                myStream.Close();
                                //_petriNet.Matrix2DToDataGridView(_petriNet.DinMatrix,dgvDin);
                                //_petriNet.Matrix2DToDataGridView(_petriNet.DoutMatrix, dgvDout);
                                //List<List<int>> tmpList =new List<List<int>>();
                                //tmpList.Add(_petriNet.Mcurrent);
                                //_petriNet.Matrix2DToDataGridView(tmpList, dataGridViewMcurrent);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + ex.StackTrace);
                    }
                }
                //String AktualnyPlikDoZapisu_nazwa = System.IO.Path.GetFileNameWithoutExtension(AktualnyPlikDoZapisu);
                _currentSavedFileName = openFileDialog1.FileName;
                if (_currentSavedFileName != "")
                {
                    this.Text = _currentSavedFileName + " - " + _titleBar;
                }
            }

        }

        private void drukujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.ShowDialog();

        }

        private void btnZerujDin_Click(object sender, EventArgs e)
        {
            FillWholeDgvWithZeros(dgvDin);
        }

        private void btnZoomDin_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dgvDin, "Macierz Dmatrix+ (DinMatrix)");
            form.Show();
        }
        private void btnZoomDout_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dgvDout, "Macierz Dmatrix-- (DoutMatrix)");
            form.Show();
        }
        private void btnZoomIncidenceMatrix_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dgvIncidenceMatrix, "Transponowana macierz incydencji (Dmatrix')");
            form.Show();
        }


        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewTcond_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            
            for (int i = dgvMhistory.RowCount - 1; i > 0; i--)
            {
                for (int j = 0; j < dgvMhistory.ColumnCount; j++)
                {
                    dgvMhistory.Rows[i].Cells[j].Value = dgvMhistory.Rows[i - 1].Cells[j].Value;
                }

            }
            for (int i = 0; i < dgvMhistory.ColumnCount; i++)
            {
                dgvMhistory.Rows[0].Cells[i].Value = _petriNet.Mcurrent[i];
            }
            _petriNet.NextStep();
            tbCurrentStep.Text = _petriNet.CurrentStep.ToString();
            //ImportMatrix2DToDataGridView(_petriNet.Tcond,dgvTcond); //to sie nei zmienia
            ImportVectorToDataGridView(_petriNet.TReady, dgvTready);
        }

        private void atest(object sender, EventArgs e)
        {
            MessageBox.Show("atest");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            UpdatePetriNet();
        }
    }
}
