using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajorHavocEditor.Interfaces.Ui
{

    /// <summary>
    /// A UI element to be displayed by the IWindowManager in the UI.
    /// </summary>
    public interface IUserInterface
    {
        /// <summary>
        /// The initial docking state for the VisualElement. Must be one of
        /// the values called out in DockingPositions
        /// </summary>
        DockingState DockingState { get; }

        /// <summary>
        /// The valid docking positions for the VisualElement.
        /// </summary>
        DockingPosition DockingPositions { get; }

        /// <summary>
        /// True if the VisualElement should be hidden. Only valid when
        /// DockingState is set to Top/Bottom/Left/Right. 
        /// </summary>
        bool HideOnClose { get; }

        /// <summary>
        /// The string title or caption that is to be placed on the
        /// container window/pane/page of the VisualElement.
        /// </summary>
        string Caption { get; }


        /// <summary>
        /// Object that represents the Icon that is displayed with the menuItem.
        /// Typically these would be Uri or string objects that point to resources.
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa970069.aspx"/>
        /// </summary>
        object Icon { get; }
    }
}
