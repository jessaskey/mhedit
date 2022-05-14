using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Krypton.Docking;
using Krypton.Navigator;
using Krypton.Workspace;
using MajorHavocEditor.Interfaces.Ui;

namespace MHavocEditor
{
    public class WindowManager : IWindowManager
    {
        private readonly KryptonDockableWorkspace _documentManager;

        private Dictionary<IUserInterface, KryptonPage> _interfaces =
            new Dictionary<IUserInterface, KryptonPage>();
        private KryptonDockingManager _dockingManager;

        public WindowManager(KryptonDockableWorkspace documentManager, KryptonDockingManager dockingManager )
        {
            this._documentManager = documentManager;
            this._dockingManager = dockingManager;

            if ( this._documentManager != null )
            {
                this._documentManager.PageCloseClicked += this.OnDocumentCloseClicked;
                //this._documentManager.ActiveCellChanged += this.OnActiveCellChanged;
                //this._documentManager.ActivePageChanged += this.OnActivePageChanged;
                //this._documentManager.AfterPageDrag += this.OnAfterPageDrag;
                //this._documentManager.BeforePageDrag += this.OnBeforePageDrag;
                //this._documentManager.GlobalLoading += this.OnGlobalLoading;
                //this._documentManager.GlobalSaving += this.OnGlobalSaving;
                //this._documentManager.MaximizedCellChanged += this.OnMaximizedCellChanged;
                //this._documentManager.WorkspaceCellAdding += this.OnWorkspaceCellAdding;
                //this._documentManager.WorkspaceCellRemoved += this.OnWorkspaceCellRemoved;
                //this._documentManager.PageLoading += this.OnPageLoading;
            }

            this._dockingManager.PageCloseRequest += this.OnDockedCloseRequest;
            //this._dockingManager.DockspaceAdding += this.OnDockspaceAdding;
            //this._dockingManager.DockspaceRemoved += this.OnDockspaceRemoved;
            //this._dockingManager.DockspaceCellAdding += this.OnDockspaceCellAdding;
            //this._dockingManager.DockspaceSeparatorAdding += this.OnDockspaceSeparatorAdding;
            //this._dockingManager.DockspaceSeparatorResize += this.OnDockspaceSeparatorResize;
            //this._dockingManager.FloatspaceAdding += this.OnFloatspaceAdding;
            //this._dockingManager.FloatingWindowAdding += this.OnFloadingWindowAdding;
            //this._dockingManager.FloatingWindowRemoved += this.OnFloatingWindowRemoved;

        }

        private void OnFloatingWindowRemoved(object sender, FloatingWindowEventArgs e)
        {
        }

        private void OnFloadingWindowAdding(object sender, FloatingWindowEventArgs e)
        {
        }

        private void OnFloatspaceAdding(object sender, FloatspaceEventArgs e)
        {
        }

        private void OnDockspaceSeparatorResize(object sender, DockspaceSeparatorResizeEventArgs e)
        {
        }

        private void OnDockspaceSeparatorAdding(object sender, DockspaceSeparatorEventArgs e)
        {
        }

        private void OnDockspaceCellAdding(object sender, DockspaceCellEventArgs e)
        {
        }

        private void OnDockspaceRemoved( object sender, DockspaceEventArgs e )
        {
        }

        private void OnDockspaceAdding( object sender, DockspaceEventArgs e )
        {
        }

        private void OnActiveCellChanged( object sender, ActiveCellChangedEventArgs e )
        {
        }

        private void OnActivePageChanged( object sender, ActivePageChangedEventArgs e )
        {
        }

        private void OnAfterPageDrag( object sender, PageDragEndEventArgs e )
        {
        }

        private void OnBeforePageDrag( object sender, PageDragCancelEventArgs e )
        {
        }

        private void OnGlobalLoading( object sender, XmlLoadingEventArgs e )
        {
        }

        private void OnGlobalSaving( object sender, XmlSavingEventArgs e )
        {
        }

        private void OnMaximizedCellChanged( object sender, EventArgs e )
        {
        }

        private void OnWorkspaceCellAdding( object sender, WorkspaceCellEventArgs e )
        {
        }

        private void OnWorkspaceCellRemoved( object sender, WorkspaceCellEventArgs e )
        {
        }

        private void OnPageLoading( object sender, PageLoadingEventArgs e )
        {
        }

        public WindowManager(KryptonDockingManager dockingManager)
            : this( null, dockingManager )
        {}

#region Implementation of IWindowManager

        public object Manager
        {
            get { return this; }
        }

        public IEnumerable<IUserInterface> Interfaces
        {
            get { return this._interfaces.Keys; }
        }

