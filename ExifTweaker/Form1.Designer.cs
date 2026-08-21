namespace ExifTweaker
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            bChange = new Button();
            bOpen = new Button();
            main = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            bGPS = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            label3 = new Label();
            label2 = new Label();
            dateTimePicker1 = new DateTimePicker();
            tType = new TextBox();
            tLon = new TextBox();
            tLat = new TextBox();
            label1 = new Label();
            tGPS = new TextBox();
            tName = new TextBox();
            splitContainer1 = new SplitContainer();
            dgv = new DataGridView();
            picBox = new PictureBox();
            pgb = new ProgressBar();
            bgw = new System.ComponentModel.BackgroundWorker();
            main.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            SuspendLayout();
            // 
            // bChange
            // 
            bChange.Dock = DockStyle.Fill;
            bChange.Location = new Point(0, 60);
            bChange.Margin = new Padding(0);
            bChange.Name = "bChange";
            bChange.Size = new Size(93, 32);
            bChange.TabIndex = 0;
            bChange.Text = "CHANGE!";
            bChange.UseVisualStyleBackColor = true;
            bChange.Click += button1_Click;
            // 
            // bOpen
            // 
            bOpen.Dock = DockStyle.Fill;
            bOpen.Location = new Point(0, 0);
            bOpen.Margin = new Padding(0);
            bOpen.Name = "bOpen";
            bOpen.Size = new Size(93, 30);
            bOpen.TabIndex = 0;
            bOpen.Text = "OPEN...";
            bOpen.UseVisualStyleBackColor = true;
            bOpen.Click += button2_Click;
            // 
            // main
            // 
            main.ColumnCount = 1;
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            main.Controls.Add(tableLayoutPanel2, 0, 1);
            main.Controls.Add(splitContainer1, 0, 0);
            main.Controls.Add(pgb, 0, 2);
            main.Dock = DockStyle.Fill;
            main.Location = new Point(0, 0);
            main.Margin = new Padding(4, 3, 4, 3);
            main.Name = "main";
            main.RowCount = 3;
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            main.Size = new Size(623, 336);
            main.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 93F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(bGPS, 0, 1);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 1, 0);
            tableLayoutPanel2.Controls.Add(bOpen, 0, 0);
            tableLayoutPanel2.Controls.Add(bChange, 0, 2);
            tableLayoutPanel2.Controls.Add(tGPS, 1, 1);
            tableLayoutPanel2.Controls.Add(tName, 1, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 221);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel2.Size = new Size(623, 92);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // bGPS
            // 
            bGPS.Dock = DockStyle.Fill;
            bGPS.Location = new Point(0, 30);
            bGPS.Margin = new Padding(0);
            bGPS.Name = "bGPS";
            bGPS.Size = new Size(93, 30);
            bGPS.TabIndex = 7;
            bGPS.Text = " FIND GPS";
            bGPS.UseVisualStyleBackColor = true;
            bGPS.Click += bGPS_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 7;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.00031F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.00031F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.99969F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.99969F));
            tableLayoutPanel3.Controls.Add(label3, 3, 0);
            tableLayoutPanel3.Controls.Add(label2, 1, 0);
            tableLayoutPanel3.Controls.Add(dateTimePicker1, 0, 0);
            tableLayoutPanel3.Controls.Add(tType, 6, 0);
            tableLayoutPanel3.Controls.Add(tLon, 4, 0);
            tableLayoutPanel3.Controls.Add(tLat, 2, 0);
            tableLayoutPanel3.Controls.Add(label1, 5, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(93, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(530, 30);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(247, 3);
            label3.Margin = new Padding(0, 3, 0, 3);
            label3.Name = "label3";
            label3.Size = new Size(35, 24);
            label3.TabIndex = 13;
            label3.Text = "Lon:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(106, 3);
            label2.Margin = new Padding(0, 3, 0, 3);
            label2.Name = "label2";
            label2.Size = new Size(35, 24);
            label2.TabIndex = 12;
            label2.Text = "Lat:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(4, 3);
            dateTimePicker1.Margin = new Padding(4, 3, 4, 3);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(98, 23);
            dateTimePicker1.TabIndex = 4;
            // 
            // tType
            // 
            tType.Dock = DockStyle.Fill;
            tType.Location = new Point(423, 3);
            tType.Margin = new Padding(0, 3, 4, 3);
            tType.Name = "tType";
            tType.ReadOnly = true;
            tType.Size = new Size(103, 23);
            tType.TabIndex = 10;
            // 
            // tLon
            // 
            tLon.Dock = DockStyle.Fill;
            tLon.Location = new Point(282, 3);
            tLon.Margin = new Padding(0, 3, 0, 3);
            tLon.Name = "tLon";
            tLon.ReadOnly = true;
            tLon.Size = new Size(106, 23);
            tLon.TabIndex = 9;
            // 
            // tLat
            // 
            tLat.Dock = DockStyle.Fill;
            tLat.Location = new Point(141, 3);
            tLat.Margin = new Padding(0, 3, 0, 3);
            tLat.Name = "tLat";
            tLat.ReadOnly = true;
            tLat.Size = new Size(106, 23);
            tLat.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(388, 3);
            label1.Margin = new Padding(0, 3, 0, 3);
            label1.Name = "label1";
            label1.Size = new Size(35, 24);
            label1.TabIndex = 11;
            label1.Text = "Typ:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tGPS
            // 
            tGPS.Dock = DockStyle.Fill;
            tGPS.Location = new Point(97, 33);
            tGPS.Margin = new Padding(4, 3, 4, 3);
            tGPS.Name = "tGPS";
            tGPS.Size = new Size(522, 23);
            tGPS.TabIndex = 5;
            tGPS.Text = "Paris";
            // 
            // tName
            // 
            tName.Dock = DockStyle.Fill;
            tName.Location = new Point(97, 63);
            tName.Margin = new Padding(4, 3, 4, 3);
            tName.Name = "tName";
            tName.ReadOnly = true;
            tName.Size = new Size(522, 23);
            tName.TabIndex = 12;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(4, 3);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgv);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(picBox);
            splitContainer1.Size = new Size(615, 215);
            splitContainer1.SplitterDistance = 324;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 4;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.BackgroundColor = SystemColors.ControlLight;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle = dataGridViewCellStyle2;
            dgv.Dock = DockStyle.Fill;
            dgv.Location = new Point(0, 0);
            dgv.Margin = new Padding(4, 3, 4, 3);
            dgv.Name = "dgv";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new Size(324, 215);
            dgv.TabIndex = 4;
            dgv.CellMouseClick += dgv_CellMouseClick;
            dgv.RowPostPaint += dgv_RowPostPaint;
            dgv.KeyDown += dgv_KeyDown;
            // 
            // picBox
            // 
            picBox.BackColor = SystemColors.ControlLight;
            picBox.BorderStyle = BorderStyle.Fixed3D;
            picBox.Dock = DockStyle.Fill;
            picBox.Location = new Point(0, 0);
            picBox.Margin = new Padding(4, 3, 4, 3);
            picBox.Name = "picBox";
            picBox.Size = new Size(286, 215);
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            picBox.TabIndex = 5;
            picBox.TabStop = false;
            // 
            // pgb
            // 
            pgb.Dock = DockStyle.Fill;
            pgb.Location = new Point(4, 316);
            pgb.Margin = new Padding(4, 3, 4, 3);
            pgb.Name = "pgb";
            pgb.Size = new Size(615, 17);
            pgb.TabIndex = 5;
            // 
            // bgw
            // 
            bgw.WorkerReportsProgress = true;
            bgw.DoWork += bgw_DoWork;
            bgw.ProgressChanged += bgw_ProgressChanged;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(623, 336);
            Controls.Add(main);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(639, 375);
            Name = "Form1";
            Text = "Exif Tweaker !";
            DragDrop += Form1_DragDrop;
            DragEnter += Form1_DragEnter;
            main.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBox).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TableLayoutPanel main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bGPS;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox tLat;
        private System.Windows.Forms.TextBox tLon;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TextBox tGPS;
        private System.Windows.Forms.TextBox tName;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.TextBox tType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ProgressBar pgb;
        private System.ComponentModel.BackgroundWorker bgw;
    }
}
