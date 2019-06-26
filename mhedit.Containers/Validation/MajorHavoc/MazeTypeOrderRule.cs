using System.Collections.Generic;

namespace mhedit.Containers.Validation.MajorHavoc
{

    /// <summary>
    /// Validates that the Mazes in a MazeCollection are ordered by MazeType in the
    /// A-B-C-D repeating pattern that is expected by the code. Any individual Maze
    /// that fails this test is processed as an individual error (called once for each
    /// for each maze that fails).
    ///
    /// ValidationAttribute.Options (none)
    ///
    /// Composite format string: 
    ///     Maze object is Index 0
    ///     Default Message is Index 1
    ///     Expected MazeType
    /// </summary>
    public class MazeTypeOrderRule : ValidationRule<IEnumerable<Maze>>
    {
        private class ErrorInfo
        {
            public Maze Maze { get; set; }

            public MazeType ExpectedType { get; set; }
        }

        public MazeTypeOrderRule( ValidationData data )
            : base( data )
        {}

        public override IValidationResult Validate( IEnumerable<Maze> mazes )
        {
            List<ErrorInfo> errors = new List<ErrorInfo>();

            int mazeTypeIndex = 0;

            foreach ( Maze maze in mazes )
            {
                /// look for proper MazeType order in maze collection
                if ( maze.MazeType != (MazeType)( mazeTypeIndex & 0x03 ) )
                {
                    errors.Add( new ErrorInfo()
                    {
                        Maze = maze,
                        ExpectedType = (MazeType)( mazeTypeIndex & 0x03 )
                    } );
                }

                mazeTypeIndex++;
            }

            ValidationResults results = null;

            if ( errors.Count > 0 )
            {
                results = new ValidationResults() { Context = mazes };

                foreach ( ErrorInfo info in errors )
                {
                    results.Add(
                        this.CreateResult(
                            info.Maze,
                            $"MazeType error, {info.Maze.Name} not in expected A-B-C-D progression. Expected {info.ExpectedType}, Actual {info.Maze.MazeType}",
                            info.ExpectedType )
                        );
                }
            }

            return results;
        }
    }

}
