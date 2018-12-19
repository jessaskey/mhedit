using GameEditor.Atari.MajorHavoc.Maze.Features;
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

            RomSerializer romSerializer = new RomSerializer();

            byte[] indexArray = romSerializer.Deserialize<byte[]>( this._programRomPage0, 12 );

            List<MazeHint> strings = new List<MazeHint>();

            int i = 0;

            foreach ( byte b in indexArray )
            {
                /// move to the ROM location for the array of uShorts.
                this._programRomPage0.Position =  0xE48b + b;

                ushort pstr = (ushort)romSerializer.Deserialize<ushort>( this._programRomPage0 );

                this._programRomPage0.Position = pstr;

                strings.Add( romSerializer.Deserialize<MazeHint>( this._programRomPage0 ) );

                this._programRomPage0.Position = pstr;

                romSerializer.Serialize( this._programRomPage0, strings[ i++ ] );
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

            RomSerializer romSerializer1 = new RomSerializer();

            Arrow[] Arrows = new Arrow[ 5 ];

            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.Down }, 0 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.Left }, 1 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.QuestionMark }, 2 );

            romSerializer1.Serialize( memoryStream, Arrows );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Arrow[] ArrowsAo = romSerializer1.Deserialize<Arrow[]>( memoryStream );
        }

        [Fact]
        public void Testt()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer();

            List<Arrow> Arrows = new List<Arrow>();

            Arrows.Add( new Arrow() { Direction = ArrowDirection.Right } );
            Arrows.Add( new Arrow() { Direction = ArrowDirection.UpLeft } );
            Arrows.Add( new Arrow() { Direction = ArrowDirection.DownRight } );

            romSerializer1.Serialize( memoryStream, Arrows );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            List<Arrow> ArrowsAo = romSerializer1.Deserialize<List<Arrow>>( memoryStream );
        }

        [Fact]
        public void Testg()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer();

            romSerializer1.Serialize( memoryStream, new Arrow() { Direction = ArrowDirection.QuestionMark } );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Arrow ArrowsAo = romSerializer1.Deserialize<Arrow>( memoryStream );
        }

        [Fact]
        public void Testk()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer();

            romSerializer1.Serialize( memoryStream, new TestEmbeddedArray() );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            TestEmbeddedArray ArrowsAo = 
                romSerializer1.Deserialize<TestEmbeddedArray>( memoryStream );
        }
    }
}
