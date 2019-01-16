using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    /// <summary>
    /// Describes the different wall types available.
    /// </summary>
    [Serializable]
    public enum MazeWallType : int
    {
        /// <summary>
        /// Horizontal Wall Type
        /// </summary>
        Horizontal = 1,
        /// <summary>
        /// Left Down Wall Type
        /// </summary>
        LeftDown,
        /// <summary>
        /// Left Up Wall Type
        /// </summary>
        LeftUp,
        /// <summary>
        /// Right Up Wall Type
        /// </summary>
        RightUp,
        /// <summary>
        /// Right Down Wall Type
        /// </summary>
        RightDown,
        /// <summary>
        /// Vertical Wall Type
        /// </summary>
        Vertical,
        /// <summary>
        /// Empty Wall Type 
        /// </summary>
        Empty
    }

    /// <summary>
    /// MazeType defines the 4 basic mazes found in Major Havoc. The base dimensions and walls are different
    /// for each MazeType
    /// </summary>
    [Serializable]
    public enum MazeType : int
    {
        /// <summary>
        /// Type A Mazes are on levels 1,5,9,13,etc. This maze area is the smallest of the four
        /// and has a bounding grid of 16x7 stamps.
        /// </summary>
        TypeA = 0,
        /// <summary>
        /// Type B Mazes are on levels 2,6,10,14,etc. This maze area has a bounding grid of 21x8 stamps.
        /// </summary>
        TypeB = 1,
        /// <summary>
        /// Type C Mazes are on levels 3,7,11,15,etc. This maze area has a bounding grid of 21x9 stamps.
        /// </summary>
        TypeC = 2,
        /// <summary>
        /// Type D Mazes are on levels 4,8,12,16,etc. This maze area has a bounding grid of 19x11 stamps.
        /// </summary>
        TypeD = 3
    }

    [Serializable]
    public enum CannonMovementType : int
    {
        Return,
        Position,
        Move,
        Pause
    }

    [Serializable]
    public enum CannonGunPosition : int
    {
        TopRight = 0,
        MiddleRight,
        BottomRight,
        Down,
        TopLeft,
        MiddleLeft,
        BottomLeft
    }

    [Serializable]
    public enum CannonGunSpeed : int
    {
        Slow = 0,
        Medium,
        Fast
    }

    /// <summary>
    /// ArrowDirection defines which way the current arrow is pointing. Unknown will display a question mark.
    /// </summary>
    [Serializable]
    public enum ArrowDirection : int
    {
        /// <summary>
        /// Arrow pointing Right
        /// </summary>
        Right = 0,
        /// <summary>
        /// Arrow pointing Left
        /// </summary>
        Left,
        /// <summary>
        /// Arrow pointing Up
        /// </summary>
        Up,
        /// <summary>
        /// Arrow pointing Down
        /// </summary>
        Down,
        /// <summary>
        /// Arrow pointing Up and to the Right
        /// </summary>
        UpRight,
        /// <summary>
        /// Arrow pointing Down and to the Left
        /// </summary>
        DownLeft,
        /// <summary>
        /// Arrow pointing Up and to the Left
        /// </summary>
        UpLeft,
        /// <summary>
        /// Arrow pointing Down and to the Right
        /// </summary>
        DownRight,
        /// <summary>
        /// A question mark
        /// </summary>
        Question
    }

    /// <summary>
    /// ArrowDirection defines which way the current arrow is pointing. Unknown will display a question mark.
    /// </summary>
    [Serializable]
    public enum ArrowOutDirection : int
    {
        /// <summary>
        /// Out Arrow pointing Right
        /// </summary>
        Right = 0,
        /// <summary>
        /// Out Arrow pointing Left
        /// </summary>
        Left,
        /// <summary>
        /// Out Arrow pointing Up
        /// </summary>
        Up,
        /// <summary>
        /// Out Arrow pointing Down
        /// </summary>
        Down
    }

    [Serializable]
    public enum TransporterDirection : int
    {
        /// <summary>
        /// Open to the right
        /// </summary>
        Right = 0,
        /// <summary>
        /// Open to the left
        /// </summary>
        Left
    }

    /// <summary>
    /// Oxoid type defines whether an oxoid point value is fixed or increases exponentially
    /// </summary>
    [Serializable]
    public enum OxoidType
    {
        /// <summary>
        /// Fixed oxoids award 500 points
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// Increasing oxoid points start at 200 and increase to 400, 600, 800, 1000, 1200
        /// </summary>
        Increasing
    }

    [Serializable]
    public enum MaxSpeed : int
    {
        Slowest = 0,
        Slow,
        Medium,
        Agressive
    }

    [Serializable]
    public enum PyroidStyle : int
    {
        Double = 0,
        Single = 1
    }

    /// <summary>
    /// The EscapePodOption defines whether a player *must* use the escape pod to leave the maze
    /// or whether they may use the normal maze exits as well.
    /// </summary>
    [Serializable]
    public enum EscapePodOption
    {
        /// <summary>
        /// If Optional is selected, then the player may optionally use the Escape Pod to leave the 
        /// maze.
        /// </summary>
        Optional = 1,
        /// <summary>
        /// If Required is selected, then the maze doors do not open when the reactoid is triggered 
        /// and the player must use the escape pod to leave the maze safely.
        /// </summary>
        Required = 2
    }

    /// <summary>
    /// The direction that a OneWay allows the player to pass.
    /// </summary>
    [Serializable]
    public enum OneWayDirection : int
    {
        /// <summary>
        /// One Way Arrows allowing travel left to right
        /// </summary>
        Right = 0,
        /// <summary>
        /// One Way Arrows allowing travel right to left
        /// </summary>
        Left
    }
}
