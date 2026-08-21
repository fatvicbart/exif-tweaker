namespace ExifTweaker.Controls;

partial class MapControl
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        browser = new Microsoft.Web.WebView2.WinForms.WebView2();
        ((System.ComponentModel.ISupportInitialize)browser).BeginInit();
        SuspendLayout();
        // 
        // browser
        // 
        browser.AllowExternalDrop = true;
        browser.CreationProperties = null;
        browser.DefaultBackgroundColor = Color.White;
        browser.Dock = DockStyle.Fill;
        browser.Name = "browser";
        browser.Size = new Size(400, 300);
        browser.TabIndex = 0;
        browser.ZoomFactor = 1D;
        // 
        // MapControl
        // 
        Controls.Add(browser);
