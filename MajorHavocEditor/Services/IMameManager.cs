using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.Containers.Validation;
using mhedit.Extensions;
using mhedit.GameControllers;

namespace MajorHavocEditor.Services
{

    public interface IMameManager
    {
        //void Preview( Maze maze );

        //void Preview( MazeCollection mazeCollection );
        void Preview( MazeCollection mazeCollection, Maze maze );
    }

    public class MameManager : IMameManager
    {
        private StringBuilder _errorOut;

#region Implementation of IMameManager

        ///// <inheritdoc />
        //public void Preview( Maze maze )
        //{
        //    // Always create a MazeCollection for the preview of a Maze. Otherwise, any
        //    // existing validation errors in the parent MazeCollection will keep us from
        //    // being able to run the Preview of this level. So build a temporary collection
        //    // from the template.
        //    IGameController mhpe = new MajorHavocPromisedEnd();

        //    if ( mhpe.LoadTemplate(
        //        Path.GetFullPath( Properties.Settings.Default.TemplatesLocation ) ) )
        //    {
        //        MazeCollection mazeCollection = mhpe.LoadMazes( new List<string>() );

        //        // inject our single maze based upon the maze type A=0,B=1,C=2,D=3
        //        mazeCollection.Mazes[ (int) maze.MazeType ] = maze;

        //        this.InternalPreview( mazeCollection, (int) maze.MazeType );
        //    }
        //}

        ///// <inheritdoc />
        //public void Preview( MazeCollection mazeCollection )
        //{
        //    // Run the entire maze.

        //}

        // Stuck with this for now... Need to fix the inputs to this process.
        public void Preview( MazeCollection mazeCollection, Maze maze )
        {
            this.InternalPreview( mazeCollection, maze );
        }

#endregion

