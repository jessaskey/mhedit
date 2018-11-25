using GameEditor.Atari.MajorHavoc.Serialization;
using GameEditor.Core.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace GameEditor.Atari.MajorHavoc.Test
{

    public class UshortArrayTests
    {
        Stream _programRomPage0;

        public UshortArrayTests()
        {
            GameProfile profile;
            string path = @"C:\Users\Public\Local Storage\MajorHavocEdit\GameEditor.Core\Xsd\GameProfile.xml";

            XmlSerializer serializer = new XmlSerializer( typeof( GameProfile ) );

            StreamReader reader = new StreamReader( path );

            profile = serializer.Deserialize( reader ) as GameProfile;

            Core.Hardware.HardwareDescription hardwareDescription
                = new Core.Hardware.HardwareDescription( profile.HardwareDescription );

            PageRef pageRef = new PageRef
            {
                Id = "AlphaProcessor.ProgramRom.Page_0"
            };

            this._programRomPage0 = hardwareDescription.FindPageStream( pageRef );
        }

        [Fact]
        public void DeserializeUshortArray()
        {
            /// move to the ROM location for the array of uShorts.
            this._programRomPage0.Seek( 0x93FE - 0x8000, SeekOrigin.Begin );

            RomSerializer ushortSerializer = new RomSerializer( typeof( byte[] ), 12 );

            byte[] indexArray = ushortSerializer.Deserialize( this._programRomPage0 ) as byte[];

            string[] strings = new string[ 12 ];

            int i = 0;

            foreach ( byte b in indexArray )
            {
                /// move to the ROM location for the array of uShorts.
                this._programRomPage0.Position =  0xE48b + b;

                RomSerializer ushortSerializer1 = new RomSerializer( typeof( ushort ) );

                ushort pstr = (ushort)ushortSerializer1.Deserialize( this._programRomPage0 );

                this._programRomPage0.Position = pstr;

                RomSerializer ushortSerializer2 = new RomSerializer( typeof(string) );

                strings[ i ] = ushortSerializer2.Deserialize( this._programRomPage0 ) as string;

                this._programRomPage0.Position = pstr;

                ushortSerializer2.Serialize( this._programRomPage0, strings[ i++ ] );
            }

            ///// move to the ROM location for the array of uShorts.
            //this._programRomPage0.Seek( 0xE48b - 0x8000, SeekOrigin.Begin );

            //RomSerializer ushortSerializer1 = new RomSerializer( typeof( ushort[] ), 12 );

            //ushort[] ptrArray = ushortSerializer1.Deserialize( this._programRomPage0 ) as ushort[];

        }

        [Fact]
        public void Testy()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer( typeof( Oxoid[] ) );

            Oxoid[] oxoids = new Oxoid[ 5 ];

            oxoids.SetValue( new Oxoid() { Value = 55 }, 0 );
            oxoids.SetValue( new Oxoid() { Value = 44 }, 1 );
            oxoids.SetValue( new Oxoid() { Value = 33 }, 2 );

            romSerializer1.Serialize( memoryStream, oxoids );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Oxoid[] oxoidsAo = romSerializer1.Deserialize( memoryStream ) as Oxoid[];
        }

        [Fact]
        public void Testt()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer( typeof( List<Oxoid> ) );

            List<Oxoid> oxoids = new List<Oxoid>();

            oxoids.Add( new Oxoid() { Value = 55 } );
            oxoids.Add( new Oxoid() { Value = 44 } );
            oxoids.Add( new Oxoid() { Value = 33 } );

            romSerializer1.Serialize( memoryStream, oxoids );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            List<Oxoid> oxoidsAo = romSerializer1.Deserialize( memoryStream ) as List<Oxoid>;
        }

        [Fact]
        public void Testg()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer( typeof( Oxoid ) );

            romSerializer1.Serialize( memoryStream, new Oxoid() { Value = 64 } );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Oxoid oxoidsAo = romSerializer1.Deserialize( memoryStream ) as Oxoid;
        }

        [Fact]
        public void Testk()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer( typeof( TestEmbeddedArray ) );

            romSerializer1.Serialize( memoryStream, new TestEmbeddedArray() );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            TestEmbeddedArray oxoidsAo = romSerializer1.Deserialize( memoryStream ) as TestEmbeddedArray;
        }
    }
}
