using System;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class SaveLayout : MenuItem
    {
        public SaveLayout()
            : base("Save Layout")
        {
            this.Display = "_Save Layout";

            this.ParentName = FileMenuName;

            this.GroupKey = RestoreLayout.LayoutGroupKey;

            this.Icon = "save.png";

            this.Command = new MenuCommand(this.OnExecute);
        }

        public IWindowManager WindowManager { get; set; }

        private void OnExecute(object parameter)
        {

            if (this.WindowManager != null)
            {
                //Microsoft.Win32.SaveFileDialog saveFileDialog =
                //    new Microsoft.Win32.SaveFileDialog
                //    {
                //        FileName = "WindowLayout",
                //        DefaultExt = ".xml",
                //        Filter = "Xml documents (.xml)|*.xml"
                //    };

                //// Show save file dialog box
                //bool? result = saveFileDialog.ShowDialog();

                //// Process save file dialog box results
                //if (result == true)
                //{
                //    this.WindowManager.SaveLayout(saveFileDialog.FileName);
                //}
            }
            else
            {
                throw new Exception("No Window Manager??");
            }
        }
    }
}
