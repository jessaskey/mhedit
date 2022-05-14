using System.Collections.Generic;

namespace MajorHavocEditor.Interfaces.Ui
{
    /// <summary>
    /// This interface is used to abstract out the Docking Window Manager.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Returns the window manager that implements this interface.
        /// </summary>
        object Manager { get; }

        /// <summary>
        /// User modifiable window manager settings.
        /// </summary>
        //UserSettings UserSettings { get; }

        /// <summary>
        /// Settings that are not meant to have user access but allow applications
        /// to change the window manager behaviour.
        /// </summary>
        //FunctionalSettings FunctionalSettings { get; }

        /// <summary>
        /// A collection of all of the !UserInterfaces currently being managed.
        /// </summary>
        IEnumerable<IUserInterface> Interfaces { get; }

        /// <summary>
        /// Add the following user interface to the active list of windows.
        /// </summary>
        /// <param name="userInterface">User interface element to add.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if userInterface is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an unrecognized
        /// user interface type is provided.</exception>
        void Add(IUserInterface userInterface);

        /// <summary>
        /// Remove the following user interface from the active list of windows being
        /// managed.
        /// </summary>
        /// <param name="userInterface">User interface element to remove.</param>
        /// <returns>True if the user interface was successfully removed.</returns>
        bool Remove(IUserInterface userInterface);

        /// <summary>
        /// Shows, or sets focus to the provided UserInterface. Adds the interface to the
        /// window manager if it isn't already.
        /// </summary>
        /// <param name="userInterface"></param>
        /// <exception cref="System.ArgumentNullException">Thrown if userInterface is null.</exception>
        void Show(IUserInterface userInterface);

        /// <summary>
        /// Hides the the provided UserInterface. Adds the interface to the
        /// window manager if it isn't already.
        /// </summary>
        /// <param name="userInterface"></param>
        /// <exception cref="System.ArgumentException">Thrown if userInterface isn't in the managers collection.</exception>
        void Hide(IUserInterface userInterface);

        /// <summary>
        /// Save the current UI layout to the active layout file.
        /// </summary>
        void SaveLayout();

        /// <summary>
        /// Save the current UI layout to the file specified. The provided file becomes the active layout file.
        /// </summary>
        /// <param name="path">The path parameter is permitted to specify relative or absolute path information. Relative
        /// path information is interpreted as relative to the current working directory.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if path is null.</exception>
        /// <exception cref="System.NotSupportedException">The path is in an invalid format.</exception>
        void SaveLayout(string path);

        /// <summary>
        /// Restore a previously saved UI Layout from the active layout file.
        /// </summary>
        void RestoreLayout();

        /// <summary>
        /// Restore a previously saved UI Layout from the active layout file. The provided file becomes the active layout file.
        /// </summary>
        /// <param name="path">The path parameter is permitted to specify relative or absolute path information. Relative
        /// path information is interpreted as relative to the current working directory.</param>
        /// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if path is null.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file specified in path was not found.</exception>
        void RestoreLayout(string path);

        /// <summary>
        /// Provides an implementation of a wizard for the current windowed environment. Or null
        /// if one doesn't exist.
        /// </summary>
        /// <returns>A wizard view or null if there is no implementaiton available.</returns>
        //IWizardView GetWizard();
    }
}
