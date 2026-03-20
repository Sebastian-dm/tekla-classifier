using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeklaClassifier {
    internal static class Output {

        public static void Status(string txt) {
			try {
                Program.ClassificationForm.SetStatus(txt);
            }
			catch (Exception) {
			}              
        }

        public static void Error(string txt) {
            MessageBox.Show(txt);
            Log(txt);
            System.Diagnostics.Debug.Print(txt);
        }

        public static void Log(string txt) {
            // Write to log file
            System.Diagnostics.Debug.Print(txt);

        }

    }
}
