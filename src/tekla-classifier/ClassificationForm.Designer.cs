namespace TeklaClassifier
{
    partial class ClassificationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_ClassifySelected = new System.Windows.Forms.Button();
            this.button_ClassifyAll = new System.Windows.Forms.Button();
            this.textBox_PathMapping = new System.Windows.Forms.TextBox();
            this.textBox_PathDatabase = new System.Windows.Forms.TextBox();
            this.label_Mapping = new System.Windows.Forms.Label();
            this.label_Database = new System.Windows.Forms.Label();
            this.button_ExplorerPathMapping = new System.Windows.Forms.Button();
            this.button_ExplorerPathDatabase = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button_DeleteSelectedUDA = new System.Windows.Forms.Button();
            this.button_DeleteAllUDA = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ClassifySelected
            // 
            this.button_ClassifySelected.Location = new System.Drawing.Point(344, 130);
            this.button_ClassifySelected.Name = "button_ClassifySelected";
            this.button_ClassifySelected.Size = new System.Drawing.Size(103, 45);
            this.button_ClassifySelected.TabIndex = 0;
            this.button_ClassifySelected.Text = "Classify selected parts";
            this.button_ClassifySelected.UseVisualStyleBackColor = true;
            this.button_ClassifySelected.Click += new System.EventHandler(this.button_ClassifySelected_Click);
            // 
            // button_ClassifyAll
            // 
            this.button_ClassifyAll.Location = new System.Drawing.Point(344, 181);
            this.button_ClassifyAll.Name = "button_ClassifyAll";
            this.button_ClassifyAll.Size = new System.Drawing.Size(103, 45);
            this.button_ClassifyAll.TabIndex = 1;
            this.button_ClassifyAll.Text = "Classify all parts";
            this.button_ClassifyAll.UseVisualStyleBackColor = true;
            this.button_ClassifyAll.Click += new System.EventHandler(this.button_ClassifyAll_Click);
            // 
            // textBox_PathMapping
            // 
            this.textBox_PathMapping.Location = new System.Drawing.Point(15, 28);
            this.textBox_PathMapping.Name = "textBox_PathMapping";
            this.textBox_PathMapping.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox_PathMapping.Size = new System.Drawing.Size(396, 22);
            this.textBox_PathMapping.TabIndex = 2;
            this.textBox_PathMapping.Text = "...";
            this.textBox_PathMapping.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox_PathDatabase
            // 
            this.textBox_PathDatabase.Location = new System.Drawing.Point(15, 82);
            this.textBox_PathDatabase.Name = "textBox_PathDatabase";
            this.textBox_PathDatabase.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox_PathDatabase.Size = new System.Drawing.Size(396, 22);
            this.textBox_PathDatabase.TabIndex = 3;
            this.textBox_PathDatabase.Text = "...";
            this.textBox_PathDatabase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label_Mapping
            // 
            this.label_Mapping.AutoSize = true;
            this.label_Mapping.Location = new System.Drawing.Point(12, 9);
            this.label_Mapping.Name = "label_Mapping";
            this.label_Mapping.Size = new System.Drawing.Size(128, 16);
            this.label_Mapping.TabIndex = 4;
            this.label_Mapping.Text = "Path for mapping file";
            // 
            // label_Database
            // 
            this.label_Database.AutoSize = true;
            this.label_Database.Location = new System.Drawing.Point(12, 63);
            this.label_Database.Name = "label_Database";
            this.label_Database.Size = new System.Drawing.Size(178, 16);
            this.label_Database.TabIndex = 5;
            this.label_Database.Text = "Path for CCI model database";
            // 
            // button_ExplorerPathMapping
            // 
            this.button_ExplorerPathMapping.Location = new System.Drawing.Point(417, 28);
            this.button_ExplorerPathMapping.Name = "button_ExplorerPathMapping";
            this.button_ExplorerPathMapping.Size = new System.Drawing.Size(30, 22);
            this.button_ExplorerPathMapping.TabIndex = 6;
            this.button_ExplorerPathMapping.Text = "...";
            this.button_ExplorerPathMapping.UseVisualStyleBackColor = true;
            this.button_ExplorerPathMapping.Click += new System.EventHandler(this.button_ExplorerPathMapping_Click);
            // 
            // button_ExplorerPathDatabase
            // 
            this.button_ExplorerPathDatabase.Location = new System.Drawing.Point(417, 84);
            this.button_ExplorerPathDatabase.Name = "button_ExplorerPathDatabase";
            this.button_ExplorerPathDatabase.Size = new System.Drawing.Size(30, 22);
            this.button_ExplorerPathDatabase.TabIndex = 7;
            this.button_ExplorerPathDatabase.Text = "...";
            this.button_ExplorerPathDatabase.UseVisualStyleBackColor = true;
            this.button_ExplorerPathDatabase.Click += new System.EventHandler(this.button_ExplorerPathDatabase_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 256);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(471, 26);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(18, 20);
            this.toolStripStatusLabel1.Text = "...";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_DeleteSelectedUDA
            // 
            this.button_DeleteSelectedUDA.Location = new System.Drawing.Point(202, 130);
            this.button_DeleteSelectedUDA.Name = "button_DeleteSelectedUDA";
            this.button_DeleteSelectedUDA.Size = new System.Drawing.Size(125, 45);
            this.button_DeleteSelectedUDA.TabIndex = 9;
            this.button_DeleteSelectedUDA.Text = "Delete CCI UDA on selected parts";
            this.button_DeleteSelectedUDA.UseVisualStyleBackColor = true;
            this.button_DeleteSelectedUDA.Click += new System.EventHandler(this.button_DeleteSelectedUDA_Click);
            // 
            // button_DeleteAllUDA
            // 
            this.button_DeleteAllUDA.Location = new System.Drawing.Point(202, 181);
            this.button_DeleteAllUDA.Name = "button_DeleteAllUDA";
            this.button_DeleteAllUDA.Size = new System.Drawing.Size(125, 45);
            this.button_DeleteAllUDA.TabIndex = 10;
            this.button_DeleteAllUDA.Text = "Delete CCI UDA on all parts";
            this.button_DeleteAllUDA.UseVisualStyleBackColor = true;
            this.button_DeleteAllUDA.Click += new System.EventHandler(this.button_DeleteAllUDA_Click);
            // 
            // CCIForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(471, 282);
            this.Controls.Add(this.button_DeleteAllUDA);
            this.Controls.Add(this.button_DeleteSelectedUDA);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button_ExplorerPathDatabase);
            this.Controls.Add(this.button_ExplorerPathMapping);
            this.Controls.Add(this.label_Database);
            this.Controls.Add(this.label_Mapping);
            this.Controls.Add(this.textBox_PathDatabase);
            this.Controls.Add(this.textBox_PathMapping);
            this.Controls.Add(this.button_ClassifyAll);
            this.Controls.Add(this.button_ClassifySelected);
            this.Name = "CCIForm";
            this.Text = "CCI classification";
            this.Load += new System.EventHandler(this.OnLoad);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_ClassifySelected;
        private System.Windows.Forms.Button button_ClassifyAll;
        private System.Windows.Forms.TextBox textBox_PathMapping;
        private System.Windows.Forms.TextBox textBox_PathDatabase;
        private System.Windows.Forms.Label label_Mapping;
        private System.Windows.Forms.Label label_Database;
        private System.Windows.Forms.Button button_ExplorerPathMapping;
        private System.Windows.Forms.Button button_ExplorerPathDatabase;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button_DeleteSelectedUDA;
        private System.Windows.Forms.Button button_DeleteAllUDA;
    }
}

