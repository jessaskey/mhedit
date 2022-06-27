using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Krypton.Docking;
using Krypton.Toolkit;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;
using MajorHavocEditor.Services;
using MajorHavocEditor.Views;
using MajorHavocEditor.Views.Dialogs;

namespace MajorHavocEditor
{

    public partial class Form2 : KryptonForm
    {
        public static string MHPHomepage = "http://mhedit.askey.org";

        private GameManager _gameManager;
        private IMenuManager _menuManager = new MenuStripManager(DockStyle.Top);
        private IWindowManager _windowManager;
        private GameExplorer _gameExplorer;
        private IValidationService _validationService;
        private KryptonManager _kryptonManager = new KryptonManager();
        private PropertyBrowser _propertyBrowser;
        private IMameManager _mameManager = new MameManager();
        private IFileManager _fileManager = new FileManager();
        private IRomManager _romManager = new RomManager();

        //private GameToolbox _gameToolbox = new GameToolbox();

        public Form2()
        {
            this.InitializeComponent();

            this._kryptonManager.GlobalPaletteMode = PaletteModeManager.SparkleBlue;

            this._windowManager = new WindowManager(this.kryptonDockableWorkspace,
                this.kryptonDockingManager);

            this._validationService = new ValidationService(this._windowManager);

            this._gameManager = new GameManager( this._fileManager, this._romManager,
                this._mameManager, this._windowManager, this._validationService );

            this._gameExplorer = new GameExplorer(this._menuManager, this._windowManager,
                this._gameManager);

            this._propertyBrowser =
                new PropertyBrowser( this._gameManager.SelectedObjects,
                    DockingState.DockRightAutoHide );

            this.Controls.Add((Control) this._menuManager.Menu);

            this.kryptonDockingManager.DefaultCloseRequest = DockingCloseRequest.RemovePage;

            //this._gameExplorer.ValidateCommand = new MenuCommand(
            //    this.ValidateCommand,
            //    this.OneOrMoreSelected );

            this._menuManager.Add(
                new MenuItem( "MainForm_Configuration" )
                {
                    Command = new MenuCommand( _ => new DialogConfiguration().ShowDialog() ),
                    Display = "Configuration",
                    ToolTip = "Displays the Configuration dialog.",
                    GroupKey = new Guid(),
                    Icon = @"Resources\Images\Menu\Configuration.png".CreateResourceUri()
                } );

            this._menuManager.Add(
                new MenuItem( "MainForm_About" )
                {
                    Command = new MenuCommand( _ => new DialogAbout().ShowDialog() ),
                    Display = "About",
                    ToolTip = "Displays the AboutBox.",
                    GroupKey = new Guid(),
                    Options = new Dictionary<string, object>() { { "Alignment", ToolStripItemAlignment.Right } },
                    Icon = @"Resources\Images\Menu\Information.bmp".CreateResourceUri()
                } );

            this._menuManager.Add(
                new MenuItem( "MainForm_HomePage" )
                {
                    /// <see cref="https://docs.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/start-internet-browser#provide-exception-handling"/>
                    Command = new MenuCommand(
                        _ =>
                        {
                            try
                            {
                                System.Diagnostics.Process.Start( MHPHomepage );
                            }
                            catch ( Win32Exception noBrowser )
                            {
                                if ( noBrowser.ErrorCode != -2147467259 )
                                {
                                    throw;
                                }
                            }
                        } ),
                    Display = "Homepage",
                    ToolTip = "Displays the MHPe homepage.",
                    GroupKey = new Guid(),
                    Options = new Dictionary<string,object>() { { "Alignment", ToolStripItemAlignment.Right } },
                    Icon = @"Resources\Images\Menu\HomeHS.png".CreateResourceUri()
                } );
        }

        //private bool OneOrMoreSelected( object notUsed )
        //{
        //    return this._gameExplorer.SelectedItems.Count > 0;
        //}

        //private void ValidateCommand( object notUsed )
        //{
        //    foreach ( object subject in this._gameExplorer.SelectedItems )
        //    {
        //        this._validationService.ValidateAndDisplayResults( subject );
        //    }
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // https://stackoverflow.com/a/32561014
            if (Properties.Settings.Default.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(Properties.Settings.Default.WindowPosition)))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.DesktopBounds = Properties.Settings.Default.WindowPosition;
                this.WindowState = FormWindowState.Normal;
            }

            // Setup docking functionality
            KryptonDockingWorkspace w = 
                this.kryptonDockingManager.ManageWorkspace(this.kryptonDockableWorkspace);

            this.kryptonDockingManager.ManageControl( this.kryptonPanel, w);
            this.kryptonDockingManager.ManageFloating(this);

            // Add docking pages
            //kryptonDockingManager.AddDockspace("Control", DockingEdge.Left, new KryptonPage[] { NewPropertyGrid() });
            //kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewInput(), NewInput() });
            //kryptonDockingManager.AddAutoHiddenGroup("Control", DockingEdge.Right, new KryptonPage[] { NewPropertyGrid() });
            //kryptonDockingManager.AddToWorkspace("Workspace", new KryptonPage[] { NewDocument(), NewDocument(), NewDocument() });

            //SetSchema(this.menuItemSchemaVS2013Blue, null);

            string configFile =
                Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            //if ( File.Exists( configFile ) )
            //{
            //    this._dockPanel.LoadFromXml( configFile, this.GetContentFromPersistString );
            //}
            //else
            {
                this._windowManager.Show(this._gameExplorer);
                this._windowManager.Show(this._propertyBrowser);
                //this._windowManager.Show(new GameToolbox());
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Properties.Settings.Default.IsMaximized = this.WindowState == FormWindowState.Maximized;
            Properties.Settings.Default.WindowPosition = this.DesktopBounds;
            Properties.Settings.Default.Save();

            //string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            ////TODO Have user option.
            //if (true)
            //{
            //    _dockPanel.SaveAsXml(configFile);
            //}
            //else if (File.Exists(configFile))
            //{
            //    File.Delete(configFile);
            //}
        }
    }
}
