using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.BZip2;
using mhedit.Containers;
using mhedit.Containers.MazeEnemies;

namespace mhedit.Serialization
{

    public static class SerializationExtensions
    {
        private static readonly XmlSerializerNamespaces XmlNamespace =
            new XmlSerializerNamespaces( new[]
                                         {
                                             new XmlQualifiedName( "MHEdit",
                                                 "http://mhedit.askey.org" )
                                         } );

        /// <summary>
        /// Serialize the object into the file using XML serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void Serialize<T>( this T obj, string fileName )
        {
            using ( FileStream fileStream = new FileStream( fileName, FileMode.Create ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( T ) );

                using ( XmlWriter xmlWriter = XmlWriter.Create( fileStream,
                    new XmlWriterSettings { Indent = true } ) )
                {
                    serializer.Serialize( xmlWriter, obj, XmlNamespace );
                }
            }
        }

        /// <summary>
        /// Serialize the object using XML serialization, compress the XML output,
        /// and write the compressed XML to the provided file.
        /// .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void SerializeAndCompress<T>( this T obj, string fileName )
        {
            using ( FileStream fileStream = new FileStream( fileName, FileMode.Create ) )
            {
                obj.SerializeAndCompress( fileStream );
            }
        }

        /// <summary>
        /// Serialize the object using XML serialization, compress the XML output,
        /// and write the compressed XML to the provided stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void SerializeAndCompress<T>( this T obj, Stream stream )
        {
            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( T ) );

                using ( XmlWriter xmlWriter = XmlWriter.Create( memoryStream,
                    new XmlWriterSettings { Indent = true } ) )
                {
                    serializer.Serialize( xmlWriter, obj, XmlNamespace );
                }

                memoryStream.Position = 0;

                BZip2.Compress( memoryStream, stream, false, 4096 );
            }
        }

        /// <summary>
        /// Open the file, expand the contents into memory and deserialize the expanded
        /// XML into the object specified by the type T. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T ExpandAndDeserialize<T>( this string fileName )
        {
            using ( FileStream fStream = new FileStream( fileName, FileMode.Open ) )
            {
                using ( MemoryStream mStream = new MemoryStream() )
                {
                    BZip2.Decompress( fStream, mStream, false );

                    mStream.Position = 0;

                    using ( XmlReader xmlReader = XmlReader.Create( mStream ) )
                    {
                        XmlSerializer serializer = new XmlSerializer( typeof( T ) );

                        return PerformPostDeserializeHacks<T>( serializer.Deserialize( xmlReader ) );
                    }
                }
            }
        }

        private static T PerformPostDeserializeHacks<T>( object deserialized )
        {
            if ( deserialized is MazeCollection collection )
            {
                foreach ( Maze maze in collection.Mazes )
                {
                    FixParentChildOnTripPads( maze );

                    FixMaxMazeObjectViolations( maze );
                }
            }
            else if ( deserialized is Maze maze )
            {
                FixParentChildOnTripPads( maze );

                FixMaxMazeObjectViolations( maze );
            }

            return (T)deserialized;
        }

        /// <summary>
        /// Another HACK: Check the Max Objects of each type and trim off the ones that
        /// exceed the limit
        /// </summary>
        /// <param name="maze"></param>
        private static void FixMaxMazeObjectViolations( Maze maze )
        {
            StringBuilder message = new StringBuilder();

            List<List<MazeObject>> mazeObjectsByType =
                maze.MazeObjects
                    .Select( x => new { Type = x.GetType(), Value = x } )
                    .GroupBy( x => x.Type )
                    .Select( x => x.Select( v => v.Value ).ToList() )
                    .ToList();

            foreach ( List<MazeObject> mazeObjects in mazeObjectsByType )
            {
                int maxObjectsAllowed = mazeObjects.First().MaxObjects;

                if ( mazeObjects.Count > maxObjectsAllowed )
                {
                    message.AppendLine(
                        $"{mazeObjects.First().GetType().Name}: " +
                        $"{mazeObjects.Count} found, {maxObjectsAllowed} supported. Excess trimmed." );

                    foreach ( MazeObject toDelete in mazeObjects.Where( ( x, i ) => i >= maxObjectsAllowed ) )
                    {
                        maze.MazeObjects.Remove( toDelete );
                    }
                }
            }

            if ( message.Length > 0 )
            {
                /// Fill in in reverse order to push into the front of the message.
                message.Insert( 0, Environment.NewLine )
                       .Insert( 0, Environment.NewLine )
                       .Insert( 0,
                           $"There are more objects defined in the \"{maze.Name}\" Maze than are allowed. Note the following adjustments:" );

                MessageBox.Show( message.ToString(), "Maze Load Issues", MessageBoxButtons.OK,
                    MessageBoxIcon.Information );
            }
        }

        /// <summary>
        /// HACK: Fixes orphaned TripPadPyroids do to Serialization process boo boo.
        /// </summary>
        /// <param name="maze"></param>
        private static void FixParentChildOnTripPads( Maze maze )
        {
            /// Strip out the loose TripPadPyroids
            foreach ( TripPadPyroid pyroid in maze.MazeObjects.OfType<TripPadPyroid>().ToList() )
            {
                maze.MazeObjects.Remove( pyroid );
            }

            /// Promote all children up to the MazeObjects collection.
            foreach ( TripPad tripPad in maze.MazeObjects.OfType<TripPad>().ToList() )
            {
                maze.MazeObjects.Add( tripPad.Pyroid );
            }
        }
    }

}