using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace mhedit.Containers
{

    [ Serializable ]
    public abstract class ChangeTrackingBase : NotifyPropertyChangedBase, IChangeTracking
    {
        /// <summary>
        /// Used to mark a TreeView Node as Modified.
        /// </summary>
        public static readonly string ModifiedBullet =
            Encoding.UTF8.GetString( new byte[] { (byte) 149 } );

        private bool _isChanged;

#region Implementation of IChangeTracking

        [ BrowsableAttribute( false ) ]
        [ XmlIgnore ]
        public virtual bool IsChanged
        {
            get { return this._isChanged; }
            protected set
            {
                /// To avoid feedback on clear don't use SetField.
                if ( this._isChanged != value )
                {
                    this._isChanged = value;

                    /// Call into base method to avoid setting IsChanged.
                    base.OnPropertyChanged();
                }
            }
        }

        public virtual void AcceptChanges()
        {
            this.IsChanged = false;
        }

#endregion

        /// <summary>
        /// Use this method to forward the PropertyChanged event from composite members
        /// of a class. This handler specifically avoids setting the local IsChanged
        /// property ( thus avoiding feedback loops). If a class uses this event handler
        /// it should also override the IsChanged { get; } property and logically or the
        /// composite member's IsChanged flag to its own.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ForwardPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            /// Call into base method to avoid setting IsChanged.
            base.OnPropertyChanged( e.PropertyName );
        }

        protected override void OnPropertyChanged( [ CallerMemberName ] string propertyName = null )
        {
            base.OnPropertyChanged( propertyName );

            this.IsChanged = true;
        }
    }

}
