using System.Windows.Forms;

namespace MajorHavocEditor.Views
{
    /// <summary>
    /// Fixes that annoying scroll when you first click in a maze.
    /// <see cref="https://stackoverflow.com/a/9317532"/>
    /// </summary>
    public partial class CustomPanel : Panel
    {
        public CustomPanel()
        {
            this.InitializeComponent();
        }

        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }
    }
}
