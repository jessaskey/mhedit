using System;
using System.IO;

namespace MajorHavocEditor.Services
{

    internal interface IFileManager
    {
        /// <summary>
        /// Attempt to save the object to file.
        /// </summary>
        /// <param name="toBeSaved"></param>
        /// <param name="path"></param>
        /// <returns>True if save succeeded. False otherwise</returns>
        bool Save( object toBeSaved, string path = null );
        //bool Save(IFileProperties toBeSaved, string path = null);

        T Load<T>( IFileProperties toBeLoaded, Func<FileStream, T> postProcessing );
    }

}