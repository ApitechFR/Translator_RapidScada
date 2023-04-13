namespace Translator_RapidScada
{
    partial class TranslatorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            button3 = new Button();
            labelCheminExcel = new Label();
            button2 = new Button();
            labelChoixEmplacementExcel = new Label();
            chosenPathLabel1 = new Label();
            button1 = new Button();
            label2 = new Label();
            label1 = new Label();
            label5 = new Label();
            button5 = new Button();
            button4 = new Button();
            label4 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SystemColors.GradientInactiveCaption;
            splitContainer1.Panel1.Controls.Add(button3);
            splitContainer1.Panel1.Controls.Add(labelCheminExcel);
            splitContainer1.Panel1.Controls.Add(button2);
            splitContainer1.Panel1.Controls.Add(labelChoixEmplacementExcel);
            splitContainer1.Panel1.Controls.Add(chosenPathLabel1);
            splitContainer1.Panel1.Controls.Add(button1);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1.Controls.Add(label1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(button5);
            splitContainer1.Panel2.Controls.Add(button4);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Size = new Size(1033, 457);
            splitContainer1.SplitterDistance = 507;
            splitContainer1.TabIndex = 0;
            // 
            // button3
            // 
            button3.Location = new Point(78, 377);
            button3.Name = "button3";
            button3.Size = new Size(355, 29);
            button3.TabIndex = 7;
            button3.Text = "Génération du tableau de traductions Excel";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // labelCheminExcel
            // 
            labelCheminExcel.AutoSize = true;
            labelCheminExcel.Location = new Point(12, 297);
            labelCheminExcel.Name = "labelCheminExcel";
            labelCheminExcel.Size = new Size(0, 20);
            labelCheminExcel.TabIndex = 6;
            // 
            // button2
            // 
            button2.Location = new Point(383, 250);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 5;
            button2.Text = "Parcourir";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // labelChoixEmplacementExcel
            // 
            labelChoixEmplacementExcel.AutoSize = true;
            labelChoixEmplacementExcel.Location = new Point(12, 254);
            labelChoixEmplacementExcel.Name = "labelChoixEmplacementExcel";
            labelChoixEmplacementExcel.Size = new Size(317, 20);
            labelChoixEmplacementExcel.TabIndex = 4;
            labelChoixEmplacementExcel.Text = "Choisissez où enregistrer le futur fichier Excel : ";
            // 
            // chosenPathLabel1
            // 
            chosenPathLabel1.AutoSize = true;
            chosenPathLabel1.ImageAlign = ContentAlignment.MiddleLeft;
            chosenPathLabel1.Location = new Point(12, 194);
            chosenPathLabel1.Name = "chosenPathLabel1";
            chosenPathLabel1.Size = new Size(0, 20);
            chosenPathLabel1.TabIndex = 3;
            // 
            // button1
            // 
            button1.Location = new Point(383, 162);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 2;
            button1.Text = "Parcourir";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 162);
            label2.Name = "label2";
            label2.Size = new Size(206, 20);
            label2.TabIndex = 1;
            label2.Text = "Choisissez le dossier SCADA : ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(112, 43);
            label1.Name = "label1";
            label1.Size = new Size(254, 37);
            label1.TabIndex = 0;
            label1.Text = "Création de l'Excel";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(34, 194);
            label5.Name = "label5";
            label5.Size = new Size(0, 20);
            label5.TabIndex = 8;
            // 
            // button5
            // 
            button5.Location = new Point(150, 377);
            button5.Name = "button5";
            button5.Size = new Size(226, 29);
            button5.TabIndex = 8;
            button5.Text = "Génération des fichiers xml";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button4
            // 
            button4.Location = new Point(400, 162);
            button4.Name = "button4";
            button4.Size = new Size(94, 29);
            button4.TabIndex = 8;
            button4.Text = "Parcourir";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(34, 166);
            label4.Name = "label4";
            label4.Size = new Size(252, 20);
            label4.TabIndex = 8;
            label4.Text = "Choisissez le fichier Excel à extraire : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(22, 43);
            label3.Name = "label3";
            label3.Size = new Size(486, 37);
            label3.TabIndex = 8;
            label3.Text = "Génération de nouveaux fichiers xml";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // TranslatorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1033, 457);
            Controls.Add(splitContainer1);
            Name = "TranslatorForm";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Label label1;
        private Button button1;
        private Label label2;
        private Label chosenPathLabel1;
        private Label labelCheminExcel;
        private Button button2;
        private Label labelChoixEmplacementExcel;
        private Button button3;
        private Label label3;
        private Label label4;
        private Button button5;
        private Button button4;
        private Label label5;
    }
}