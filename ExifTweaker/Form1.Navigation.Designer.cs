namespace ExifTweaker;

partial class Form1
{
    private MenuStrip navigationMenu = null!;
    private ToolStripMenuItem fileMenu = null!;
    private ToolStripMenuItem editMenu = null!;
    private ToolStripMenuItem dateMenu = null!;
    private ToolStripMenuItem locationMenu = null!;
    private ToolStripMenuItem viewMenu = null!;
    private ToolStripMenuItem actionsMenu = null!;
    private ToolStripMenuItem helpMenu = null!;
    private ToolStripMenuItem filterMenu = null!;

    private ToolStripMenuItem openFilesMenuItem = null!;
    private ToolStripMenuItem removeFromSessionMenuItem = null!;
    private ToolStripMenuItem exitMenuItem = null!;
    private ToolStripMenuItem undoMenuItem = null!;
    private ToolStripMenuItem redoMenuItem = null!;
    private ToolStripMenuItem selectAllMenuItem = null!;
    private ToolStripMenuItem findGpsMenuItem = null!;
    private ToolStripMenuItem previewMenuItem = null!;
    private ToolStripMenuItem applyMenuItem = null!;
    private ToolStripMenuItem cancelMenuItem = null!;
    private ToolStripMenuItem guideMenuItem = null!;
    private ToolStripMenuItem logsMenuItem = null!;
    private ToolStripMenuItem verifyExifToolMenuItem = null!;
    private ToolStripMenuItem aboutMenuItem = null!;

    private ToolStripDropDownButton openQuickCommand = null!;
    private ToolStripButton dateQuickCommand = null!;
    private ToolStripDropDownButton locationQuickCommand = null!;
    private ToolStripButton mapQuickCommand = null!;
    private ToolStripDropDownButton filterQuickCommand = null!;

    private ToolStripMenuItem openFilesQuickItem = null!;
    private ToolStripMenuItem openFolderQuickItem = null!;
    private ToolStripMenuItem findGpsQuickItem = null!;
    private ToolStripMenuItem setGpsQuickItem = null!;
    private ToolStripMenuItem copyGpsQuickItem = null!;
    private ToolStripMenuItem pasteGpsQuickItem = null!;
    private ToolStripMenuItem removeGpsQuickItem = null!;
    private ToolStripMenuItem reverseGpsQuickItem = null!;
    private ToolStripMenuItem allFilterQuickItem = null!;
    private ToolStripMenuItem modifiedFilterQuickItem = null!;
    private ToolStripMenuItem noGpsFilterQuickItem = null!;
    private ToolStripMenuItem noDateFilterQuickItem = null!;
    private ToolStripMenuItem errorsFilterQuickItem = null!;

