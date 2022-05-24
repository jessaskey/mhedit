using System;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class RestoreLayout : MenuItem
    {
        public static object LayoutGroupKey = new object();

        public RestoreLayout()
            : base("Restore Layout")
        {
            this.Display = "_Restore Layout";

            this.ParentName = FileMenuName;

            this.GroupKey = LayoutGroupKey;

            this.Command = new MenuCommand(this.OnExecute);
        }

        public IWindowManager WindowManager { get; set; }

        private void OnExecute(object obj)
        {
            if (this.WindowManager != null)
            {
                //Microsoft.Win32.OpenFileDialog openFileDialog =
                //    new Microsoft.Win32.OpenFileDialog
                //    {
                //        FileName = "WindowLayout",
                //        DefaultExt = ".xml",
                //        Filter = "Xml documents (.xml)|*.xml"
                //    };

                //// Show save file dialog box
                //bool? result = openFileDialog.ShowDialog();

                //// Process save file dialog box results
                //if (result == true)
                //{
                //    this.WindowManager.RestoreLayout(openFileDialog.FileName);
                //}
            }
            else
            {
                throw new Exception("No Window Manager??");
            }
        }
    }
}
