using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// Escape pod may be placed on Type 2 mazes only. It allows the player an alternate 
    /// way to get out of the maze after touching the reactor.
    /// </summary>
    [Serializable]
    public class EscapePod : MazeObject
    {
        // Has fixed position in maze.
        private static Point _position = new Point( 1184, 352 );

        private EscapePodOption _option = EscapePodOption.Optional;

        public EscapePod()
            : base( 1,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.pod_obj.png" ),
                    new Point( 0x00, 0x80 ),
                    Point.Empty )
        { }

        [CategoryAttribute("Location")]
        [ReadOnly(true)]
        [DescriptionAttribute("The start location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set {  }
        }

        [DescriptionAttribute("Sets whether the player must use the escape pod to exit the maze or if they may also exit the maze through the main maze doors.")]
        public EscapePodOption Option
        {
            get { return _option; }
            set { this.SetField( ref this._option, value ); }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)_option);
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
