using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{
    public class PropertyBrowser : KryptonPropertyGrid, IUserInterface
    {
        private readonly IList _selectedItems;

        public PropertyBrowser(IList selectedItems)
        {
            this._selectedItems = selectedItems;

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size(200, 200);
            
            if (selectedItems is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += this.OnItemsCollectionChanged;
            }
        }

        private void OnItemsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            this.SelectedObjects = this._selectedItems.Cast<object>().ToArray();
        }

#region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.DockRight; }
        }

        public DockingPosition DockingPositions
        {
            get
            {
                return DockingPosition.Left |
                    DockingPosition.Right |
                    DockingPosition.Top |
                    DockingPosition.Bottom;
            }
        }

        public bool HideOnClose
        {
            get { return true; }
        }

        public string Caption
        {
            get { return "Properties"; }
        }

        public object Icon
        {
            get { return null; }
        }

#endregion
    }
}
