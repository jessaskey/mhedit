using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.Extensions;

namespace MajorHavocEditor.Services
{
    public interface IFileManager
    {
        /// <summary>
        /// Attempt to save the object to file. Save As will be performed
        /// if the IFileProperties don't point to a file. Save As can be
        /// forced by setting the parameter to true.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveAs"></param>
        /// <returns>True if save succeeded. False otherwise</returns>
        bool Save(IFileProperties file, bool saveAs = false);

        /// <summary>
        /// Load File into the editor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toBeLoaded"></param>
        /// <param name="postProcessing"></param>
        /// <returns></returns>
        T Load<T>( string fileName ) where T : class;
    }

    public class FileManager : IFileManager
    {
#region Implementation of IFileManager

        /// <inheritdoc />
        public bool Save(IFileProperties file, bool saveAs = false)
        {
            DialogResult result = DialogResult.OK;

            // if there isn't a file associated with this MazeCollection then ask
            // for the fileName. 
            if (string.IsNullOrWhiteSpace(file.Path) || saveAs)
            {
                SaveFileDialog sfd =
                    new SaveFileDialog
                    {
                        InitialDirectory = file.Path ??
                                           Environment.GetFolderPath(
                                               Environment.SpecialFolder.MyDocuments),
                        FileName = file.Name,
                        Filter = $"{file.GetType().Name} Files (*{file.Extension})|*{file.Extension}|All files (*.*)|*.*",
                        AddExtension = true,
                        OverwritePrompt = true
                    };

                // capture user choice for save operation below.
                result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    file.Name = Path.GetFileName(sfd.FileName);
                    file.Path = Path.GetDirectoryName(sfd.FileName);
                }
            }

            try
            {
                if (result == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Application.DoEvents();

                    if ( Properties.Settings.Default.CompressOnSave )
                    {
                        file.SerializeAndCompress();
                    }
                    else
                    {
                        file.Serialize();
                    }

                    if (file is IChangeTracking ict)
                    {
                        ict.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $@"An error has occurred while trying to save: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "An Error Occurred",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result == DialogResult.OK;
        }

        /// <inheritdoc />
        public T Load<T>( string fileName ) where T : class
        {
            Cursor.Current = Cursors.WaitCursor;

            Application.DoEvents();

            object result = null;

            try
            {
                try
                {
                    // Try to deserialize as raw XML
                    result = fileName.Deserialize<T>(HandleNotifications);
                }
                catch ( Exception )
                {
                    // retry as compressed...
                    result = fileName.ExpandAndDeserialize<T>(HandleNotifications);
                }

                void HandleNotifications(string message)
                {
                    MessageBox.Show(message, "Maze Load Issues",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                if (result is IFileProperties fileProperties)
                {
                    fileProperties.Name = Path.GetFileName(fileName);

                    fileProperties.Path = Path.GetDirectoryName(fileName);
                }

                if (result is IChangeTracking ict)
                {
                    ict.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($@"Maze could not be opened: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Bryan, I put this here as an example of how to report Exceptions that are caught, but you still
                //may want to log them. All un-handled exceptions will still log.
                //Program.SendException("MazeOpen", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result as T;
        }

        #endregion
    }
}