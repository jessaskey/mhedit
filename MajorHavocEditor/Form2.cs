using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Krypton.Docking;
using Krypton.Toolkit;
using Krypton.Workspace;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;
using MajorHavocEditor.Services;
using MajorHavocEditor.Views;
using MHavocEditor;

namespace MajorHavocEditor
{

    public partial class Form2 : KryptonForm
    {
        private IMenuManager _menuManager = new MenuStripManager(DockStyle.Top);
        private IWindowManager _windowManager;
        private GameExplorer _gameExplorer;
        private IValidationService _validationService;
        private KryptonManager _kryptonManager = new KryptonManager();

        //private GameToolbox _gameToolbox = new GameToolbox();

        public Form2()
        {
            this.InitializeComponent();

            this._kryptonManager.GlobalPaletteMode = PaletteModeManager.SparkleBlue;

            this._windowManager = new WindowManager(this.kryptonDockableWorkspace,
                this.kryptonDockingManager);

            this._gameExplorer = new GameExplorer(this._menuManager, this._windowManager);

            this._validationService = new ValidationService( this._windowManager );

            this.Controls.Add((Control) this._menuManager.Menu);

            this.kryptonDockingManager.DefaultCloseRequest = DockingCloseRequest.RemovePage;
            //this.kryptonDockableWorkspace.WorkspaceCellAdding += this.kryptonDockableWorkspace_WorkspaceCellAdding;

            this._gameExplorer.ValidateCommand = new MenuCommand(
                this.ValidateCommand,
                this.CanValidate );
        }

        private void kryptonDockableWorkspace_WorkspaceCellAdding( object sender, WorkspaceCellEventArgs e )
        {

        }

        private bool CanValidate(object notUsed )
        {
            // Should always be true since it just reflects over objects to
            // look for validation attributes...
            return this._gameExplorer.SelectedItems.Count > 0;
        }

        private void ValidateCommand(object notUsed )
        {
            foreach ( object subject in this._gameExplorer.SelectedItems)
            {
                this._validationService.ValidateAndDisplayResults(subject);
            }
        }

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
                this._windowManager.Show(new GameToolbox());
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
