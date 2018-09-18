namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// 
    /// </summary>
    internal class BankReference
    {
        private readonly IMemoryBank _bank;
        private readonly int _bankOffset;
        private readonly int _pageOffset;
        private readonly int _size;

        public BankReference( IMemoryBank bank, int bankOffset, int pageOffset, int size )
        {
            this._bank = bank;

            this._bankOffset = bankOffset;

            this._pageOffset = pageOffset;

            this._size = size;
        }

        public IMemoryBank Bank
        {
            get
            {
                return this._bank;
            }
        }

        public int BankOffset
        {
            get
            {
                return this._bankOffset;
            }
        }

        public int PageOffset
        {
            get
            {
                return this._pageOffset;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
        }

        internal int Read( uint address, byte[] buffer, int offset, int count )
        {
            int bytesRead = 0;

            address -= (uint)this._pageOffset;

            if ( 0 <= address && address < this._size )
            {
                /// Convert bytes count to words.
                count /= this._bank.Width / 8;

                bytesRead = this._bank.Read(
                    (uint)( address + this._bankOffset ), buffer, offset, count );
            }

            return bytesRead;
        }

        internal int Write( uint address, byte[] buffer, int offset, int count )
        {
            int bytesWritten = 0;

            address -= (uint)this._pageOffset;

            if ( 0 <= address && address < this._size )
            {
                /// Convert bytes count to words.
                count /= this._bank.Width / 8;

                bytesWritten = this._bank.Write(
                    (uint)( address + this._bankOffset ), buffer, offset, count );
            }

            return bytesWritten;
        }
    }
}