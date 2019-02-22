using System.ComponentModel;

namespace mhedit.Containers
{
    public interface IName
    {
        string Name { get; set; }
    }

    public interface ISaveInformation : IName, ITrackEdits
    {
        string FileName { get; set; }
    }

    public interface ITrackEdits : INotifyPropertyChanged
    {
        bool IsDirty { get; set; }
    }
}
