using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace TeklaClassifier {
    internal static class Program {

        public static ClassificationForm ClassificationForm;

        public static string ModelFolderPath = Environment.GetCommandLineArgs()[0];

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ClassificationForm = new ClassificationForm(); 
            Application.Run(ClassificationForm);

        }
    }
}
