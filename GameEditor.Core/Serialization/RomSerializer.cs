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

        public abstract object Deserialize( Stream serializationStream );

        public abstract void Serialize( Stream serializationStream, object graph );

        protected object Deserialize( BinaryReader binaryReader )
        {
            ObjectReader objectReader = new ObjectReader( binaryReader, this.Context );

            return objectReader.Deserialize( this._type );
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

        internal static byte GetTerminationByte( Type type )
        {
            IEnumerable<CollectionTerminationAttribute> attrs =
                type.GetCustomAttributes<CollectionTerminationAttribute>( true );

            CollectionTerminationAttribute attr = attrs.FirstOrDefault();

            /// The default is null (0x00) termination. Return that if no attribute
            /// is found.
            return attr == null ? (byte)0 : attr.TerminationByte;
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
