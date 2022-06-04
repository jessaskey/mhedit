using System;
using System.Windows.Forms;
using MajorHavocEditor;

namespace mhedit
{
    /// <summary>
    /// Temporary, hides Interface that is being removed.
    /// </summary>
    public interface ITreeObject { }
}

namespace MHavocEditor
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }
}
