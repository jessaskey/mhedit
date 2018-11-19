using System;

namespace GameEditor.Core.Serialization
{
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = true,
                 Inherited = true )]
    public class ConcreteTypeAttribute : Attribute
    {
        private Type _concrete;
        private string _property;

        public ConcreteTypeAttribute( Type concrete )
            : this( concrete, string.Empty )
        { }

        public ConcreteTypeAttribute( Type concrete, string property )
        {
            this._concrete = concrete;

            this._property = property;
        }

        public string PropertyName
        {
            get { return this._property; }
        }

        public Type ConcreteType
        {
            get { return this._concrete; }
        }
    }
}
