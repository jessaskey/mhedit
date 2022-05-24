namespace MajorHavocEditor.Services
{

    public interface IFileProperties
    {
        //bool PromptToSave { get; } Just use changetracking

        /// <summary>
        /// Description of file type
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Filename for this object, or null if it doesn't have one. 
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// The file extension for this object.
        /// </summary>
        string Extension { get; set; }

        /// <summary>
        /// The path on disk for this object, or null if it doesn't have one. If the
        /// <see cref="Filename"/> is null then this represents the default storage location.
        /// </summary>
        string Path { get; set; }
    }

}