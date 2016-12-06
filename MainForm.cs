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
        private int _numberOfPlacesDgvCols;
        private int _numberOfTransitionsDgvRows;
        private PetriNet _petriNet;
        private string _currentSavedFileName;
        private string _titleBar;
        public MainForm()
        {
            InitializeComponent();
        //List<int> Mcurrent = new List<int>(new int[] { 1, 0, 0, 0 });
            _numberOfPlacesDgvCols = int.Parse(textBoxNumPlaces.Text);
            _numberOfTransitionsDgvRows = int.Parse(textBoxNumTransitions.Text);
            UpdateDataGrid(dataGridViewDin);
            UpdateDataGrid(dataGridViewDout);
            UpdateDataGrid(dataGridViewMcurrent,1);
            UpdatePetriNet();
            UpdateDataGrid(dataGridViewIncidenceMatrix);
            ImportMatrix2DToDataGridView(_petriNet.D,dataGridViewIncidenceMatrix);
        }

        public void UpdateDataGrid(DataGridView target,int rows=0)
        {
            if (rows==0)
            {
                target.RowCount = _numberOfTransitionsDgvRows;
            }
            else
            {
                target.RowCount = rows; //czyli zmieniam mcurrent - tylko jeden wiersz
                if (_numberOfPlacesDgvCols>13)  //szer dgv/szer col== 325/25=13
                {
                    target.Height = 46 + SystemInformation.HorizontalScrollBarHeight;
                }
                else
                {
                    target.Height = 46;
                }
                
            }
            
            target.ColumnCount = _numberOfPlacesDgvCols;
            FillNullsInDgvWithZeros(target);
            for (int i = 0; i < target.ColumnCount; i++)
            {
                target.Columns[i].HeaderText = (i+1).ToString();
                target.Columns[i].Width = 25;

            }
            for (int i = 0; i < target.RowCount; i++)
            {
                target.Rows[i].HeaderCell.Value = (i + 1).ToString();
                target.RowHeadersWidth = 48;
            }
        }

        private void UpdateAllDataGrids()
        {
            UpdateDataGrid(dataGridViewDin);
            UpdateDataGrid(dataGridViewDout);
            UpdateDataGrid(dataGridViewMcurrent, 1);
            UpdateDataGrid(dataGridViewIncidenceMatrix);
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
UpdateAllDataGrids();
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
                _numberOfPlacesDgvCols = int.Parse(textBoxNumTransitions.Text);
                UpdateAllDataGrids();
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
                _petriNet = new PetriNet(dataGridViewDin, dataGridViewDout, dataGridViewMcurrent);
                ImportMatrix2DToDataGridView(_petriNet.D, dataGridViewIncidenceMatrix);
            }
            catch (Exception ex)
            {
                Raport(ex);
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
            FillWholeDgvWithZeros(dataGridViewDout);
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
        
        private void btnUpdateNetProp_Click(object sender, EventArgs e)
        {
            UpdatePetriNet();
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
                        using (StreamWriter writer = new StreamWriter(myStream))
                        {
                            // Add some text to the file.
                            //writer.Write("This is the ");
                            //writer.WriteLine("header for the file.");
                            //writer.WriteLine("-------------------");
                            // Arbitrary objects can also be written to the file.
                            var json = new JavaScriptSerializer().Serialize(_petriNet);
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
                    MessageBox.Show("Nic jeszcze nie zapisywałeś!",
                        "Błąd!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1
                        //MessageBoxOptions.,
                        );

                    return;
                }
                ;
                FileStream file = new FileStream(_currentSavedFileName, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(file))
                {
                    // Add some text to the file.
                    //writer.Write("This is the ");
                    //writer.WriteLine("header for the file.");
                    //writer.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.

                    //                    var json = new JavaScriptSerializer().Serialize(_petriNet);
                    string json = JsonConvert.SerializeObject(_petriNet);//, Formatting.Indented);
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
                            using (StreamReader reader = new StreamReader(myStream))
                            {
                                // Insert code to read the stream here.
                                while (!reader.EndOfStream)
                                {
                                    string json =reader.ReadToEnd();
                                    //PetriNet newnet= JsonConvert.DeserializeObject<PetriNet>(json);


                                   // _petriNet = new JavaScriptSerializer().Deserialize<PetriNet>(json);
                                }

                                myStream.Close();
                                //_petriNet.Matrix2DToDataGridView(_petriNet.Din,dataGridViewDin);
                                //_petriNet.Matrix2DToDataGridView(_petriNet.Dout, dataGridViewDout);
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
                    this.Text = _currentSavedFileName + " - "+_titleBar;
                }
            }

        }

        private void drukujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.ShowDialog();

        }

        private void btnZerujDin_Click(object sender, EventArgs e)
        {
            FillWholeDgvWithZeros(dataGridViewDin);
    }

        private void btnZoomDin_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dataGridViewDin, "Macierz D+ (Din)");
            form.Show();
        }
        private void btnZoomDout_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dataGridViewDout, "Macierz D-- (Dout)");
            form.Show();
        }
        private void btnZoomIncidenceMatrix_Click(object sender, EventArgs e)
        {
            ZoomMatrixForm form = new ZoomMatrixForm(dataGridViewIncidenceMatrix, "Transponowana macierz incydencji (D')");
            form.Show();
        }

        
    }
}
