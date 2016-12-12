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

 }
}
