namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class Copy : MenuItem
    {
        public Copy()
            : base("Copy")
        {
            this.Display = "_Copy";

            this.ParentName = EditMenuName;

            // try to force to left.
            this.SortOrder = int.MinValue + 1;

            // make private group key.
            this.GroupKey = Cut.CutCopyPasteGroupKey;
        }
    }
}
