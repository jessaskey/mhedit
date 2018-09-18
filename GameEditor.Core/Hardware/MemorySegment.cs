using System;
using System.Collections.Generic;
using GameEditor.Core.Xml;

namespace GameEditor.Core.Hardware
{
    public class MemorySegment : IMemorySegment
    {
        private readonly Xml.Rom _description;
        private readonly uint _address;
        private readonly int _size;
        private readonly IMemoryMap _map;
        private readonly List<IMemoryBank> _banks = new List<IMemoryBank>();
        private readonly List<IMemoryPage> _pages = new List<IMemoryPage>();
        private readonly List<BankReference> _bankReferences = new List<BankReference>();

        public MemorySegment( IMemoryMap map, Xml.Rom description )
        {
            this._map = map;

            this._description = description;

            this._address = Convert.ToUInt32( this._description.Address, 16 );

            this._size = Convert.ToInt32( this._description.Size, 16 );

            foreach ( Xml.Bank bank in description.Banks )
            {
                this._banks.Add( MemoryBank.Factory( bank, map.DataWidth ) );
            }

            /// A seed BankReference that starts us at the base of the
            /// paged memory.
            BankReference last = new BankReference( this._banks[ 0 ], 0, 0, 0 );

            foreach ( Xml.Page pageDescription in description.Pages )
            {
                MemoryPage page = this.CreatePage( pageDescription, last );

                last = page.BankReferences[ ( page.BankReferences.Count - 1 ) ];

                this._pages.Add( page );
            }
        }

        private MemoryPage CreatePage( Xml.Page description, BankReference previous )
        {
            MemoryPage page = new MemoryPage( this, description );

            /// The page's size is that of the memory segment.
            int pageSize = this.Size;

            /// Assumption is that we have implied order from the GameProfile.xml file,
            /// lowest address bank listed first.
            using ( IEnumerator<IMemoryBank> enumerator = this._banks.GetEnumerator() )
            {
                /// Fast forward to the previous bank
                while ( enumerator.MoveNext() && enumerator.Current != previous.Bank );

                /// Initial offset is where previous bank ends.
                int bankOffset = previous.BankOffset + previous.Size;

                /// Loop through to end of banks to assign memory to this page.
                do
                {
                    IMemoryBank bank = enumerator.Current;

                    /// if the offset into the bank is less than it's size that means
                    /// there is room for some of this page in the bank.
                    if ( bankOffset < bank.Devices[ 0 ].Size )
                    {
                        /// all devices in any given bank are the same size.
                        int remainingSpaceInBank = bank.Devices[ 0 ].Size - bankOffset;

                        /// PageOffset is based upon how much of the page we have already
                        /// assigned to previous banks.
                        int pageOffset = this.Size - pageSize;

                        page.BankReferences.Add( new BankReference(
                            bank, bankOffset, pageOffset, remainingSpaceInBank >= pageSize ?
                                                            pageSize :
                                                            remainingSpaceInBank ) );

                        /// adjust the amount of page memory we still need to assign.
                        pageSize -= remainingSpaceInBank;
                    }

                    /// Reset to beginning to allocate memory from the next bank. 
                    bankOffset = 0;
                }
                while ( pageSize > 0 && enumerator.MoveNext() );
            }

            if ( pageSize > 0 )
            {
                throw new InsufficientMemoryException( string.Format(
                    "Insufficient memory in {0} segment to support page {1}.",
                    this.Id, description.Id ) );
            }

            return page;
        }

        public string Id
        {
            get
            {
                return this._description.Id;
            }
        }

        public uint Address
        {
            get
            {
                return this._address;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
        }

        public IMemoryMap Map
        {
            get
            {
                return this._map;
            }
        }

        public IList<IMemoryBank> Banks
        {
            get
            {
                return this._banks.AsReadOnly();
            }
        }

        public IList<IMemoryPage> Pages
        {
            get
            {
                return this._pages.AsReadOnly();
            }
        }
    }

}
