using System;
using System.Collections.Generic;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// Abstract MemoryBank base class. 
    /// </summary>
    internal abstract class MemoryBank : IMemoryBank
    {
        private readonly Xml.Bank _description;
        private readonly int _size;
        private readonly int _width;
        protected readonly List<IMemoryDevice> _devices = new List<IMemoryDevice>();

        public MemoryBank( Xml.Bank description, int dataBusWidth )
        {
            this._description = description;

            this._width = dataBusWidth;

            int widthOfDevices = 0;

            /// All devices in a bank are of the same address size.
            /// If this throws that's fine as a bank always needs at least 1 device!!
            this._size = description.Devices[ 0 ].Size;

            foreach ( Xml.MemoryDevice deviceDescription in description.Devices )
            {
                /// All devices must be the same size as the first!
                if ( deviceDescription.Size != this._size )
                {
                    throw new DataMisalignedException( string.Format(
                        "Device size mismatch, Bank {0}, device {1}",
                        this.Id, deviceDescription.Id ) );
                }

                IMemoryDevice device = MemoryDevice.Factory( deviceDescription );

                widthOfDevices += device.Width;

                this._devices.Add( device );
            }

            if ( widthOfDevices != dataBusWidth )
            {
                throw new DataMisalignedException( string.Format(
                    "DataBus width mismatch, Bank {0} width {1} != MemoryMap width {2}",
                    this.Id, widthOfDevices, dataBusWidth) );
            }
        }

        /// <summary>
        ///  Factory pattern to create the proper MemoryBank.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        internal static IMemoryBank Factory( Xml.Bank bank, int dataBusWidth )
        {
            /// This is a bit ridiculous at the moment but if we ever need
            /// to support different banks this is the factory.
            if ( bank.Devices.Count == 1 )
            {
                /// Specialization for banks that reference only a single device. 
                return new SingleDeviceBank( bank, dataBusWidth );
            }
            else if ( bank.Devices.Count > 1 )
            {
                return new MultipleDeviceBank( bank, dataBusWidth );
            }

            throw new NotSupportedException( string.Format(
                "Bank Id: {0}", bank.Id ) );
        }

        public string Id
        {
            get
            {
                return this._description.Id;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
        }

        public IList<IMemoryDevice> Devices
        {
            get
            {
                return this._devices.AsReadOnly();
            }
        }

        public abstract int Read( uint address, byte[] buffer, int offset = 0, int count = -1 );

        public abstract int Write( uint address, byte[] buffer, int offset = 0, int count = -1 );
    }
}