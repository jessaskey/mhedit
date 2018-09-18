using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Find Extension for implementations of IList<T> derived to support
        /// read only collection properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>( this IList<T> list, Predicate<T> predicate )
        {
            foreach( var element in list )
            {
                if ( predicate( element ) )
                {
                    return element;
                }
            }

            return default( T );
        }
    }
}
