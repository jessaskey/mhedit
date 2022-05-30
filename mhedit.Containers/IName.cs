using System.ComponentModel;

namespace mhedit.Containers
{
    public interface IName
    {
        string Name { get; set; }
    }

    public interface IFileProperties
    {
        /// <summary>
        /// Description of file type
        /// </summary>
        //string Description { get; }

        /// <summary>
        /// The file extension for this object.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// The Filename for this object, or null if it doesn't have one. 
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The path on disk for this object, or null if it doesn't have one. If the
        /// <see cref="Filename"/> is null then this represents the default storage location.
        /// </summary>
        string Path { get; set; }
    }

}
