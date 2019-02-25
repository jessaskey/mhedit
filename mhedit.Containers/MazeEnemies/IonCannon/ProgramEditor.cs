﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    public partial class CannonProgramEditor : Form
    {
        private IonCannonProgram _program;

        public CannonProgramEditor( IonCannonProgram program )
        {
            InitializeComponent();

            LoadPresets();

            this._program = program;

            this.AddNewItems( 0, program );

            this._program.CollectionChanged += this.OnProgramCollectionChanged;
        }

        public IonCannonProgram Program
        {
            get
            {
                return _program;
            }
        }

        private void OnProgramCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if ( e.NewItems != null )
            {
                AddNewItems( e.NewStartingIndex, e.NewItems );
            }
        }

        private void AddNewItems( int index, IList newItems )
        {
            foreach ( IonCannonInstruction instruction in newItems )
            {
                TreeNode node = treeViewProgram.Nodes.Insert( index++, instruction.ToString() );

                node.Tag = instruction;

                treeViewProgram.SelectedNode = node;

                instruction.PropertyChanged += this.OnInstructionPropertyChanged;
            }
        }

        private void OnInstructionPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            Debug.WriteLine( $"OnInstructionPropertyChanged {sender.ToString()} {e.PropertyName}" );

            if ( e.PropertyName == "IsDirty" &&
                treeViewProgram.SelectedNode?.Tag is IonCannonInstruction instruction )
            {
                treeViewProgram.SelectedNode.Text = instruction.ToString();
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

        private void listBoxProgram_SelectedIndexChanged( object sender, EventArgs e )
        {
            propertyGridProgram.SelectedObject = treeViewProgram.SelectedNode.Tag;
        }

        private void toolStripButtonAddMove_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new Move() { IsDirty = true } );
        }

        private void toolStripButtonAddAngle_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new OrientAndFire() { IsDirty = true } );
        }

        private void toolStripButtonAddPause_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new Pause() { IsDirty = true } );
        }

        private void toolStripButtonAddRepeat_Click( object sender, EventArgs e )
        {
            this.AddInstruction( new ReturnToStart() { IsDirty = true } );
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
            if ( treeViewProgram.SelectedNode != null )
            {
                DialogResult result = MessageBox.Show(
                    $"{treeViewProgram.SelectedNode.Text} will be deleted permanently?",
                    "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

                if ( result == DialogResult.OK )
                {
                    TreeNode saved = treeViewProgram.SelectedNode.PrevNode;

                    IonCannonInstruction instruction = 
                        (IonCannonInstruction)treeViewProgram.SelectedNode.Tag;

                    treeViewProgram.SelectedNode.Remove();

                    treeViewProgram.SelectedNode = saved;

                    instruction.PropertyChanged -= this.OnInstructionPropertyChanged;

                    this._program.Remove( instruction );
                }
            }
        }

        private void toolStripButtonMoveUp_Click( object sender, EventArgs e )
        {
            if ( treeViewProgram.SelectedNode != null )
            {
                int prevIndex = treeViewProgram.Nodes.IndexOf( treeViewProgram.SelectedNode.PrevNode );

                /// index will be negative on the end of the list so don't move
                if ( prevIndex >= 0 )
                {
                    TreeNode toBeMoved = treeViewProgram.SelectedNode;

                    treeViewProgram.SelectedNode.Remove();

                    treeViewProgram.Nodes.Insert( prevIndex, toBeMoved );

                    treeViewProgram.SelectedNode = toBeMoved;
                }
            }
        }

        private void toolStripButtonMoveDown_Click( object sender, EventArgs e )
        {
            if ( treeViewProgram.SelectedNode != null )
            {
                int nextIndex = treeViewProgram.Nodes.IndexOf( treeViewProgram.SelectedNode.NextNode );

                /// index will be negative on the end of the list so don't move
                if ( nextIndex >= 0 )
                {
                    TreeNode toBeMoved = treeViewProgram.SelectedNode;

                    treeViewProgram.SelectedNode.Remove();

                    treeViewProgram.Nodes.Insert( nextIndex, toBeMoved );

                    treeViewProgram.SelectedNode = toBeMoved;
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
                                serializer.Serialize( writer, _program, Constants.XmlNamespace );
                            }
                        }

                        this._program.IsDirty = false;

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

            if ( this._program.IsDirty )
            {
                result = MessageBox.Show(
                    $"There are unsaved changes. " +
                    $"Press OK to discard changes and load, or Cancel otherwise.",
                    "Load Program",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question );
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
                            _program = (IonCannonProgram)serializer.Deserialize( reader );

                            _program.IsDirty = false;

                            this.treeViewProgram.Nodes.Clear();

                            this.AddNewItems( 0, _program );
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
            if ( this._program.IsDirty && this.DialogResult == DialogResult.Cancel )
            {
                DialogResult result = MessageBox.Show(
                    $"There are unsaved changes. " +
                    $"Press OK to exit and discard changes, or Cancel to return to the editor.",
                    "Exit Editor",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question );

                e.Cancel = result == DialogResult.Cancel;
            }

            ///TODO: Validate the Program

            if ( !e.Cancel )
            {
                foreach( IonCannonInstruction instruction in this._program )
                {
                    instruction.PropertyChanged -= this.OnInstructionPropertyChanged;
                }
            }
        }
    }
}
