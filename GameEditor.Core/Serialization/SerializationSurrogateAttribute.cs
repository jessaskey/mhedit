using System;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Sometimes two or more object's have serialization semantics that are
    /// tightly coupled and/or intertwined. As a result these objects are
    /// typically subjected to serialization in a single process that originates
    /// from one (the most significant) object in the chain. This attribute
    /// identifies the surrogate object that is responsible for the decorated
    /// objects serialization operations.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = false,
                 Inherited = false )]
    public class SerializationSurrogateAttribute : Attribute
    {
        private readonly Type _surrogate;

        public SerializationSurrogateAttribute( Type surrogate )
        {
            this._surrogate = surrogate;
        }

        /// <summary>
        /// The object that the serialization process is deferred to.
        /// </summary>
        public Type SurrogateType
        {
            get { return this._surrogate; }
        }
    }
}
