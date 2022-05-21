using System;
using System.Windows.Input;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public class MenuItem : NotifyPropertyChangedBase, IMenuItem
    {
        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the File Menu.
        /// </summary>
        public static readonly string FileMenuName = "File";

        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the File Menu.
        /// </summary>
        public static readonly string EditMenuName = "Edit";

        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the View Menu.
        /// </summary>
        public static readonly string ViewMenuName = "View";

        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the Windows Menu.
        /// </summary>
        public static readonly string WindowsMenuName = "Windows";

        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the Help Menu.
        /// </summary>
        public static readonly string HelpMenuName = "Help";

        /// <summary>
        /// Assign this value to an IMenuItem.ParentName to add to the File Menu.
        /// </summary>
        public static readonly string DebugMenuName = "Debug";

        private string _name;
        private string _parentName = string.Empty;
        private object _display;

        public MenuItem( string name )
        {
            this.Name = name;
        }

#region Overrides of Object

        public override string ToString()
        {
            return string.IsNullOrEmpty( this._parentName )
                       ? string.Format( "{0} [Top Level]", this.Name )
                       : string.Format( "{0} [Parent:{1}]", this.Name, this.ParentName );
        }

#endregion

#region Implementation of IMenuItem

        public string Name
        {
            get { return this._name; }
            private set
            {
                if ( value == null )
                {
                    throw new ArgumentNullException( "value",
                        "Must provide a name for the MenuItem." );
                }

                if ( this._parentName.Equals( value ) )
                {
                    throw new ArgumentException( "Name can't equal ParentName.", value );
                }

                this._name = value;
            }
        }

        public string ParentName
        {
            get { return this._parentName; }
            set
            {
                if ( value != null && this._name.Equals( value ) )
                {
                    throw new ArgumentException( "ParentName can't equal Name.", "value" );
                }

                this._parentName = value;
            }
        }

        public object Display
        {
            get { return this._display; }
            set { this.SetField( ref this._display, value ); }
        }

        /// <inheritdoc />
        public string ToolTipText { get; set; }

        public object Icon { get; set; }

        public bool IsChecked { get; set; }

        public object GroupKey { get; set; }

        public int SortOrder { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

#endregion
    }

}
