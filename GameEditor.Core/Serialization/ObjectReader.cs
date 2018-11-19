using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Serialization
{
    internal class ObjectReader
    {
        private StreamingContext _context;
        private BinaryReader _reader;

        public ObjectReader( BinaryReader reader, StreamingContext context )
        {
            this._reader = reader;

            this._context = context;
        }

        public object Deserialize( Type type )//, BinaryDeserializationEvents events )
        {
            if ( RomSerializer.IsPrimitiveType( type ) )
            {
                return this.DeserializePrimitive( type );
            }

            Type iserializable = type.GetInterfaces()
                .FirstOrDefault( iface => iface == typeof( IRomSerializable ) );

            /// does the type implement our serialization interface?
            if ( iserializable == null )
            {
                /// if not then it assume it to be a collection class..
                return this.DeserializeCollection( type );
            }

            RomSerializationInfo si = new RomSerializationInfo( type, this );

            ConstructorInfo ctor = GetIRomSerializableConstructor( type );

            return ctor.Invoke( new object[] { si, this._context } );
        }

        internal object DeserializePrimitive( Type type )//, BinaryDeserializationEvents events )
        {
            object o;

            switch ( Type.GetTypeCode( type ) )
            {
                case TypeCode.String:
                    o = this._reader.ReadString();
                    break;
                case TypeCode.Int32:
                    o = this._reader.ReadInt32();
                    break;
                case TypeCode.Boolean:
                    o = this._reader.ReadBoolean();
                    break;
                case TypeCode.Int16:
                    o = this._reader.ReadInt16();
                    break;
                case TypeCode.Int64:
                    o = this._reader.ReadInt64();
                    break;
                case TypeCode.Single:
                    o = this._reader.ReadSingle();
                    break;
                case TypeCode.Double:
                    o = this._reader.ReadDouble();
                    break;
                case TypeCode.Decimal:
                    o = this._reader.ReadDecimal();
                    break;
                case TypeCode.Char:
                    o = this._reader.ReadChar();
                    break;
                case TypeCode.Byte:
                    o = this._reader.ReadByte();
                    break;
                case TypeCode.SByte:
                    o = this._reader.ReadSByte();
                    break;
                case TypeCode.UInt16:
                    o = this._reader.ReadUInt16();
                    break;
                case TypeCode.UInt32:
                    o = this._reader.ReadUInt32();
                    break;
                case TypeCode.UInt64:
                    o = this._reader.ReadUInt64();
                    break;

                default:
                    if ( type == typeof( byte[] ) )
                    {
                        o = this._reader.ReadBytes( 1 );
                    }
                    else
                    {
                        throw new InvalidOperationException( string.Format(
                            "Unexpected Type {0}", type.FullName ) );
                    }
                    break;
            }

            return o;
        }

        private object DeserializeCollection( Type type )
        {
            Type collectionElementType;

            /// I don't really know what to check here to limit the serialization
            /// to Generic collections.
            if ( type.IsGenericType &&
                    type.IsClass &&
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

            IList collection = (IList)Activator.CreateInstance(
                typeof( List<> ).MakeGenericType( collectionElementType ) );

            /// Don't use PeekChar() because Characters are custom encoded
            /// in Atari (and possibly other) games thus PeekChar returns
            /// a converted value rather than the actual char/byte in the
            /// stream.
            while ( this._reader.ReadByte() != terminationByte )
            {
                /// must back up 1 char/byte to compensate for the ReadByte()
                /// call above.
                this._reader.BaseStream.Seek( -1, SeekOrigin.Current );

                collection.Add( this.Deserialize( collectionElementType ) );
            }

            if ( type.IsArray )
            {
                /// List<> has a method ToArray which will produce the array
                /// output needed but because we used CreateInstance I don't 
                /// have the exact type allowing access. So use reflection.
                return collection.GetType().GetMethod( "ToArray" )
                        .Invoke( collection, new object[ 0 ] );
            }
            else
            {
                return GetIEnumerableConstructor( type, collectionElementType )
                    .Invoke( new object[] { collection } );
            }
        }

        private static ConstructorInfo GetIRomSerializableConstructor( Type type )
        {
            ConstructorInfo[] ctorInfos = type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

            foreach ( ConstructorInfo ctorInfo in ctorInfos )
            {
                ParameterInfo[] parameters = ctorInfo.GetParameters();

                if ( parameters.Length == 2 &&
                    parameters[ 0 ].ParameterType == typeof( RomSerializationInfo ) &&
                    parameters[ 1 ].ParameterType == typeof( StreamingContext ) )
                {
                    return ctorInfo;
                }
            }

            throw new InvalidOperationException();
        }

        private static ConstructorInfo GetIEnumerableConstructor( Type type, Type parameterType )
        {
            ConstructorInfo[] ctorInfos = type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );

            foreach ( ConstructorInfo ctorInfo in ctorInfos )
            {
                ParameterInfo[] parameters = ctorInfo.GetParameters();

                if ( parameters.Length == 1 &&
                    parameters[ 0 ].ParameterType.GenericTypeArguments.Length == 1 &&
                    parameters[ 0 ].ParameterType.GenericTypeArguments[ 0 ] == parameterType )
                {
                    return ctorInfo;
                }
            }

            throw new InvalidOperationException();
        }
    }
}