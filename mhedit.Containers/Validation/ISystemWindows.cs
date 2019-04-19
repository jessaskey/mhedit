using System.Windows.Forms;

namespace mhedit.Containers.Validation
{
    /// <summary>
    /// This is a shim for a real interface that we might want to add at some
    /// point... when we come up with a generic UX for the editor.
    /// </summary>
    public interface ISystemWindows
    {
        void Add( UserControl view );
    }
}