using System;
using System.Collections;
using System.IO;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Encapsulate the iteration behavior during deserialization operations as it
    /// depends upon several different variables and has a non trivial implementation.
    /// </summary>
    internal class CollectionReaderEnumerator : IEnumerator
    {
        private readonly ObjectReader _reader;
        private readonly object _terminationValue;
        private int? _predeterminedLength;
        private bool? _continueIterating;

        internal CollectionReaderEnumerator( ObjectReader reader, Type type, int? predeterminedLength )
        {
            this._reader = reader;

            this._predeterminedLength = predeterminedLength;

            /// if the collection has predetermined length then don't use a
            /// termination object.
            this._terminationValue = this._predeterminedLength != null ?
                null :
                RomSerializer.GetTerminationObject( type );

            if ( this._predeterminedLength == null && this._terminationValue == null )
            {
                throw new InvalidOperationException(
                    "Expected fixed length or termination object." );
            }
        }

        public object Current
        {
            get
            {
                /// Not valid until first call to MoveNext().
                if ( !this._continueIterating.HasValue ||
                     this._continueIterating.Value == false )
                {
                    throw new InvalidOperationException();
                }

                /// If fixed length collection return # remaining.
                if ( this._predeterminedLength.HasValue )
                {
                    return this._predeterminedLength.Value;
                }

                return this._terminationValue;
            }
        }

        public bool MoveNext()
        {
            this._continueIterating = false;

            if ( this._predeterminedLength.HasValue )
            {
                /// Fixed length collection decrements count.
                this._continueIterating = --this._predeterminedLength >= 0;
            }
            /// Working with a Termination object in the stream.
            else
            {
                long savedPosition = this._reader.BaseReader.BaseStream.Position;

                object peekTerminationValue =
                    this._reader.DeserializePrimitive( this._terminationValue.GetType() );

                /// Compare objects to determine if at end.
                if ( !peekTerminationValue.Equals( this._terminationValue ) )
                {
                    /// Rewind stream as termination object was not found.
                    this._reader.BaseReader.BaseStream.Position = savedPosition;

                    /// return true to continue iterating
                    this._continueIterating = true;
                }
            }

            return this._continueIterating.Value;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}