using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace mhedit.Containers
{
    public enum MultiSelectMode
    {
        Single,  // no keys 
        Range = Keys.Shift,
        Multiple = Keys.Control,
        Invalid = Keys.Control | Keys.Shift
    }

    public enum MultiSelectState
    {
        Canceled,
        MouseDown,
        MouseDownSecondClick,
        MouseUp,
        ItemDrag,
        BeforeSelect,
        AfterSelect
    }

    public class AugmentedTreeview : TreeView
    {
        private readonly List<TreeNode> _selectedNodes = new List<TreeNode>();
        private TreeNode _currentSelection;
        private MultiSelectMode _mode;
        private MultiSelectState _state;
        private bool _cancelSelectedNode;
        private bool _isMultSelection;
        private bool _beginUpdateCalled;

        public AugmentedTreeview()
        {
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;

            this.FullRowSelect = true;

            this.HideSelection = false;
        }

        public List<TreeNode> SelectedNodes
        {
            get { return this._selectedNodes; }
            //set { this._selectedNodes = value; }
        }

#region Overrides of TreeView

        /// Used to draw selection highlight for the Selected Node.
        private static Pen DashedPen = new Pen( Color.DarkOrange )
                                       {
                                           DashStyle = DashStyle.Dot
                                       };

        protected override void OnDrawNode( DrawTreeNodeEventArgs e )
        {
            /// Force the default of this event to be true, but give the user a chance to
            /// override.
            e.DrawDefault = true;

            base.OnDrawNode( e );

            /// Only draw node text if the user asks us to.
            if ( e.DrawDefault )
            {
                e.DrawDefault = false;

                /// There's something weird going on here. The bounds provided by the event are different
                /// than those that the node has, AND if you use the given bounds and try to draw lines
                /// around the outside to show it's selected... it doesn't show the line on the bottom.
                /// Almost like the nodes are overlapped?!? Anyway, make the rectangle smaller and use that
                /// to get a decent selected node border that matches the stock TreeView control.
                Rectangle adjustedBounds = new Rectangle( e.Bounds.X, e.Bounds.Y,
                    e.Bounds.Width - 1, e.Bounds.Height - 1 );

                e.Graphics.FillRectangle(
                    new SolidBrush( e.Node.IsEditing ? e.Node.TreeView.BackColor : e.Node.BackColor ),
                    adjustedBounds );

                if ( e.Node.BackColor == SystemColors.Highlight )
                {
                    e.Graphics.DrawRectangle( Pens.Black, adjustedBounds );
                    e.Graphics.DrawRectangle( DashedPen, adjustedBounds );
                }

                /// Draw the empty string when editing to keep it from showing up behind during the
                /// edit.
                TextRenderer.DrawText( e.Graphics,
                    e.Node.IsEditing ? string.Empty : e.Node.Text,
                    e.Node.TreeView.Font,
                    adjustedBounds,
                    e.Node.ForeColor );
            }
        }

        protected override void OnMouseDown( MouseEventArgs e )
        {
            try
            {
                base.OnMouseDown( e );

                this._state = MultiSelectState.MouseDown;

                this._cancelSelectedNode = false;

                this._mode = (MultiSelectMode)( ModifierKeys & ( Keys.Control | Keys.Shift ) );

                /// Always need to know what, if any, node is clicked on.
                TreeNode selected = this.GetNodeAt( e.Location );

                this._currentSelection = selected != null &&
                                         e.Button == MouseButtons.Left &&
                                         e.Location.X >= selected.Bounds.Left &&
                                         e.Location.X <= selected.Bounds.Right ?
                                             selected : null;

                this._isMultSelection = e.Clicks == 1 &&
                                        this._currentSelection != null &&
                                        ( this._mode == MultiSelectMode.Multiple ||
                                          this._mode == MultiSelectMode.Range );

                if ( this._isMultSelection )
                {
                    this.BeginUpdate();
                }
                else if ( e.Clicks == 2 )
                {
                    this._state = MultiSelectState.MouseDownSecondClick;

                    this.CancelMultiSelect();
                }

                Debug.WriteLine( $"OnMouseDown {e.Clicks} {this._isMultSelection} {this._cancelSelectedNode} {this._mode} {this._currentSelection}" );
            }
            catch ( Exception ex )
            {
                this.HandleException( ex );
            }
        }

        protected override void OnItemDrag( ItemDragEventArgs e )
        {
            this._state = MultiSelectState.ItemDrag;

            /// Drag drop operations cancel all MultSelection ops.
            this.CancelMultiSelect();

            Debug.WriteLine( $"OnItemDrag" );

            base.OnItemDrag( e );
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            try
            {
                base.OnMouseUp( e );

                this._state = MultiSelectState.MouseUp;

                /// Only deal with UNSELECTING a node in OnMouseUp.
                if ( this._currentSelection == this.SelectedNode )
                {
                    if ( this._selectedNodes.Count <= 1 )
                    {
                        /// This is the only selected node. Ignore the "reselect". This
                        /// is how the base TreeView works for a single selected node.
                    }
                    else
                    {
                        /// There are multiple selected nodes, unselect the current node
                        this._cancelSelectedNode =
                            this.RemoveSelectedNode( this._currentSelection );

                        /// but don't reselect a previously selected node.
                        //this._currentSelection = null;

                        /// or possibly do select the previously selected node.
                        this.SelectedNode = this._selectedNodes.FirstOrDefault();
                    }
                }
                else if ( this._isMultSelection && this._selectedNodes.Count > 1 )
                {
                    this._cancelSelectedNode =
                        this.RemoveSelectedNode( this._currentSelection );
                }

                Debug.WriteLine( $"OnMouseUp {e.Clicks} {this._isMultSelection} {this._cancelSelectedNode} {this._mode} {this._currentSelection}" );
            }
            catch ( Exception ex )
            {
                this.HandleException( ex );
            }
        }

        protected override void OnBeforeSelect( TreeViewCancelEventArgs e )
        {
            try
            {
                if ( e.Action == TreeViewAction.ByKeyboard )
                {
                    /// user uses arrow keys to navigate tree
                    /// Keys.Shift will cause event.
                    /// Keys.Control will not cause event. (used to scroll the TreeView
                    /// without changing selection)

                    this._isMultSelection = false;
                }
                else if ( e.Action == TreeViewAction.ByMouse )
                {
                    /// user clicks on Node
                    if ( this._isMultSelection )
                    {
                        e.Cancel = this._cancelSelectedNode;
                    }
                    else
                    {
                        e.Cancel = this._currentSelection == null;
                    }
                }
                else if ( e.Action == TreeViewAction.Unknown )
                {
                    /// code sets CurrentSelection
                    /// User code could do this
                    /// MultiSelect ops may change TreeView._currentSelection.
                    e.Cancel = this._state == MultiSelectState.MouseDown;
                }

                /// Cancel if requested...
                e.Cancel = e.Cancel | this._state == MultiSelectState.Canceled;

                /// if the multiselect actions didn't already cancel the Select.
                if ( !e.Cancel )
                {
                    /// Give the user a chance to cancel the select operation.
                    //TreeViewCancelEventArgs userArgs =
                    //    new TreeViewCancelEventArgs( e.Node, false, e.Action );

                    base.OnBeforeSelect( e );

                    /// Act on previous decisions if user didn't cancel.
                    if ( !e.Cancel )
                    {
                        if ( !this._isMultSelection )
                        {
                            this.ClearSelectedNodes();
                        }

                        this.AddSelectedNode( e.Node );
                    }

                    //e.Cancel = userArgs.Cancel;
                }
            }
            catch ( Exception ex )
            {
                e.Cancel = true;

                this.HandleException( ex );
            }
            finally
            {
                this._state = MultiSelectState.BeforeSelect;

                Debug.WriteLine( $"OnBeforeSelect {e.Action} {e.Node} {this._isMultSelection} {this._cancelSelectedNode} {this._mode} {this._currentSelection}" );

                this.EndUpdate();
            }
        }

        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            try
            {
                this._state = MultiSelectState.AfterSelect;

                base.OnAfterSelect( e );

                this._isMultSelection = false;

                Debug.WriteLine( $"OnAfterSelect {e.Action} {e.Node} {this._isMultSelection} {this._cancelSelectedNode} {this._mode} {this._currentSelection}" );
            }
            catch ( Exception ex )
            {
                this.HandleException( ex );
            }
        }

#endregion

        private new void BeginUpdate()
        {
            if ( !this._beginUpdateCalled )
            {
                base.BeginUpdate();

                this._beginUpdateCalled = true;

                Debug.WriteLine( $"BeginUpdate" );
            }
        }

        private new void EndUpdate()
        {
            if ( this._beginUpdateCalled )
            {
                this._beginUpdateCalled = false;

                base.EndUpdate();

                Debug.WriteLine( $"EndUpdate" );
            }
        }

        private void CancelMultiSelect()
        {
            this.EndUpdate();

            this._isMultSelection = false;

            this._cancelSelectedNode = true;

            this._currentSelection = null;

            this._state = MultiSelectState.Canceled;
        }

        private void ClearSelectedNodes()
        {
            Debug.WriteLine( $"Clear All" );

            foreach ( TreeNode node in this._selectedNodes )
            {
                node.BackColor = this.BackColor;

                node.ForeColor = this.ForeColor;
            }

            this._selectedNodes.Clear();
        }

        private void AddSelectedNode( TreeNode node )
        {
            if ( !this._selectedNodes.Contains( node ) )
            {
                Debug.WriteLine( $"Add {node}" );

                /// last selected node is always first in list.
                this._selectedNodes.Insert( 0, node );

                node.BackColor = SystemColors.Highlight;

                node.ForeColor = SystemColors.HighlightText;
            }
        }

        private bool RemoveSelectedNode( TreeNode node )
        {
            if ( this._selectedNodes.Remove( node ) )
            {
                Debug.WriteLine( $"Remove {node}" );

                node.BackColor = this.BackColor;

                node.ForeColor = this.ForeColor;

                return true;
            }

            return false;
        }

        private void HandleException( Exception ex )
        {
            /// CancelMultiSelect all MultSelection ops.
            this.CancelMultiSelect();

            // Perform some error handling here.
            //MessageBox.Show( ex.Message );
        }
    }
}
