using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Views
{

    public partial class MultiSelectTreeView : TreeView
    {
        private NodeManager _nodeManager;
        private ISelectedNodes _selectedNodes;
        private TreeNode _mouseDownMultiSelectNode;
        private TreeNode _mouseDownSelectedNode;
        private bool _cancelUnwantedLabelEdit;

        public MultiSelectTreeView()
        {
            this._selectedNodes = new SelectedNodes( this.SearchNodesForTag );
            this._nodeManager = new NodeManager( this );
            this.LabelEdit = true;
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.HideSelection = false;
            this.ShowLines = false;
        }

        /// <summary>
        /// A hierarchical collection of items that are contained in the TreeView.
        /// An item that implements <see cref="Enumerable"/> will be set as a parent
        /// node with it's children expanded below.
        /// </summary>
        public IList ItemsSource
        {
            get { return this._nodeManager.Items; }
            set
            {
                this._nodeManager.Items.Clear();

                /// Forward the ItemsDelegate, as it would typically be set before
                /// the ItemsSource so that existing items in the collection get
                /// added properly.
                this._nodeManager =
                    new NodeManager( value, this._nodeManager.ItemsDelegate, this );
            }
        }

        /// <summary>
        /// Could put this beside the ItemSource and allow users to set the ItemSource
        /// with their own collection....
        /// </summary>
        public IItemsSourceDelegate ItemsDelegate
        {
            get { return this._nodeManager.ItemsDelegate; }
            set { this._nodeManager.ItemsDelegate = value; }
        }

        /// <summary>
        /// The collection of selected Items in the TreeView. If the user wants to
        /// be able to manipulate the set of SelectedItems they should set the property
        /// with their own <see cref="IList"/> that implements INotifyCollectionChanged.
        /// </summary>
        public IList SelectedItems
        {
            get { return this._selectedNodes.SelectedItems; }
            set
            {
                this._selectedNodes.Clear();

                this._selectedNodes = new SelectedNodes(value, this.SearchNodesForTag);
            }
        }

        /// <summary>
        /// Gets the selected Item. If <see cref="SelectedItems.Count"/> is greater
        /// than 1, it gets the most recently selected item. 
        /// Sets the selected Item. If <see cref="SelectedItems.Count"/> is greater
        /// than 1, it will clear the <see cref="SelectedItems"/> and then select
        /// the item. 
        /// </summary>
        public object SelectedItem
        {
            get { return this.SelectedNode?.Tag; }
            set
            {
                this.SelectedItems.Clear();

                this.SelectedItems.Add( value );
            }
        }

#region Overrides of TreeView

#region Label Edit Overrides

        /// <inheritdoc />
        protected override void OnBeforeLabelEdit( NodeLabelEditEventArgs e )
        {
            base.OnBeforeLabelEdit( e );

            if ( ( ModifierKeys & ( Keys.Control | Keys.Shift ) ) > 0 ||
                 this._cancelUnwantedLabelEdit )
            {
                e.CancelEdit = true;
            }
        }

        /// <inheritdoc />
        protected override void OnAfterLabelEdit( NodeLabelEditEventArgs e )
        {
            base.OnAfterLabelEdit( e );

            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.nodelabelediteventargs?view=netframework-4.7.2
            if ( !e.CancelEdit && e.Label != null )
            {
                try
                {
                    // Stop editing without canceling the label change.
                    e.Node.EndEdit( false );

                    // Expect the IName implementer to throw if the name is invalid.
                    if ( e.Node.Tag is IName iName )
                    {
                        iName.Name = e.Label;
                    }
                }
                catch ( Exception ex )
                {
                    // Cancel the label edit action, inform the user, and 
                    // place the node in edit mode again.
                    e.CancelEdit = true;

                    MessageBox.Show( $"{ex.Message}", "Label Edit Error" );

                    e.Node.BeginEdit();
                }
            }
        }

#endregion

#region Draw Node Overrides

        /// <inheritdoc />
        /// TODO: Move Editing/ChangeTracking outside of the Treeview? 
        protected override void OnDrawNode( DrawTreeNodeEventArgs e )
        {
            /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.drawnode?redirectedfrom=MSDN&view=netframework-4.7.2
            //TODO: User Override
            /// Force the default of this event to be true, but give the user a chance to
            /// override.
            //e.DrawDefault = true;

            base.OnDrawNode( e );

            ///// Only draw node text if the user asks us to.
            //if ( !e.DrawDefault )
            //{
            //    return;
            //}

            //e.DrawDefault = false;

            Rectangle adjustedBounds =
                new Rectangle( e.Bounds.X, e.Bounds.Y, this.Bounds.Width, e.Bounds.Height );

            // Retrieve the node font. If the node font has not been set, use the TreeView font.
            Font nodeFont = e.Node.NodeFont ?? this.Font;

            if ( e.Node.IsEditing )
            {
                // While editing the Node Text don't paint the existing name behind.
                e.Graphics.FillRectangle( new SolidBrush( SystemColors.Window ), adjustedBounds );
            }

            // Draw the background and node text for a selected node.
            else if ( ( e.State & ( TreeNodeStates.Selected | TreeNodeStates.Focused ) ) != 0 ||
                      e.Node.Checked )
            {
                // Draw the background of the selected node. 
                //TODO: Don't paint in background when Focused.
                e.Graphics.FillRectangle( new SolidBrush( SystemColors.Highlight ),
                    adjustedBounds );

                // Draw the node text.
                TextRenderer.DrawText( e.Graphics, e.Node.Text, nodeFont, adjustedBounds,
                    SystemColors.HighlightText, TextFormatFlags.Left );
            }
            else // Use the default background and node text.
            {
                e.DrawDefault = true;
            }

            // Draw an extra char to the right if the node has been edited.
            if ( e.Node.Tag is IChangeTracking ict )
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    ict.IsChanged ? "*" : "",
                    nodeFont,
                    new Point( e.Node.Bounds.Right + 4, e.Node.Bounds.Top ),
                    ( e.State & TreeNodeStates.Selected ) != 0 ?
                        SystemColors.HighlightText :
                        SystemColors.ControlText,
                    TextFormatFlags.Left );
            }

            // If the node has focus, draw the focus rectangle.
            if ( ( e.State & TreeNodeStates.Focused ) != 0 )
            {
                using ( Pen focusPen = new Pen( Color.Black ) )
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                    Rectangle focusBounds = adjustedBounds;

                    focusBounds.Size =
                        new Size( focusBounds.Width - 1, focusBounds.Height - 1 );

                    e.Graphics.DrawRectangle( focusPen, focusBounds );
                }
            }
        }

