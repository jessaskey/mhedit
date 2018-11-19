using System;

namespace GameEditor.Core.Serialization
{
    public struct SerializationEntry
    {
        private Type m_type;
        private object m_value;
        private string m_name;

        internal SerializationEntry( string entryName, object entryValue, Type entryType )
        {
            this.m_value = entryValue;
            this.m_name = entryName;
            this.m_type = entryType;
        }

        public object Value
        {
            get
            {
                return this.m_value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public Type ObjectType
        {
            get
            {
                return this.m_type;
            }
        }
    }
}
