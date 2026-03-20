using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace TeklaClassifier {
    internal static class Program {

        public static ClassificationForm ClassificationForm;

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ClassificationForm = new ClassificationForm(); 
            Application.Run(ClassificationForm);

        }
    }
}
