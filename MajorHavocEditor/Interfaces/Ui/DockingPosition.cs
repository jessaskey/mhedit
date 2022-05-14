using System;

namespace MajorHavocEditor.Interfaces.Ui
{
    public enum DockingState
    {
        Unknown,
        Float,
        DockTopAutoHide,
        DockLeftAutoHide,
        DockBottomAutoHide,
        DockRightAutoHide,
        Document,
        DockTop,
        DockLeft,
        DockBottom,
        DockRight,
        Hidden,
    }

    [Flags]
    public enum DockingPosition
    {
        /// <summary>
        /// Not docked to the application main window, but floating.
        /// </summary>
        Float = 1,

        /// <summary>
        /// Docked to the Left border.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Docked to the Right border.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Docked to the Top border.
        /// </summary>
        Top = 8,

        /// <summary>
        /// Docked to the Bottom border.
        /// </summary>
        Bottom = 16,

        /// <summary>
        /// Docked to the "Document" position.
        /// </summary>
        Document = 32,
    }

}