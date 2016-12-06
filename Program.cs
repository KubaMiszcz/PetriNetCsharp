using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;


namespace PetriNetCsharp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainWindow = new MainForm();
            Application.Run(mainWindow);

            //List<List<int>> aa=new List<List<int>>(new List<int>[3]);

            

            //List<int> aa = new List<int>(3);
            //aa.Insert(0, 2);
            //aa.Insert(1, 6);
            //aa.Insert(2, 8);

            //List<int> bb = new List<int>(3);
            //bb.Insert(0, 2);
            //bb.Insert(1, 6);
            //bb.Insert(2, 8);

            //List<int> cc = new List<int>(3);

            //for (int i = 0; i < cc.Capacity; i++)
            //{
            //    cc.Insert(i,aa[i]+bb[i]);
            //}

            //foreach (var VARIABLE in cc)
            //{
            //    Console.WriteLine(VARIABLE);
            //}

            //Console.WriteLine("cnt2 " + cc.Count);
            //Console.WriteLine("cap " + cc.Capacity);

            //Console.WriteLine(cc.First());


            //Console.ReadKey();

        }
    }
}
