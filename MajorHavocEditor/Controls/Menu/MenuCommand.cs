using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace MajorHavocEditor.Controls.Menu
{

    /// <summary>
    /// Implements the basics of the ICommand interface for use with the IMenuManager.
    /// </summary>
    public class MenuCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public MenuCommand( Action<object> execute)
            : this( execute, _ => true )
        {
        }

        public MenuCommand( Action<object> execute, Func<object, bool> canExecute )
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

#region Implementation of ICommand

        public virtual bool CanExecute( object parameter )
        {
            try
            {
                return this._canExecute?.Invoke( parameter ) ?? true;
            }
            catch ( Exception e )
            {
                //BUG: Add a dialog box for error..
                Console.WriteLine( e );

                throw;
            }
        }

        public void Execute( object parameter )
        {
            Cursor original = Cursor.Current;

            try
            {
                // Use Cursors.Current to make immedate change to cursor.
                // <see cref="http://timl.net/2008/06/mouse-pointers-and-cursors.html"/>
                Cursor.Current = Cursors.WaitCursor;

                // Should I test or leave it up to the menu system?
                if ( this._canExecute( parameter ) )
                {
                    this._execute( parameter );
                }
            }
            catch ( Exception e )
            {
                //BUG: Add a dialog box for error..
                Console.WriteLine( e );

                throw;
            }
            finally
            {
                Cursor.Current = original;
            }
        }

        public event EventHandler CanExecuteChanged;

#endregion

        public void UpdateCanExecute()
        {
            this.OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke( this, EventArgs.Empty );
        }
    }

}
