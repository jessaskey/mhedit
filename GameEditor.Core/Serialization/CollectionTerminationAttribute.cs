using System;

namespace GameEditor.Core.Serialization
{
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = false,
                 Inherited = true )]
    public class CollectionTerminationAttribute : Attribute
    {
        private byte _terminationByte;

        public CollectionTerminationAttribute( byte terminationByte )
        {
            this._terminationByte = terminationByte;
        }

        public byte TerminationByte
        {
            get { return this._terminationByte; }
        }
    }
}
