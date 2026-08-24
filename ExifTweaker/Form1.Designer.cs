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
            dateTimePicker1 = new DateTimePicker();
            tGPS = new ComboBox();
            tName = new TextBox();
            splitContainer1 = new SplitContainer();
            dgv = new DataGridView();
            selectedColumn = new DataGridViewCheckBoxColumn();
            thumbnailColumn = new DataGridViewImageColumn();
            fileNameColumn = new DataGridViewTextBoxColumn();
            dateColumn = new DataGridViewTextBoxColumn();
            timezoneColumn = new DataGridViewTextBoxColumn();
            locationColumn = new DataGridViewTextBoxColumn();
            deviceColumn = new DataGridViewTextBoxColumn();
            dimensionsColumn = new DataGridViewTextBoxColumn();
            latitudeColumn = new DataGridViewTextBoxColumn();
            longitudeColumn = new DataGridViewTextBoxColumn();
            altitudeColumn = new DataGridViewTextBoxColumn();
            statusColumn = new DataGridViewTextBoxColumn();
            detailsColumn = new DataGridViewTextBoxColumn();
            picBox = new PictureBox();
            pgb = new ProgressBar();
            mapControl = new ExifTweaker.Controls.MapControl();
            commands = new ToolStrip();
            applyCommand = new ToolStripButton();
            openFolderCommand = new ToolStripMenuItem();
            dateEditorCommand = new ToolStripMenuItem();
            settingsCommand = new ToolStripMenuItem();
            cancelCommand = new ToolStripButton();
            operationStatus = new ToolStripLabel();
            undoCommand = new ToolStripButton();
            redoCommand = new ToolStripButton();
            commandSeparator1 = new ToolStripSeparator();
            resetSelectedCommand = new ToolStripMenuItem();
            resetAllCommand = new ToolStripMenuItem();
            commandSeparator2 = new ToolStripSeparator();
            minusHourCommand = new ToolStripMenuItem();
            plusHourCommand = new ToolStripMenuItem();
            minusMinuteCommand = new ToolStripMenuItem();
            plusMinuteCommand = new ToolStripMenuItem();
            commandSeparator3 = new ToolStripSeparator();
            removeGpsCommand = new ToolStripMenuItem();
            copyGpsCommand = new ToolStripMenuItem();
            pasteGpsCommand = new ToolStripMenuItem();
            reverseGpsCommand = new ToolStripMenuItem();
            mapCommand = new ToolStripMenuItem();
            commandSeparator4 = new ToolStripSeparator();
            allFilterCommand = new ToolStripMenuItem();
            modifiedFilterCommand = new ToolStripMenuItem();
            noGpsFilterCommand = new ToolStripMenuItem();
            noDateFilterCommand = new ToolStripMenuItem();
            errorsFilterCommand = new ToolStripMenuItem();
            commandSeparator5 = new ToolStripSeparator();
            restoreBackupCommand = new ToolStripMenuItem();
            main.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            commands.SuspendLayout();
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
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(dateTimePicker1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(93, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(530, 30);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Location = new Point(4, 3);
            dateTimePicker1.Margin = new Padding(4, 3, 4, 3);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(522, 23);
            dateTimePicker1.TabIndex = 4;
            // 
            // tGPS
            // 
            tGPS.Dock = DockStyle.Fill;
            tGPS.Location = new Point(97, 33);
            tGPS.Margin = new Padding(4, 3, 4, 3);
            tGPS.DropDownStyle = ComboBoxStyle.DropDown;
            tGPS.FormattingEnabled = true;
            tGPS.Name = "tGPS";
            tGPS.Size = new Size(522, 23);
            tGPS.TabIndex = 5;
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
            splitContainer1.Panel2.Controls.Add(mapControl);
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
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.BackgroundColor = SystemColors.ControlLight;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new DataGridViewColumn[] { selectedColumn, thumbnailColumn, fileNameColumn, dateColumn, timezoneColumn, locationColumn, deviceColumn, dimensionsColumn, latitudeColumn, longitudeColumn, altitudeColumn, statusColumn, detailsColumn });
            dgv.RowTemplate.Height = 72;
            // 
            // selectedColumn
            // 
            selectedColumn.DataPropertyName = "IsSelected";
            selectedColumn.HeaderText = "✓";
            selectedColumn.Name = "selectedColumn";
            selectedColumn.Width = 28;
            // 
            // thumbnailColumn
            // 
            thumbnailColumn.HeaderText = "Preview";
            thumbnailColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            thumbnailColumn.Name = "thumbnailColumn";
            thumbnailColumn.Width = 96;
            // 
            // fileNameColumn
            // 
            fileNameColumn.DataPropertyName = "FileName";
            fileNameColumn.HeaderText = "FileName";
            fileNameColumn.Name = "FileName";
            // 
            // dateColumn
            // 
            dateColumn.DataPropertyName = "Date";
            dateColumn.HeaderText = "Date";
            dateColumn.Name = "Date";
            // 
            // timezoneColumn
            // 
            timezoneColumn.DataPropertyName = "Timezone";
            timezoneColumn.HeaderText = "Timezone";
            timezoneColumn.Name = "timezoneColumn";
            // 
            // locationColumn
            // 
            locationColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            locationColumn.DataPropertyName = "Location";
            locationColumn.HeaderText = "Localisation";
            locationColumn.MinimumWidth = 220;
            locationColumn.Name = "locationColumn";
            locationColumn.Width = 280;
            // 
            // deviceColumn
            // 
            deviceColumn.DataPropertyName = "Device";
            deviceColumn.HeaderText = "Device";
            deviceColumn.Name = "deviceColumn";
            // 
            // dimensionsColumn
            // 
            dimensionsColumn.DataPropertyName = "Dimensions";
            dimensionsColumn.HeaderText = "Dimensions";
            dimensionsColumn.Name = "dimensionsColumn";
            // 
            // latitudeColumn
            // 
            latitudeColumn.DataPropertyName = "Latitude";
            latitudeColumn.HeaderText = "Latitude";
            latitudeColumn.Name = "Latitude";
            // 
            // longitudeColumn
            // 
            longitudeColumn.DataPropertyName = "Longitude";
            longitudeColumn.HeaderText = "Longitude";
            longitudeColumn.Name = "Longitude";
            // 
            // altitudeColumn
            // 
            altitudeColumn.DataPropertyName = "Altitude";
            altitudeColumn.HeaderText = "Altitude";
            altitudeColumn.Name = "Altitude";
            // statusColumn
            // 
            statusColumn.DataPropertyName = "Status";
            statusColumn.HeaderText = "Status";
            statusColumn.Name = "Status";
            //
            // detailsColumn
            //
            detailsColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            detailsColumn.DataPropertyName = "Details";
            detailsColumn.HeaderText = "Détails";
            detailsColumn.MinimumWidth = 220;
            detailsColumn.Name = "detailsColumn";
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
            dgv.MultiSelect = true;
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
            dgv.CellFormatting += dgv_CellFormatting;
            dgv.RowPostPaint += dgv_RowPostPaint;
            dgv.KeyDown += dgv_KeyDown;
            // 
            // picBox
            // 
            mapControl.Dock = DockStyle.Fill;
            mapControl.Name = "mapControl";
            mapControl.Visible = false;
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
            // commands
            // 
            commands.Dock = DockStyle.Top;
            commands.GripStyle = ToolStripGripStyle.Hidden;
            commands.Items.AddRange(new ToolStripItem[] { applyCommand, openFolderCommand, dateEditorCommand, settingsCommand, cancelCommand, operationStatus, undoCommand, redoCommand, commandSeparator1, resetSelectedCommand, resetAllCommand, commandSeparator2, minusHourCommand, plusHourCommand, minusMinuteCommand, plusMinuteCommand, commandSeparator3, removeGpsCommand, copyGpsCommand, pasteGpsCommand, reverseGpsCommand, mapCommand, commandSeparator4, allFilterCommand, modifiedFilterCommand, noGpsFilterCommand, noDateFilterCommand, errorsFilterCommand, commandSeparator5, restoreBackupCommand });
            commands.Location = new Point(0, 0);
            commands.Name = "commands";
            commands.Size = new Size(623, 25);
            commands.TabIndex = 4;
            commands.Text = "commands";
            // 
            // applyCommand
            // 
            applyCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            applyCommand.Name = "applyCommand";
            applyCommand.Text = "Apply";
            // 
            // openFolderCommand
            // 
            openFolderCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openFolderCommand.Name = "openFolderCommand";
            openFolderCommand.Text = "Open folder";
            // 
            // dateEditorCommand
            // 
            dateEditorCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            dateEditorCommand.Name = "dateEditorCommand";
            dateEditorCommand.Text = "Date editor";
            // 
            // settingsCommand
            // 
            settingsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            settingsCommand.Name = "settingsCommand";
            settingsCommand.Text = "Settings";
            // 
            // cancelCommand
            // 
            cancelCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            cancelCommand.Enabled = false;
            cancelCommand.Name = "cancelCommand";
            cancelCommand.Text = "Cancel";
            // 
            // operationStatus
            // 
            operationStatus.Alignment = ToolStripItemAlignment.Right;
            operationStatus.Name = "operationStatus";
            operationStatus.Text = "Ready";
            // 
            // undoCommand
            // 
            undoCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            undoCommand.Name = "undoCommand";
            undoCommand.Text = "Undo";
            // 
            // redoCommand
            // 
            redoCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            redoCommand.Name = "redoCommand";
            redoCommand.Text = "Redo";
            // 
            // resetSelectedCommand
            // 
            resetSelectedCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resetSelectedCommand.Name = "resetSelectedCommand";
            resetSelectedCommand.Text = "Reset selected";
            // 
            // resetAllCommand
            // 
            resetAllCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resetAllCommand.Name = "resetAllCommand";
            resetAllCommand.Text = "Reset all";
            // 
            // minusHourCommand
            // 
            minusHourCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            minusHourCommand.Name = "minusHourCommand";
            minusHourCommand.Text = "-1 hour";
            // 
            // plusHourCommand
            // 
            plusHourCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            plusHourCommand.Name = "plusHourCommand";
            plusHourCommand.Text = "+1 hour";
            // 
            // minusMinuteCommand
            // 
            minusMinuteCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            minusMinuteCommand.Name = "minusMinuteCommand";
            minusMinuteCommand.Text = "-1 minute";
            // 
            // plusMinuteCommand
            // 
            plusMinuteCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            plusMinuteCommand.Name = "plusMinuteCommand";
            plusMinuteCommand.Text = "+1 minute";
            // 
            // removeGpsCommand
            // 
            removeGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            removeGpsCommand.Name = "removeGpsCommand";
            removeGpsCommand.Text = "Remove GPS";
            // 
            // copyGpsCommand
            // 
            copyGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            copyGpsCommand.Name = "copyGpsCommand";
            copyGpsCommand.Text = "Copy GPS";
            // 
            // pasteGpsCommand
            // 
            pasteGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pasteGpsCommand.Name = "pasteGpsCommand";
            pasteGpsCommand.Text = "Paste GPS";
            // 
            // reverseGpsCommand
            // 
            reverseGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            reverseGpsCommand.Name = "reverseGpsCommand";
            reverseGpsCommand.Text = "Reverse GPS";
            // 
            // mapCommand
            // 
            mapCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mapCommand.Name = "mapCommand";
            mapCommand.Text = "Map";
            // 
            // allFilterCommand
            // 
            allFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            allFilterCommand.Name = "allFilterCommand";
            allFilterCommand.Text = "All";
            // 
            // modifiedFilterCommand
            // 
            modifiedFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            modifiedFilterCommand.Name = "modifiedFilterCommand";
            modifiedFilterCommand.Text = "Modified";
            // 
            // noGpsFilterCommand
            // 
            noGpsFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            noGpsFilterCommand.Name = "noGpsFilterCommand";
            noGpsFilterCommand.Text = "No GPS";
            // 
            // noDateFilterCommand
            // 
            noDateFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            noDateFilterCommand.Name = "noDateFilterCommand";
            noDateFilterCommand.Text = "No date";
            // 
            // errorsFilterCommand
            // 
            errorsFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            errorsFilterCommand.Name = "errorsFilterCommand";
            errorsFilterCommand.Text = "Errors";
            // 
            // restoreBackupCommand
            // 
            restoreBackupCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            restoreBackupCommand.Name = "restoreBackupCommand";
            restoreBackupCommand.Text = "Restore backup";
            // 
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            Controls.Add(main);
            Controls.Add(commands);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(900, 560);
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
            commands.ResumeLayout(false);
            commands.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TableLayoutPanel main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bGPS;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ComboBox tGPS;
        private System.Windows.Forms.TextBox tName;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn selectedColumn;
        private System.Windows.Forms.DataGridViewImageColumn thumbnailColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timezoneColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dimensionsColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn latitudeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn longitudeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn altitudeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn detailsColumn;
        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ProgressBar pgb;
        private ExifTweaker.Controls.MapControl mapControl;
        private System.Windows.Forms.ToolStrip commands;
        private System.Windows.Forms.ToolStripButton applyCommand;
        private System.Windows.Forms.ToolStripMenuItem openFolderCommand;
        private System.Windows.Forms.ToolStripMenuItem dateEditorCommand;
        private System.Windows.Forms.ToolStripMenuItem settingsCommand;
        private System.Windows.Forms.ToolStripButton cancelCommand;
        private System.Windows.Forms.ToolStripLabel operationStatus;
        private System.Windows.Forms.ToolStripButton undoCommand;
        private System.Windows.Forms.ToolStripButton redoCommand;
        private System.Windows.Forms.ToolStripSeparator commandSeparator1;
        private System.Windows.Forms.ToolStripMenuItem resetSelectedCommand;
        private System.Windows.Forms.ToolStripMenuItem resetAllCommand;
        private System.Windows.Forms.ToolStripSeparator commandSeparator2;
        private System.Windows.Forms.ToolStripMenuItem minusHourCommand;
        private System.Windows.Forms.ToolStripMenuItem plusHourCommand;
        private System.Windows.Forms.ToolStripMenuItem minusMinuteCommand;
        private System.Windows.Forms.ToolStripMenuItem plusMinuteCommand;
        private System.Windows.Forms.ToolStripSeparator commandSeparator3;
        private System.Windows.Forms.ToolStripMenuItem removeGpsCommand;
        private System.Windows.Forms.ToolStripMenuItem copyGpsCommand;
        private System.Windows.Forms.ToolStripMenuItem pasteGpsCommand;
        private System.Windows.Forms.ToolStripMenuItem reverseGpsCommand;
        private System.Windows.Forms.ToolStripMenuItem mapCommand;
        private System.Windows.Forms.ToolStripSeparator commandSeparator4;
        private System.Windows.Forms.ToolStripMenuItem allFilterCommand;
        private System.Windows.Forms.ToolStripMenuItem modifiedFilterCommand;
        private System.Windows.Forms.ToolStripMenuItem noGpsFilterCommand;
        private System.Windows.Forms.ToolStripMenuItem noDateFilterCommand;
        private System.Windows.Forms.ToolStripMenuItem errorsFilterCommand;
        private System.Windows.Forms.ToolStripSeparator commandSeparator5;
        private System.Windows.Forms.ToolStripMenuItem restoreBackupCommand;
    }
}
