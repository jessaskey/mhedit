using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.GameControllers;
using MajorHavocEditor.Views.Dialogs;

namespace MajorHavocEditor.Services
{
    public interface IRomManager
    {
        MazeCollection LoadFrom( string path = null );

        /// <summary>
        /// Splice an entire MazeCollection into a set of ROMs. The collection
        /// should be validated before it is spliced as there is no validation
        /// performed at this stage.
        /// </summary>
        /// <param name="mazeCollection"></param>
        /// <param name="path"></param>
        void Splice( MazeCollection mazeCollection, string path = null );

        /// <summary>
        /// Splice a collection of mazes to their given locations
        /// </summary>
        /// <param name="mazes">Dictionary of Mazes to be spliced into the ROMs at the
        /// given Level position. The Level position is ONE oriented.</param>
        /// <param name="path"></param>
        void Splice( IDictionary<int, Maze> mazes, string path = null );
    }

    public class RomManager : IRomManager
    {
#region Implementation of IRomManager

        /// <inheritdoc />
        public MazeCollection LoadFrom( string path = null )
        {
            DialogLoadROM dlr = new DialogLoadROM( path );

            DialogResult dr = dlr.ShowDialog();

            return dr == DialogResult.OK ? dlr.MazeCollection : null;
        }

        /// <inheritdoc />
        public void Splice( MazeCollection mazeCollection, string path = null )
        {
            OpenFileDialog ofd =
                new OpenFileDialog
                {
                    Title = "Select Paged ROM",
                    InitialDirectory =
                        Directory.Exists( Properties.Settings.Default.LastSpliceLocation ) ?
                            Properties.Settings.Default.LastSpliceLocation :
                            Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                    Filter = "ROM Files (*.bin;*.1np)|*.bin;*.1np|All files (*.*)|*.*",
                    CheckFileExists = true
                };

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                Properties.Settings.Default.LastSpliceLocation =
                    Path.GetDirectoryName( ofd.FileName );

                Properties.Settings.Default.Save();

                try
                {
                    Rom rom = new Rom( 0x8000, ofd.FileName );

                    rom.Load();

                    ExportsFile exports = new ExportsFile(Path.GetFullPath(Properties.Settings.Default.TemplatesLocation));

                    exports.Load();

                    IGameController controller =
                        new MajorHavocPromisedEnd( rom, exports );

                    controller.EncodeObjects( mazeCollection );

                    //Update page csums.
                    byte page6Csum = rom.CalculateChecksum( 0x4000, 0x1fff, 6 );
                    byte page7Csum = rom.CalculateChecksum( 0x6000, 0x1fff, 7 );

                    //Bouncing back to Processor mem address to write csum at top of page
                    rom.WritePagedROM( 0x3fff, new[] { (byte) ( 0x08 ^ page6Csum ) }, 0, 6 );
                    rom.WritePagedROM( 0x3fff, new[] { (byte) ( 0x09 ^ page7Csum ) }, 0, 7 );

                    /// Skip writing version info for now.
                    ///MarkPagedROM(6);
                    ///MarkPagedROM(7);

                    rom.SaveToFile(
                        Path.ChangeExtension( ofd.FileName,
                            $"spliced{Path.GetExtension( ofd.FileName )}" ) );
                }
                catch ( Exception e )
                {
                    Debugger.Break();
                }
            }
        }

        /// <inheritdoc />
        public void Splice( IDictionary<int, Maze> mazes, string path = null )
        {

        }

#endregion
    }
}
