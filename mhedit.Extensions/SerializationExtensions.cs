using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.BZip2;
using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;

namespace mhedit.Extensions
{

    public static class SerializationExtensions
    {
        private static string _lastError = String.Empty;

        private static readonly XmlSerializerNamespaces XmlNamespace =
            new XmlSerializerNamespaces( new[]
                                         {
                                             new XmlQualifiedName( "MHEdit",
                                                 "http://mhedit.askey.org" )
                                         } );

        /// <summary>
        /// Contains the last error thrown in the extension.
        /// </summary>
        public static string LastError
        {
            get
            {
                return _lastError;
            }
        }
        /// <summary>
        /// Serialize the object into the file using XML serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void Serialize<T>( this T obj, string fileName )
        {
            _lastError = String.Empty;
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
            _lastError = String.Empty;
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
            _lastError = String.Empty;
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
            _lastError = String.Empty;
            using ( FileStream fStream = new FileStream( fileName, FileMode.Open ) )
            {
                using ( MemoryStream mStream = new MemoryStream() )
                {
                    BZip2.Decompress( fStream, mStream, false );
                    mStream.Position = 0;

                    using ( XmlReader xmlReader = XmlReader.Create( mStream ) )
                    {
                        XmlSerializer serializer = new XmlSerializer( typeof( T ) );
                        serializer.UnknownElement += OnUnknownElement;
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
                    PerformDeserializeHacksOn( maze );
                }
            }
            else if ( deserialized is Maze maze )
            {
                PerformDeserializeHacksOn( maze );
            }

            return (T)deserialized;
        }

        private static void PerformDeserializeHacksOn( Maze maze )
        {
            FixParentChildOnTripPads( maze );
            FixMaxMazeObjectViolations( maze );
            FixExcessiveCannonPauseValues( maze );
        }

        /// <summary>
        /// Another HACK: Check the Max Objects of each type and trim off the ones that
        /// exceed the limit
        /// </summary>
        /// <param name="maze"></param>
        private static bool FixMaxMazeObjectViolations( Maze maze )
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

                _lastError = message.ToString();
                //MessageBox.Show( message.ToString(), "Maze Load Issues", MessageBoxButtons.OK,
                //    MessageBoxIcon.Information );
            }
            return message.Length == 0;
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

        /// <summary>
        /// HACK: Fixes issue where the WaitFrames were being multiplied by 4 and didn't
        /// need to be.
        /// </summary>
        /// <param name="maze"></param>
        private static void FixExcessiveCannonPauseValues( Maze maze)
        {
            foreach (IonCannon cannon in maze.MazeObjects.OfType<IonCannon>().ToList())
            {
                foreach(IonCannonInstruction instruction in cannon.Program.ToList())
                {
                    Move moveCommand = instruction as Move;
                    if (moveCommand != null)
                    {
                        if (moveCommand.WaitFrames >= 64)
                        {
                            moveCommand.WaitFrames = moveCommand.WaitFrames >> 2;
                        }
                    }
                    Pause pauseCommand = instruction as Pause;
                    if (pauseCommand != null)
                    {
                        if (pauseCommand.WaitFrames >= 64)
                        {
                            pauseCommand.WaitFrames = pauseCommand.WaitFrames >> 2;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Convert properties that no longer exist on objects to their new format/property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUnknownElement( object sender, XmlElementEventArgs e )
        {
            TripPadPyroidUnknownElement( e );

            /// Add future conversion methods here! Make new method for each type.
        }

        /// <summary>
        /// Handle XML Property conversions for TripPadPyroids here.
        /// </summary>
        /// <param name="args"></param>
        private static void TripPadPyroidUnknownElement( XmlElementEventArgs args )
        {
            if ( args.ObjectBeingDeserialized is TripPadPyroid tripPadPyroid )
            {
                /// The Velocity Property has been converted to a SpeedIndex and a
                /// Direction Property. 
                if ( args.Element.Name == "Velocity" )
                {
                    if ( int.TryParse( args.Element.InnerText, out int val ) )
                    {
                        tripPadPyroid.Direction = val < 0 ?
                                                      TripPyroidDirection.Left :
                                                      TripPyroidDirection.Right;

                        val = Math.Abs( val );

                        tripPadPyroid.SpeedIndex = val > 7 ?
                                                       TripPyroidSpeedIndex.SupaFast :
                                                       (TripPyroidSpeedIndex)val;
                    }
                    else
                    {
                        throw new SerializationException(
                            $"{typeof( TripPadPyroid ).Namespace} Velocity to Speed property conversion " +
                            $"failed for value: {args.Element.InnerText}" );
                    }
                }
            }
        }
    }

}