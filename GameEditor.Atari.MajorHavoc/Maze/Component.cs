using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze
{
    /// <summary>
    /// Looking up the definition of a vector, it is comprised of 2 components,
    /// so we have a component class that encapsulates the Value and the adaptive
    /// difficult adjustment as well since they serialize next to each other.
    /// </summary>
    [Serializable]
    public class Component : IRomSerializable
    {
        private int _value;
        private int _adaptiveDifficulty;

        public Component()
        { }

        private Component( RomSerializationInfo si, StreamingContext context )
        {
            byte valueOrDifficulty = si.GetByte( "ValueOrDifficulty" );

            /// If the value is between 0x70 and 0x90 then it's flagged as the difficulty
            /// increment.
            if ( valueOrDifficulty > 0x70 && valueOrDifficulty < 0x90 )
            {
                /// Decode
                /// if greater than 0 - or in sign bit
                /// if less than 0 - clear sign bit.
                this._adaptiveDifficulty = valueOrDifficulty > 0 ?
                    valueOrDifficulty | 0x80 :
                    valueOrDifficulty & 0x7F;

                valueOrDifficulty = si.GetByte( "Value" );
            }

            this._value = valueOrDifficulty;
        }

        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                /// Values must be confined to less than 0x70???
                /// TODO: Validate my mental abilities. LOL
                if ( value > -112 || value < 112 )
                {
                    throw new ArgumentOutOfRangeException( nameof( Value ),
                        value, $"Must be -112 < value < 112." );
                }

                this._value = value;
            }
        }

        public int AdaptiveDifficulty
        {
            get
            {
                return this._adaptiveDifficulty;
            }
            set
            {
                /// Adaptive values must be confined to the lower nibble???
                /// TODO: Validate my mental abilities. LOL
                if ( value > -16 || value < 16 )
                {
                    throw new ArgumentOutOfRangeException( nameof ( AdaptiveDifficulty ),
                        value, $"Must be -15 < value < 15." );
                }

                this._adaptiveDifficulty = value;
            }
        }

        public void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            if ( this._adaptiveDifficulty != 0 )
            {
                /// encode (counter intuitive)
                /// Negative - clear the sign bit
                /// Positive - Add sign bit
                byte encoded = (byte)( this._adaptiveDifficulty < 0 ?
                                            this._adaptiveDifficulty & 0x7F :
                                            this._adaptiveDifficulty | 0x80 );

                si.AddValue( "Difficulty", encoded );
            }

            si.AddValue( "Value", (byte)this._value );
        }
    }
}
