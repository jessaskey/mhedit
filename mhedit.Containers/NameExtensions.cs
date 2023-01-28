using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mhedit.Containers
{
    public static class NameExtensions
    {
        private static Dictionary<string, int> _nameStore =
            new Dictionary<string, int>();

        private static Dictionary<Type, Dictionary<string, int>> _store =
            new Dictionary<Type, Dictionary<string, int>>();

        private static string Create(string root)
        {
            if (!_nameStore.ContainsKey(root))
            {
                _nameStore[root] = 0;
            }

            return $"{root}{++_nameStore[root]}";
        }

        public static string CreateName( this object key, string root = null )
        {
            Dictionary<string, int> nameStore;

            Type keyType = key.GetType();

            root ??= keyType.Name;

            if ( !_store.TryGetValue( keyType, out nameStore ) )
            {
                _store[ keyType ] = nameStore = new Dictionary<string, int>();
            }

            if ( !nameStore.ContainsKey( root ) )
            {
                nameStore[ root ] = 0;
            }

            nameStore[ root ]+=1;

            return $"{root}{nameStore[ root ]}";
        }

        public static string CreateName( this IName key, string root = null )
        {
            key.Name = CreateName( (object)key, root );

            return key.Name;
        }
    }
}
