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
            bOpen = new Button();
            main = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            bGPS = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            dateTimePicker1 = new ExifTweaker.Controls.ThemedDateTimeInput();
            tGPS = new TextBox();
            pgb = new ProgressBar();
            applyAllButton = new Button();
            dateGroup = new GroupBox();
            gpsGroup = new GroupBox();
            immichGroup = new GroupBox();
            gpsLayout = new TableLayoutPanel();
            immichLayout = new TableLayoutPanel();
            applyPanel = new TableLayoutPanel();
            dateButtons = new FlowLayoutPanel();
            gpsButtons = new FlowLayoutPanel();
            immichButtons = new FlowLayoutPanel();
            bOpenAll = new Button();
            bGPSAll = new Button();
            immichAlbum = new ComboBox();
            immichNewAlbum = new TextBox();
            immichSendSelected = new Button();
            immichSendAll = new Button();
            splitContainer1 = new SplitContainer();
            dgv = new DataGridView();
            thumbnailColumn = new DataGridViewImageColumn();
            picBox = new PictureBox();
            mapControl = new ExifTweaker.Controls.MapControl();
            commands = new ToolStrip();
            openQuickCommand = new ToolStripDropDownButton();
            openFilesQuickItem = new ToolStripMenuItem();
            openFolderQuickItem = new ToolStripMenuItem();
            dateQuickCommand = new ToolStripButton();
            locationQuickCommand = new ToolStripDropDownButton();
            findGpsQuickItem = new ToolStripMenuItem();
            menuSeparator14 = new ToolStripSeparator();
            copyGpsQuickItem = new ToolStripMenuItem();
            pasteGpsQuickItem = new ToolStripMenuItem();
            menuSeparator15 = new ToolStripSeparator();
            removeGpsQuickItem = new ToolStripMenuItem();
            reverseGpsQuickItem = new ToolStripMenuItem();
            mapQuickCommand = new ToolStripButton();
            commandSeparator1 = new ToolStripSeparator();
            undoCommand = new ToolStripButton();
            redoCommand = new ToolStripButton();
            filterQuickCommand = new ToolStripDropDownButton();
            allFilterQuickItem = new ToolStripMenuItem();
            modifiedFilterQuickItem = new ToolStripMenuItem();
            noGpsFilterQuickItem = new ToolStripMenuItem();
            noDateFilterQuickItem = new ToolStripMenuItem();
            errorsFilterQuickItem = new ToolStripMenuItem();
            immichQuickCommand = new ToolStripDropDownButton();
            uploadImmichSelectedQuickItem = new ToolStripMenuItem();
            uploadImmichAllQuickItem = new ToolStripMenuItem();
            commandSeparator2 = new ToolStripSeparator();
            cancelCommand = new ToolStripButton();
            operationStatus = new ToolStripLabel();
            openFolderCommand = new ToolStripMenuItem();
            dateEditorCommand = new ToolStripMenuItem();
            settingsCommand = new ToolStripMenuItem();
            resetSelectedCommand = new ToolStripMenuItem();
            resetAllCommand = new ToolStripMenuItem();
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
            navigationMenu = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            openFilesMenuItem = new ToolStripMenuItem();
            menuSeparator1 = new ToolStripSeparator();
            removeFromSessionMenuItem = new ToolStripMenuItem();
            menuSeparator2 = new ToolStripSeparator();
            menuSeparator3 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();
            editMenu = new ToolStripMenuItem();
            undoMenuItem = new ToolStripMenuItem();
            redoMenuItem = new ToolStripMenuItem();
            menuSeparator4 = new ToolStripSeparator();
            selectAllMenuItem = new ToolStripMenuItem();
            dateMenu = new ToolStripMenuItem();
            menuSeparator5 = new ToolStripSeparator();
            locationMenu = new ToolStripMenuItem();
            findGpsMenuItem = new ToolStripMenuItem();
            menuSeparator6 = new ToolStripSeparator();
            menuSeparator7 = new ToolStripSeparator();
            viewMenu = new ToolStripMenuItem();
            previewMenuItem = new ToolStripMenuItem();
            informationMenuItem = new ToolStripMenuItem();
            menuSeparator8 = new ToolStripSeparator();
            quickActionsMenuItem = new ToolStripMenuItem();
            menuSeparator9 = new ToolStripSeparator();
            filterMenu = new ToolStripMenuItem();
            actionsMenu = new ToolStripMenuItem();
            applyMenuItem = new ToolStripMenuItem();
            applySelectedMenuItem = new ToolStripMenuItem();
            menuSeparator10 = new ToolStripSeparator();
            menuSeparator11 = new ToolStripSeparator();
            uploadImmichSelectedMenuItem = new ToolStripMenuItem();
            uploadImmichAllMenuItem = new ToolStripMenuItem();
            menuSeparator12 = new ToolStripSeparator();
            cancelMenuItem = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            guideMenuItem = new ToolStripMenuItem();
            logsMenuItem = new ToolStripMenuItem();
            verifyExifToolMenuItem = new ToolStripMenuItem();
            checkUpdatesMenuItem = new ToolStripMenuItem();
            menuSeparator13 = new ToolStripSeparator();
            aboutMenuItem = new ToolStripMenuItem();
            quickActionsToggleItem = new ToolStripMenuItem();
            main.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            dateGroup.SuspendLayout();
            gpsGroup.SuspendLayout();
            immichGroup.SuspendLayout();
            gpsLayout.SuspendLayout();
            immichLayout.SuspendLayout();
            applyPanel.SuspendLayout();
            dateButtons.SuspendLayout();
            gpsButtons.SuspendLayout();
            immichButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            commands.SuspendLayout();
            navigationMenu.SuspendLayout();
            SuspendLayout();
            // 
            // bOpen
            // 
            bOpen.AutoSize = true;
            bOpen.Enabled = false;
            bOpen.Margin = new Padding(6, 3, 0, 3);
            bOpen.Name = "bOpen";
            bOpen.Size = new Size(250, 28);
            bOpen.TabIndex = 0;
            bOpen.Text = "PRÉPARER LA SÉLECTION (0)";
            bOpen.UseVisualStyleBackColor = true;
            bOpen.Click += PrepareDateForSelection;
            // 
            // bOpenAll
            // 
            bOpenAll.AutoSize = true;
            bOpenAll.Enabled = false;
            bOpenAll.Margin = new Padding(6, 3, 0, 3);
            bOpenAll.Name = "bOpenAll";
            bOpenAll.Size = new Size(210, 28);
            bOpenAll.TabIndex = 1;
            bOpenAll.Text = "PRÉPARER TOUT (0)";
            bOpenAll.UseVisualStyleBackColor = true;
            bOpenAll.Click += PrepareDateForAll;
            // 
            // main
            // 
            main.ColumnCount = 1;
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            main.Controls.Add(tableLayoutPanel2, 0, 1);
            main.Controls.Add(splitContainer1, 0, 0);
            main.Dock = DockStyle.Fill;
            main.Location = new Point(0, 24);
            main.Margin = new Padding(4, 3, 4, 3);
            main.Name = "main";
            main.RowCount = 2;
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 226F));
            main.Size = new Size(1200, 696);
            main.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(dateGroup, 0, 0);
            tableLayoutPanel2.Controls.Add(gpsGroup, 0, 1);
            tableLayoutPanel2.Controls.Add(immichGroup, 0, 2);
            tableLayoutPanel2.Controls.Add(applyPanel, 0, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 470);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1200, 226);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // dateGroup
            // 
            dateGroup.Controls.Add(tableLayoutPanel3);
            dateGroup.Dock = DockStyle.Fill;
            dateGroup.Margin = new Padding(4, 0, 4, 2);
            dateGroup.Name = "dateGroup";
            dateGroup.Padding = new Padding(6, 2, 6, 4);
            dateGroup.TabIndex = 0;
            dateGroup.TabStop = false;
            dateGroup.Text = "Date et heure";
            // 
            // gpsGroup
            // 
            gpsGroup.Controls.Add(gpsLayout);
            gpsGroup.Dock = DockStyle.Fill;
            gpsGroup.Margin = new Padding(4, 0, 4, 2);
            gpsGroup.Name = "gpsGroup";
            gpsGroup.Padding = new Padding(6, 2, 6, 4);
            gpsGroup.TabIndex = 1;
            gpsGroup.TabStop = false;
            gpsGroup.Text = "Localisation GPS";
            // 
            // immichGroup
            // 
            immichGroup.Controls.Add(immichLayout);
            immichGroup.Dock = DockStyle.Fill;
            immichGroup.Margin = new Padding(4, 0, 4, 2);
            immichGroup.Name = "immichGroup";
            immichGroup.Padding = new Padding(6, 2, 6, 4);
            immichGroup.TabIndex = 2;
            immichGroup.TabStop = false;
            immichGroup.Text = "Immich";
            // 
            // bGPS
            // 
            bGPS.AutoSize = true;
            bGPS.Enabled = false;
            bGPS.Margin = new Padding(6, 3, 0, 3);
            bGPS.Name = "bGPS";
            bGPS.Size = new Size(250, 28);
            bGPS.TabIndex = 7;
            bGPS.Text = "PRÉPARER LA SÉLECTION (0)";
            bGPS.UseVisualStyleBackColor = true;
            bGPS.Click += PrepareGpsForSelection;
            // 
            // bGPSAll
            // 
            bGPSAll.AutoSize = true;
            bGPSAll.Enabled = false;
            bGPSAll.Margin = new Padding(6, 3, 0, 3);
            bGPSAll.Name = "bGPSAll";
            bGPSAll.Size = new Size(210, 28);
            bGPSAll.TabIndex = 8;
            bGPSAll.Text = "PRÉPARER TOUT (0)";
            bGPSAll.UseVisualStyleBackColor = true;
            bGPSAll.Click += PrepareGpsForAll;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel3.Controls.Add(dateTimePicker1, 0, 0);
            tableLayoutPanel3.Controls.Add(dateButtons, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(6, 18);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.TabIndex = 1;
            // 
            // dateButtons
            // 
            dateButtons.AutoSize = true;
            dateButtons.Controls.Add(bOpen);
            dateButtons.Controls.Add(bOpenAll);
            dateButtons.Dock = DockStyle.Fill;
            dateButtons.FlowDirection = FlowDirection.LeftToRight;
            dateButtons.Margin = new Padding(0);
            dateButtons.Name = "dateButtons";
            dateButtons.TabIndex = 2;
            dateButtons.WrapContents = false;
            // 
            // gpsLayout
            // 
            gpsLayout.ColumnCount = 2;
            gpsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            gpsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            gpsLayout.Controls.Add(tGPS, 0, 0);
            gpsLayout.Controls.Add(gpsButtons, 1, 0);
            gpsLayout.Dock = DockStyle.Fill;
            gpsLayout.Margin = new Padding(0);
            gpsLayout.Name = "gpsLayout";
            gpsLayout.RowCount = 1;
            gpsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gpsLayout.TabIndex = 0;
            // 
            // gpsButtons
            // 
            gpsButtons.AutoSize = true;
            gpsButtons.Controls.Add(bGPS);
            gpsButtons.Controls.Add(bGPSAll);
            gpsButtons.Dock = DockStyle.Fill;
            gpsButtons.FlowDirection = FlowDirection.LeftToRight;
            gpsButtons.Margin = new Padding(0);
            gpsButtons.Name = "gpsButtons";
            gpsButtons.TabIndex = 1;
            gpsButtons.WrapContents = false;
            // 
            // immichLayout
            // 
            immichLayout.ColumnCount = 3;
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            immichLayout.Controls.Add(immichAlbum, 0, 0);
            immichLayout.Controls.Add(immichNewAlbum, 1, 0);
            immichLayout.Controls.Add(immichButtons, 2, 0);
            immichLayout.Dock = DockStyle.Fill;
            immichLayout.Margin = new Padding(0);
            immichLayout.Name = "immichLayout";
            immichLayout.RowCount = 1;
            immichLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            immichLayout.TabIndex = 0;
            // 
            // immichAlbum
            // 
            immichAlbum.Dock = DockStyle.Top;
            immichAlbum.DropDownStyle = ComboBoxStyle.DropDownList;
            immichAlbum.Enabled = false;
            immichAlbum.Margin = new Padding(0, 3, 6, 3);
            immichAlbum.Name = "immichAlbum";
            immichAlbum.TabIndex = 0;
            immichAlbum.SelectedIndexChanged += immichAlbum_SelectedIndexChanged;
            immichAlbum.DropDown += immichAlbum_DropDown;
            // 
            // immichNewAlbum
            // 
            immichNewAlbum.Dock = DockStyle.Top;
            immichNewAlbum.Enabled = false;
            immichNewAlbum.Margin = new Padding(0, 3, 6, 3);
            immichNewAlbum.Name = "immichNewAlbum";
            immichNewAlbum.PlaceholderText = "Nom du nouvel album";
            immichNewAlbum.TabIndex = 1;
            // 
            // immichButtons
            // 
            immichButtons.AutoSize = true;
            immichButtons.Controls.Add(immichSendSelected);
            immichButtons.Controls.Add(immichSendAll);
            immichButtons.Dock = DockStyle.Fill;
            immichButtons.FlowDirection = FlowDirection.LeftToRight;
            immichButtons.Margin = new Padding(0);
            immichButtons.Name = "immichButtons";
            immichButtons.TabIndex = 2;
            immichButtons.WrapContents = false;
            // 
            // immichSendSelected
            // 
            immichSendSelected.AutoSize = true;
            immichSendSelected.Enabled = false;
            immichSendSelected.Margin = new Padding(6, 3, 0, 3);
            immichSendSelected.Name = "immichSendSelected";
            immichSendSelected.Size = new Size(250, 28);
            immichSendSelected.TabIndex = 0;
            immichSendSelected.Text = "ENVOYER LA SÉLECTION (0)";
            immichSendSelected.UseVisualStyleBackColor = true;
            immichSendSelected.Click += uploadImmichSelected_Click;
            // 
            // immichSendAll
            // 
            immichSendAll.AutoSize = true;
            immichSendAll.Enabled = false;
            immichSendAll.Margin = new Padding(6, 3, 0, 3);
            immichSendAll.Name = "immichSendAll";
            immichSendAll.Size = new Size(210, 28);
            immichSendAll.TabIndex = 1;
            immichSendAll.Text = "ENVOYER TOUT (0)";
            immichSendAll.UseVisualStyleBackColor = true;
            immichSendAll.Click += uploadImmichAll_Click;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Top;
            dateTimePicker1.Margin = new Padding(0, 3, 6, 3);
            dateTimePicker1.Mask = "0000-00-00 00:00:00";
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.PromptChar = ' ';
            dateTimePicker1.TabIndex = 4;
            dateTimePicker1.Text = "20260825115954";
            dateTimePicker1.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            // 
            // tGPS
            // 
            tGPS.Dock = DockStyle.Top;
            tGPS.Margin = new Padding(0, 3, 6, 3);
            tGPS.Name = "tGPS";
            tGPS.PlaceholderText = "Rechercher un lieu ou saisir des coordonnées…";
            tGPS.TabIndex = 5;
            tGPS.TextChanged += tGPS_TextChanged;
            tGPS.KeyDown += GpsSearchKeyDown;
            // 
            // applyPanel
            // 
            applyPanel.ColumnCount = 2;
            applyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            applyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            applyPanel.Controls.Add(pgb, 0, 0);
            applyPanel.Controls.Add(applyAllButton, 1, 0);
            applyPanel.Dock = DockStyle.Fill;
            applyPanel.Margin = new Padding(4, 0, 4, 0);
            applyPanel.Name = "applyPanel";
            applyPanel.RowCount = 1;
            applyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            applyPanel.TabIndex = 3;
            // 
            // pgb
            // 
            pgb.Dock = DockStyle.Fill;
            pgb.Margin = new Padding(0, 3, 6, 3);
            pgb.Name = "pgb";
            pgb.TabIndex = 5;
            // 
            // applyAllButton
            // 
            applyAllButton.AutoSize = true;
            applyAllButton.Dock = DockStyle.Fill;
            applyAllButton.Enabled = false;
            applyAllButton.Margin = new Padding(6, 2, 0, 2);
            applyAllButton.Name = "applyAllButton";
            applyAllButton.Size = new Size(466, 28);
            applyAllButton.TabIndex = 8;
            applyAllButton.Text = "VÉRIFIER ET APPLIQUER TOUT (0)";
            applyAllButton.UseVisualStyleBackColor = true;
            applyAllButton.Click += applyAllButton_Click;
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
            splitContainer1.Size = new Size(1192, 598);
            splitContainer1.SplitterDistance = 627;
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
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
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
            dgv.Columns.AddRange(new DataGridViewColumn[] { thumbnailColumn });
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
            dgv.RowTemplate.Height = 72;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new Size(627, 598);
            dgv.TabIndex = 4;
            dgv.CellFormatting += dgv_CellFormatting;
            dgv.CellMouseClick += dgv_CellMouseClick;
            dgv.RowPostPaint += dgv_RowPostPaint;
            dgv.SelectionChanged += dgv_SelectionChanged;
            dgv.KeyDown += dgv_KeyDown;
            // 
            // thumbnailColumn
            // 
            thumbnailColumn.HeaderText = "Preview";
            thumbnailColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            thumbnailColumn.Name = "thumbnailColumn";
            thumbnailColumn.Width = 51;
            // 
            // picBox
            // 
            picBox.BackColor = SystemColors.ControlLight;
            picBox.BorderStyle = BorderStyle.Fixed3D;
            picBox.Dock = DockStyle.Fill;
            picBox.Location = new Point(0, 0);
            picBox.Margin = new Padding(4, 3, 4, 3);
            picBox.Name = "picBox";
            picBox.Size = new Size(560, 598);
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            picBox.TabIndex = 5;
            picBox.TabStop = false;
            // 
            // mapControl
            // 
            mapControl.Dock = DockStyle.Fill;
            mapControl.Location = new Point(0, 0);
            mapControl.Name = "mapControl";
            mapControl.Size = new Size(560, 598);
            mapControl.TabIndex = 6;
            mapControl.Visible = false;
            // 
            // commands
            // 
            commands.GripStyle = ToolStripGripStyle.Hidden;
            commands.Items.AddRange(new ToolStripItem[] { openQuickCommand, dateQuickCommand, locationQuickCommand, mapQuickCommand, commandSeparator1, undoCommand, redoCommand, filterQuickCommand, immichQuickCommand, commandSeparator2, cancelCommand, operationStatus });
            commands.Location = new Point(0, 24);
            commands.Name = "commands";
            commands.Padding = new Padding(4, 2, 4, 2);
            commands.Size = new Size(1200, 27);
            commands.TabIndex = 4;
            commands.Text = "commands";
            commands.Visible = false;
            // 
            // openQuickCommand
            // 
            openQuickCommand.DropDownItems.AddRange(new ToolStripItem[] { openFilesQuickItem, openFolderQuickItem });
            openQuickCommand.Name = "openQuickCommand";
            openQuickCommand.Size = new Size(53, 20);
            openQuickCommand.Text = "Ouvrir";
            // 
            // openFilesQuickItem
            // 
            openFilesQuickItem.Name = "openFilesQuickItem";
            openFilesQuickItem.Size = new Size(178, 22);
            openFilesQuickItem.Text = "Ouvrir des fichiers…";
            openFilesQuickItem.Click += openFiles_Click;
            // 
            // openFolderQuickItem
            // 
            openFolderQuickItem.Name = "openFolderQuickItem";
            openFolderQuickItem.Size = new Size(178, 22);
            openFolderQuickItem.Text = "Ouvrir un dossier…";
            openFolderQuickItem.Click += openFolderCommand_Click;
            // 
            // dateQuickCommand
            // 
            dateQuickCommand.Name = "dateQuickCommand";
            dateQuickCommand.Size = new Size(81, 20);
            dateQuickCommand.Text = "Date et heure";
            dateQuickCommand.Click += dateEditorCommand_Click;
            // 
            // locationQuickCommand
            // 
            locationQuickCommand.DropDownItems.AddRange(new ToolStripItem[] { findGpsQuickItem, menuSeparator14, copyGpsQuickItem, pasteGpsQuickItem, menuSeparator15, removeGpsQuickItem, reverseGpsQuickItem });
            locationQuickCommand.Name = "locationQuickCommand";
            locationQuickCommand.Size = new Size(83, 20);
            locationQuickCommand.Text = "Localisation";
            // 
            // findGpsQuickItem
            // 
            findGpsQuickItem.Name = "findGpsQuickItem";
            findGpsQuickItem.Size = new Size(237, 22);
            findGpsQuickItem.Text = "Rechercher un lieu…";
            findGpsQuickItem.Click += findGps_Click;
            // 
            // menuSeparator14
            // 
            menuSeparator14.Name = "menuSeparator14";
            menuSeparator14.Size = new Size(234, 6);
            // 
            // copyGpsQuickItem
            // 
            copyGpsQuickItem.Name = "copyGpsQuickItem";
            copyGpsQuickItem.Size = new Size(237, 22);
            copyGpsQuickItem.Text = "Copier le GPS";
            copyGpsQuickItem.Click += copyGpsCommand_Click;
            // 
            // pasteGpsQuickItem
            // 
            pasteGpsQuickItem.Name = "pasteGpsQuickItem";
            pasteGpsQuickItem.Size = new Size(237, 22);
            pasteGpsQuickItem.Text = "Coller le GPS";
            pasteGpsQuickItem.Click += pasteGpsCommand_Click;
            // 
            // menuSeparator15
            // 
            menuSeparator15.Name = "menuSeparator15";
            menuSeparator15.Size = new Size(234, 6);
            // 
            // removeGpsQuickItem
            // 
            removeGpsQuickItem.Name = "removeGpsQuickItem";
            removeGpsQuickItem.Size = new Size(237, 22);
            removeGpsQuickItem.Text = "Préparer la suppression du GPS";
            removeGpsQuickItem.Click += removeGpsCommand_Click;
            // 
            // reverseGpsQuickItem
            // 
            reverseGpsQuickItem.Name = "reverseGpsQuickItem";
            reverseGpsQuickItem.Size = new Size(237, 22);
            reverseGpsQuickItem.Text = "Identifier les coordonnées";
            reverseGpsQuickItem.Click += reverseGpsCommand_Click;
            // 
            // mapQuickCommand
            // 
            mapQuickCommand.Name = "mapQuickCommand";
            mapQuickCommand.Size = new Size(39, 20);
            mapQuickCommand.Text = "Carte";
            mapQuickCommand.Click += mapCommand_Click;
            // 
            // commandSeparator1
            // 
            commandSeparator1.Name = "commandSeparator1";
            commandSeparator1.Size = new Size(6, 23);
            // 
            // undoCommand
            // 
            undoCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            undoCommand.Name = "undoCommand";
            undoCommand.Size = new Size(53, 20);
            undoCommand.Text = "Annuler";
            undoCommand.Click += undoCommand_Click;
            // 
            // redoCommand
            // 
            redoCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            redoCommand.Name = "redoCommand";
            redoCommand.Size = new Size(51, 20);
            redoCommand.Text = "Rétablir";
            redoCommand.Click += redoCommand_Click;
            // 
            // filterQuickCommand
            // 
            filterQuickCommand.DropDownItems.AddRange(new ToolStripItem[] { allFilterQuickItem, modifiedFilterQuickItem, noGpsFilterQuickItem, noDateFilterQuickItem, errorsFilterQuickItem });
            filterQuickCommand.Name = "filterQuickCommand";
            filterQuickCommand.Size = new Size(80, 20);
            filterQuickCommand.Text = "Filtre : Tous";
            // 
            // allFilterQuickItem
            // 
            allFilterQuickItem.Name = "allFilterQuickItem";
            allFilterQuickItem.Size = new Size(124, 22);
            allFilterQuickItem.Text = "Tous";
            allFilterQuickItem.Click += allFilterCommand_Click;
            // 
            // modifiedFilterQuickItem
            // 
            modifiedFilterQuickItem.Name = "modifiedFilterQuickItem";
            modifiedFilterQuickItem.Size = new Size(124, 22);
            modifiedFilterQuickItem.Text = "Modifiés";
            modifiedFilterQuickItem.Click += modifiedFilterCommand_Click;
            // 
            // noGpsFilterQuickItem
            // 
            noGpsFilterQuickItem.Name = "noGpsFilterQuickItem";
            noGpsFilterQuickItem.Size = new Size(124, 22);
            noGpsFilterQuickItem.Text = "Sans GPS";
            noGpsFilterQuickItem.Click += noGpsFilterCommand_Click;
            // 
            // noDateFilterQuickItem
            // 
            noDateFilterQuickItem.Name = "noDateFilterQuickItem";
            noDateFilterQuickItem.Size = new Size(124, 22);
            noDateFilterQuickItem.Text = "Sans date";
            noDateFilterQuickItem.Click += noDateFilterCommand_Click;
            // 
            // errorsFilterQuickItem
            // 
            errorsFilterQuickItem.Name = "errorsFilterQuickItem";
            errorsFilterQuickItem.Size = new Size(124, 22);
            errorsFilterQuickItem.Text = "Erreurs";
            errorsFilterQuickItem.Click += errorsFilterCommand_Click;
            // 
            // immichQuickCommand
            // 
            immichQuickCommand.DropDownItems.AddRange(new ToolStripItem[] { uploadImmichSelectedQuickItem, uploadImmichAllQuickItem });
            immichQuickCommand.Name = "immichQuickCommand";
            immichQuickCommand.Size = new Size(61, 20);
            immichQuickCommand.Text = "Immich";
            // 
            // uploadImmichSelectedQuickItem
            // 
            uploadImmichSelectedQuickItem.Name = "uploadImmichSelectedQuickItem";
            uploadImmichSelectedQuickItem.Size = new Size(236, 22);
            uploadImmichSelectedQuickItem.Text = "Envoyer la sélection… (0)";
            uploadImmichSelectedQuickItem.Click += uploadImmichSelected_Click;
            // 
            // uploadImmichAllQuickItem
            // 
            uploadImmichAllQuickItem.Name = "uploadImmichAllQuickItem";
            uploadImmichAllQuickItem.Size = new Size(236, 22);
            uploadImmichAllQuickItem.Text = "Envoyer toutes les images… (0)";
            uploadImmichAllQuickItem.Click += uploadImmichAll_Click;
            // 
            // commandSeparator2
            // 
            commandSeparator2.Name = "commandSeparator2";
            commandSeparator2.Size = new Size(6, 23);
            // 
            // cancelCommand
            // 
            cancelCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            cancelCommand.Enabled = false;
            cancelCommand.Name = "cancelCommand";
            cancelCommand.Size = new Size(182, 20);
            cancelCommand.Text = "Interrompre l’opération en cours";
            cancelCommand.Click += cancelCommand_Click;
            // 
            // operationStatus
            // 
            operationStatus.Alignment = ToolStripItemAlignment.Right;
            operationStatus.Name = "operationStatus";
            operationStatus.Size = new Size(39, 20);
            operationStatus.Text = "Ready";
            // 
            // openFolderCommand
            // 
            openFolderCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openFolderCommand.Name = "openFolderCommand";
            openFolderCommand.Size = new Size(221, 22);
            openFolderCommand.Text = "Ouvrir un dossier…";
            openFolderCommand.Click += openFolderCommand_Click;
            // 
            // dateEditorCommand
            // 
            dateEditorCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            dateEditorCommand.Name = "dateEditorCommand";
            dateEditorCommand.Size = new Size(209, 22);
            dateEditorCommand.Text = "Ouvrir l’éditeur complet…";
            dateEditorCommand.Click += dateEditorCommand_Click;
            // 
            // settingsCommand
            // 
            settingsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            settingsCommand.Name = "settingsCommand";
            settingsCommand.Size = new Size(221, 22);
            settingsCommand.Text = "Paramètres…";
            settingsCommand.Click += settingsCommand_Click;
            // 
            // resetSelectedCommand
            // 
            resetSelectedCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resetSelectedCommand.Name = "resetSelectedCommand";
            resetSelectedCommand.Size = new Size(304, 22);
            resetSelectedCommand.Text = "Restaurer la sélection";
            resetSelectedCommand.Click += resetSelectedCommand_Click;
            // 
            // resetAllCommand
            // 
            resetAllCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resetAllCommand.Name = "resetAllCommand";
            resetAllCommand.Size = new Size(304, 22);
            resetAllCommand.Text = "Restaurer tout";
            resetAllCommand.Click += resetAllCommand_Click;
            // 
            // minusHourCommand
            // 
            minusHourCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            minusHourCommand.Name = "minusHourCommand";
            minusHourCommand.Size = new Size(209, 22);
            minusHourCommand.Text = "Reculer d’une heure";
            minusHourCommand.Click += minusHourCommand_Click;
            // 
            // plusHourCommand
            // 
            plusHourCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            plusHourCommand.Name = "plusHourCommand";
            plusHourCommand.Size = new Size(209, 22);
            plusHourCommand.Text = "Avancer d’une heure";
            plusHourCommand.Click += plusHourCommand_Click;
            // 
            // minusMinuteCommand
            // 
            minusMinuteCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            minusMinuteCommand.Name = "minusMinuteCommand";
            minusMinuteCommand.Size = new Size(209, 22);
            minusMinuteCommand.Text = "Reculer d’une minute";
            minusMinuteCommand.Click += minusMinuteCommand_Click;
            // 
            // plusMinuteCommand
            // 
            plusMinuteCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            plusMinuteCommand.Name = "plusMinuteCommand";
            plusMinuteCommand.Size = new Size(209, 22);
            plusMinuteCommand.Text = "Avancer d’une minute";
            plusMinuteCommand.Click += plusMinuteCommand_Click;
            // 
            // commandSeparator3
            // 
            commandSeparator3.Name = "commandSeparator3";
            commandSeparator3.Size = new Size(6, 6);
            // 
            // removeGpsCommand
            // 
            removeGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            removeGpsCommand.Name = "removeGpsCommand";
            removeGpsCommand.Size = new Size(237, 22);
            removeGpsCommand.Text = "Préparer la suppression du GPS";
            removeGpsCommand.Click += removeGpsCommand_Click;
            // 
            // copyGpsCommand
            // 
            copyGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            copyGpsCommand.Name = "copyGpsCommand";
            copyGpsCommand.Size = new Size(237, 22);
            copyGpsCommand.Text = "Copier le GPS";
            copyGpsCommand.Click += copyGpsCommand_Click;
            // 
            // pasteGpsCommand
            // 
            pasteGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pasteGpsCommand.Name = "pasteGpsCommand";
            pasteGpsCommand.Size = new Size(237, 22);
            pasteGpsCommand.Text = "Coller le GPS";
            pasteGpsCommand.Click += pasteGpsCommand_Click;
            // 
            // reverseGpsCommand
            // 
            reverseGpsCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            reverseGpsCommand.Name = "reverseGpsCommand";
            reverseGpsCommand.Size = new Size(237, 22);
            reverseGpsCommand.Text = "Identifier les coordonnées";
            reverseGpsCommand.Click += reverseGpsCommand_Click;
            // 
            // mapCommand
            // 
            mapCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mapCommand.Name = "mapCommand";
            mapCommand.Size = new Size(250, 22);
            mapCommand.Text = "Afficher la carte";
            mapCommand.Click += mapCommand_Click;
            // 
            // commandSeparator4
            // 
            commandSeparator4.Name = "commandSeparator4";
            commandSeparator4.Size = new Size(6, 6);
            // 
            // allFilterCommand
            // 
            allFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            allFilterCommand.Name = "allFilterCommand";
            allFilterCommand.Size = new Size(124, 22);
            allFilterCommand.Text = "Tous";
            allFilterCommand.Click += allFilterCommand_Click;
            // 
            // modifiedFilterCommand
            // 
            modifiedFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            modifiedFilterCommand.Name = "modifiedFilterCommand";
            modifiedFilterCommand.Size = new Size(124, 22);
            modifiedFilterCommand.Text = "Modifiés";
            modifiedFilterCommand.Click += modifiedFilterCommand_Click;
            // 
            // noGpsFilterCommand
            // 
            noGpsFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            noGpsFilterCommand.Name = "noGpsFilterCommand";
            noGpsFilterCommand.Size = new Size(124, 22);
            noGpsFilterCommand.Text = "Sans GPS";
            noGpsFilterCommand.Click += noGpsFilterCommand_Click;
            // 
            // noDateFilterCommand
            // 
            noDateFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            noDateFilterCommand.Name = "noDateFilterCommand";
            noDateFilterCommand.Size = new Size(124, 22);
            noDateFilterCommand.Text = "Sans date";
            noDateFilterCommand.Click += noDateFilterCommand_Click;
            // 
            // errorsFilterCommand
            // 
            errorsFilterCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            errorsFilterCommand.Name = "errorsFilterCommand";
            errorsFilterCommand.Size = new Size(124, 22);
            errorsFilterCommand.Text = "Erreurs";
            errorsFilterCommand.Click += errorsFilterCommand_Click;
            // 
            // commandSeparator5
            // 
            commandSeparator5.Name = "commandSeparator5";
            commandSeparator5.Size = new Size(6, 6);
            // 
            // restoreBackupCommand
            // 
            restoreBackupCommand.DisplayStyle = ToolStripItemDisplayStyle.Text;
            restoreBackupCommand.Name = "restoreBackupCommand";
            restoreBackupCommand.Size = new Size(221, 22);
            restoreBackupCommand.Text = "Restaurer une sauvegarde…";
            restoreBackupCommand.Click += restoreBackupCommand_Click;
            // 
            // navigationMenu
            // 
            navigationMenu.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, dateMenu, locationMenu, viewMenu, actionsMenu, helpMenu, quickActionsToggleItem });
            navigationMenu.Location = new Point(0, 0);
            navigationMenu.Name = "navigationMenu";
            navigationMenu.Size = new Size(1200, 24);
            navigationMenu.TabIndex = 5;
            // 
            // fileMenu
            // 
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { openFilesMenuItem, openFolderCommand, menuSeparator1, removeFromSessionMenuItem, restoreBackupCommand, menuSeparator2, settingsCommand, menuSeparator3, exitMenuItem });
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new Size(54, 20);
            fileMenu.Text = "&Fichier";
            // 
            // openFilesMenuItem
            // 
            openFilesMenuItem.Name = "openFilesMenuItem";
            openFilesMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openFilesMenuItem.Size = new Size(221, 22);
            openFilesMenuItem.Text = "Ouvrir des &fichiers…";
            openFilesMenuItem.Click += openFiles_Click;
            // 
            // menuSeparator1
            // 
            menuSeparator1.Name = "menuSeparator1";
            menuSeparator1.Size = new Size(218, 6);
            // 
            // removeFromSessionMenuItem
            // 
            removeFromSessionMenuItem.Name = "removeFromSessionMenuItem";
            removeFromSessionMenuItem.Size = new Size(221, 22);
            removeFromSessionMenuItem.Text = "Retirer de la session";
            removeFromSessionMenuItem.Click += removeFromSessionMenuItem_Click;
            // 
            // menuSeparator2
            // 
            menuSeparator2.Name = "menuSeparator2";
            menuSeparator2.Size = new Size(218, 6);
            // 
            // menuSeparator3
            // 
            menuSeparator3.Name = "menuSeparator3";
            menuSeparator3.Size = new Size(218, 6);
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitMenuItem.Size = new Size(221, 22);
            exitMenuItem.Text = "&Quitter";
            exitMenuItem.Click += exitMenuItem_Click;
            // 
            // editMenu
            // 
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoMenuItem, redoMenuItem, menuSeparator4, selectAllMenuItem });
            editMenu.Name = "editMenu";
            editMenu.Size = new Size(56, 20);
            editMenu.Text = "&Édition";
            // 
            // undoMenuItem
            // 
            undoMenuItem.Name = "undoMenuItem";
            undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoMenuItem.Size = new Size(207, 22);
            undoMenuItem.Text = "&Annuler";
            undoMenuItem.Click += undoCommand_Click;
            // 
            // redoMenuItem
            // 
            redoMenuItem.Name = "redoMenuItem";
            redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoMenuItem.Size = new Size(207, 22);
            redoMenuItem.Text = "&Rétablir";
            redoMenuItem.Click += redoCommand_Click;
            // 
            // menuSeparator4
            // 
            menuSeparator4.Name = "menuSeparator4";
            menuSeparator4.Size = new Size(204, 6);
            // 
            // selectAllMenuItem
            // 
            selectAllMenuItem.Name = "selectAllMenuItem";
            selectAllMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            selectAllMenuItem.Size = new Size(207, 22);
            selectAllMenuItem.Text = "Tout &sélectionner";
            selectAllMenuItem.Click += selectAllMenuItem_Click;
            // 
            // dateMenu
            // 
            dateMenu.DropDownItems.AddRange(new ToolStripItem[] { dateEditorCommand, menuSeparator5, minusHourCommand, plusHourCommand, minusMinuteCommand, plusMinuteCommand });
            dateMenu.Name = "dateMenu";
            dateMenu.Size = new Size(89, 20);
            dateMenu.Text = "&Date et heure";
            // 
            // menuSeparator5
            // 
            menuSeparator5.Name = "menuSeparator5";
            menuSeparator5.Size = new Size(206, 6);
            // 
            // locationMenu
            // 
            locationMenu.DropDownItems.AddRange(new ToolStripItem[] { findGpsMenuItem, menuSeparator6, copyGpsCommand, pasteGpsCommand, menuSeparator7, removeGpsCommand, reverseGpsCommand });
            locationMenu.Name = "locationMenu";
            locationMenu.Size = new Size(82, 20);
            locationMenu.Text = "&Localisation";
            // 
            // findGpsMenuItem
            // 
            findGpsMenuItem.Name = "findGpsMenuItem";
            findGpsMenuItem.Size = new Size(237, 22);
            findGpsMenuItem.Text = "&Rechercher un lieu…";
            findGpsMenuItem.Click += findGps_Click;
            // 
            // menuSeparator6
            // 
            menuSeparator6.Name = "menuSeparator6";
            menuSeparator6.Size = new Size(234, 6);
            // 
            // menuSeparator7
            // 
            menuSeparator7.Name = "menuSeparator7";
            menuSeparator7.Size = new Size(234, 6);
            // 
            // viewMenu
            // 
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { previewMenuItem, mapCommand, informationMenuItem, menuSeparator8, quickActionsMenuItem, menuSeparator9, filterMenu });
            viewMenu.Name = "viewMenu";
            viewMenu.Size = new Size(70, 20);
            viewMenu.Text = "&Affichage";
            // 
            // previewMenuItem
            // 
            previewMenuItem.Name = "previewMenuItem";
            previewMenuItem.Size = new Size(250, 22);
            previewMenuItem.Text = "Afficher l’&aperçu";
            previewMenuItem.Click += previewMenuItem_Click;
            // 
            // informationMenuItem
            // 
            informationMenuItem.Name = "informationMenuItem";
            informationMenuItem.Size = new Size(250, 22);
            informationMenuItem.Text = "Afficher les &informations";
            informationMenuItem.Click += informationMenuItem_Click;
            // 
            // menuSeparator8
            // 
            menuSeparator8.Name = "menuSeparator8";
            menuSeparator8.Size = new Size(247, 6);
            // 
            // quickActionsMenuItem
            // 
            quickActionsMenuItem.Name = "quickActionsMenuItem";
            quickActionsMenuItem.Size = new Size(250, 22);
            quickActionsMenuItem.Text = "Afficher la barre d’actions &rapides";
            quickActionsMenuItem.Click += quickActions_Click;
            // 
            // menuSeparator9
            // 
            menuSeparator9.Name = "menuSeparator9";
            menuSeparator9.Size = new Size(247, 6);
            // 
            // filterMenu
            // 
            filterMenu.DropDownItems.AddRange(new ToolStripItem[] { allFilterCommand, modifiedFilterCommand, noGpsFilterCommand, noDateFilterCommand, errorsFilterCommand });
            filterMenu.Name = "filterMenu";
            filterMenu.Size = new Size(250, 22);
            filterMenu.Text = "&Filtrer";
            // 
            // actionsMenu
            // 
            actionsMenu.DropDownItems.AddRange(new ToolStripItem[] { applyMenuItem, applySelectedMenuItem, menuSeparator10, resetAllCommand, resetSelectedCommand, menuSeparator11, uploadImmichSelectedMenuItem, uploadImmichAllMenuItem, menuSeparator12, cancelMenuItem });
            actionsMenu.Name = "actionsMenu";
            actionsMenu.Size = new Size(59, 20);
            actionsMenu.Text = "&Actions";
            // 
            // applyMenuItem
            // 
            applyMenuItem.Name = "applyMenuItem";
            applyMenuItem.Size = new Size(304, 22);
            applyMenuItem.Text = "Vérifier et appliquer";
            applyMenuItem.Click += applyAllButton_Click;
            // 
            // applySelectedMenuItem
            // 
            applySelectedMenuItem.Name = "applySelectedMenuItem";
            applySelectedMenuItem.Size = new Size(304, 22);
            applySelectedMenuItem.Text = "Vérifier et appliquer la sélection";
            applySelectedMenuItem.Click += applySelectedMenuItem_Click;
            // 
            // menuSeparator10
            // 
            menuSeparator10.Name = "menuSeparator10";
            menuSeparator10.Size = new Size(301, 6);
            // 
            // menuSeparator11
            // 
            menuSeparator11.Name = "menuSeparator11";
            menuSeparator11.Size = new Size(301, 6);
            // 
            // uploadImmichSelectedMenuItem
            // 
            uploadImmichSelectedMenuItem.Name = "uploadImmichSelectedMenuItem";
            uploadImmichSelectedMenuItem.Size = new Size(304, 22);
            uploadImmichSelectedMenuItem.Text = "Envoyer la sélection vers Immich… (0)";
            uploadImmichSelectedMenuItem.Click += uploadImmichSelected_Click;
            // 
            // uploadImmichAllMenuItem
            // 
            uploadImmichAllMenuItem.Name = "uploadImmichAllMenuItem";
            uploadImmichAllMenuItem.Size = new Size(304, 22);
            uploadImmichAllMenuItem.Text = "Envoyer toutes les images vers Immich… (0)";
            uploadImmichAllMenuItem.Click += uploadImmichAll_Click;
            // 
            // menuSeparator12
            // 
            menuSeparator12.Name = "menuSeparator12";
            menuSeparator12.Size = new Size(301, 6);
            // 
            // cancelMenuItem
            // 
            cancelMenuItem.Name = "cancelMenuItem";
            cancelMenuItem.Size = new Size(304, 22);
            cancelMenuItem.Text = "Interrompre l’opération en cours";
            cancelMenuItem.Click += cancelCommand_Click;
            // 
            // helpMenu
            // 
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { guideMenuItem, logsMenuItem, verifyExifToolMenuItem, checkUpdatesMenuItem, menuSeparator13, aboutMenuItem });
            helpMenu.Name = "helpMenu";
            helpMenu.Size = new Size(43, 20);
            helpMenu.Text = "&Aide";
            // 
            // guideMenuItem
            // 
            guideMenuItem.Name = "guideMenuItem";
            guideMenuItem.ShortcutKeys = Keys.F1;
            guideMenuItem.Size = new Size(225, 22);
            guideMenuItem.Text = "&Guide utilisateur";
            guideMenuItem.Click += guideMenuItem_Click;
            // 
            // logsMenuItem
            // 
            logsMenuItem.Name = "logsMenuItem";
            logsMenuItem.Size = new Size(225, 22);
            logsMenuItem.Text = "Afficher les &journaux…";
            logsMenuItem.Click += logsMenuItem_Click;
            // 
            // verifyExifToolMenuItem
            // 
            verifyExifToolMenuItem.Name = "verifyExifToolMenuItem";
            verifyExifToolMenuItem.Size = new Size(225, 22);
            verifyExifToolMenuItem.Text = "&Vérifier ExifTool";
            verifyExifToolMenuItem.Click += verifyExifToolMenuItem_Click;
            // 
            // checkUpdatesMenuItem
            // 
            checkUpdatesMenuItem.Name = "checkUpdatesMenuItem";
            checkUpdatesMenuItem.Size = new Size(225, 22);
            checkUpdatesMenuItem.Text = "Rechercher les &mises à jour…";
            checkUpdatesMenuItem.Click += checkUpdatesMenuItem_Click;
            // 
            // menuSeparator13
            // 
            menuSeparator13.Name = "menuSeparator13";
            menuSeparator13.Size = new Size(222, 6);
            // 
            // aboutMenuItem
            // 
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Size = new Size(225, 22);
            aboutMenuItem.Text = "À &propos d’ExifTweaker";
            aboutMenuItem.Click += aboutMenuItem_Click;
            // 
            // quickActionsToggleItem
            // 
            quickActionsToggleItem.Alignment = ToolStripItemAlignment.Right;
            quickActionsToggleItem.Name = "quickActionsToggleItem";
            quickActionsToggleItem.Size = new Size(162, 20);
            quickActionsToggleItem.Text = "Actions rapides : masquées";
            quickActionsToggleItem.Click += quickActions_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 720);
            Controls.Add(main);
            Controls.Add(commands);
            Controls.Add(navigationMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = navigationMenu;
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
            dateButtons.ResumeLayout(false);
            dateButtons.PerformLayout();
            gpsButtons.ResumeLayout(false);
            gpsButtons.PerformLayout();
            immichButtons.ResumeLayout(false);
            immichButtons.PerformLayout();
            gpsLayout.ResumeLayout(false);
            gpsLayout.PerformLayout();
            immichLayout.ResumeLayout(false);
            immichLayout.PerformLayout();
            applyPanel.ResumeLayout(false);
            applyPanel.PerformLayout();
            dateGroup.ResumeLayout(false);
            dateGroup.PerformLayout();
            gpsGroup.ResumeLayout(false);
            gpsGroup.PerformLayout();
            immichGroup.ResumeLayout(false);
            immichGroup.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBox).EndInit();
            commands.ResumeLayout(false);
            commands.PerformLayout();
            navigationMenu.ResumeLayout(false);
            navigationMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.TableLayoutPanel main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button bGPS;
        private System.Windows.Forms.Button applyAllButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox dateGroup;
        private System.Windows.Forms.GroupBox gpsGroup;
        private System.Windows.Forms.GroupBox immichGroup;
        private System.Windows.Forms.TableLayoutPanel gpsLayout;
        private System.Windows.Forms.TableLayoutPanel immichLayout;
        private System.Windows.Forms.TableLayoutPanel applyPanel;
        private System.Windows.Forms.FlowLayoutPanel dateButtons;
        private System.Windows.Forms.FlowLayoutPanel gpsButtons;
        private System.Windows.Forms.FlowLayoutPanel immichButtons;
        private System.Windows.Forms.Button bOpenAll;
        private System.Windows.Forms.Button bGPSAll;
        private System.Windows.Forms.ComboBox immichAlbum;
        private System.Windows.Forms.TextBox immichNewAlbum;
        private System.Windows.Forms.Button immichSendSelected;
        private System.Windows.Forms.Button immichSendAll;
        private ExifTweaker.Controls.ThemedDateTimeInput dateTimePicker1;
        private System.Windows.Forms.TextBox tGPS;
        private System.Windows.Forms.DataGridView dgv;
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
        private System.Windows.Forms.MenuStrip navigationMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem dateMenu;
        private System.Windows.Forms.ToolStripMenuItem locationMenu;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem actionsMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem filterMenu;
        private System.Windows.Forms.ToolStripMenuItem openFilesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFromSessionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findGpsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickActionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickActionsToggleItem;
        private System.Windows.Forms.ToolStripMenuItem applyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applySelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadImmichSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadImmichAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verifyExifToolMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkUpdatesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton openQuickCommand;
        private System.Windows.Forms.ToolStripMenuItem openFilesQuickItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderQuickItem;
        private System.Windows.Forms.ToolStripButton dateQuickCommand;
        private System.Windows.Forms.ToolStripDropDownButton locationQuickCommand;
        private System.Windows.Forms.ToolStripMenuItem findGpsQuickItem;
        private System.Windows.Forms.ToolStripMenuItem copyGpsQuickItem;
        private System.Windows.Forms.ToolStripMenuItem pasteGpsQuickItem;
        private System.Windows.Forms.ToolStripMenuItem removeGpsQuickItem;
        private System.Windows.Forms.ToolStripMenuItem reverseGpsQuickItem;
        private System.Windows.Forms.ToolStripButton mapQuickCommand;
        private System.Windows.Forms.ToolStripDropDownButton filterQuickCommand;
        private System.Windows.Forms.ToolStripMenuItem allFilterQuickItem;
        private System.Windows.Forms.ToolStripMenuItem modifiedFilterQuickItem;
        private System.Windows.Forms.ToolStripMenuItem noGpsFilterQuickItem;
        private System.Windows.Forms.ToolStripMenuItem noDateFilterQuickItem;
        private System.Windows.Forms.ToolStripMenuItem errorsFilterQuickItem;
        private System.Windows.Forms.ToolStripDropDownButton immichQuickCommand;
        private System.Windows.Forms.ToolStripMenuItem uploadImmichSelectedQuickItem;
        private System.Windows.Forms.ToolStripMenuItem uploadImmichAllQuickItem;
        private System.Windows.Forms.ToolStripSeparator menuSeparator1;
        private System.Windows.Forms.ToolStripSeparator menuSeparator2;
        private System.Windows.Forms.ToolStripSeparator menuSeparator3;
        private System.Windows.Forms.ToolStripSeparator menuSeparator4;
        private System.Windows.Forms.ToolStripSeparator menuSeparator5;
        private System.Windows.Forms.ToolStripSeparator menuSeparator6;
        private System.Windows.Forms.ToolStripSeparator menuSeparator7;
        private System.Windows.Forms.ToolStripSeparator menuSeparator8;
        private System.Windows.Forms.ToolStripSeparator menuSeparator9;
        private System.Windows.Forms.ToolStripSeparator menuSeparator10;
        private System.Windows.Forms.ToolStripSeparator menuSeparator11;
        private System.Windows.Forms.ToolStripSeparator menuSeparator12;
        private System.Windows.Forms.ToolStripSeparator menuSeparator13;
        private System.Windows.Forms.ToolStripSeparator menuSeparator14;
        private System.Windows.Forms.ToolStripSeparator menuSeparator15;
    }
}
