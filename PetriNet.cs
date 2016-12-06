using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PetriNetCsharp.Helpers;

namespace PetriNetCsharp
{
    class PetriNet
    {
        /// <summary>
        /// sss
        /// </summary>

        //private //members
        private List<List<int>> _Tcond; //macierz warunki konieczne odpalenia dla kazdej tranzycji [TxP]????
        private List<List<int>> _Teye;    //macierz diagonalna wektorow jedynkowych do wyznaczenia warunkow odpalania tranzycji [TxT]
        //private List<List<int>> _D;     //macierz incydencji transponowana	[TxP]
        private int _currentlyCheckedTransition; //tranzycja aktualnie testowana
        //private List<List<int>> _tmp2D;
        private List<int> _tmp1D;

        //public members
        public int NumberOfPlaces { get; }
        public int NumberOfTransitions { get; }
        public List<List<int>> D { get; }     //macierz incydencji transponowana	[TxP]
        public List<List<int>> Din { get; }   //macierz wejsc - dodawania zetonow [TxP]
        public List<List<int>> Dout { get; }  //macierz wyjsc - odejmowania zetonow [TxP]
        public List<List<int>> Tcond { get; }   //macierz warunki konieczne odpalenia dla kazdej tranzycji [TxP]????
        public List<int> TReady { get; }         //wektro tranzycji gotowych do odpalenia po sprawdzeniu warunkow [T]
        public int CurrentlyCheckedTransition { get; } //tranzycja aktualnie testowana
        public List<int> Mbegin { get; set; }        //wektro znakowania poczatkowego [P]
        public List<int> Mcurrent { get; }        //wektro znakowania poczatkowego [P]

        //private //methods
        private List<List<int>> MakeIncidenceMatrix()    //generowanie macierzy incydencji ==> D=Din-Dout
        {
            return SubtractElementWise(Din, Dout);
        }
        private void MakeTransitionConditions()
        {
            //		generowanie wektorow jednostkowych T[P]=eye dl akazdej tranzycji
            _tmp1D = new List<int>(NumberOfTransitions);
            FillListWithValue(_tmp1D, 0);

            _Teye = new List<List<int>>(NumberOfTransitions);
            FillListWithValue(_Teye, _tmp1D);

            //		generowanie macierzy Tcond - warunkow odpalenia tranzycji
            //		warunki odpalenia dla kazdej trnazycji
            //		Tcond = Teye * Dout;
            _tmp1D = new List<int>(NumberOfPlaces);
            FillListWithValue(_tmp1D, 0);
            _Tcond = new List<List<int>>(NumberOfTransitions);
            FillListWithValue(_Tcond, _tmp1D);


            //		mnozenie macierzy
            _Tcond = Matrix2DMultiply(_Teye, Dout);

        }
        private int IsTransitionReadyToFire(List<int> condition)
        {
            int result = 1;
            int check = 1;
            for (int i = 0; i < NumberOfPlaces; i++)    //czy warunek spelniony dla wszystkich miejsc
            {
                //teraz dla kazdego miejsca sprawdzamy czy caly warunek jest spelniony
                //czy liczba znacznikow w M jest >= od wymaganych w tranzycji
                if (Mcurrent[i] < condition[i])
                {
                    check = 0;
                }
            }
            return result * check;
        }

        //public //methods
        public PetriNet() { }
        public PetriNet(DataGridView dgvDin, DataGridView dgvDout, DataGridView dgvMo)   //konstruktor
        {
            _currentlyCheckedTransition = 0;
            Din = ImportDataGridToMatrix2D(dgvDin);
            Dout = ImportDataGridToMatrix2D(dgvDout);
            Mbegin = ImportDataGridToMatrix2D(dgvMo)[0];
            Mcurrent = Mbegin;

            NumberOfTransitions = Din.Count;
            NumberOfPlaces = Din[0].Count;

            //generowanie macierzy incydencji ==> D = Din - Dout
            D = MakeIncidenceMatrix();


            //generowanie macierzy Tcond - warunkow odpalenia tranzycji
            //warunki odpalenia dla kazdej trnazycji
            //		Tcond = Teye * Dout;
            MakeTransitionConditions();
        }
        public void NextStep()
        {
            //to trzeba pojedynczko po kolei bo kazda tranzycja moze zmienc nam stan sieci
            //wiec nie mozemy przemnozyc wszytskiego naraz
            if (IsTransitionReadyToFire(_Tcond[_currentlyCheckedTransition]) == 1)
            {
                //teraz jesli warunek spelniony to uaktualniamy znakowanie, 
                //kolejna tranzycja bedzie operowac na tym nowym znakowaniu
                //newM=M+Teye[i]*D, czyli nei trzeba nawet mnozyc macierzy

                AddElementWise(Mcurrent, _Tcond[_currentlyCheckedTransition]);
                _currentlyCheckedTransition++;
            }

            if (_currentlyCheckedTransition >= NumberOfTransitions)
                _currentlyCheckedTransition = 0;
        }

        //######HELPERS####
        List<List<int>> ImportDataGridToMatrix2D(DataGridView source)
        {
            try
            {
                List<List<int>> result = new List<List<int>>(source.RowCount);
                List<int> resultRow = new List<int>(source.ColumnCount);
                for (int i = 0; i < source.RowCount; i++)
                {
                    for (int j = 0; j < source.ColumnCount; j++)
                    {
                        resultRow.Add(int.Parse(source.Rows[i].Cells[j].Value.ToString()));
                    }
                    result.Add(resultRow);
                    resultRow = new List<int>(source.ColumnCount);
                }
                return result;
            }
            catch (Exception ex)
            {

                Raport(ex);
            }
            return null;
        }
    }
}



