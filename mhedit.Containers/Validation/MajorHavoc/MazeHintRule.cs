namespace mhedit.Containers.Validation.MajorHavoc
{
    public class MazeHintRule : StringRegexRule
    {
        private const int MaxLength = 128;

        public MazeHintRule( ValidationData data )
            : base( data )
        {
            this._pattern = $"^[a-zA-Z0-9 .!-,%:]{{0,{MaxLength}}}$";

            this._data.Message =
                $"Maze Hint \"{{0}}\", contains invalid characters or is > {MaxLength} characters long. " +
                "Valid characters are \" 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:\"";
        }
    }
}
