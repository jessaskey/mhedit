using System;

namespace GameEditor.Core.Serialization
{
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = true,
                 Inherited = true )]
    public class ConcreteTypeAttribute : Attribute
    {
        private readonly Type _concrete;
        private readonly string _memberName;

        public ConcreteTypeAttribute( Type concrete )
            : this( concrete, string.Empty )
        { }

        public ConcreteTypeAttribute( Type concrete, string memberName )
        {
            this._concrete = concrete;

            this._memberName = memberName;
        }

        public string MemberName
        {
            get { return this._memberName; }
        }

        public Type ConcreteType
        {
            get { return this._concrete; }
        }
    }
}
