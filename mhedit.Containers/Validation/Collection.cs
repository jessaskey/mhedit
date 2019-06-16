using System.Collections;
using System.Collections.Generic;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// Utility class to allow the validation of a collection of objects.
    /// </summary>
    public class Collection : IName
    {
        private readonly List<object> _objects;
        private string _name;

        public Collection()
        {
            this._objects = new List<object>();
        }

        public Collection( IEnumerable<object> enumerable )
        {
            this._objects = new List<object>( enumerable );
        }

        [Validation( typeof( ElementsRule ) )]
        public IEnumerable Elements
        {
            get { return this._objects; }
        }

        public void Add( object obj )
        {
            this._objects.Add( obj );
        }

#region Implementation of IName

        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

#endregion
    }
}
