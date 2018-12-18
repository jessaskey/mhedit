using System;
using System.Collections;

namespace GameEditor.Core.Serialization
{
    /// <summary>
    /// Identifies the collection object that should be used in conjunction
    /// with serialization operations for the decorated type. 
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct,
                 AllowMultiple = false,
                 Inherited = false )]
    public class CollectionTypeAttribute : Attribute
    {
        private readonly Type _collection;

        public CollectionTypeAttribute( Type collection )
        {
            this._collection = collection;
        }

        /// <summary>
        /// The collection Type that should be used by the serialization engine
        /// when working with the decorated type.
        /// </summary>
        public Type CollectionType
        {
            get
            {
                /// Can't perform validation tests in the constructor because
                /// it's not used during the compilation process.
                /// 
                /// Throw here if the type doesn't support the IList interface
                /// which is required by the serialization engine.
                if ( !typeof(IList).IsAssignableFrom( this._collection) )
                {
                    throw new ArgumentException(
                        "Doesn't implement IList.", nameof( CollectionType ) );
                }

                return this._collection;
            }
        }
    }
}
