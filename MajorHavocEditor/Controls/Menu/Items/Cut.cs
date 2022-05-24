namespace MajorHavocEditor.Controls.Menu.Items
{

    internal class Cut : MenuItem
    {
        public static object CutCopyPasteGroupKey = new object();

        public Cut()
            : base( "Cut" )
        {
            this.Display = "_Cut";

            this.ParentName = EditMenuName;

            // make private group key.
            this.GroupKey = CutCopyPasteGroupKey;
        }
    }

}
