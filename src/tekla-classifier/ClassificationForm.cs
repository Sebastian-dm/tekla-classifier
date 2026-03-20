using System;
using System.Windows.Forms;
using Tekla.Structures.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TeklaClassifier {
    public partial class ClassificationForm : Form {

        public string DatabasefilePath {  get => textBox_PathDatabase.Text; }
        public string MappingfilePath { get => textBox_PathMapping.Text; }

        private readonly Model _model;

        public IClassifier ClassifierInstance { get; set; }

        public ClassificationForm() {
            InitializeComponent();

            _model = new Model();
            if (_model.GetConnectionStatus())
                Output.Log("Connected to Tekla successfully");
            else
                Output.Error("Unable to find Tekla");

        }

        private void OnLoad(object sender, EventArgs e) {
            SetDefaultPaths();
        }

        private void SetDefaultPaths() {
            try {
                string modelFolder = _model.GetInfo().ModelPath;
                textBox_PathDatabase.Text = modelFolder + "\\Classification\\database.csv";
                textBox_PathMapping.Text = modelFolder + "\\Classification\\mapping.csv";
            }
            catch (Exception ex) {
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

        private void button_ClassifySelected_Click(object sender, EventArgs e) {
            LockInput();

            var classifier = new Classifier(MappingfilePath, DatabasefilePath);
            var selectionHelper = new SelectionHelper();
            classifier.Classify(selectionHelper.GetSelectedParts());
            UnlockInput();
        }

        private void button_ClassifyAll_Click(object sender, EventArgs e) {
            LockInput();
            var classifier = new Classifier(MappingfilePath, DatabasefilePath);
            var selectionHelper = new SelectionHelper();
            classifier.Classify(selectionHelper.GetAllParts(_model));
            UnlockInput();
        }


        public void SetStatus(string txt) {
            toolStripStatusLabel1.Text = txt;
            statusStrip1.Update();
        }


        private void button_ExplorerPathMapping_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog {
                InitialDirectory = _model.GetInfo().ModelPath,
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

        private void button_ExplorerPathDatabase_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog {
                InitialDirectory = _model.GetInfo().ModelPath,
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

        private void button_DeleteSelectedUDA_Click(object sender, EventArgs e) {
            var confirmResult = MessageBox.Show("Are you sure to delete CCI UDA values for selected parts?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes) {
                var classifier = new Classifier(MappingfilePath, DatabasefilePath);
                var selectionHelper = new SelectionHelper();
                classifier.DeleteClassificationUDAValues(selectionHelper.GetSelectedParts());
            }
        }

        private void button_DeleteAllUDA_Click(object sender, EventArgs e) {
            var confirmResult = MessageBox.Show("Are you sure to delete CCI UDA values for ALL parts?",
                                     "Confirmation",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes) {
                var classifier = new Classifier(MappingfilePath, DatabasefilePath);
                var selectionHelper = new SelectionHelper();
                classifier.DeleteClassificationUDAValues(selectionHelper.GetAllParts(_model));
            }
        }
    }
}
