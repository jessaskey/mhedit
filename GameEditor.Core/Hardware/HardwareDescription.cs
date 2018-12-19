using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEditor.Core.Extensions;
using GameEditor.Core.Xml;

namespace GameEditor.Core.Hardware
{
    public class HardwareDescription : IHardwareDescription
    {
        private readonly List<IMemoryMap> _maps = new List<IMemoryMap>();

        public HardwareDescription( Xml.HardwareDescription description )
        {
            foreach ( Xml.MemoryMap map in description.MemoryMaps )
            {
                this._maps.Add( new MemoryMap( map ) );
            }
        }

        public IList<IMemoryMap> MemoryMaps
        {
            get
            {
                return this._maps.AsReadOnly();
            }
        }

        public MemoryPageStream FindPageStream( PageRef pageRef )
        {
            return new MemoryPageStream( this.FindMemoryPage( pageRef ) );
        }

        public IMemoryPage FindMemoryPage( PageRef pageRef )
        {
            /// path is dot notation
            string[] path = pageRef.Id.Split( '.' );

            if ( path.Length != 3 )
            {
                throw new FormatException( "Invalid Path. Expected 3 segments, actual " +
                    path.Length + " - " + pageRef.Id );
            }

            int pathIndex = 0;

            IMemoryPage found =
                this._maps.FirstOrDefault( map => map.Id == path[ pathIndex ] )?.
                      Segments.FirstOrDefault( segment => segment.Id == path[ pathIndex = 1 ] )?.
                      Pages.FirstOrDefault( page => page.Id == path[ pathIndex = 2 ] );

            if ( found == null )
            {
                throw new KeyNotFoundException( string.Format(
                    "Invalid Path to Page. Unable to locate key {0} in path '{1}'.",
                    path[ pathIndex ], pageRef.Id ) );
            }

            return found;
        }
    }

}
