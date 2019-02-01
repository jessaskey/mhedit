using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    public partial class CannonProgramEditor : Form
    {
        IonCannonProgram _program = new IonCannonProgram();

        public IonCannonProgram Program
        {
            get
            {
                return _program; ;
            }
            set
            {
                _program = value;
                BindListBox();
            }
        }

        public CannonProgramEditor()
        {
            InitializeComponent();
            LoadPresets();
        }

        public void LoadPresets()
        {
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonProgramPath = Path.Combine(applicationPath, "IonCannonPrograms" );

            toolStripComboBoxLoadPreset.Items.Clear();

            if (Directory.Exists(cannonProgramPath))
            {
                foreach(string filename in Directory.GetFiles(cannonProgramPath))
                {
                    string name = Path.GetFileNameWithoutExtension(filename);
                    toolStripComboBoxLoadPreset.Items.Add(name);
                }
            }
        }

        private void listBoxProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProgram.SelectedItem != null)
            {
                propertyGridProgram.SelectedObject = listBoxProgram.SelectedItem;
            }
            else
            {
                propertyGridProgram.SelectedObject = null;
            }
        }

        private void toolStripButtonAddMove_Click(object sender, EventArgs e)
        {
            Move ionCannonBehavior = new Move();
            _program.Add( ionCannonBehavior );
            BindListBox();
            listBoxProgram.SelectedIndex = _program.Count - 1;
        }

        private void toolStripButtonAddAngle_Click(object sender, EventArgs e)
        {
            OrientAndFire ionCannonBehavior = new OrientAndFire();
            _program.Add( ionCannonBehavior );
            BindListBox();
            listBoxProgram.SelectedIndex = _program.Count - 1;
        }

        private void toolStripButtonAddPause_Click(object sender, EventArgs e)
        {
            Pause ionCannonBehavior = new Pause();
            _program.Add( ionCannonBehavior );
            BindListBox();
            listBoxProgram.SelectedIndex = _program.Count - 1;
        }

        private void toolStripButtonAddRepeat_Click(object sender, EventArgs e)
        {
            ReturnToStart ionCannonBehavior = new ReturnToStart();
            _program.Add( ionCannonBehavior );
            BindListBox();
            listBoxProgram.SelectedIndex = _program.Count - 1;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxProgram.SelectedIndex > -1)
            {
                _program.RemoveAt(listBoxProgram.SelectedIndex);
            }
            BindListBox();
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            if (listBoxProgram.SelectedIndex > 0)
            {
                int index = listBoxProgram.SelectedIndex;
                IonCannonInstruction o = _program[index];
                _program.RemoveAt(index);
                _program.Insert(index - 1, o);
                BindListBox();
                listBoxProgram.SelectedIndex = index - 1;
            }
        }

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxProgram.SelectedIndex < (listBoxProgram.Items.Count - 2))
            {
                int index = listBoxProgram.SelectedIndex;
                IonCannonInstruction o = _program[index];
                _program.RemoveAt(index);
                _program.Insert(index + 1, o);
                BindListBox();
                listBoxProgram.SelectedIndex = index + 1;
            }
        }

        private void propertyGridProgram_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            BindListBox();
        }

        private void BindListBox()
        {
            listBoxProgram.Items.Clear();
            foreach ( IonCannonInstruction behavior in _program)
            {
                listBoxProgram.Items.Add(behavior);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonPreview_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Preview no workie. Preview in MAME please!");
            //CannonProgramPreview pd = new CannonProgramPreview(_movements);
            //pd.ShowDialog();
        }

        private void toolStripButtonSaveProgram_Click(object sender, EventArgs e)
        {
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonProgramPath = Path.Combine(applicationPath, "IonCannonPrograms" );

            if (!Directory.Exists(cannonProgramPath))
            {
                Directory.CreateDirectory(cannonProgramPath);
            }

            // Displays an OpenFileDialog so the user can select a Cursor.  
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Ion Cannon Program|*.can",
                Title = "Save Ion Cannon Program",
                InitialDirectory = cannonProgramPath,
                CheckFileExists = false,
                RestoreDirectory = true,
                CreatePrompt = false,
                //https://stackoverflow.com/a/53836140
                // GAH.. double prompt bug in latest Windows 10 Update!!!
                OverwritePrompt = false
            };

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if ( saveFileDialog.ShowDialog() == DialogResult.OK )
            {
                string name = Path.GetFileNameWithoutExtension( saveFileDialog.FileName );

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
#if DEBUG
#else
                    try
#endif
                    {
                        using ( FileStream fStream = new FileStream( saveFileDialog.FileName, FileMode.Create ) )
                        {
                            var serializer = new XmlSerializer( typeof( IonCannonProgram ) );

                            using ( var writer = XmlWriter.Create( fStream, new XmlWriterSettings { Indent = true } ) )
                            {
                                serializer.Serialize( writer, _program, Constants.XmlNamespace );
                            }
                        }

                        LoadPresets();
                    }
#if DEBUG
#else
                    catch ( Exception ex )
                    {
                        MessageBox.Show( ex.Message, "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                    }
#endif
                }
            }
        }

        private void toolStripComboBoxLoadPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            //load a preset
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonProgramPath = Path.Combine(applicationPath, "IonCannonPrograms" );

            string programFile = Path.Combine(cannonProgramPath, toolStripComboBoxLoadPreset.Text + ".can");
            if (File.Exists(programFile))
            {
#if DEBUG
#else
                try
#endif
                {
                    using ( FileStream fStream = new FileStream( programFile, FileMode.Open ) )
                    {
                        var serializer = new XmlSerializer( typeof( IonCannonProgram ) );

                        using ( var reader = XmlReader.Create( fStream ) )
                        {
                            _program = (IonCannonProgram)serializer.Deserialize( reader );
                        }
                        BindListBox();
                    }
                }
#if DEBUG
#else
                catch ( Exception ex )
                {
                    MessageBox.Show( ex.Message, "Deserialization Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
#endif
            }
        }
    }
}