        public void Add(IUserInterface userInterface)
        {
            if (!this._interfaces.TryGetValue(userInterface, out KryptonPage window))
            {
                window = userInterface as KryptonPage ??
                         CreateUserInterfaceContainer( userInterface );

                KryptonPageFlags flags =
                    KryptonPageFlags.AllowConfigSave |
                    KryptonPageFlags.DockingAllowClose |
                    KryptonPageFlags.DockingAllowDropDown |
                    KryptonPageFlags.AllowPageReorder |
                    KryptonPageFlags.AllowPageDrag;

                if (userInterface.DockingPositions.HasFlag(
                    DockingPosition.Left |
                    DockingPosition.Right |
                    DockingPosition.Top |
                    DockingPosition.Bottom ))
                {
                    flags |= KryptonPageFlags.DockingAllowDocked |
                             KryptonPageFlags.DockingAllowAutoHidden;
                }

                if (userInterface.DockingPositions.HasFlag(DockingPosition.Float))
                {
                    flags |= KryptonPageFlags.DockingAllowFloating;
                }

                if (userInterface.DockingPositions.HasFlag(DockingPosition.Document))
                {
                    flags |= KryptonPageFlags.DockingAllowWorkspace;
                }

                window.Flags = (int) flags;

                this._interfaces.Add(userInterface, window);
            }

            KryptonPage CreateUserInterfaceContainer(IUserInterface uI)
            {
                KryptonPage kp = new KryptonPage
                                 {
                                     Text = uI.Caption,
                                     TextTitle = uI.Caption,
                                     TextDescription = uI.Caption,
                                     Dock = DockStyle.Fill
                                     //ImageSmall = uI.Icon
                                 };

                kp.Controls.Add((Control)userInterface);

                return kp;
            }
        }

        private void OnDockedCloseRequest(object sender, CloseRequestEventArgs e)
        {
            IUserInterface found =
                this._interfaces.FirstOrDefault( kvp => kvp.Value.UniqueName == e.UniqueName )
                    .Key;

            // Ignore set if not found?
            e.CloseRequest = found?.HideOnClose ?? false ?
                                 DockingCloseRequest.HidePage :
                                 DockingCloseRequest.RemovePageAndDispose;

            if ( e.CloseRequest != DockingCloseRequest.HidePage & found != null )
            {
                this._interfaces.Remove( found );
            }
        }

        private void OnDocumentCloseClicked(object sender, UniqueNameEventArgs e)
        {
            _ = this._interfaces.FirstOrDefault(
                kvp => kvp.Value.UniqueName == e.UniqueName &&
                       this._interfaces.Remove( kvp.Key ) );

            //KeyValuePair<IUserInterface, KryptonPage> found =
            //    this._interfaces.FirstOrDefault(kvp => kvp.Value.UniqueName == e.UniqueName);

            ///// Documents are always closed???
            //this._interfaces.Remove( found.Key );
        }

        //private void OnWindowClosed( object sender, EventArgs e )
        //{
        //    if ( sender is DockContent content && !content.HideOnClose )
        //    {
        //        this._interfaces.Remove(  )
        //    }
        //}

        public bool Remove(IUserInterface userInterface)
        {
            throw new NotImplementedException();
        }

        public void Show(IUserInterface userInterface)
        {
            this.InternalShow(userInterface, false);
        }
         
        private void InternalShow(IUserInterface userInterface, bool hidden)
        {
            if (this._interfaces.TryGetValue(userInterface, out KryptonPage window))
            {
                if (userInterface.DockingState == DockingState.Document)
                {
                    /// BUG: Better way to do this with a notification?
                    if ( this._documentManager == null )
                    {
                        return;
                    }

                    // Get access to current active cell or create new cell if none are present
                    KryptonWorkspaceCell cell = this._documentManager.ActiveCell;

                    if (cell == null)
                    {
                        cell = new KryptonWorkspaceCell();
                        this._documentManager.Root.Children.Add(cell);
                    }

                    if (!cell.Pages.Contains(window))
                    {
                        cell.Pages.Add(window);
                    }

                    cell.SelectedPage = window;
                }
                else if ( userInterface.DockingState == DockingState.Float )
                {
                    // Add a new floating window with a single page
                    this._dockingManager.AddFloatingWindow( "Floating", new[] { window } );
                }
                else
                {
                    DockingEdge dockingEdge = userInterface.DockingState == DockingState.DockLeft ?
                                DockingEdge.Left :
                                userInterface.DockingState == DockingState.DockRight ?
                                    DockingEdge.Right :
                                    userInterface.DockingState == DockingState.DockBottom ?
                                        DockingEdge.Bottom :
                                        DockingEdge.Top;
                    if (hidden)
                    {
                        this._dockingManager.AddAutoHiddenGroup(
                            "Control", dockingEdge, new[] { window });
                    }
                    else
                    {
                        this._dockingManager.AddDockspace(
                            "Control", dockingEdge, new[] { window });
                    }
                }
            }
            else
            {
                this.Add(userInterface);

                this.InternalShow(userInterface, hidden);
            }
        }

        public void Hide(IUserInterface userInterface)
        {
            this.InternalShow(userInterface, true);
        }

        public void SaveLayout()
        {
            throw new NotImplementedException();
        }

        public void SaveLayout(string path)
        {
            throw new NotImplementedException();
        }

        public void RestoreLayout()
        {
            throw new NotImplementedException();
        }

        public void RestoreLayout(string path)
        {
            throw new NotImplementedException();
        }

#endregion
    }

}