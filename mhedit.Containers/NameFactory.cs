using System.Collections.Generic;

namespace mhedit.Containers
{
    public static class NameFactory
    {
        private static Dictionary<string, int> _nameStore =
            new Dictionary<string, int>();

        public static string Create( string root )
        {
            if ( !_nameStore.ContainsKey( root ) )
            {
                _nameStore[ root ] = 0;
            }

            return $"{root}{++_nameStore[ root ]}";
        }
    }
}