#endregion

        /// <inheritdoc />
        protected override void OnBeforeSelect( TreeViewCancelEventArgs e )
        {
            base.OnBeforeSelect( e );

            if ( !ModifierKeys.HasFlag( Keys.Control ) && e.Action != TreeViewAction.Unknown )
            {
                this._selectedNodes.Clear();
            }
            else if ( e.Node == this._mouseDownMultiSelectNode )
            {
                e.Cancel = true;

                this._mouseDownMultiSelectNode = null;
            }
        }

        /// <inheritdoc />
        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            base.OnAfterSelect( e );

            this._selectedNodes.Add( e.Node );
        }

#endregion

#region Overrides of Control

        /// <inheritdoc />
        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            // Reset every Down/up cycle...
            this._cancelUnwantedLabelEdit = false;
            this._mouseDownMultiSelectNode = null;
            this._mouseDownSelectedNode = null;

            TreeNode clickedNode = this.GetNodeAt( e.X, e.Y );

            if ( clickedNode != null && e.Button == MouseButtons.Left )
            {
                // This captures clicking on an already selected node, which isn't
                // the Treeview.SelectedNode, to unselect.
                this._mouseDownMultiSelectNode =
                    clickedNode.Checked &&
                    this._selectedNodes.Count > 1 &&
                    ModifierKeys.HasFlag( Keys.Control ) &&
                    clickedNode.Bounds.Contains( e.X, e.Y ) ? clickedNode : null;

                // This captures clicking on the Treeview.SelectedNode to unselect.
                this._mouseDownSelectedNode =
                    this._selectedNodes.Count > 1 &&
                    clickedNode == this.SelectedNode ? clickedNode : null;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseUp( MouseEventArgs e )
        {
            if ( this._mouseDownMultiSelectNode != null )
            {
                // If control key still applied and MouseUp occurs on the same (MouseDown) Node.
                if ( ModifierKeys.HasFlag( Keys.Control ) &&
                     this._mouseDownMultiSelectNode.Bounds.Contains( e.X, e.Y ) )
                {
                    this._mouseDownMultiSelectNode.Checked = false;

                    this._selectedNodes.Remove( this._mouseDownMultiSelectNode );

                    this.SelectedNode = this._selectedNodes.LastOrDefault();
                }
                else
                {
                    this._mouseDownMultiSelectNode = null;
                }
            }

            if ( this._mouseDownSelectedNode != null )
            {
                // If No Control Key (single select) and MouseUp also occurs on
                // TreeView.SelectedNode
                if ( !ModifierKeys.HasFlag( Keys.Control ) &&
                     this._mouseDownSelectedNode.Bounds.Contains( e.X, e.Y ) )
                {
                    this._selectedNodes.Clear();

                    // restore the selected node.
                    this._selectedNodes.Add(this._mouseDownSelectedNode);

                    this._cancelUnwantedLabelEdit = this.LabelEdit;
                }
            }
        }

#endregion

        private bool SearchNodesForTag( object key, out TreeNode found )
        {
            return ( found = this.FindNodeOrDefault( key ) ) != null;
        }
    }

}
