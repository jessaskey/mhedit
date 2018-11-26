using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace GameEditor.Core.Serialization
{
    public abstract class RomSerializer
    {
        //private readonly BinaryDeserializationEvents _events = new BinaryDeserializationEvents();
        private StreamingContext _context = new StreamingContext( StreamingContextStates.All );

        /// <summary>
        /// Initializes a new instance of the GameEditor.Core.Serialization.RomSerializer
        /// class that can serialize objects into EPROMs, and deserialize EPROMs into 
        /// objects of the specified type.
        /// </summary>
        public RomSerializer()
        {}

        public StreamingContext Context
        {
            get
            {
                return this._context;
            }

            set
            {
                this._context = value;
            }
        }

        /// <summary>
        /// Deserializes the ROM contained by the specified BinaryReader into an
        /// object of the type specfied.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="binaryReader">The BinaryReader that contains the ROM 
        /// information to deserialize.</param>
        /// <returns>The Type being deserialized.</returns>
        public T Deserialize<T>( BinaryReader binaryReader )
        {
            return this.Deserialize<T>( binaryReader, null );
        }

        /// <summary>
        /// Deserializes the ROM contained by the specified BinaryReader into an object
        /// of the type specfied. EPROMs typically have implicit length Arrays/Collections
        /// embedded within and can be deserialized when an enumerable type and length are
        /// provided.
        /// </summary>
        /// <typeparam name="T">The type that will be deserialized. If a fixed length is
        /// not null then this type must implement System.Collections.IEnumerable.</typeparam>
        /// <param name="binaryReader">The BinaryReader that contains the ROM 
        /// information to deserialize.</param>
        /// <param name="length">The number of objects to be deserialized, or null to
        /// signify that the length is embedded in the binaryReader.</param>
        /// <returns>The Type being deserialized.</returns>
        public T Deserialize<T>( BinaryReader binaryReader, int? length )
        {
            try
            {
                if ( binaryReader == null )
                {
                    throw new ArgumentNullException( nameof( binaryReader ) );
                }

                if ( !typeof( T ).IsSerializable )
                {
                    throw new ArgumentException( "Type is not serializable" );
                }

                /// if fixed length is provided then the type MUST implement IEnumerable
                if ( length.HasValue )
                {
                    /// must be an enumerable collection class.
                    if ( !typeof( T ).GetInterfaces().Contains( typeof( IEnumerable ) ) )
                    {
                        throw new ArgumentException( "Type is not IEnumerable" );
                    }

                    if ( length.Value < 0 )
                    {
                        throw new ArgumentOutOfRangeException( "Length is < 0" );
                    }
                }

                ObjectReader objectReader = new ObjectReader( binaryReader, this.Context );

                return (T)objectReader.Deserialize( typeof( T ), length );
            }
            catch ( Exception e )
            {
                if ( e is ThreadAbortException ||
                     e is StackOverflowException ||
                     e is OutOfMemoryException )
                {
                    throw;
                }

                throw new SerializationException( $"Deserialization of {typeof( T ).Name}.", e );
            }
        }

        /// <summary>
        /// Serializes the specified System.Object and writes to a ROM using the specified
        /// BinaryWriter that references the ROM images.
        /// </summary>
        /// <param name="binaryWriter">The BinaryWriter that contains the ROM information
        /// to deserialize.</param>
        /// <param name="graph">The System.Object to serialize.</param>
        public void Serialize( BinaryWriter binaryWriter, object graph )
        {
            try
            {
                if ( binaryWriter == null || graph == null )
                {
                    throw new ArgumentNullException(
                        binaryWriter == null ? nameof( binaryWriter ) : nameof( graph ) );
                }

                if ( !graph.GetType().IsSerializable )
                {
                    throw new ArgumentException( nameof( graph ), "Object is not serializable" );
                }

                ObjectWriter objectWriter = new ObjectWriter( binaryWriter, this.Context );

                objectWriter.Serialize( graph.GetType(), graph );
            }
            catch ( Exception e )
            {
                if ( e is ThreadAbortException ||
                     e is StackOverflowException ||
                     e is OutOfMemoryException )
                {
                    throw;
                }

                throw new SerializationException( $"Serialization of { graph.GetType().Name}.", e );
            }
        }

        internal static object GetTerminationObject( Type type )
        {
            IEnumerable<TerminationObjectAttribute> attrs =
                type.GetCustomAttributes<TerminationObjectAttribute>( true );

            TerminationObjectAttribute attr = attrs.FirstOrDefault();

            /// return the termination value or null if no attribute present.
            return attr?.Value;
        }

        internal static bool IsPrimitiveType( Type type )
        {
            if ( type.IsEnum )
                return true;

            switch ( Type.GetTypeCode( type ) )
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    if ( type == typeof( byte[] ) )
                        return true;
                    break;
            }

            return false;
        }
    }

}
