using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MajorHavocEditor.Controls.Menu.Items;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public abstract class MenuManager : IMenuManager
    {
        private class CommandLink : IDisposable
        {
            public CommandLink(IMenuItem menuItem, ToolStripItem toolStripItem)
            {
                this.MenuItem = menuItem;

                this.ToolStripItem = toolStripItem;

                if (menuItem.Command != null)
                {
                    toolStripItem.Click += this.OnMenuItemClicked;

                    menuItem.Command.CanExecuteChanged += this.OnCanExecuteChanged;

                    this.OnCanExecuteChanged(menuItem.Command, EventArgs.Empty);
                }
            }

            public IMenuItem MenuItem { get; }

            public ToolStripItem ToolStripItem { get; }

            private void OnCanExecuteChanged(object sender, EventArgs e)
            {
                this.ToolStripItem.Enabled =
                    this.MenuItem.Command.CanExecute(this.MenuItem.CommandParameter);
            }

            private void OnMenuItemClicked(object sender, EventArgs e)
            {
                this.MenuItem.Command.Execute(this.MenuItem.CommandParameter);
            }

#region Implementation of IDisposable

            /// <inheritdoc />
            public void Dispose()
            {
                if (this.MenuItem.Command != null)
                {
                    this.ToolStripItem.Click -= this.OnMenuItemClicked;

                    this.MenuItem.Command.CanExecuteChanged -= this.OnCanExecuteChanged;
                }
            }

#endregion
        }

        /// <summary>
        /// Used when a parent is implied with <see cref="IMenuItem.ParentName"/>
        /// but at the time of reference one had not been explicitly added.
        /// Therefore, these always end up as a TopLevel menuItem.
        /// </summary>
        private class ImpliedParent : MenuItem
        {
            public ImpliedParent(string name)
                : base(name)
            {
            }
        }

        private readonly Exit _exit = new Exit();
        private readonly SaveLayout _saveLayout = new SaveLayout();
        private readonly RestoreLayout _restoreLayout = new RestoreLayout();
        private readonly Dictionary<IMenuItem, ToolStripItem> _menuItems =
            new Dictionary<IMenuItem, ToolStripItem>();

        private readonly ToolStrip _toolStrip;

        protected MenuManager(ToolStrip toolStrip)
        {
            this._toolStrip = toolStrip;
        }
        
#region Implementation of IMenuManager

        public object Menu
        {
            get { return this._toolStrip; }
        }

        public void Add(IMenuItem itemToAdd)
        {
            /// Only for MenuStrip?
            //if ( string.IsNullOrEmpty( itemToAdd.ParentName ) && itemToAdd.Command != null )
            //{
            //    throw new InvalidOperationException(
            //        $"Command as a top level menu item: {itemToAdd}" );
            //}

            if (string.IsNullOrEmpty(itemToAdd.Name))
            {
                throw new ArgumentException(
                    nameof(itemToAdd.Name), "MenuItem requires Name");
            }

            if (itemToAdd.Display == null)
            {
                throw new ArgumentNullException(
                    nameof(itemToAdd.Display), "MenuItem requires Display");
            }

            // Menu items are all about executing a command. If no command is provided
            // then must be adding hierarchy to the menu system. If this hierarchy does
            // exist then just ignore the add.
            //if ( itemToAdd.Command == null )
            //{
            //    this.AddMenuHierarchy( itemToAdd );
            //}
            //else // Adding a command
            {
                this.AddMenuCommand(itemToAdd);
            }
        }

        public void Add(IEnumerable<IMenuItem> menuItems)
        {
            foreach (IMenuItem item in menuItems)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Attempt to remove a menu item from the menu tree. As long as the item is at the
        /// bottom of a tree (has no children/isn't a submenu) it will be removed. If the item
        /// removed is the last child of a parent menu item the parent is removed as well.
        /// </summary>
        /// <param name="menuItem"></param>
        public void Remove(IMenuItem menuItem)
        {
            //if ( this._menuItems.Contains( menuItem )

            //    //&&
            //    //this._menuItems.GetChildItems(menuItem.Name).IsEmpty
            //    )
            {
                this.RemoveFromMenu(menuItem);

                string parentName = menuItem.ParentName;

                // Remove hierarchy if this was the only command at that level.
                while (!string.IsNullOrEmpty(parentName)

                    //   &&
                    //this._menuItems.GetChildItems(parentName).IsEmpty
                    )
                {
                    foreach (IMenuItem item in this._menuItems.Keys)
                    {
                        if (item.Name == parentName)
                        {
                            this.RemoveFromMenu(item);

                            parentName = item.ParentName;

                            break;
                        }
                    }
                }
            }
        }

        public void Remove(IEnumerable<IMenuItem> menuItems)
        {
            foreach (IMenuItem item in menuItems)
            {
                this.Remove(item);
            }
        }

        #endregion

        private void AddMenuHierarchy(IMenuItem itemToAdd)
        {
            IMenuItem existing = null;

            foreach (IMenuItem item in this._menuItems.Keys)
            {
                // BUG? This tests all menus for individuality!
                if (itemToAdd.Name == item.Name)
                {
                    // Throw if someone is expecting a subMenu to be located at this
                    // menu command.
                    if (item.Command != null)
                    {
                        throw new InvalidOperationException(
                            $"Menu command collision with Hierarchy: {itemToAdd}");
                    }

                    existing = item;
                }

                ThrowOnDuplicate(itemToAdd, item);
            }

            // Add hierarchy if nothing found.
            if (existing == null)
            {
                this.AddToMenu(itemToAdd, this.CreateMenuItem(itemToAdd));
            }
            // If parent was implied then remove and replace.
            else if (existing is ImpliedParent)
            {
                //this.MoveChildren(
                //    this.RemoveFromMenu(existing), this.CreateMenuItem(itemToAdd) );
            }
            //else //Do nothing, as the hierarchy already exists.
        }

        private void AddMenuCommand(IMenuItem itemToAdd)
        {
            IMenuItem parent = this._menuItems.Keys.FirstOrDefault(
                k => k.Name.Equals(itemToAdd.ParentName));

            ToolStripItem stripItemToAdd = this.CreateMenuItem(itemToAdd);

            if (parent == null)
            {
                if (string.IsNullOrWhiteSpace(itemToAdd.ParentName))
                {
                    // add at top level
                    this._toolStrip.Items.Add(stripItemToAdd);
                }
                else
                {
                    parent = new ImpliedParent(itemToAdd.ParentName);

                    this.AddMenuCommand(parent);

                    ((ToolStripDropDownItem)this._menuItems[parent])
                        .DropDownItems.Add(stripItemToAdd);
                }
            }
            else if (this._menuItems.TryGetValue(parent, out ToolStripItem parentStripItem))
            {
                // Any parent automatically supports children!
                ((ToolStripDropDownItem)parentStripItem).DropDownItems.Add(stripItemToAdd);
            }

            this.AddToMenu(itemToAdd, stripItemToAdd);
        }

        private void AddToMenu(IMenuItem itemToAdd, ToolStripItem stripItemToAdd)
        {
            stripItemToAdd.Tag = new CommandLink( itemToAdd, stripItemToAdd );

            this._menuItems.Add( itemToAdd, stripItemToAdd);
        }

        private static void ThrowOnDuplicate(IMenuItem itemToAdd, IMenuItem existing)
        {
            // Make sure items at the same level don't share identical names.
            if (itemToAdd.ParentName == existing.ParentName)
            {
                if (itemToAdd.Name == existing.Name)
                {
                    throw new InvalidOperationException(
                        $"Menu item {itemToAdd} already exists.");
                }

                if (itemToAdd.Display.Equals(existing.Display))
                {
                    throw new InvalidOperationException(
                        $"{(itemToAdd.ParentName == string.Empty ? "Top Level" : itemToAdd.ParentName)} already has item displaying {itemToAdd.Display}.");
                }
            }
        }

        protected abstract ToolStripItem CreateMenuItem(IMenuItem menuItem);

        private ToolStripItem RemoveFromMenu(IMenuItem key)
        {
            if (this._menuItems.TryGetValue(key, out ToolStripItem item))
            {
                if ( item.Tag is IDisposable disposable )
                {
                    disposable.Dispose();
                }

                // BUG? Does this traverse hierarchy?
                this._toolStrip.Items.Remove(item);

                this._menuItems.Remove(key);
            }

            return item;
        }

        private void AddBasicMenus()
        {
            this.Add(new File());
            this.Add(this._saveLayout);
            this.Add(this._restoreLayout);
            this.Add(new Separator(this._exit, this._exit.GroupKey));
            this.Add(this._exit);
            this.Add(new About());
            this.Add(new Cut());
            this.Add(new Copy());
            this.Add(new Paste());
        }
    }

}