using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CCI {
    public partial class CCIForm : Form {

        public string DatabasefilePath {  get => textBox_PathDatabase.Text; }
        public string MappingfilePath { get => textBox_PathMapping.Text; }

        public IClassifier ClassifierInstance { get; set; }

        public CCIForm() {
            InitializeComponent();
        }

        private void OnLoad(object sender, System.EventArgs e) {
            SetDefaultPaths();
        }

        private void SetDefaultPaths() {
            try {
                string modelFolder = Program.TeklaModel.GetInfo().ModelPath;
                textBox_PathDatabase.Text = modelFolder + "\\CCI\\CCI_database.csv";
                textBox_PathMapping.Text = modelFolder + "\\CCI\\CCI_mapping.csv";
            }
            catch (System.Exception ex) {
                Output.Error(ex.ToString());
            }
            
        }

        private void LockInput() {
            button_ClassifySelected.Enabled = false;
            button_ClassifyAll.Enabled = false;
        }

        private void UnlockInput() {
            button_ClassifySelected.Enabled = true;
            button_ClassifyAll.Enabled = true;
        }

        private void button_ClassifySelected_Click(object sender, System.EventArgs e) {
            LockInput();

            var classifier = new Classifier(MappingfilePath, DatabasefilePath);
            classifier.ClassifySelection();
            UnlockInput();
        }

        private void button_ClassifyAll_Click(object sender, System.EventArgs e) {
            LockInput();
            var classifier = new Classifier(MappingfilePath, DatabasefilePath);
            classifier.ClassifyAll();
            UnlockInput();
        }


        public void SetStatus(string txt) {
            toolStripStatusLabel1.Text = txt;
            statusStrip1.Update();
        }


        private void button_ExplorerPathMapping_Click(object sender, System.EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog {
                InitialDirectory = Program.TeklaModel.GetInfo().ModelPath,
                Title = "Choose mapping file",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv",
                FilterIndex = 2
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                textBox_PathMapping.Text = openFileDialog1.FileName;
            }
        }

        private void button_ExplorerPathDatabase_Click(object sender, System.EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog {
                InitialDirectory = Program.TeklaModel.GetInfo().ModelPath,
                Title = "Choose database file",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv",
                FilterIndex = 2
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                textBox_PathDatabase.Text = openFileDialog1.FileName;
            }
        }

        private void button_DeleteSelectedUDA_Click(object sender, System.EventArgs e) {
            var confirmResult = MessageBox.Show("Are you sure to delete CCI UDA values for selected parts?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes) {
                var classifier = new Classifier(MappingfilePath, DatabasefilePath);
                classifier.DeleteCCIUDAFromSelection();
            }
        }

        private void button_DeleteAllUDA_Click(object sender, System.EventArgs e) {
            var confirmResult = MessageBox.Show("Are you sure to delete CCI UDA values for ALL parts?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes) {
                var classifier = new Classifier(MappingfilePath, DatabasefilePath);
                classifier.DeleteCCIUDAFromAll();
            }
        }
    }
}
