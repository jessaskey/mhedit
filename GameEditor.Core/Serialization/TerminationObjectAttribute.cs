using System;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Declares a Termination Value to be used in conjunction with the decorated
    /// object when it is serialized as part of a collection. The Value is placed
    /// into the serialization stream after the last element of the collection.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = false,
                 Inherited = true )]
    public class TerminationObjectAttribute : Attribute
    {
        private readonly object _value;

        /// <summary>
        /// 16-bit char termination value.
        /// </summary>
        /// <param name="value"></param>
        //public TerminationObjectAttribute( char value )
        //{
        //    this._value = value;
        //}

        /// <summary>
        /// Byte termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( byte value )
        {
            this._value = value;
        }

        /// <summary>
        /// Uint16 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( ushort value )
        {
            this._value = value;
        }

        /// <summary>
        /// Uint32 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( uint value )
        {
            this._value = value;
        }

        /// <summary>
        /// Uint64 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( ulong value )
        {
            this._value = value;
        }

        /// <summary>
        /// Signed byte termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( sbyte value )
        {
            this._value = value;
        }

        /// <summary>
        /// Int16 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( short value )
        {
            this._value = value;
        }

        /// <summary>
        /// Int32 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( int value )
        {
            this._value = value;
        }

        /// <summary>
        /// Int64 termination value.
        /// </summary>
        /// <param name="value"></param>
        public TerminationObjectAttribute( long value )
        {
            this._value = value;
        }

        public object Value
        {
            get
            {
                //if ( !RomSerializer.IsPrimitiveType( this._value.GetType() ) )
                //{
                //    /// Don't throw in constructor it would occur anytime this attribute
                //    /// is created (when any get attributes call is made). So throw when
                //    /// the value is accessed as it will occur at the use point and have
                //    /// a valid context.
                //    /// <see cref="https://stackoverflow.com/a/24576879"/>
                //    throw new InvalidOperationException(
                //        "TerminationObjectAttribute value must be a Primitive type." );
                //}

                return this._value;
            }
        }
    }
}
