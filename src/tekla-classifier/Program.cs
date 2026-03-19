using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace CCI {
    internal static class Program {

        public static CCIForm cciForm;
        public static Model TeklaModel;
        public static string ModelFolderPath = Environment.GetCommandLineArgs()[0];

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            cciForm = new CCIForm(); 
            ConnectToTekla();
            Application.Run(cciForm);

        }

        static public void ConnectToTekla() {
            TeklaModel = new Model();

            try {
                if (TeklaModel.GetConnectionStatus())
                    Output.Log("Connected to Tekla successfully");
                else
                    Output.Error("Unable to find Tekla");
            }
            catch (Exception) {
                Output.Error("Unable to find Tekla"); ;
            }
        }
    }
}