    private void InitializeNavigation()
    {
        navigationMenu = new MenuStrip { Name = "navigationMenu", Dock = DockStyle.Top };
        fileMenu = TopMenu("&Fichier", "fileMenu");
        editMenu = TopMenu("&Édition", "editMenu");
        dateMenu = TopMenu("&Date et heure", "dateMenu");
        locationMenu = TopMenu("&Localisation", "locationMenu");
        viewMenu = TopMenu("&Affichage", "viewMenu");
        actionsMenu = TopMenu("&Actions", "actionsMenu");
        helpMenu = TopMenu("&Aide", "helpMenu");
        filterMenu = TopMenu("&Filtrer", "filterMenu");

        openFilesMenuItem = MenuItem("Ouvrir des &fichiers…", "openFilesMenuItem", Keys.Control | Keys.O);
        removeFromSessionMenuItem = MenuItem("Retirer de la session", "removeFromSessionMenuItem");
        exitMenuItem = MenuItem("&Quitter", "exitMenuItem", Keys.Alt | Keys.F4);
        undoMenuItem = MenuItem("&Annuler", "undoMenuItem", Keys.Control | Keys.Z);
        redoMenuItem = MenuItem("&Rétablir", "redoMenuItem", Keys.Control | Keys.Y);
        selectAllMenuItem = MenuItem("Tout &sélectionner", "selectAllMenuItem", Keys.Control | Keys.A);
        findGpsMenuItem = MenuItem("&Rechercher un lieu…", "findGpsMenuItem");
        previewMenuItem = MenuItem("Afficher l’&aperçu", "previewMenuItem");
        applyMenuItem = MenuItem("Vérifier et appliquer", "applyMenuItem");
        cancelMenuItem = MenuItem("Annuler l’opération", "cancelMenuItem");
        guideMenuItem = MenuItem("&Guide utilisateur", "guideMenuItem", Keys.F1);
        logsMenuItem = MenuItem("Ouvrir le dossier des &journaux", "logsMenuItem");
        verifyExifToolMenuItem = MenuItem("&Vérifier ExifTool", "verifyExifToolMenuItem");
        aboutMenuItem = MenuItem("À &propos d’ExifTweaker", "aboutMenuItem");

        ConfigureExistingMenuCommands();

        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            openFilesMenuItem, openFolderCommand, new ToolStripSeparator(),
            removeFromSessionMenuItem, restoreBackupCommand, new ToolStripSeparator(),
            settingsCommand, new ToolStripSeparator(), exitMenuItem
        });
        editMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            undoMenuItem, redoMenuItem, new ToolStripSeparator(),
            resetSelectedCommand, resetAllCommand, new ToolStripSeparator(), selectAllMenuItem
        });
        dateMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            dateEditorCommand, new ToolStripSeparator(),
            minusHourCommand, plusHourCommand, minusMinuteCommand, plusMinuteCommand
        });
        locationMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            findGpsMenuItem, setGpsCommand, new ToolStripSeparator(),
            copyGpsCommand, pasteGpsCommand, new ToolStripSeparator(),
            removeGpsCommand, reverseGpsCommand
        });
        filterMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            allFilterCommand, modifiedFilterCommand, noGpsFilterCommand, noDateFilterCommand, errorsFilterCommand
        });
        viewMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            previewMenuItem, mapCommand, new ToolStripSeparator(), filterMenu
        });
        actionsMenu.DropDownItems.AddRange(new ToolStripItem[] { applyMenuItem, cancelMenuItem });
        helpMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            guideMenuItem, logsMenuItem, verifyExifToolMenuItem, new ToolStripSeparator(), aboutMenuItem
        });
        navigationMenu.Items.AddRange(new ToolStripItem[]
        {
            fileMenu, editMenu, dateMenu, locationMenu, viewMenu, actionsMenu, helpMenu
        });

        BuildQuickToolbar();
        MainMenuStrip = navigationMenu;
        Controls.Add(navigationMenu);
    }

    private void BuildQuickToolbar()
    {
        openQuickCommand = new ToolStripDropDownButton("Ouvrir") { Name = "openQuickCommand" };
        openFilesQuickItem = MenuItem("Ouvrir des fichiers…", "openFilesQuickItem");
        openFolderQuickItem = MenuItem("Ouvrir un dossier…", "openFolderQuickItem");
        openQuickCommand.DropDownItems.AddRange(new ToolStripItem[] { openFilesQuickItem, openFolderQuickItem });

        dateQuickCommand = new ToolStripButton("Date et heure") { Name = "dateQuickCommand" };
        locationQuickCommand = new ToolStripDropDownButton("Localisation") { Name = "locationQuickCommand" };
        findGpsQuickItem = MenuItem("Rechercher un lieu…", "findGpsQuickItem");
        setGpsQuickItem = MenuItem("Préparer le GPS saisi", "setGpsQuickItem");
        copyGpsQuickItem = MenuItem("Copier le GPS", "copyGpsQuickItem");
        pasteGpsQuickItem = MenuItem("Coller le GPS", "pasteGpsQuickItem");
        removeGpsQuickItem = MenuItem("Préparer la suppression du GPS", "removeGpsQuickItem");
        reverseGpsQuickItem = MenuItem("Identifier les coordonnées", "reverseGpsQuickItem");
        locationQuickCommand.DropDownItems.AddRange(new ToolStripItem[]
        {
            findGpsQuickItem, setGpsQuickItem, new ToolStripSeparator(),
            copyGpsQuickItem, pasteGpsQuickItem, new ToolStripSeparator(),
            removeGpsQuickItem, reverseGpsQuickItem
        });

        mapQuickCommand = new ToolStripButton("Carte") { Name = "mapQuickCommand", CheckOnClick = false };
        filterQuickCommand = new ToolStripDropDownButton("Filtre : Tous") { Name = "filterQuickCommand" };
        allFilterQuickItem = MenuItem("Tous", "allFilterQuickItem");
        modifiedFilterQuickItem = MenuItem("Modifiés", "modifiedFilterQuickItem");
        noGpsFilterQuickItem = MenuItem("Sans GPS", "noGpsFilterQuickItem");
        noDateFilterQuickItem = MenuItem("Sans date", "noDateFilterQuickItem");
        errorsFilterQuickItem = MenuItem("Erreurs", "errorsFilterQuickItem");
        filterQuickCommand.DropDownItems.AddRange(new ToolStripItem[]
        {
            allFilterQuickItem, modifiedFilterQuickItem, noGpsFilterQuickItem, noDateFilterQuickItem, errorsFilterQuickItem
        });

        commands.Items.Clear();
        commands.Items.AddRange(new ToolStripItem[]
        {
            openQuickCommand, dateQuickCommand, locationQuickCommand, mapQuickCommand,
            new ToolStripSeparator(), undoCommand, redoCommand, filterQuickCommand,
            new ToolStripSeparator(), applyCommand, cancelCommand, operationStatus
        });
        commands.Padding = new Padding(4, 2, 4, 2);
        commands.AutoSize = true;
    }

    private void ConfigureExistingMenuCommands()
    {
        SetText(openFolderCommand, "Ouvrir un dossier…");
        SetText(settingsCommand, "Paramètres…");
        SetText(restoreBackupCommand, "Restaurer une sauvegarde…");
        SetText(resetSelectedCommand, "Réinitialiser la sélection");
        SetText(resetAllCommand, "Réinitialiser toutes les modifications");
        SetText(dateEditorCommand, "Ouvrir l’éditeur complet…");
        SetText(minusHourCommand, "Reculer d’une heure");
        SetText(plusHourCommand, "Avancer d’une heure");
        SetText(minusMinuteCommand, "Reculer d’une minute");
        SetText(plusMinuteCommand, "Avancer d’une minute");
        SetText(setGpsCommand, "Préparer le GPS saisi");
        SetText(copyGpsCommand, "Copier le GPS");
        SetText(pasteGpsCommand, "Coller le GPS");
        SetText(removeGpsCommand, "Préparer la suppression du GPS");
        SetText(reverseGpsCommand, "Identifier les coordonnées");
        SetText(mapCommand, "Afficher la carte");
        SetText(allFilterCommand, "Tous");
        SetText(modifiedFilterCommand, "Modifiés");
        SetText(noGpsFilterCommand, "Sans GPS");
        SetText(noDateFilterCommand, "Sans date");
        SetText(errorsFilterCommand, "Erreurs");
        SetText(undoCommand, "Annuler");
        SetText(redoCommand, "Rétablir");
        SetText(applyCommand, "Vérifier et appliquer");
        SetText(cancelCommand, "Annuler l’opération");
    }

    private static ToolStripMenuItem TopMenu(string text, string name) => new(text) { Name = name };

    private static ToolStripMenuItem MenuItem(string text, string name, Keys shortcut = Keys.None) =>
        new(text) { Name = name, ShortcutKeys = shortcut };

    private static void SetText(ToolStripItem item, string text)
    {
        item.Text = text;
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
    }
}
