using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace mhedit.GameControllers
{
    public interface IRom//, IFileProperties
    {
    }

    public class Rom : IRom
    {
        private readonly int _sizeInBytes;
        private byte[] _memoryBytes;
        private readonly string _filePath;

        public static string Page2367ROM = "mhpe100.1np";
        //public static string AlphaHighROM = "mhavocpe.1l";

        public Rom( int sizeInBytes, string filePath )
        {
            this._sizeInBytes = sizeInBytes;

            this._filePath = !string.IsNullOrWhiteSpace(filePath) ?
                                 filePath : throw new ArgumentException(nameof(filePath));

            //detect whether its a directory or file
            if ((File.GetAttributes(filePath) & FileAttributes.Directory) ==
                FileAttributes.Directory)
            {
                this._filePath = Path.Combine(filePath, Page2367ROM);
            }

        }

        public void Load()
        {
            this._memoryBytes = File.ReadAllBytes( this._filePath );

            if ( this._memoryBytes.Length != this._sizeInBytes )
            {
                throw new InvalidDataException(
                    $"Rom file size {this._memoryBytes.Length}, is not the expected: {this._sizeInBytes}" );
            }
        }

        public byte[] GetBuffer()
        {
            return this._memoryBytes;
        }

        public byte ReadByte(ushort address, int offset, int page)
        {
            return this._memoryBytes[ this.BaseAddressForPage(address, offset, page) ];
        }

        public byte[] ReadBytes(ushort address, int length, int page)
        {
            return this.ReadPagedROM(address, 0, length, page);
        }

        public ushort ReadWord(ushort address, int offset, int page)
        {
            int firstByteAddress = this.BaseAddressForPage( address, offset, page );

            return (ushort) ( ( this._memoryBytes[ firstByteAddress + 1 ] << 8 ) +
                              this._memoryBytes[ firstByteAddress ] );
        }

        public byte[] ReadPagedROM( ushort address, int offset, int length, int page )
        {
            byte[] bytes = new byte[length];

            Array.Copy(
                this._memoryBytes, this.BaseAddressForPage( address, offset, page ),
                bytes, 0,
                length );

            return bytes;
        }

        public int WritePagedROM(ushort address, byte[] bytes, int offset, int page)
        {
            Array.Copy(
                bytes, 0,
                this._memoryBytes, this.BaseAddressForPage(address, offset, page),
                bytes.Length );

            return bytes.Length;
        }

        public byte CalculateChecksum( int start, int length, int page )
        {
            byte calculatedCsum = 0;
            int upperBound = start + length;

            for (int i = start; i < upperBound; i++)
            {
                calculatedCsum ^= this._memoryBytes[i];
            }

            return calculatedCsum;
        }

        public void SaveToFile( string filePath )
        {
            File.WriteAllBytes( filePath, this.GetBuffer() );
        }

        private int BaseAddressForPage( ushort address, int offset, int page )
        {
            // a paged address starts at 0x2000, therefore the base address of that page
            // in the ROM is 0x2000 less that where it actually is.
            int pageBase = page switch
            {
                6 => 0x2000, // 0x4000 
                7 => 0x4000, // 0x6000
                _ => throw new ArgumentOutOfRangeException(
                         $"Invalid page {page}, expected 6 or 7")
            };

            // adjust to targeted memory location.
            address += (ushort)offset;

            // If the provided address is within a page of memory, 0x2000 wide then it's valid.
            return address >= 0x2000 && address <= 0x3fff ? pageBase + address :
                       throw new ArgumentOutOfRangeException(
                           $"Address 0x{address.ToString( "X" )} outside of ROM" );
        }

    }

    public class ExportsFile
    {
        private readonly string _filePath;
        private Dictionary<string, ushort> _exports = new Dictionary<string, ushort>();

        public static string FileExtension = "exp";
        private static readonly Regex ReplaceRegex = new Regex(@"\s+\.EQU\s+");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Directory of the default exports file or the full path
        /// to an alternate file.</param>
        public ExportsFile(string filePath)
        {
            this._filePath = !string.IsNullOrWhiteSpace( filePath ) ?
                                 filePath : throw new ArgumentException(nameof(filePath));

            //detect whether its a directory or file
            if ( ( File.GetAttributes( filePath ) & FileAttributes.Directory ) ==
                 FileAttributes.Directory )
            {
                this._filePath = Directory.EnumerateFiles(
                    filePath,
                    $"*.{FileExtension}",
                    new EnumerationOptions()
                    {
                        RecurseSubdirectories
                            = false,
                        MatchType =
                            MatchType.Simple,
                    } ).FirstOrDefault();
            }
        }

        public ushort this[ string key ]
        {
            get
            {
                return this._exports.TryGetValue( key, out ushort val ) ?
                    val : throw new Exception( $"Address not found: {key}" );
            }
        }

        public void Load()
        {
            string[] exportLines = File.ReadAllLines( this._filePath );

            foreach ( string exportLine in exportLines )
            {
                string[] exportDefinition =
                    ReplaceRegex.Replace( exportLine, string.Empty ).Split( '$' );

                if ( exportDefinition.Length == 2 )
                {
                    ushort value = ushort.Parse( exportDefinition[ 1 ],
                        System.Globalization.NumberStyles.HexNumber );

                    this._exports.Add( exportDefinition[ 0 ], value );
                }
            }
        }
    }

}
