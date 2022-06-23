using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    [Serializable]
    public class ExtendedObservableCollection<T> : ObservableCollection<T>, IChangeTracking
        where T : INotifyPropertyChanged, IChangeTracking
    {
        #region Implementation of IChangeTracking

        private bool _isChanged = false;

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public bool IsChanged
        {
            get
            {
                return this._isChanged |
                    this.Any( item => item.IsChanged );
            }
            private set
            {
                if ( this._isChanged != value )
                {
                    this._isChanged = value;

                    /// Call into base method to avoid setting IsChanged.
                    this.OnPropertyChanged(
                        new PropertyChangedEventArgs( nameof(ChangeTrackingBase.IsChanged) ) );
                }
            }
        }

        public virtual void AcceptChanges()
        {
            this.All( item => { item.AcceptChanges(); return true; } );

            this._isChanged = false;
        }

        #endregion

        protected override void ClearItems()
        {
            foreach ( var item in Items )
                item.PropertyChanged -= ItemPropertyChanged;

            base.ClearItems();
        }

        protected override void InsertItem( int index, T item )
        {
            item.PropertyChanged += ItemPropertyChanged;

            base.InsertItem( index, item );
        }

        protected override void RemoveItem( int index )
        {
            this[ index ].PropertyChanged -= ItemPropertyChanged;

            base.RemoveItem( index );
        }

        protected override void SetItem( int index, T item )
        {
            this[ index ].PropertyChanged -= ItemPropertyChanged;

            item.PropertyChanged += ItemPropertyChanged;

            base.SetItem( index, item );
        }

        protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            base.OnCollectionChanged( e );

            this.IsChanged = true;
        }

        private void ItemPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            /// Fwd the change if necessary but don't set local IsChanged.
            this.OnPropertyChanged( e );
        }
    }
}
