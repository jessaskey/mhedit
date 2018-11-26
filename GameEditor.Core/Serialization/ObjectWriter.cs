using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace GameEditor.Core.Serialization
{
    internal class ObjectWriter
    {
        private readonly StreamingContext _context;
        private readonly BinaryWriter _writer;

        public ObjectWriter( BinaryWriter writer, StreamingContext context )
        {
            this._writer = writer;

            this._context = context;
        }

        public void Serialize( Type type, object graph )
        {
            if ( RomSerializer.IsPrimitiveType( type ) )
            {
                this.SerializePrimitive( graph );
            }
            else
            {
                if ( !( graph is IRomSerializable iSerializable ) )
                {
                    /// Doesn't implement interface...
                    /// So try as collection class.
                    this.SerializeCollection( type, graph );
                }
                else
                {
                    /// Collect the objects that make up this object type and serialize
                    /// them individually, in the order added.
                    RomSerializationInfo si = new RomSerializationInfo( type, null );

                    iSerializable.GetObjectData( si, this._context );

                    var enumerator = si.GetEnumerator();

                    while ( enumerator.MoveNext() )
                    {
                        this.Serialize( enumerator.Current.ObjectType,
                            enumerator.Current.Value );
                    }
                }
            }
        }

        private void SerializeCollection( Type type, object graph )
        {
            /// I don't really know what to check here to limit the serialization
            /// to Generic collections but they all seem to implement IEnumerable
            /// and I need that to iterate over them anyway.
            if ( graph is IEnumerable enumerable && type.IsClass )
            {
                Type collectionElementType;

                if ( type.IsGenericType &&
                     type.GenericTypeArguments.Length == 1 )
                {
                    collectionElementType = type.GenericTypeArguments.First();
                }
                else if ( type.IsArray )
                {
                    collectionElementType = type.GetElementType();
                }
                else
                {
                    throw new NotSupportedException( $"Collection {type.FullName}." );
                }

                var enumerator = enumerable.GetEnumerator();

                ///Collection class so enumerate and serialize..
                while ( enumerator.MoveNext() && enumerator.Current != null )
                {
                    /// serialize the collection objects using the element Type.
                    this.Serialize( collectionElementType, enumerator.Current );
                }

                /// terminate the collection with the terminator if provided.
                object terminationValue =
                    RomSerializer.GetTerminationObject( collectionElementType );

                if ( terminationValue != null )
                {
                    this.SerializePrimitive( terminationValue );
                }
            }
        }

        private void SerializePrimitive( object obj )//, BinaryDeserializationEvents events )
        {
            switch ( Type.GetTypeCode( obj.GetType() ) )
            {
                case TypeCode.String:
                    this._writer.Write( (string)obj );
                    break;
                case TypeCode.Int32:
                    this._writer.Write( (Int32)obj );
                    break;
                case TypeCode.Boolean:
                    this._writer.Write( (Boolean)obj );
                    break;
                case TypeCode.Int16:
                    this._writer.Write( (Int16)obj );
                    break;
                case TypeCode.Int64:
                    this._writer.Write( (Int64)obj );
                    break;
                case TypeCode.Single:
                    this._writer.Write( (Single)obj );
                    break;
                case TypeCode.Double:
                    this._writer.Write( (Double)obj );
                    break;
                case TypeCode.Decimal:
                    this._writer.Write( (Decimal)obj );
                    break;
                case TypeCode.Char:
                    this._writer.Write( (Char)obj );
                    break;
                case TypeCode.Byte:
                    this._writer.Write( (Byte)obj );
                    break;
                case TypeCode.SByte:
                    this._writer.Write( (SByte)obj );
                    break;
                case TypeCode.UInt16:
                    this._writer.Write( (UInt16)obj );
                    break;
                case TypeCode.UInt32:
                    this._writer.Write( (UInt32)obj );
                    break;
                case TypeCode.UInt64:
                    this._writer.Write( (UInt64)obj );
                    break;

                default:
                    if ( obj.GetType() == typeof( byte[] ) )
                    {
                        this._writer.Write( (byte[])obj );
                    }

                    throw new NotSupportedException(
                        $"Unsupported or Unexpected primitive type {obj.GetType().FullName}." );
            }
        }
    }
}