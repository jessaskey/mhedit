using mhedit.Containers;
using mhedit.GameControllers;
using System.IO;
using System.Windows.Forms;
using mhedit.Containers.Validation;

namespace mhedit.Controllers
{
    static class MAMEHelper
    {
        public static bool SaveROM(MazeCollection collection, Maze maze)
        {
            bool success = false;

            if (collection != null && maze != null)
            {
                string mamePath = Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + "\\";

                string templatePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\template\\";

                if (!File.Exists(Properties.Settings.Default.MameExecutable))
                {
                    MessageBox.Show("MAME Executable not found. Check path in Preferences", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return false;
                }

                if (!Directory.Exists(mamePath))
                {
                    Directory.CreateDirectory(mamePath);
                }

                string backupPath = mamePath + "\\_backup\\";

                //delete the current backup folder so we can make a fresh copy
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, true);
                }

                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                if (File.Exists(Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + ".zip"))
                {
                    MessageBox.Show("The MAME driver you have specified is using a zipped ROM archive. The level editor does not support zipped ROM's. Please extract your '" + Properties.Settings.Default.MameDriver + ".zip' file to a '" + Properties.Settings.Default.MameDriver + "' folder under the 'roms' folder and delete the .zip file. The level editor will then overwrite your mhavoc ROM's as needed in order to run the level you have created.", "MAME Configuration Issue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                foreach (string file in Directory.GetFiles(mamePath))
                {
                    //copy each file here into _backup folder
                    File.Copy(file, backupPath + Path.GetFileName(file), true);
                }

                IValidationResult validationResult = maze.Validate();

                if ( validationResult.Level < ValidationLevel.Error )
                {
                    //we will always serialize to target 'The Promised End' here in this editor.
                    IGameController controller = new MajorHavocPromisedEnd();
                    bool loadSuccess = controller.LoadTemplate(templatePath);
                    if (loadSuccess)
                    {
                        bool serializeSuccess = controller.EncodeObjects(collection);
                        if (serializeSuccess)
                        {
                            controller.SetStartingMaze(collection.Mazes.IndexOf(maze));
                            success = controller.WriteFiles(mamePath, Properties.Settings.Default.MameDriver);
                            if (!success)
                            {
                                MessageBox.Show("There was an error writing ROM files to the MAME directory: " + controller.LastError, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("There was an issue serializing the maze objects to binary: " + controller.LastError, "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("There was an issue loading the maze objects: " + controller.LastError, "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show( validationResult.ToString(), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    success = false;
                }
            }
            return success;
        }

    }
}
