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

            RomSerializer romSerializer1 = new RomSerializer( typeof( Oxoid[] ) );

            Oxoid[] oxoids = new Oxoid[ 5 ];

            oxoids.SetValue( new Oxoid() { Value = 55 }, 0 );
            oxoids.SetValue( new Oxoid() { Value = 44 }, 1 );
            oxoids.SetValue( new Oxoid() { Value = 33 }, 2 );

            romSerializer1.Serialize( memoryStream, oxoids );

            memoryStream.Seek( 0, SeekOrigin.Begin );

            Oxoid[] oxoidsAo = romSerializer1.Deserialize( memoryStream ) as Oxoid[];

            Assert.Equal( oxoids[ 0 ], oxoidsAo[ 0 ] );
        }

        [Fact( Skip = "Not Completed" )]
        public void Test2()
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

            Assert.Equal( oxoids[ 0 ], oxoidsAo[ 0 ] );
        }

    }
}
