using System.Runtime.Serialization;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Represents an object that can be serialized into/out of game
    /// memory. Typically this is game object data used to detail
    /// levels.
    /// </summary>
    public interface IRomSerializable
    {
        void GetObjectData( RomSerializationInfo si, StreamingContext context );
    }
}
