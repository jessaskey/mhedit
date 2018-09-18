using System;
using System.IO;
using GameEditor.Core.Xml;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// IMemoryDevice implementation that supports all byte based devices.
    /// </summary>
    public class MemoryDevice : IMemoryDevice
    {
        protected readonly Xml.MemoryDevice _description;
        protected readonly byte[] _data;
        protected readonly int _bytesPerDeviceWord;

        public MemoryDevice( Xml.MemoryDevice description )
        {
            this._description = description;
            
            /// Validate XML author's math skills.
            /// Yes, this is constraining but good enough for now.
            if ( (description.Size % 1024) != 0 )
            {
                throw new ArgumentException( string.Format(
                    "Memory device {0} isn't in 1k chunk.", description.Id,
                    description.Path ) );
            }

            this._bytesPerDeviceWord = description.Width / 8;

            this._data = new byte[ description.Size * this._bytesPerDeviceWord ];

            try
            {
                using ( FileStream stream = File.Open( description.Path, FileMode.Open ) )
                {
                    stream.Read( this._data, 0, this._data.Length );
                }
            }
            catch ( Exception e )
            {
                throw new FileLoadException( string.Format(
                    "Problem Reading memory device {0} from file.", description.Id ),
                    description.Path, e );
            }
        }

        /// <summary>
        ///  Factory pattern to create the proper MemoryDevice.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        internal static IMemoryDevice Factory( Xml.MemoryDevice device )
        {
            /// This is a bit ridiculous at the moment but if we ever need
            /// to support different width devices this is the factory.
            Xml.Eprom eprom = device as Xml.Eprom;
            if ( eprom != null )
            {
                return new MemoryDevice( device );
            }

            throw new NotSupportedException( string.Format(
                "Device Id: {0}", device.Id ) );
        }

        public string Id
        {
            get
            {
                return this._description.Id;
            }
        }

        public string Path
        {
            get
            {
                return this._description.Path;
            }
        }

        public int Size
        {
            get
            {
                return this._description.Size;
            }
        }

        public int Width
        {
            get
            {
                return this._description.Width;
            }
        }

        public int Read( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            try
            {
                /// Convert address to byte offset in data array 
                uint byteAddress = address * (uint)this._bytesPerDeviceWord;

                /// Count value is in words so convert to bytes.
                int readLengthInBytes = words * this._bytesPerDeviceWord;

                /// Clamp the length to the size of the device.
                readLengthInBytes = ( byteAddress + readLengthInBytes ) > this._data.Length ?
                    (int)( this._data.Length - byteAddress ) :
                    readLengthInBytes;

                for ( int i = 0; i < readLengthInBytes; ++i, ++address )
                {
                    buffer[ offset + i ] = this._data[ address ];
                }

                return readLengthInBytes;
            }
            catch ( Exception e )
            {
                throw new ArgumentException( string.Format(
                    "Read from {0}-bit device {1}.",
                        this._description.Width, this._description.Id ), e );
            }
        }

        public int Write( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            try
            {
                /// Convert address to byte offset in data array 
                uint byteAddress = address * (uint)this._bytesPerDeviceWord;

                /// Count value is in words so convert to bytes.
                int writeLengthInBytes = words * this._bytesPerDeviceWord;

                /// Clamp the length to the size of the device.
                writeLengthInBytes = ( byteAddress + writeLengthInBytes ) > this._data.Length ?
                    (int)( this._data.Length - byteAddress ) :
                    writeLengthInBytes;

                for ( int i = 0; i < writeLengthInBytes; ++i, ++address )
                {
                    this._data[ address ] = buffer[ offset + i ];
                }

                return writeLengthInBytes;
            }
            catch ( Exception e )
            {
                throw new ArgumentException( string.Format(
                    "Write from {0}-bit device {1}.",
                        this._description.Width, this._description.Id ), e );
            }
        }
    }
}