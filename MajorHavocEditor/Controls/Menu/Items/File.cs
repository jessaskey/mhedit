namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class File : MenuItem
    {
        public File()
            : base(FileMenuName)
        {
            this.Display = "_File";

            // try to force to left.
            this.SortOrder = int.MinValue;

            // make private group key.
            //this.GroupKey = new object();
        }
    }

}
