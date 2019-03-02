using System.ComponentModel;

namespace mhedit.Containers
{
    public interface IName
    {
        string Name { get; set; }
    }

    public interface IFileInformation : IName
    {
        string FileName { get; set; }
    }
}
