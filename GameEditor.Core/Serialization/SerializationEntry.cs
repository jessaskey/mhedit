using System;

namespace GameEditor.Core.Serialization
{
    internal struct SerializationEntry
    {
        private readonly Type _type;
        private readonly object _value;
        private readonly string _name;

        internal SerializationEntry( string entryName, object entryValue, Type entryType )
        {
            this._value = entryValue;
            this._name = entryName;
            this._type = entryType;
        }

        public object Value
        {
            get
            {
                return this._value;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public Type ObjectType
        {
            get
            {
                return this._type;
            }
        }
    }
}