        private void InternalPreview( MazeCollection mazeCollection, Maze maze )
        {
            // Now that we have a Maze in a MazeCollection we can build a ROM set to preview from.
            if ( this.SaveROM( mazeCollection, maze ) )
            {
                //now launch MAME for mhavoc..
                string mameExe =
                    Path.GetFullPath( Properties.Settings.Default.MameExecutable );

                string args = "";

                if ( Properties.Settings.Default.MameDebug )
                {
                    args += "-debug ";
                }

                if ( Properties.Settings.Default.MameWindow )
                {
                    args += "-window ";
                }

                if ( !string.IsNullOrWhiteSpace(
                         Properties.Settings.Default.MameCommandLineOptions ) )
                {
                    // force at least 1 space on end.
                    args += $"{Properties.Settings.Default.MameCommandLineOptions} ";
                }

                args += Properties.Settings.Default.MameDriver;

                try
                {
                    Process p =
                        new Process
                        {
                            EnableRaisingEvents = true,
                            StartInfo = new ProcessStartInfo( mameExe, args )
                                        {
                                            UseShellExecute = false,
                                            WorkingDirectory = Path.GetDirectoryName( mameExe ),
                                            //RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                        }
                        };

                    /// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.beginerrorreadline?view=net-6.0
                    //p.OutputDataReceived += this.ProcessOutputDataHandler;
                    p.ErrorDataReceived += this.ProcessErrorDataHandler;
                    p.Exited += this.ProcessExited;
                    p.Start();
                    //p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    //p.WaitForExit();
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( $"There was an error launching HBMAME (" + mameExe + " " +
                                     args + ")." +
                                     $" Verify your HBMAME paths in the configuration. {ex.Message}" );
                }
            }
        }

        public bool SaveROM( MazeCollection collection, Maze maze )
        {
            bool success = false;

            if ( collection != null && maze != null )
            {
                if ( !File.Exists( Properties.Settings.Default.MameExecutable ) )
                {
                    MessageBox.Show( "MAME Executable not found. Check path in Preferences",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk );

                    return false;
                }

                // Path.Combine is WONKY!
                // https://stackoverflow.com/a/53118
                string mameRomPath = Path.Combine(
                    Path.GetDirectoryName( Properties.Settings.Default.MameExecutable ),
                    "roms",
                    Properties.Settings.Default.MameDriver );

                if ( !Directory.Exists( mameRomPath ) )
                {
                    Directory.CreateDirectory( mameRomPath );
                }

                string backupPath = Path.Combine( mameRomPath + @"\_backup" );

                //delete the current backup folder so we can make a fresh copy
                if ( Directory.Exists( backupPath ) )
                {
                    Directory.Delete( backupPath, true );
                }

                Directory.CreateDirectory( backupPath );

                if ( File.Exists( Path.Combine(
                    Path.GetDirectoryName( Properties.Settings.Default.MameExecutable ),
                    "roms",
                    $"{Properties.Settings.Default.MameDriver}.zip" ) ) )
                {
                    MessageBox.Show(
                        "The MAME driver you have specified is using a zipped ROM archive. The level editor does not support zipped ROM's. Please extract your '" +
                        Properties.Settings.Default.MameDriver + ".zip' file to a '" +
                        Properties.Settings.Default.MameDriver +
                        "' folder under the 'roms' folder and delete the .zip file. The level editor will then overwrite your mhavoc ROM's as needed in order to run the level you have created.",
                        "MAME Configuration Issue", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation );

                    return false;
                }

                //copy each file here into _backup folder
                foreach ( string file in
                    Directory.EnumerateFiles( mameRomPath, "*.*", SearchOption.TopDirectoryOnly ) )
                {
                    File.Copy( file, Path.Combine( backupPath, Path.GetFileName( file ) ), true );
                }

                IValidationResult validationResult = maze.Validate();

                if ( validationResult.Level < ValidationLevel.Error )
                {
                    string templatePath = Path.Combine(
                        Path.GetDirectoryName( Application.ExecutablePath ), "template" );

                    //we will always serialize to target 'The Promised End' here in this editor.
                    IGameController controller = new MajorHavocPromisedEnd();
                    bool loadSuccess = controller.LoadTemplate( templatePath );

                    if ( loadSuccess )
                    {
                        bool serializeSuccess = controller.EncodeObjects( collection );

                        if ( serializeSuccess )
                        {
                            controller.SetStartingMaze( collection.Mazes.IndexOf( maze ) );

                            success = controller.WriteFiles( mameRomPath,
                                Properties.Settings.Default.MameDriver );
                        }
                        else
                        {
                            MessageBox.Show(
                                "There was an issue serializing the maze objects to binary: " +
                                controller.LastError, "Serialization Errors", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation );
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "There was an issue loading the maze objects: " + controller.LastError,
                            "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                    }
                }
                else
                {
                    MessageBox.Show( validationResult.ToString(), "Validation Errors",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
            }

            return success;
        }

        private void ProcessOutputDataHandler(object sender, DataReceivedEventArgs e)
        {
            /// TODO: This should be redirected to a Logger/Output window.
        }

        private void ProcessErrorDataHandler( object sender, DataReceivedEventArgs e )
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                this._errorOut ??= new StringBuilder();

                this._errorOut.AppendLine( e.Data );
            }
            else if ( sender is Process p )
            {
                if ( p.HasExited && p.ExitCode != 0 && this._errorOut?.Length > 0 )
                {
                    MessageBox.Show(
                        this._errorOut.ToString(),
                        "MAME Execution Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error );
                }
            }
        }

        private void ProcessExited( object sender, EventArgs e )
        {
            //put back the ROM's from backup
            string mamePath = Path.Combine(
                Path.GetDirectoryName(Properties.Settings.Default.MameExecutable),
                "roms",
                Properties.Settings.Default.MameDriver);

            string backupPath = Path.Combine(mamePath + @"\_backup");

            foreach (string file in
                Directory.EnumerateFiles(backupPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                File.Copy(file, Path.Combine(mamePath, Path.GetFileName(file)), true);
            }
        }
    }

}
