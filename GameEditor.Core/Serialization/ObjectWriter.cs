﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace GameEditor.Core.Serialization
{
    internal class ObjectWriter
    {
        private StreamingContext _context;
        private BinaryWriter _writer;

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
            IEnumerable enumerable = graph as IEnumerable;

            /// I don't really know what to check here to limit the serialization
            /// to Generic collections but they all seem to implement IEnumerable
            /// and I need that to iterate over them anyway.
            if ( enumerable != null && type.IsClass )
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
                    throw new SerializationException();
                }

                byte terminationByte = 
                    RomSerializer.GetTerminationByte( collectionElementType );

                var enumerator = enumerable.GetEnumerator();

                ///Collection class so enumerate and serialize..
                while ( enumerator.MoveNext() && enumerator.Current != null )
                {
                    this.Serialize( collectionElementType, enumerator.Current );

                    /// serialize the objects with the provided Type.
                }

                this._writer.Write( terminationByte );
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
                    else
                    {
                        throw new InvalidOperationException( string.Format(
                            "Unexpected Type {0}", obj.GetType().FullName ) );
                    }
                    break;
            }
        }
    }
}