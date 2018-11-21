﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace GameEditor.Core.Serialization
{
    public class RomSerializationInfo
    {
        private ObjectReader _reader;
        private Dictionary<string, SerializationEntry> _nameToIndex =
            new Dictionary<string, SerializationEntry>();
        private Type _type;

        internal RomSerializationInfo( Type type )
            :this( type, null )
        {}

        internal RomSerializationInfo( Type type, ObjectReader reader )
        {
            this._type = type;

            this._reader = reader;
        }

        public IEnumerator<SerializationEntry> GetEnumerator()
        {
            return this._nameToIndex.Values.GetEnumerator();
        }

        public void AddValue( string name, object value, Type type )
        {
            if ( name == null )
            {
                throw new ArgumentNullException( nameof( name ) );
            }
            if ( type == null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            this.AddValueInternal( name, value, type );
        }

        public void AddValue( string name, object value )
        {
            if ( value == null )
            {
                this.AddValue( name, value, typeof( object ) );
            }
            else
            {
                this.AddValue( name, value, value.GetType() );
            }
        }

        public void AddValue( string name, bool value )
        {
            this.AddValue( name, value, typeof( bool ) );
        }

        public void AddValue( string name, char value )
        {
            this.AddValue( name, value, typeof( char ) );
        }

        public void AddValue( string name, sbyte value )
        {
            this.AddValue( name, value, typeof( sbyte ) );
        }

        public void AddValue( string name, byte value )
        {
            this.AddValue( name, value, typeof( byte ) );
        }

        public void AddValue( string name, short value )
        {
            this.AddValue( name, value, typeof( short ) );
        }

        public void AddValue( string name, ushort value )
        {
            this.AddValue( name, value, typeof( ushort ) );
        }

        public void AddValue( string name, int value )
        {
            this.AddValue( name, value, typeof( int ) );
        }

        public void AddValue( string name, uint value )
        {
            this.AddValue( name, value, typeof( uint ) );
        }

        public void AddValue( string name, long value )
        {
            this.AddValue( name, value, typeof( long ) );
        }

        public void AddValue( string name, ulong value )
        {
            this.AddValue( name, value, typeof( ulong ) );
        }

        public void AddValue( string name, float value )
        {
            this.AddValue( name, value, typeof( float ) );
        }

        public void AddValue( string name, double value )
        {
            this.AddValue( name, value, typeof( double ) );
        }

        public void AddValue( string name, Decimal value )
        {
            this.AddValue( name, value, typeof( Decimal ) );
        }

        public void AddValue( string name, DateTime value )
        {
            this.AddValue( name, value, typeof( DateTime ) );
        }

        private void AddValueInternal( string name, object value, Type type )
        {
            if ( this._nameToIndex.ContainsKey( name ) )
            {
                throw new SerializationException( "Serialization_SameNameTwice" );
            }

            this._nameToIndex.Add( name,
                new SerializationEntry( name, value, type) );
        }

        private object FindElementInternal( string name, object value, Type type )
        {
            if ( this._nameToIndex.ContainsKey( name ) )
            {
                throw new SerializationException( "Serialization_SameNameTwice" );
            }

            this._nameToIndex.Add( name,
                new SerializationEntry( name, value, type ) );

            return value;
        }

        private Type GetConcreteType( Type type, string name )
        {
            IEnumerable<ConcreteTypeAttribute> attrs =
                this._type.GetCustomAttributes<ConcreteTypeAttribute>( true );

            IEnumerable<ConcreteTypeAttribute> concreteTypes =
                attrs.Where<ConcreteTypeAttribute>( attr =>
                {
                    /// If Member name is provided then use that in the qualification
                    return string.IsNullOrEmpty( attr.MemberName ) ?
                           type.IsAssignableFrom( attr.ConcreteType )
                           :
                           name.Equals( attr.MemberName, StringComparison.InvariantCultureIgnoreCase ) &&
                           type.IsAssignableFrom( attr.ConcreteType );
                } );

            /// if there are multiple hits for a concrete type (when a member name
            /// isn't specified) then pull the first match.
            return concreteTypes.First().ConcreteType;
        }

        private T GetObject<T>( string name )
        {
            if ( this._reader == null )
            {
                throw new InvalidOperationException();
            }

            return (T)this.FindElementInternal(
                name, this._reader.DeserializePrimitive( typeof( T ) ), typeof( T ) );
        }

        public object GetValue( string name, Type type )
        {
            if ( string.IsNullOrEmpty(name) )
            {
                throw new ArgumentNullException();
            }

            if ( type == null )
            {
                throw new ArgumentNullException();
            }

            if ( this._reader == null )
            {
                throw new InvalidOperationException();
            }

            /// if the type we are deserializing is abstract then look for
            /// the attribute that provides the concrete type to be used. 
            Type concreteType = type.IsAbstract ?
                this.GetConcreteType( type, name ) :
                type;

            /// Don't allow any object to deserialize the same element twice.
            return this.FindElementInternal(
                name, this._reader.Deserialize( concreteType ), type );
        }

        public bool GetBoolean( string name )
        {
            return this.GetObject<bool>( name );
        }

        public char GetChar( string name )
        {
            return this.GetObject<char>( name );
        }

        public sbyte GetSByte( string name )
        {
            return this.GetObject<sbyte>( name );
        }

        public byte GetByte( string name )
        {
            return this.GetObject<byte>( name );
        }

        public short GetInt16( string name )
        {
            return this.GetObject<short>( name );
        }

        public ushort GetUInt16( string name )
        {
            return this.GetObject<ushort>( name );
        }

        public int GetInt32( string name )
        {
            return this.GetObject<int>( name );
        }

        public uint GetUInt32( string name )
        {
            return this.GetObject<uint>( name );
        }

        public long GetInt64( string name )
        {
            return this.GetObject<long>( name );
        }

        public ulong GetUInt64( string name )
        {
            return this.GetObject<ulong>( name );
        }

        public float GetSingle( string name )
        {
            /// Not sure how this would convert as encodings might be
            /// different on different architectures.
            throw new NotImplementedException();
            //return this.GetObject<float>( name );
        }

        public double GetDouble( string name )
        {
            /// Not sure how this would convert as encodings might be
            /// different on different architectures.
            throw new NotImplementedException();
            //return this.GetObject<double>( name );
        }

        public Decimal GetDecimal( string name )
        {
            /// Not sure how this would convert as encodings might be
            /// different on different architectures.
            throw new NotImplementedException();
            //return this.GetObject<Decimal>( name );
        }

        public DateTime GetDateTime( string name )
        {
            /// Not sure how this would convert as encodings might be
            /// different on different architectures.
            throw new NotImplementedException();
            //return this.GetObject<DateTime>( name );
        }

        public string GetString( string name )
        {
            return this.GetObject<string>( name );
        }
    }
}
