using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    /// <summary>
    /// https://stackoverflow.com/a/1316417
    /// </summary>
    [Serializable]
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        protected bool SetField<T>(
            ref T field, T value, [CallerMemberName] string propertyName = null )
        {
            if ( EqualityComparer<T>.Default.Equals( field, value ) )
                return false;

            field = value;

            OnPropertyChanged( propertyName );

            return true;
        }
    }

    [Serializable]
    public abstract class TrackEditsBase : NotifyPropertyChangedBase, ITrackEdits
    {
        private bool _isDirty = false;

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public virtual bool IsDirty
        {
            get { return this._isDirty; }
            set
            {
                /// To avoid feedback don't use SetField.
                if ( this._isDirty != value )
                {
                    this._isDirty = value;

                    base.OnPropertyChanged();
                }
            }
        }

        protected void ForwardIsDirtyPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == "IsDirty" )
            {
                /// Call base to avoid feedback.
                base.OnPropertyChanged( e.PropertyName );
            }
        }

        protected override void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            /// Call base to avoid feedback.
            base.OnPropertyChanged( propertyName );

            this.IsDirty = true;
        }
    }

    [Serializable]
    public abstract class ExtendedObservableCollection<T> : ObservableCollection<T>, ITrackEdits where T : INotifyPropertyChanged, ITrackEdits
    {
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return this.Any( item => item.IsDirty );
            }
            set
            {
                this.All( item => { item.IsDirty = value; return true; } );
            }
        }

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

        private void ItemPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == "IsDirty" )
            {
                this.OnPropertyChanged( e );
            }
        }
    }
}
