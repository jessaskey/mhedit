using System;
using System.IO;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Services
{

    internal interface IFileManager
    {
        /// <summary>
        /// Attempt to save the object to file.
        /// </summary>
        /// <param name="toBeSaved"></param>
        /// <returns>True if save succeeded. False otherwise</returns>
        bool Save( IFileProperties toBeSaved );

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toBeLoaded"></param>
        /// <param name="postProcessing"></param>
        /// <returns></returns>
        T Open<T>( IFileProperties toBeLoaded, Func<FileStream, T> postProcessing );
    }

//    public class FileManager : IFileManager
//    {
//#region Implementation of IFileManager

//        /// <inheritdoc />
//        public bool Save( IFileProperties toBeSaved )
//        {
//        }

//        /// <inheritdoc />
//        public T Open<T>( IFileProperties toBeLoaded, Func<FileStream, T> postProcessing )
//        {
//            OpenFileDialog ofd = new OpenFileDialog
//                                 {
//                                     Title = "Open Maze or Maze Collection",
//                                     InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
//                                     Filter = "Editor Files (*.mhz;*.mhc)|*.mhz;*.mhc|Mazes (*.mhz)|*.mhz|Maze Collections (*.mhc)|*.mhc",
//                                     CheckFileExists = true,
//                                 };

//            if (ofd.ShowDialog() == DialogResult.OK)
//            {
//                string fileExtension = Path.GetExtension(ofd.FileName);

//                if (fileExtension.Equals(".mhz", StringComparison.CurrentCultureIgnoreCase))
//                {
//                    this.OpenMaze(ofd.FileName);
//                }
//                else if (fileExtension.Equals(".mhc", StringComparison.CurrentCultureIgnoreCase))
//                {
//                    this.OpenCollection(ofd.FileName);
//                }
//            }
//        }

//        #endregion
//    }
}