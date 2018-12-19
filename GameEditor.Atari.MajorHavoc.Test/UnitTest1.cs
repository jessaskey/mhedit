using GameEditor.Atari.MajorHavoc.Maze.Features;
using GameEditor.Atari.MajorHavoc.Serialization;
using GameEditor.Core.Hardware;
using GameEditor.Core.Xml;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace GameEditor.Atari.MajorHavoc.Test
{
    public class UnitTest1
    {
        [Fact( Skip = "Not Completed" )]
        public void Test1()
        {
            //GameProfile profile;
            //string path = @"C:\Users\Public\Local Storage\MajorHavocEdit\GameEditor.Core\Xsd\GameProfile.xml";

            //XmlSerializer serializer = new XmlSerializer( typeof( GameProfile ) );

            //StreamReader reader = new StreamReader( path );

            //profile = serializer.Deserialize( reader ) as GameProfile;

            //Core.Hardware.HardwareDescription hardwareDescription
            //    = new Core.Hardware.HardwareDescription( profile.HardwareDescription );

            //PageRef pageRef = new PageRef
            //{
            //    Id = "AlphaProcessor.PagedProgramRom.Page_0"
            //};

            //Stream found = hardwareDescription.FindPageStream( pageRef );

            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer();

            Arrow[] Arrows = new Arrow[ 5 ];

            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.Down }, 0 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.DownLeft }, 1 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.DownRight }, 2 );

            romSerializer1.Serialize( memoryStream, Arrows );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Arrow[] ArrowsAo = romSerializer1.Deserialize<Arrow[]>( memoryStream );

            Assert.Equal( Arrows[ 0 ], ArrowsAo[ 0 ] );
        }

        [Fact( Skip = "Not Completed" )]
        public void Test2()
        {
            Stream memoryStream = new MemoryStream( 32 );

            RomSerializer romSerializer1 = new RomSerializer();

            Arrow[] Arrows = new Arrow[ 5 ];

            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.UpRight }, 0 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.UpLeft }, 1 );
            Arrows.SetValue( new Arrow() { Direction = ArrowDirection.Up }, 2 );

            romSerializer1.Serialize( memoryStream, Arrows );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Arrow[] ArrowsAo = romSerializer1.Deserialize<Arrow[]>( memoryStream );

            Assert.Equal( Arrows[ 0 ], ArrowsAo[ 0 ] );
        }

    }
}
