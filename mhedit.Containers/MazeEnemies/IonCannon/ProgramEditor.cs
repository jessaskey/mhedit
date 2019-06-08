using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    public partial class CannonProgramEditor : Form
    {
        public enum EditState
        {
            /// <summary>
            /// Indicates that this edit session started with a program that was
            /// previously unchanged. Thus the IChangeTracking.IsChanged property
            /// is set to False. I.e. This edit session started with an unmodified
            /// or previously saved program.
            /// </summary>
            UneditedProgram,

            /// <summary>
            /// Indicates that this edit session started with a program that was
            /// previously edited/changed. Thus the IChangeTracking.IsChanged property
            /// is set to True. I.e. This edit session started with a modified or
            /// previously altered program.
            /// </summary>
            EditedProgramNewSession,

            /// <summary>
            /// Indicates that this edit session has resulted in changes to the
            /// program. The IChangeTracking.IsChanged property will return True.
            /// </summary>
            ProgramEditsOccured
        }

        private IonCannonProgram _program;
        private EditState _state;

        public CannonProgramEditor( IonCannonProgram program )
        {
            InitializeComponent();

            LoadPresets();

            this._program = program;

            this.State = program.IsChanged ?
                EditState.EditedProgramNewSession : EditState.UneditedProgram;

            this.AddNewItems( 0, program );

            treeViewProgram.SelectedNode = treeViewProgram.Nodes.Count > 0 ?
                treeViewProgram.Nodes[0] :
                null;

            this._program.CollectionChanged += this.OnProgramCollectionChanged;

            ((INotifyPropertyChanged)this._program).PropertyChanged +=
                this.OnInstructionPropertyChanged;
        }

        public IonCannonProgram Program
        {
            get
            {
                return _program;
            }
        }

        public EditState State
        {
            get { return this._state; }
            private set { this._state = value; }
        }

        private void OnProgramCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            this.State = EditState.ProgramEditsOccured;

            if ( e.Action == NotifyCollectionChangedAction.Reset )
            {
                /// All elements removed
                this.treeViewProgram.Nodes.Clear();
            }
            else if ( e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null )
            {
                this.AddNewItems( e.NewStartingIndex, e.NewItems );
            }
            else if ( e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null )
            {
                this.RemoveExistingItems( e.OldStartingIndex, e.OldItems );
            }
            else if ( e.Action == NotifyCollectionChangedAction.Move )
            {
                this.MoveItems( e.NewStartingIndex, e.OldStartingIndex );
            }
            else
            {
                throw new NotSupportedException( e.Action.ToString() );
            }
        }

        private void AddNewItems( int index, IList newItems )
        {
            foreach ( IonCannonInstruction instruction in newItems )
            {
                TreeNode node = this.treeViewProgram.Nodes.Insert( index++, instruction.ToString() );

                node.Tag = instruction;

                this.treeViewProgram.SelectedNode = node;

                instruction.PropertyChanged += this.OnInstructionPropertyChanged;
            }
        }

        private void RemoveExistingItems( int index, IList eOldItems )
        {
            foreach ( IonCannonInstruction instruction in eOldItems )
            {
                this.treeViewProgram.Nodes[ index ].Remove();

                instruction.PropertyChanged -= this.OnInstructionPropertyChanged;
            }

            this.treeViewProgram.SelectedNode = this.treeViewProgram.SelectedNode?.PrevNode;
        }

        private void MoveItems( int newIndex, int oldIndex )
        {
            TreeNode toBeMoved = this.treeViewProgram.Nodes[ oldIndex ];

            toBeMoved.Remove();

            this.treeViewProgram.Nodes.Insert( newIndex, toBeMoved );

            this.treeViewProgram.SelectedNode = toBeMoved;
        }

        private void OnInstructionPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            this.State = EditState.ProgramEditsOccured;

            this.treeViewProgram.Refresh();
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.drawnode?redirectedfrom=MSDN&view=netframework-4.7.2
        /// </summary>
        private void treeViewProgram_DrawNode( object sender, DrawTreeNodeEventArgs e )
        {
            /// Only mark editor with changed if an edit has occurred during this session.
            this.Text =
                "Ion Cannon Program Editor " +
                $"{( this._state == EditState.ProgramEditsOccured ? ChangeTrackingBase.ModifiedBullet : "" )}";

            // Use the default background and node text.
            e.DrawDefault = true;

            // Extract the set font/color from the tree.
            Font nodeFont =
                new Font( e.Node.NodeFont ?? ( (TreeView)sender ).Font, FontStyle.Bold );

            SolidBrush nodeBrush = new SolidBrush( ( (TreeView)sender ).ForeColor );

            // If a node tag is present, draw the IsChanged if necessary.
            if ( e.Node.Tag is IChangeTracking changeTracking )
            {
                e.Graphics.DrawString( changeTracking.IsChanged ? ChangeTrackingBase.ModifiedBullet : "",
                    nodeFont, nodeBrush, e.Bounds.Right + 4, e.Bounds.Top );
            }
        }

        private void LoadPresets()
        {
            string applicationPath = Path.GetDirectoryName( Application.ExecutablePath );
            string cannonProgramPath = Path.Combine( applicationPath, "IonCannonPrograms" );

            toolStripComboBoxLoadPreset.Items.Clear();

            if ( Directory.Exists( cannonProgramPath ) )
            {
                foreach ( string filename in Directory.GetFiles( cannonProgramPath ) )
                {
                    string name = Path.GetFileNameWithoutExtension( filename );
                    toolStripComboBoxLoadPreset.Items.Add( name );
                }
            }
        }

        private void toolStripButtonAddMove_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new Move() );
        }

        private void toolStripButtonAddAngle_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new OrientAndFire() );
        }

        private void toolStripButtonAddPause_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new Pause() );
        }

        private void toolStripButtonAddRepeat_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new ReturnToStart() );
        }

        private void AddInstruction( IonCannonInstruction ionCannonInstruction )
        {
            int index = -1;

            if ( treeViewProgram.SelectedNode != null )
            {
                index = this._program.IndexOf(
                    treeViewProgram.SelectedNode.Tag as IonCannonInstruction );
            }

            this._program.Insert( ++index, ionCannonInstruction );
        }

        private void toolStripButtonDelete_Click( object sender, EventArgs e )
        {
            if ( treeViewProgram.SelectedNodes.Count > 0 )
            {
                DialogResult result = MessageBox.Show(
                    this.treeViewProgram.SelectedNodes.Count == 1 ?
                        $"{this.treeViewProgram.SelectedNodes.First().Name} will be deleted permanently!" :
                        $"All Selected instructions will be deleted permanently!",
                    "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

                if ( result == DialogResult.OK )
                {
                    List<TreeNode> toDelete = new List<TreeNode>( this.treeViewProgram.SelectedNodes );

                    foreach ( TreeNode node in toDelete )
                    {
                        this._program.Remove( (IonCannonInstruction) node.Tag );
                    }

                    this.treeViewProgram.SelectedNodes.Clear();

                    this.treeViewProgram.SelectedNodes.Add( this.treeViewProgram.SelectedNode );
                }
            }
        }

        private void toolStripButtonMoveUp_Click( object sender, EventArgs e )
        {
            if ( this.treeViewProgram.SelectedNode != null )
            {
                int prevIndex =
                    this.treeViewProgram.Nodes.IndexOf(
                        this.treeViewProgram.SelectedNode.PrevNode );

                /// index will be negative on the end of the list so don't move
                if ( prevIndex >= 0 )
                {
                    this._program.Move( prevIndex + 1, prevIndex );
                }
            }
        }

        private void toolStripButtonMoveDown_Click( object sender, EventArgs e )
        {
            if ( this.treeViewProgram.SelectedNode != null )
            {
                int nextIndex =
                    this.treeViewProgram.Nodes.IndexOf(
                        this.treeViewProgram.SelectedNode.NextNode );

                /// index will be negative on the end of the list so don't move
                if ( nextIndex >= 0 )
                {
                    this._program.Move( nextIndex - 1, nextIndex );
                }
            }
        }

        private void toolStripButtonPreview_Click( object sender, EventArgs e )
        {
            MessageBox.Show( "Preview no workie. Preview in MAME please!" );
            //CannonProgramPreview pd = new CannonProgramPreview(_movements);
            //pd.ShowDialog();
        }

        private void toolStripButtonSaveProgram_Click( object sender, EventArgs e )
        {
            string applicationPath = Path.GetDirectoryName( Application.ExecutablePath );
            string cannonProgramPath = Path.Combine( applicationPath, "IonCannonPrograms" );

            if ( !Directory.Exists( cannonProgramPath ) )
            {
                Directory.CreateDirectory( cannonProgramPath );
            }

            // Displays an OpenFileDialog so the user can select a Cursor.  
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Ion Cannon Program|*.can",
                Title = "Save Ion Cannon Program",
                InitialDirectory = cannonProgramPath,
                CheckFileExists = false,
                RestoreDirectory = true,
                CreatePrompt = false,
                //https://stackoverflow.com/a/53836140
                // GAH.. double prompt bug in latest Windows 10 Update!!!
                // This is apparently now fixed... lol
                OverwritePrompt = true
            };

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if ( sfd.ShowDialog() == DialogResult.OK )
            {
                string name = Path.GetFileNameWithoutExtension( sfd.FileName );

                // Define a regular expression that is true for the default program file names.
                Regex rx = new Regex(
                    @"Atari Level ([0-9][0-9]?)[abcd]? Cannon(.can)?$", RegexOptions.IgnoreCase );

                if ( rx.IsMatch( name ) )
                {
                    MessageBox.Show(
                        "Overwriting the original Cannon program sequences isn't supported.", "Overwrite Error" );
                }
                else if ( !String.IsNullOrEmpty( name ) )
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        using ( FileStream fStream = new FileStream( sfd.FileName, FileMode.Create ) )
                        {
                            var serializer = new XmlSerializer( typeof( IonCannonProgram ) );

                            using ( var writer = XmlWriter.Create( fStream, new XmlWriterSettings { Indent = true } ) )
                            {
                                /// This needs to be pulled into common serialization code but I need to create a
                                /// base/core project first.
                                serializer.Serialize( writer, _program,
                                    new XmlSerializerNamespaces( new[]
                                                                 {
                                                                     new XmlQualifiedName( "MHEdit",
                                                                         "http://mhedit.askey.org" )
                                                                 } ) );
                            }
                        }

                        this._program.AcceptChanges();

                        LoadPresets();
                    }
#if DEBUG
#else
                    catch ( Exception ex )
                    {
                        MessageBox.Show( ex.Message, "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                    }
#endif
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void toolStripComboBoxLoadPreset_SelectedIndexChanged( object sender, EventArgs e )
        {
            DialogResult result = DialogResult.OK;

            if ( this._program.IsChanged )
            {
                result = MessageBox.Show(
                    $"There are unsaved changes from the current or a previous editor session. " +
                    Environment.NewLine +
                    $"Press OK to discard changes and Load, or Cancel to return to the editor.",
                    "Load Program",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning );
            }

            //load a preset
            string applicationPath = Path.GetDirectoryName( Application.ExecutablePath );
            string cannonProgramPath = Path.Combine( applicationPath, "IonCannonPrograms" );

            string programFile = Path.Combine( cannonProgramPath, toolStripComboBoxLoadPreset.Text + ".can" );

            if ( result == DialogResult.OK && File.Exists( programFile ) )
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    using ( FileStream fStream = new FileStream( programFile, FileMode.Open ) )
                    {
                        var serializer = new XmlSerializer( typeof( IonCannonProgram ) );

                        using ( var reader = XmlReader.Create( fStream ) )
                        {
                            IonCannonProgram loadedProgram = (IonCannonProgram)serializer.Deserialize( reader );

                            loadedProgram.AcceptChanges();

                            this._program.Clear();

                            foreach ( IonCannonInstruction ionCannonInstruction in loadedProgram )
                            {
                                this._program.Add( ionCannonInstruction );
                            }
                        }
                    }
                }
#if DEBUG
#else
                catch ( Exception ex )
                {
                    MessageBox.Show( ex.Message, "Deserialization Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
#endif
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void treeViewProgram_AfterSelect( object sender, TreeViewEventArgs e )
        {
            propertyGridProgram.SelectedObject = treeViewProgram.SelectedNode?.Tag;
        }

        private void CannonProgramEditor_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( this.State == EditState.ProgramEditsOccured &&
                 this.DialogResult == DialogResult.Cancel )
            {
                DialogResult result = MessageBox.Show(
                    $"There are unsaved changes from this edit session. " +
                    Environment.NewLine +
                    $"Press OK to exit and discard changes, or Cancel to return to the editor.",
                    "Exit Editor",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning );

                e.Cancel = result == DialogResult.Cancel;
            }

            ///TODO: Validate the Program

            if ( !e.Cancel )
            {
                this._program.CollectionChanged -= this.OnProgramCollectionChanged;

                ( (INotifyPropertyChanged)this._program ).PropertyChanged -=
                    this.OnInstructionPropertyChanged;

                foreach ( IonCannonInstruction instruction in this._program )
                {
                    instruction.PropertyChanged -= this.OnInstructionPropertyChanged;
                }
            }
        }

        private void toolStripButtonValidate_Click( object sender, EventArgs e )
        {
            this._program.ValidateToMessageBox();
        }
    }
}
