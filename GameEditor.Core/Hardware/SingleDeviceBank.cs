using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEditor.Core.Xml;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// A bank of memory that wraps up a single IMemoryDevice. This class is a
    /// specialization of IMemoryBank as a single device results in less
    /// complicated access rules.
    /// </summary>
    internal class SingleDeviceBank : MemoryBank
    {
        public SingleDeviceBank( Bank description, int dataBusWidth )
            : base( description, dataBusWidth )
        {}

        public override int Read( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            ///// convert the byte count into the number of read operations by
            ///// dividing the bytes requested by the byte width of the bank.
            ///// This forces a min read length of the bank width.
            //words /= ( this.Width / 8 );

            return this._devices[ 0 ].Read( address, buffer, offset, words );
        }

        public override int Write( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            ///// convert the byte count into the number of read operations by
            ///// dividing the bytes requested by the byte width of the bank.
            ///// This forces a min read length of the bank width.
            //words /= ( this.Width / 8 );

            return this._devices[ 0 ].Write( address, buffer, offset, words );
        }

    }
}
