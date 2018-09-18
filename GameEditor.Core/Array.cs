using GameEditor.Core.Hardware;
using System;
using System.Collections.Generic;

namespace GameEditor.Core
{

    interface IArray
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array<T>
    {
        /// <summary>
        ///  Funny that I use a list for an Array...
        /// </summary>
        private readonly List<T> _elements = new List<T>();
        private Xml.Array _definition;

        Array( Xml.Array definition, IMemoryMap memoryMap )
        {
            this._definition = definition;

            /// storing pointers means integer storage since we are
            /// talking about hardware pointers here.
            if ( definition.Storage == Xml.ArrayStorage.Pointer &&
                 typeof( T ) != typeof( uint ) )
            {
                throw new ArrayTypeMismatchException();
            }

            //memoryMap.TryFindPage( definition.PageRef, this._memoryPage );
        }
    }
}
