using System;
using System.Collections.Generic;
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
        public static readonly string AboutMenuName = "About";

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

        /// <inheritdoc />
        public override string ToString()
        {
            return string.IsNullOrEmpty( this._parentName )
                       ? $"{this.Name} [Top Level]"
                       : $"{this.Name} [Parent: {this.ParentName}]";
        }

#endregion

#region Implementation of IMenuItem

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public object Display
        {
            get { return this._display; }
            set { this.SetField( ref this._display, value ); }
        }

        /// <inheritdoc />
        public object ToolTip { get; set; }

        /// <inheritdoc />
        public object Icon { get; set; }

        /// <inheritdoc />
        public bool IsChecked { get; set; }

        /// <inheritdoc />
        public object ShortcutKey { get; set; }

        /// <inheritdoc />
        public object GroupKey { get; set; }

        /// <inheritdoc />
        public int SortOrder { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> Options { get; set; }

        /// <inheritdoc />
        public ICommand Command { get; set; }

        /// <inheritdoc />
        public object CommandParameter { get; set; }

#endregion
    }

}
