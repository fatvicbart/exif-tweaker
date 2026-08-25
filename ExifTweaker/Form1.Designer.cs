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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            bOpen = new Button();
            main = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            dateGroup = new GroupBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            bOpenAll = new Button();
            dateTimePicker1 = new ExifTweaker.Controls.ThemedDateTimeInput();
            gpsGroup = new GroupBox();
            gpsLayout = new TableLayoutPanel();
            bGPS = new Button();
            tGPS = new TextBox();
            bGPSAll = new Button();
            immichGroup = new GroupBox();
            immichLayout = new TableLayoutPanel();
            immichSendSelected = new Button();
            immichSendAll = new Button();
            immichAlbum = new ComboBox();
            immichNewAlbum = new TextBox();
            applyPanel = new TableLayoutPanel();
            pgb = new ProgressBar();
            applyAllButton = new Button();
            splitContainer1 = new SplitContainer();
            dgv = new DataGridView();
            thumbnailColumn = new DataGridViewImageColumn();
            gridContextMenu = new ContextMenuStrip(components);
            ctxCopy = new ToolStripMenuItem();
            ctxCopyGps = new ToolStripMenuItem();
            ctxCopyDate = new ToolStripMenuItem();
            ctxCopyBoth = new ToolStripMenuItem();
            ctxPaste = new ToolStripMenuItem();
            ctxPasteGps = new ToolStripMenuItem();
            ctxPasteDate = new ToolStripMenuItem();
            ctxPasteBoth = new ToolStripMenuItem();
            ctxSeparator1 = new ToolStripSeparator();
            ctxPrepare = new ToolStripMenuItem();
            ctxPrepareGps = new ToolStripMenuItem();
            ctxPrepareDate = new ToolStripMenuItem();
            ctxPrepareBoth = new ToolStripMenuItem();
            ctxSeparator2 = new ToolStripSeparator();
            ctxDateEditor = new ToolStripMenuItem();
            ctxShift = new ToolStripMenuItem();
            ctxMinusHour = new ToolStripMenuItem();
            ctxPlusHour = new ToolStripMenuItem();
            ctxMinusMinute = new ToolStripMenuItem();
            ctxPlusMinute = new ToolStripMenuItem();
            ctxRemoveGps = new ToolStripMenuItem();
            ctxResetSelection = new ToolStripMenuItem();
            ctxSeparator3 = new ToolStripSeparator();
            ctxApply = new ToolStripMenuItem();
            ctxImmich = new ToolStripMenuItem();
            ctxImmichLoading = new ToolStripMenuItem();
            ctxRestoreBackup = new ToolStripMenuItem();
            ctxSeparator4 = new ToolStripSeparator();
            ctxView = new ToolStripMenuItem();
            ctxShowOnMap = new ToolStripMenuItem();
            ctxShowInformation = new ToolStripMenuItem();
            ctxOpenLocation = new ToolStripMenuItem();
            ctxCopyPath = new ToolStripMenuItem();
            ctxSeparator5 = new ToolStripSeparator();
            ctxRemove = new ToolStripMenuItem();
            picBox = new PictureBox();
            mapControl = new ExifTweaker.Controls.MapControl();
            headerContextMenu = new ContextMenuStrip(components);
            hdrFilter = new ToolStripMenuItem();
            hdrGranularity = new ToolStripMenuItem();
            hdrGranularityYear = new ToolStripMenuItem();
            hdrGranularityMonth = new ToolStripMenuItem();
            hdrGranularityDay = new ToolStripMenuItem();
            hdrClearColumnFilter = new ToolStripMenuItem();
            hdrClearAllFilters = new ToolStripMenuItem();
            hdrSeparator1 = new ToolStripSeparator();
            hdrSortAscending = new ToolStripMenuItem();
            hdrSortDescending = new ToolStripMenuItem();
            hdrSeparator2 = new ToolStripSeparator();
            hdrColumns = new ToolStripMenuItem();
            hdrAutoSize = new ToolStripMenuItem();
            columnsMenuItem = new ToolStripMenuItem();
            clearColumnFiltersMenuItem = new ToolStripMenuItem();
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
            dateGroup.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            gpsGroup.SuspendLayout();
            gpsLayout.SuspendLayout();
            immichGroup.SuspendLayout();
            immichLayout.SuspendLayout();
            applyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            gridContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picBox).BeginInit();
            headerContextMenu.SuspendLayout();
            commands.SuspendLayout();
            navigationMenu.SuspendLayout();
            SuspendLayout();
            // 
            // bOpen
            // 
            bOpen.AutoSize = true;
            bOpen.Dock = DockStyle.Fill;
            bOpen.Enabled = false;
            bOpen.Location = new Point(903, 3);
            bOpen.Name = "bOpen";
            bOpen.Size = new Size(144, 40);
            bOpen.TabIndex = 0;
            bOpen.Text = "PRÉPARER LA SÉLECTION (0)";
            bOpen.UseVisualStyleBackColor = true;
            bOpen.Click += PrepareDateForSelection;
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
            main.Size = new Size(1200, 698);
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
            tableLayoutPanel2.Location = new Point(0, 472);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 27.7777786F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 27.7777786F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 27.7777786F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.Size = new Size(1200, 226);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // dateGroup
            // 
            dateGroup.Controls.Add(tableLayoutPanel3);
            dateGroup.Dock = DockStyle.Fill;
            dateGroup.Location = new Point(0, 0);
            dateGroup.Margin = new Padding(0);
            dateGroup.Name = "dateGroup";
            dateGroup.Padding = new Padding(0);
            dateGroup.Size = new Size(1200, 62);
            dateGroup.TabIndex = 0;
            dateGroup.TabStop = false;
            dateGroup.Text = "Date et heure";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayoutPanel3.Controls.Add(bOpen, 1, 0);
            tableLayoutPanel3.Controls.Add(bOpenAll, 2, 0);
            tableLayoutPanel3.Controls.Add(dateTimePicker1, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 16);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(1200, 46);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // bOpenAll
            // 
            bOpenAll.AutoSize = true;
            bOpenAll.Dock = DockStyle.Fill;
            bOpenAll.Enabled = false;
            bOpenAll.Location = new Point(1053, 3);
            bOpenAll.Name = "bOpenAll";
            bOpenAll.Size = new Size(144, 40);
            bOpenAll.TabIndex = 1;
            bOpenAll.Text = "PRÉPARER TOUT (0)";
            bOpenAll.UseVisualStyleBackColor = true;
            bOpenAll.Click += PrepareDateForAll;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Fill;
            dateTimePicker1.Location = new Point(3, 3);
            dateTimePicker1.Mask = "0000-00-00";
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.PromptChar = ' ';
            dateTimePicker1.Size = new Size(894, 23);
            dateTimePicker1.TabIndex = 4;
            dateTimePicker1.Text = "20260825";
            dateTimePicker1.TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            // 
            // gpsGroup
            // 
            gpsGroup.Controls.Add(gpsLayout);
            gpsGroup.Dock = DockStyle.Fill;
            gpsGroup.Location = new Point(0, 62);
            gpsGroup.Margin = new Padding(0);
            gpsGroup.Name = "gpsGroup";
            gpsGroup.Padding = new Padding(0);
            gpsGroup.Size = new Size(1200, 62);
            gpsGroup.TabIndex = 1;
            gpsGroup.TabStop = false;
            gpsGroup.Text = "Localisation GPS";
            // 
            // gpsLayout
            // 
            gpsLayout.ColumnCount = 3;
            gpsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            gpsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            gpsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            gpsLayout.Controls.Add(bGPS, 1, 0);
            gpsLayout.Controls.Add(tGPS, 0, 0);
            gpsLayout.Controls.Add(bGPSAll, 2, 0);
            gpsLayout.Dock = DockStyle.Fill;
            gpsLayout.Location = new Point(0, 16);
            gpsLayout.Margin = new Padding(0);
            gpsLayout.Name = "gpsLayout";
            gpsLayout.RowCount = 1;
            gpsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gpsLayout.Size = new Size(1200, 46);
            gpsLayout.TabIndex = 0;
            // 
            // bGPS
            // 
            bGPS.AutoSize = true;
            bGPS.Dock = DockStyle.Fill;
            bGPS.Enabled = false;
            bGPS.Location = new Point(903, 3);
            bGPS.Name = "bGPS";
            bGPS.Size = new Size(144, 40);
            bGPS.TabIndex = 7;
            bGPS.Text = "PRÉPARER LA SÉLECTION (0)";
            bGPS.UseVisualStyleBackColor = true;
            bGPS.Click += PrepareGpsForSelection;
            // 
            // tGPS
            // 
            tGPS.Dock = DockStyle.Fill;
            tGPS.Location = new Point(3, 3);
            tGPS.Name = "tGPS";
            tGPS.PlaceholderText = "Rechercher un lieu ou saisir des coordonnées…";
            tGPS.Size = new Size(894, 23);
            tGPS.TabIndex = 5;
            tGPS.TextChanged += tGPS_TextChanged;
            tGPS.KeyDown += GpsSearchKeyDown;
            // 
            // bGPSAll
            // 
            bGPSAll.AutoSize = true;
            bGPSAll.Dock = DockStyle.Fill;
            bGPSAll.Enabled = false;
            bGPSAll.Location = new Point(1053, 3);
            bGPSAll.Name = "bGPSAll";
            bGPSAll.Size = new Size(144, 40);
            bGPSAll.TabIndex = 8;
            bGPSAll.Text = "PRÉPARER TOUT (0)";
            bGPSAll.UseVisualStyleBackColor = true;
            bGPSAll.Click += PrepareGpsForAll;
            // 
            // immichGroup
            // 
            immichGroup.Controls.Add(immichLayout);
            immichGroup.Dock = DockStyle.Fill;
            immichGroup.Location = new Point(0, 124);
            immichGroup.Margin = new Padding(0);
            immichGroup.Name = "immichGroup";
            immichGroup.Padding = new Padding(0);
            immichGroup.Size = new Size(1200, 62);
            immichGroup.TabIndex = 2;
            immichGroup.TabStop = false;
            immichGroup.Text = "Immich upload";
            // 
            // immichLayout
            // 
            immichLayout.ColumnCount = 4;
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            immichLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            immichLayout.Controls.Add(immichSendSelected, 2, 0);
            immichLayout.Controls.Add(immichSendAll, 3, 0);
            immichLayout.Controls.Add(immichAlbum, 0, 0);
            immichLayout.Controls.Add(immichNewAlbum, 1, 0);
            immichLayout.Dock = DockStyle.Fill;
            immichLayout.Location = new Point(0, 16);
            immichLayout.Margin = new Padding(0);
            immichLayout.Name = "immichLayout";
            immichLayout.RowCount = 1;
            immichLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            immichLayout.Size = new Size(1200, 46);
            immichLayout.TabIndex = 0;
            // 
            // immichSendSelected
            // 
            immichSendSelected.AutoSize = true;
            immichSendSelected.Dock = DockStyle.Fill;
            immichSendSelected.Enabled = false;
            immichSendSelected.Location = new Point(903, 3);
            immichSendSelected.Name = "immichSendSelected";
            immichSendSelected.Size = new Size(144, 40);
            immichSendSelected.TabIndex = 0;
            immichSendSelected.Text = "ENVOYER LA SÉLECTION (0)";
            immichSendSelected.UseVisualStyleBackColor = true;
            immichSendSelected.Click += uploadImmichSelected_Click;
            // 
            // immichSendAll
            // 
            immichSendAll.AutoSize = true;
            immichSendAll.Dock = DockStyle.Fill;
            immichSendAll.Enabled = false;
            immichSendAll.Location = new Point(1053, 3);
            immichSendAll.Name = "immichSendAll";
            immichSendAll.Size = new Size(144, 40);
            immichSendAll.TabIndex = 1;
            immichSendAll.Text = "ENVOYER TOUT (0)";
            immichSendAll.UseVisualStyleBackColor = true;
            immichSendAll.Click += uploadImmichAll_Click;
            // 
            // immichAlbum
            // 
            immichAlbum.Dock = DockStyle.Fill;
            immichAlbum.DropDownStyle = ComboBoxStyle.DropDownList;
            immichAlbum.Enabled = false;
            immichAlbum.Location = new Point(3, 3);
            immichAlbum.Name = "immichAlbum";
            immichAlbum.Size = new Size(444, 23);
            immichAlbum.TabIndex = 0;
            immichAlbum.DropDown += immichAlbum_DropDown;
            immichAlbum.SelectedIndexChanged += immichAlbum_SelectedIndexChanged;
            // 
            // immichNewAlbum
            // 
            immichNewAlbum.Dock = DockStyle.Fill;
            immichNewAlbum.Enabled = false;
            immichNewAlbum.Location = new Point(453, 3);
            immichNewAlbum.Name = "immichNewAlbum";
            immichNewAlbum.PlaceholderText = "Nom du nouvel album";
            immichNewAlbum.Size = new Size(444, 23);
            immichNewAlbum.TabIndex = 1;
            // 
            // applyPanel
            // 
            applyPanel.ColumnCount = 2;
            applyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            applyPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            applyPanel.Controls.Add(pgb, 0, 0);
            applyPanel.Controls.Add(applyAllButton, 1, 0);
            applyPanel.Dock = DockStyle.Fill;
            applyPanel.Location = new Point(0, 186);
            applyPanel.Margin = new Padding(0);
            applyPanel.Name = "applyPanel";
            applyPanel.RowCount = 1;
            applyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            applyPanel.Size = new Size(1200, 40);
            applyPanel.TabIndex = 3;
            // 
            // pgb
            // 
            pgb.Dock = DockStyle.Fill;
            pgb.Location = new Point(3, 3);
            pgb.Name = "pgb";
            pgb.Size = new Size(894, 34);
            pgb.TabIndex = 5;
            // 
            // applyAllButton
            // 
            applyAllButton.AutoSize = true;
            applyAllButton.Dock = DockStyle.Fill;
            applyAllButton.Enabled = false;
            applyAllButton.Location = new Point(903, 3);
            applyAllButton.Name = "applyAllButton";
            applyAllButton.Size = new Size(294, 34);
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
            splitContainer1.Size = new Size(1192, 466);
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
            dgv.ContextMenuStrip = gridContextMenu;
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
            dgv.Size = new Size(627, 466);
            dgv.TabIndex = 4;
            dgv.CellFormatting += dgv_CellFormatting;
            dgv.CellMouseClick += dgv_CellMouseClick;
            dgv.CellMouseDown += dgv_CellMouseDown;
            dgv.DataBindingComplete += dgv_DataBindingComplete;
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
            // gridContextMenu
            // 
            gridContextMenu.Items.AddRange(new ToolStripItem[] { ctxCopy, ctxPaste, ctxSeparator1, ctxPrepare, ctxShift, ctxRemoveGps, ctxResetSelection, ctxSeparator3, ctxApply, ctxImmich, ctxRestoreBackup, ctxSeparator4, ctxView, ctxOpenLocation, ctxCopyPath, ctxSeparator5, ctxRemove });
            gridContextMenu.Name = "gridContextMenu";
            gridContextMenu.Size = new Size(252, 314);
            gridContextMenu.Opening += gridContextMenu_Opening;
            // 
            // ctxCopy
            // 
            ctxCopy.DropDownItems.AddRange(new ToolStripItem[] { ctxCopyGps, ctxCopyDate, ctxCopyBoth });
            ctxCopy.Name = "ctxCopy";
            ctxCopy.Size = new Size(251, 22);
            ctxCopy.Text = "Copier";
            // 
            // ctxCopyGps
            // 
            ctxCopyGps.Name = "ctxCopyGps";
            ctxCopyGps.Size = new Size(175, 22);
            ctxCopyGps.Text = "GPS";
            ctxCopyGps.Click += ctxCopyGps_Click;
            // 
            // ctxCopyDate
            // 
            ctxCopyDate.Name = "ctxCopyDate";
            ctxCopyDate.Size = new Size(175, 22);
            ctxCopyDate.Text = "Date";
            ctxCopyDate.Click += ctxCopyDate_Click;
            // 
            // ctxCopyBoth
            // 
            ctxCopyBoth.Name = "ctxCopyBoth";
            ctxCopyBoth.ShortcutKeyDisplayString = "Ctrl+C";
            ctxCopyBoth.Size = new Size(175, 22);
            ctxCopyBoth.Text = "GPS + Date";
            ctxCopyBoth.Click += ctxCopyBoth_Click;
            // 
            // ctxPaste
            // 
            ctxPaste.DropDownItems.AddRange(new ToolStripItem[] { ctxPasteGps, ctxPasteDate, ctxPasteBoth });
            ctxPaste.Name = "ctxPaste";
            ctxPaste.Size = new Size(251, 22);
            ctxPaste.Text = "Coller";
            // 
            // ctxPasteGps
            // 
            ctxPasteGps.Name = "ctxPasteGps";
            ctxPasteGps.Size = new Size(174, 22);
            ctxPasteGps.Text = "GPS";
            ctxPasteGps.Click += ctxPasteGps_Click;
            // 
            // ctxPasteDate
            // 
            ctxPasteDate.Name = "ctxPasteDate";
            ctxPasteDate.Size = new Size(174, 22);
            ctxPasteDate.Text = "Date";
            ctxPasteDate.Click += ctxPasteDate_Click;
            // 
            // ctxPasteBoth
            // 
            ctxPasteBoth.Name = "ctxPasteBoth";
            ctxPasteBoth.ShortcutKeyDisplayString = "Ctrl+V";
            ctxPasteBoth.Size = new Size(174, 22);
            ctxPasteBoth.Text = "GPS + Date";
            ctxPasteBoth.Click += ctxPasteBoth_Click;
            // 
            // ctxSeparator1
            // 
            ctxSeparator1.Name = "ctxSeparator1";
            ctxSeparator1.Size = new Size(248, 6);
            // 
            // ctxPrepare
            // 
            ctxPrepare.DropDownItems.AddRange(new ToolStripItem[] { ctxPrepareGps, ctxPrepareDate, ctxPrepareBoth, ctxSeparator2, ctxDateEditor });
            ctxPrepare.Name = "ctxPrepare";
            ctxPrepare.Size = new Size(251, 22);
            ctxPrepare.Text = "Préparer";
            // 
            // ctxPrepareGps
            // 
            ctxPrepareGps.Name = "ctxPrepareGps";
            ctxPrepareGps.Size = new Size(202, 22);
            ctxPrepareGps.Text = "GPS";
            ctxPrepareGps.Click += ctxPrepareGps_Click;
            // 
            // ctxPrepareDate
            // 
            ctxPrepareDate.Name = "ctxPrepareDate";
            ctxPrepareDate.Size = new Size(202, 22);
            ctxPrepareDate.Text = "Date";
            ctxPrepareDate.Click += ctxPrepareDate_Click;
            // 
            // ctxPrepareBoth
            // 
            ctxPrepareBoth.Name = "ctxPrepareBoth";
            ctxPrepareBoth.Size = new Size(202, 22);
            ctxPrepareBoth.Text = "GPS + Date";
            ctxPrepareBoth.Click += ctxPrepareBoth_Click;
            // 
            // ctxSeparator2
            // 
            ctxSeparator2.Name = "ctxSeparator2";
            ctxSeparator2.Size = new Size(199, 6);
            // 
            // ctxDateEditor
            // 
            ctxDateEditor.Name = "ctxDateEditor";
            ctxDateEditor.Size = new Size(202, 22);
            ctxDateEditor.Text = "Éditeur de date avancé…";
            ctxDateEditor.Click += dateEditorCommand_Click;
            // 
            // ctxShift
            // 
            ctxShift.DropDownItems.AddRange(new ToolStripItem[] { ctxMinusHour, ctxPlusHour, ctxMinusMinute, ctxPlusMinute });
            ctxShift.Name = "ctxShift";
            ctxShift.Size = new Size(251, 22);
            ctxShift.Text = "Décaler l’heure";
            // 
            // ctxMinusHour
            // 
            ctxMinusHour.Name = "ctxMinusHour";
            ctxMinusHour.Size = new Size(129, 22);
            ctxMinusHour.Text = "-1 heure";
            ctxMinusHour.Click += minusHourCommand_Click;
            // 
            // ctxPlusHour
            // 
            ctxPlusHour.Name = "ctxPlusHour";
            ctxPlusHour.Size = new Size(129, 22);
            ctxPlusHour.Text = "+1 heure";
            ctxPlusHour.Click += plusHourCommand_Click;
            // 
            // ctxMinusMinute
            // 
            ctxMinusMinute.Name = "ctxMinusMinute";
            ctxMinusMinute.Size = new Size(129, 22);
            ctxMinusMinute.Text = "-1 minute";
            ctxMinusMinute.Click += minusMinuteCommand_Click;
            // 
            // ctxPlusMinute
            // 
            ctxPlusMinute.Name = "ctxPlusMinute";
            ctxPlusMinute.Size = new Size(129, 22);
            ctxPlusMinute.Text = "+1 minute";
            ctxPlusMinute.Click += plusMinuteCommand_Click;
            // 
            // ctxRemoveGps
            // 
            ctxRemoveGps.Name = "ctxRemoveGps";
            ctxRemoveGps.Size = new Size(251, 22);
            ctxRemoveGps.Text = "Supprimer le GPS";
            ctxRemoveGps.Click += removeGpsCommand_Click;
            // 
            // ctxResetSelection
            // 
            ctxResetSelection.Name = "ctxResetSelection";
            ctxResetSelection.Size = new Size(251, 22);
            ctxResetSelection.Text = "Restaurer la sélection";
            ctxResetSelection.Click += resetSelectedCommand_Click;
            // 
            // ctxSeparator3
            // 
            ctxSeparator3.Name = "ctxSeparator3";
            ctxSeparator3.Size = new Size(248, 6);
            // 
            // ctxApply
            // 
            ctxApply.Name = "ctxApply";
            ctxApply.Size = new Size(251, 22);
            ctxApply.Text = "Vérifier et appliquer la sélection";
            ctxApply.Click += applySelectedMenuItem_Click;
            // 
            // ctxImmich
            // 
            ctxImmich.DropDownItems.AddRange(new ToolStripItem[] { ctxImmichLoading });
            ctxImmich.Name = "ctxImmich";
            ctxImmich.Size = new Size(251, 22);
            ctxImmich.Text = "Envoyer sur Immich";
            ctxImmich.DropDownOpening += ctxImmich_DropDownOpening;
            // 
            // ctxImmichLoading
            // 
            ctxImmichLoading.Enabled = false;
            ctxImmichLoading.Name = "ctxImmichLoading";
            ctxImmichLoading.Size = new Size(212, 22);
            ctxImmichLoading.Text = "Chargement des albums…";
            // 
            // ctxRestoreBackup
            // 
            ctxRestoreBackup.Name = "ctxRestoreBackup";
            ctxRestoreBackup.Size = new Size(251, 22);
            ctxRestoreBackup.Text = "Restaurer la sauvegarde ExifTool…";
            ctxRestoreBackup.Click += restoreBackupCommand_Click;
            // 
            // ctxSeparator4
            // 
            ctxSeparator4.Name = "ctxSeparator4";
            ctxSeparator4.Size = new Size(248, 6);
            // 
            // ctxView
            // 
            ctxView.DropDownItems.AddRange(new ToolStripItem[] { ctxShowOnMap, ctxShowInformation });
            ctxView.Name = "ctxView";
            ctxView.Size = new Size(251, 22);
            ctxView.Text = "Afficher";
            // 
            // ctxShowOnMap
            // 
            ctxShowOnMap.Name = "ctxShowOnMap";
            ctxShowOnMap.Size = new Size(167, 22);
            ctxShowOnMap.Text = "Sur la carte";
            ctxShowOnMap.Click += ctxShowOnMap_Click;
            // 
            // ctxShowInformation
            // 
            ctxShowInformation.Name = "ctxShowInformation";
            ctxShowInformation.Size = new Size(167, 22);
            ctxShowInformation.Text = "Informations EXIF";
            ctxShowInformation.Click += informationMenuItem_Click;
            // 
            // ctxOpenLocation
            // 
            ctxOpenLocation.Name = "ctxOpenLocation";
            ctxOpenLocation.Size = new Size(251, 22);
            ctxOpenLocation.Text = "Ouvrir l’emplacement du fichier";
            ctxOpenLocation.Click += ctxOpenLocation_Click;
            // 
            // ctxCopyPath
            // 
            ctxCopyPath.Name = "ctxCopyPath";
            ctxCopyPath.ShortcutKeyDisplayString = "Ctrl+Maj+C";
            ctxCopyPath.Size = new Size(251, 22);
            ctxCopyPath.Text = "Copier le chemin";
            ctxCopyPath.Click += ctxCopyPath_Click;
            // 
            // ctxSeparator5
            // 
            ctxSeparator5.Name = "ctxSeparator5";
            ctxSeparator5.Size = new Size(248, 6);
            // 
            // ctxRemove
            // 
            ctxRemove.Name = "ctxRemove";
            ctxRemove.ShortcutKeyDisplayString = "Suppr";
            ctxRemove.Size = new Size(251, 22);
            ctxRemove.Text = "Retirer de la session";
            ctxRemove.Click += removeFromSessionMenuItem_Click;
            // 
            // picBox
            // 
            picBox.BackColor = SystemColors.ControlLight;
            picBox.BorderStyle = BorderStyle.Fixed3D;
            picBox.Dock = DockStyle.Fill;
            picBox.Location = new Point(0, 0);
            picBox.Margin = new Padding(4, 3, 4, 3);
            picBox.Name = "picBox";
            picBox.Size = new Size(560, 466);
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            picBox.TabIndex = 5;
            picBox.TabStop = false;
            // 
            // mapControl
            // 
            mapControl.Dock = DockStyle.Fill;
            mapControl.Location = new Point(0, 0);
            mapControl.Name = "mapControl";
            mapControl.Size = new Size(560, 466);
            mapControl.TabIndex = 6;
            mapControl.Visible = false;
            // 
            // headerContextMenu
            // 
            headerContextMenu.Items.AddRange(new ToolStripItem[] { hdrFilter, hdrGranularity, hdrClearColumnFilter, hdrClearAllFilters, hdrSeparator1, hdrSortAscending, hdrSortDescending, hdrSeparator2, hdrColumns, hdrAutoSize });
            headerContextMenu.Name = "headerContextMenu";
            headerContextMenu.Size = new Size(248, 192);
            headerContextMenu.Opening += headerContextMenu_Opening;
            // 
            // hdrFilter
            // 
            hdrFilter.Name = "hdrFilter";
            hdrFilter.Size = new Size(247, 22);
            hdrFilter.Text = "Filtrer";
            // 
            // hdrGranularity
            // 
            hdrGranularity.DropDownItems.AddRange(new ToolStripItem[] { hdrGranularityYear, hdrGranularityMonth, hdrGranularityDay });
            hdrGranularity.Name = "hdrGranularity";
            hdrGranularity.Size = new Size(247, 22);
            hdrGranularity.Text = "Regrouper par";
            // 
            // hdrGranularityYear
            // 
            hdrGranularityYear.Name = "hdrGranularityYear";
            hdrGranularityYear.Size = new Size(108, 22);
            hdrGranularityYear.Text = "Année";
            hdrGranularityYear.Click += hdrGranularityYear_Click;
            // 
            // hdrGranularityMonth
            // 
            hdrGranularityMonth.Name = "hdrGranularityMonth";
            hdrGranularityMonth.Size = new Size(108, 22);
            hdrGranularityMonth.Text = "Mois";
            hdrGranularityMonth.Click += hdrGranularityMonth_Click;
            // 
            // hdrGranularityDay
            // 
            hdrGranularityDay.Name = "hdrGranularityDay";
            hdrGranularityDay.Size = new Size(108, 22);
            hdrGranularityDay.Text = "Jour";
            hdrGranularityDay.Click += hdrGranularityDay_Click;
            // 
            // hdrClearColumnFilter
            // 
            hdrClearColumnFilter.Name = "hdrClearColumnFilter";
            hdrClearColumnFilter.Size = new Size(247, 22);
            hdrClearColumnFilter.Text = "Effacer le filtre de cette colonne";
            hdrClearColumnFilter.Click += hdrClearColumnFilter_Click;
            // 
            // hdrClearAllFilters
            // 
            hdrClearAllFilters.Name = "hdrClearAllFilters";
            hdrClearAllFilters.Size = new Size(247, 22);
            hdrClearAllFilters.Text = "Effacer tous les filtres de colonne";
            hdrClearAllFilters.Click += clearColumnFilters_Click;
            // 
            // hdrSeparator1
            // 
            hdrSeparator1.Name = "hdrSeparator1";
            hdrSeparator1.Size = new Size(244, 6);
            // 
            // hdrSortAscending
            // 
            hdrSortAscending.Name = "hdrSortAscending";
            hdrSortAscending.Size = new Size(247, 22);
            hdrSortAscending.Text = "Trier de A à Z";
            hdrSortAscending.Click += hdrSortAscending_Click;
            // 
            // hdrSortDescending
            // 
            hdrSortDescending.Name = "hdrSortDescending";
            hdrSortDescending.Size = new Size(247, 22);
            hdrSortDescending.Text = "Trier de Z à A";
            hdrSortDescending.Click += hdrSortDescending_Click;
            // 
            // hdrSeparator2
            // 
            hdrSeparator2.Name = "hdrSeparator2";
            hdrSeparator2.Size = new Size(244, 6);
            // 
            // hdrColumns
            // 
            hdrColumns.Name = "hdrColumns";
            hdrColumns.Size = new Size(247, 22);
            hdrColumns.Text = "Colonnes";
            hdrColumns.DropDownOpening += columnsMenu_DropDownOpening;
            // 
            // hdrAutoSize
            // 
            hdrAutoSize.Name = "hdrAutoSize";
            hdrAutoSize.Size = new Size(247, 22);
            hdrAutoSize.Text = "Ajuster les largeurs";
            hdrAutoSize.Click += hdrAutoSize_Click;
            // 
            // columnsMenuItem
            // 
            columnsMenuItem.Name = "columnsMenuItem";
            columnsMenuItem.Size = new Size(250, 22);
            columnsMenuItem.Text = "&Colonnes";
            columnsMenuItem.DropDownOpening += columnsMenu_DropDownOpening;
            // 
            // clearColumnFiltersMenuItem
            // 
            clearColumnFiltersMenuItem.Name = "clearColumnFiltersMenuItem";
            clearColumnFiltersMenuItem.Size = new Size(250, 22);
            clearColumnFiltersMenuItem.Text = "Effacer les filtres de colonne";
            clearColumnFiltersMenuItem.Click += clearColumnFilters_Click;
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
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { previewMenuItem, mapCommand, informationMenuItem, menuSeparator8, quickActionsMenuItem, columnsMenuItem, menuSeparator9, filterMenu, clearColumnFiltersMenuItem });
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
            ClientSize = new Size(1200, 722);
            Controls.Add(main);
            Controls.Add(navigationMenu);
            Controls.Add(commands);
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
            dateGroup.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            gpsGroup.ResumeLayout(false);
            gpsLayout.ResumeLayout(false);
            gpsLayout.PerformLayout();
            immichGroup.ResumeLayout(false);
            immichLayout.ResumeLayout(false);
            immichLayout.PerformLayout();
            applyPanel.ResumeLayout(false);
            applyPanel.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            gridContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picBox).EndInit();
            headerContextMenu.ResumeLayout(false);
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
        private System.Windows.Forms.Button bOpenAll;
        private System.Windows.Forms.ComboBox immichAlbum;
        private System.Windows.Forms.TextBox immichNewAlbum;
        private System.Windows.Forms.Button immichSendSelected;
        private System.Windows.Forms.Button immichSendAll;
        private ExifTweaker.Controls.ThemedDateTimeInput dateTimePicker1;
        private System.Windows.Forms.TextBox tGPS;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.ContextMenuStrip gridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxCopy;
        private System.Windows.Forms.ToolStripMenuItem ctxCopyGps;
        private System.Windows.Forms.ToolStripMenuItem ctxCopyDate;
        private System.Windows.Forms.ToolStripMenuItem ctxCopyBoth;
        private System.Windows.Forms.ToolStripMenuItem ctxPaste;
        private System.Windows.Forms.ToolStripMenuItem ctxPasteGps;
        private System.Windows.Forms.ToolStripMenuItem ctxPasteDate;
        private System.Windows.Forms.ToolStripMenuItem ctxPasteBoth;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxPrepare;
        private System.Windows.Forms.ToolStripMenuItem ctxPrepareGps;
        private System.Windows.Forms.ToolStripMenuItem ctxPrepareDate;
        private System.Windows.Forms.ToolStripMenuItem ctxPrepareBoth;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ctxDateEditor;
        private System.Windows.Forms.ToolStripMenuItem ctxShift;
        private System.Windows.Forms.ToolStripMenuItem ctxMinusHour;
        private System.Windows.Forms.ToolStripMenuItem ctxPlusHour;
        private System.Windows.Forms.ToolStripMenuItem ctxMinusMinute;
        private System.Windows.Forms.ToolStripMenuItem ctxPlusMinute;
        private System.Windows.Forms.ToolStripMenuItem ctxRemoveGps;
        private System.Windows.Forms.ToolStripMenuItem ctxResetSelection;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ctxApply;
        private System.Windows.Forms.ToolStripMenuItem ctxImmich;
        private System.Windows.Forms.ToolStripMenuItem ctxImmichLoading;
        private System.Windows.Forms.ToolStripMenuItem ctxRestoreBackup;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator4;
        private System.Windows.Forms.ToolStripMenuItem ctxView;
        private System.Windows.Forms.ToolStripMenuItem ctxShowOnMap;
        private System.Windows.Forms.ToolStripMenuItem ctxShowInformation;
        private System.Windows.Forms.ToolStripMenuItem ctxOpenLocation;
        private System.Windows.Forms.ToolStripMenuItem ctxCopyPath;
        private System.Windows.Forms.ToolStripSeparator ctxSeparator5;
        private System.Windows.Forms.ToolStripMenuItem ctxRemove;
        private System.Windows.Forms.ContextMenuStrip headerContextMenu;
        private System.Windows.Forms.ToolStripMenuItem hdrFilter;
        private System.Windows.Forms.ToolStripMenuItem hdrGranularity;
        private System.Windows.Forms.ToolStripMenuItem hdrGranularityYear;
        private System.Windows.Forms.ToolStripMenuItem hdrGranularityMonth;
        private System.Windows.Forms.ToolStripMenuItem hdrGranularityDay;
        private System.Windows.Forms.ToolStripMenuItem hdrClearColumnFilter;
        private System.Windows.Forms.ToolStripMenuItem hdrClearAllFilters;
        private System.Windows.Forms.ToolStripSeparator hdrSeparator1;
        private System.Windows.Forms.ToolStripMenuItem hdrSortAscending;
        private System.Windows.Forms.ToolStripMenuItem hdrSortDescending;
        private System.Windows.Forms.ToolStripSeparator hdrSeparator2;
        private System.Windows.Forms.ToolStripMenuItem hdrColumns;
        private System.Windows.Forms.ToolStripMenuItem hdrAutoSize;
        private System.Windows.Forms.ToolStripMenuItem columnsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearColumnFiltersMenuItem;
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
        private Button bGPSAll;
    }
}
