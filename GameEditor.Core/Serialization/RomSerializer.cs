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
    public abstract class RomSerializer : IFormatter
    {
        //private readonly BinaryDeserializationEvents _events = new BinaryDeserializationEvents();
        private readonly Type _type;
        private StreamingContext _context = new StreamingContext( StreamingContextStates.All );
        private readonly int? _enumerableLength = null;

        public ISurrogateSelector SurrogateSelector
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public SerializationBinder Binder
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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
        /// Initializes a new instance of the GameEditor.Core.Serialization.RomSerializer
        /// class that can serialize objects of the specified type into EPROMs, and
        /// deserialize EPROMs into objects of the specified type.
        /// </summary>
        /// <param name="type">The type of the object that this RomSerializer can serialize.</param>
        public RomSerializer( Type type )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( "type" );
            }

            if ( !type.IsSerializable )
            {
                throw new ArgumentException( "Type is not serializable" );
            }

            this._type = type;
        }

        /// <summary>
        /// Initializes a new instance of the GameEditor.Core.Serialization.RomSerializer
        /// class that can serialize objects of the specified type into EPROMs, and
        /// deserialize EPROMs into objects of the specified type. EPROMs typically have
        /// implicit length Arrays/Collections embedded within and can be deserialized when
        /// enumerable type and length are provided.
        /// </summary>
        /// <param name="iEnumerable">The enumerable type that this RomSerializer
        /// can serialize.</param>
        /// <param name="length">The number of objects to be deserialized.</param>
        public RomSerializer( Type iEnumerable, int length )
            :this( iEnumerable )
        {
            /// must be an enumerable collection class.
            if ( !iEnumerable.GetInterfaces().Contains( typeof( IEnumerable ) ) )
            {
                throw new ArgumentException( "Type is not IEnumerable" );
            }

            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( "Length is < 0" );
            }

            this._enumerableLength = length;
        }

        public abstract object Deserialize( Stream serializationStream );

        public abstract void Serialize( Stream serializationStream, object graph );

        protected object Deserialize( BinaryReader binaryReader )
        {
            ObjectReader objectReader = new ObjectReader( binaryReader, this.Context );

            return objectReader.Deserialize( this._type, this._enumerableLength );
        }

        protected void Serialize( BinaryWriter binaryWriter, object graph )
        {
            if ( graph == null )
            {
                throw new ArgumentNullException( "object" );
            }

            if ( graph.GetType() != this._type )
            {
                throw new InvalidOperationException( "not proper type" );
            }

            ObjectWriter objectWriter = new ObjectWriter( binaryWriter, this.Context );

            objectWriter.Serialize( this._type, graph );
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
