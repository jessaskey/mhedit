using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Strings are not encoded in standardized ways in ROMs. I.e. Null
    /// terminated or size first (Pascal string). So this allows us to inject
    /// in a custom string read/write algo into the RomSerializer.
    /// </summary>
    public interface IStringEncoding
    {
        /// <summary>
        /// Reads a string from the current stream and advances the position within
        /// the stream in accordance with the encoding used and the specific
        /// characters decoded from the stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <returns>A string object that was decoded from the stream.</returns>
        string ReadString( Stream stream );

        /// <summary>
        /// Writes a string to the current stream and advances the current position
        /// of the stream in accordance with the encoding used and the specific
        /// characters being written to the stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="value">The string to encode into the stream.</param>
        void Write( Stream stream, string value );
    }
}
