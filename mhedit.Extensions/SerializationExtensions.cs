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
using mhedit.Containers.MazeObjects;

namespace mhedit.Extensions
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
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void SerializeAndCompress(this object obj, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                obj.SerializeAndCompress(fileStream);
            }
        }

        /// <summary>
        /// Serialize the object using XML serialization, compress the XML output,
        /// and write the compressed XML to the provided file.
        /// </summary>
        /// <param name="file"></param>
        public static void SerializeAndCompress( this IFileProperties file )
        {
            using (FileStream fileStream = new FileStream(
                Path.Combine( file.Path, file.Name ), FileMode.Create))
            {
                file.SerializeAndCompress(fileStream);
            }
        }


        /// <summary>
        /// Serialize the object using XML serialization, compress the XML output,
        /// and write the compressed XML to the provided stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void SerializeAndCompress( this object obj, Stream stream )
        {
            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                XmlSerializer serializer = new XmlSerializer( obj.GetType() );

                using ( XmlWriter xmlWriter = XmlWriter.Create( memoryStream,
                    new XmlWriterSettings { Indent = true } ) )
                {
                    serializer.Serialize( xmlWriter, obj, XmlNamespace );
                }

                memoryStream.Position = 0;
                BZip2.Compress( memoryStream, stream, false, 4096 );
            }
        }

        private static Action<string> NotificationHandler { get; set; }

        /// <summary>
        /// Open the file, expand the contents into memory and deserialize the expanded
        /// XML into the object specified by the type T. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="resultType"></param>
        /// <param name="onNotifications"></param>
        /// <returns></returns>
        public static object ExpandAndDeserialize(
            this string fileName, Type resultType, Action<string> onNotifications = null )
        {
            NotificationHandler = onNotifications;

            using ( FileStream fStream = new FileStream( fileName, FileMode.Open ) )
            {
                using ( MemoryStream mStream = new MemoryStream() )
                {
                    BZip2.Decompress( fStream, mStream, false );
                    mStream.Position = 0;

                    using ( XmlReader xmlReader = XmlReader.Create( mStream ) )
                    {
                        XmlSerializer serializer = new XmlSerializer( resultType );
                        serializer.UnknownElement += OnUnknownElement;
                        return PerformPostDeserializeHacks( serializer.Deserialize( xmlReader ) );
                    }
                }
            }

            NotificationHandler = null;
        }

        private static object PerformPostDeserializeHacks( object deserialized )
        {
            if ( deserialized is MazeCollection collection )
            {
                foreach ( Maze maze in collection.Mazes )
                {
                    PerformPostDeserializeHacksOn( maze );
                }
            }
            else if ( deserialized is Maze maze )
            {
                PerformPostDeserializeHacksOn( maze );
            }

            return deserialized;
        }

        private static void PerformPostDeserializeHacksOn( Maze maze )
        {
            FixParentChildOnTripPads( maze );

            if ( FixMaxMazeObjectViolations( maze, out string violationNotification ) )
            {
                NotificationHandler?.Invoke( violationNotification );
            }

            //FixExcessiveCannonPauseValues( maze );
        }

        /// <summary>
        /// Another HACK: Check the Max Objects of each type and trim off the ones that
        /// exceed the limit
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="s"></param>
        private static bool FixMaxMazeObjectViolations( Maze maze, out string violationNotification)
        {
            violationNotification = null;

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

                violationNotification = message.ToString();
            }

            return message.Length != 0;
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
        ///
        /// THIS IS CURRENTLY DISABLED because we cannot determine when this should be
        /// performed or ignored.
        /// </summary>
        /// <param name="maze"></param>
        //private static void FixExcessiveCannonPauseValues( Maze maze )
        //{
        //    foreach ( IonCannon cannon in maze.MazeObjects.OfType<IonCannon>() )
        //    {
        //        foreach ( IonCannonInstruction instruction in cannon.Program )
        //        {
        //            if ( instruction is Move moveCommand )
        //            {
        //                moveCommand.WaitFrames = moveCommand.WaitFrames >> 2;
        //            }
        //            else if ( instruction is Pause pauseCommand )
        //            {
        //                pauseCommand.WaitFrames = pauseCommand.WaitFrames >> 2;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Convert properties that no longer exist on objects to their new format/property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnUnknownElement( object sender, XmlElementEventArgs e )
        {
            TripPadPyroidUnknownElement( e );
            TransporterUnknownElement( e );

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

        /// <summary>
        /// Handle XML Property conversions for TripPadPyroids here.
        /// </summary>
        /// <param name="args"></param>
        private static void TransporterUnknownElement( XmlElementEventArgs args )
        {
            if ( args.ObjectBeingDeserialized is Transporter transporter )
            {
                /// The Velocity Property has been converted to a SpeedIndex and a
                /// Direction Property. 
                if ( args.Element.Name == "IsBroken" )
                {
                    if ( bool.TryParse( args.Element.InnerText, out bool val ) )
                    {
                        transporter.IsSpecial = val;
                    }
                    else
                    {
                        throw new SerializationException(
                            $"{typeof( Transporter ).Namespace} IsBroken to IsSpecial property conversion " +
                            $"failed for value: {args.Element.InnerText}" );
                    }
                }
            }
        }
    }

}