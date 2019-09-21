using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// The Hand may be placed on any maze. If it is not disabled by the player, it will
    /// automatically turn off the reactoid if it is triggered.
    /// </summary>
    [Serializable]
    public class Hand : MazeObject
    {
        /// <summary>
        /// The Width of the hand in Atari Vector units. So not the entire "Hand" object
        /// but the actual drawn hand at the end of the X Accordions.
        /// </summary>
        public const int VectorWidth = 64;

        /// <summary>
        /// The maximum length of an Accordion in Atari Vector units. I.e. when it's
        /// totally stretched out.
        /// </summary>
        public const int AccordionMaxLength = 128;

        /// <summary>
        /// This value indicates that the Accordion should be stretched to it's fully
        /// expanded position (effectively a straight line). When an accordion is fully
        /// expanded it is AccordionMaxLength Atari Vector units long. Turns out this
        /// represents a 90 deg angle for full expansion.
        /// </summary>
        public const int AccordionFullExpansion = 64;

        public Hand()
            : base( Constants.MAXOBJECTS_HAND,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.hand_obj.png" ),
                    new Point( 0x3c, 0x01 ),
                    new Point( 24, 10 ) )
        {}

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                -32 : 32;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            throw new Exception("Hand must be serialized with Reactoid position passed.");
        }

        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();

            /// Hand expects Reactoid to determine its makeup.
            if (obj is Reactoid reactoid )
            {
                /// Get the position of the Hand and Reactoid in high resolution Atari Units.
                Point handPosition = DataConverter.ConvertPixelsToVector( this.Position );
                Point reactoidPosition = DataConverter.ConvertPixelsToVector( reactoid.Position );

                /// Get the X and Y displacements between them. Note: Hand MUST be up and to
                /// the left of the Reactoid or things go bad, really bad. There is a
                /// validation rule for this. As Y displacement is in a negative direction use
                /// Abs() to get the magnitude for these calculations.
                int xDisplacement = reactoidPosition.X - handPosition.X;
                int yDisplacement = Math.Abs( reactoidPosition.Y - handPosition.Y );

                /// Using the displacement determine a bounding square that contains the Hand
                /// when outstretched to disable the Reactoid. This is effectively the difference
                /// in Atari Major units (Maze Stamps). Always bump by 1 as 0 is invalid.
                byte xSize = (byte)( ( xDisplacement >> 8 ) + 1 );
                byte ySize = (byte)( ( yDisplacement >> 8 ) + 1 );

                /// To determine the # of Accordions in the X direction the displacement must
                /// be reduced for both the Reactoid and the Hand. Remove half the Reactoid width
                /// (it's centered on X) and the full width of the drawn hand. The leftover
                /// displacement is the total length of all the X Accordions.
                int xAccordionDisplacement = xDisplacement - ( reactoid.VectorWidth / 2 ) - Hand.VectorWidth;

                ///There aren't any adjustments to the displacement in the Y direction.
                int yAccordionDisplacement = yDisplacement;

                /// A single Accordion is made up of 2 diamonds, which at full expansion is
                /// AccordionMaxLength vector units long. Divide the total displacement by the
                /// AccordionMaxLength and add 1 so that there are enough to expand to the total
                /// distance. Zero is invalid.
                int xAccordions = ( xAccordionDisplacement / AccordionMaxLength ) + 1;
                int yAccordions = ( yAccordionDisplacement / AccordionMaxLength ) + 1;

                /// Accordion Expansion seems to be based upon the Sine (Trig func). I'm basing
                /// this off of 3 things:
                /// - I sampled outcome of the expansion values on vector length and found it
                ///   to be non-linear.
                /// - If I used a value of 64 it expanded to a straight line.
                /// - Any value over 64 would rebound off a straight line and "repeat" the
                ///   accordion behavior. Sorta like a spring would bounce.
                ///
                /// An educated guess:
                /// I theorized that the expansion value represented an angle, where 0 was fully
                /// collapsed and 64 was full expansion. This follows the behavior of the Sine
                /// from 0 - 0.5*PI (0-90 deg). So our "angle" range of 0-90 degrees must be
                /// mapped to 0-64 counts.
                /// 
                /// And since the Sine of the angle over this range is 0-1, it can represent a
                /// % of full expansion! So our goal is to work backwards to get an angle value
                /// for the ROM.
                ///
                /// First calculate the % expansion of each Accordion based upon total displacement
                /// and # of Accordions.
                double xAccordionPercentExpansion = (double)xAccordionDisplacement /
                                             (double)xAccordions / (double)AccordionMaxLength;

                double yAccordionPercentExpansion = (double)yAccordionDisplacement /
                                             (double)yAccordions / (double)AccordionMaxLength;

                /// Use Inverse Sine to get the angle represented by the percentage. This value is
                /// returned in radians. Divide by 0.5*PI to normalize (clamp value range to 0-1)
                /// and multiply by full scale to determine the value to pass to the ROMs.
                byte xAccordionExpansion = (byte) Math.Floor(
                    AccordionFullExpansion *
                    ( Math.Asin( xAccordionPercentExpansion ) / ( Math.PI * 0.5 ) ) );

                byte yAccordionExpansion = (byte) Math.Floor(
                    AccordionFullExpansion *
                    ( Math.Asin( yAccordionPercentExpansion ) / ( Math.PI * 0.5 ) ) );


                /// The last value to calculate is how far the Y Accordions drop down when the
                /// hand is "active" (hand waiting to deactivate the Reactoid). This value is
                /// also an angle (0-64) detailing how much to expand the yAccordions when in the
                /// active mode. Production ROMs kept this value at 11. This should be a really
                /// small value to keep the accordions scrunched up and not blocking the hallway
                /// but not totally collapsed.
                /// At small angles Sine gives us approx a 1 deg to 1 vector unit ratio. Therefore
                /// simply choose the max drop in Vector units and divide by the # of diamonds. 
                byte activeYExpansion = (byte)( ( ySize > 2 ? 64 : 44 ) / yAccordions / 2 );

                //if ( reactoid.Name == "testing" )
                //{
                //    Debug.WriteLine( $"Xdisp:{xDisplacement} Ac:{xAccordionDisplacement} Sz:{xSize}" );
                //    Debug.WriteLine( $"Ydisp:{yDisplacement} Ac:{yAccordionDisplacement} Sz:{ySize}" );
                //    Debug.WriteLine( $"X:{xAccordions} Ex:{xAccordionExpansion}" );
                //    Debug.WriteLine( $"Y:{yAccordions} Ex:{yAccordionExpansion}" );
                //}

                bytes.AddRange( DataConverter.PointToByteArrayShort( this.Position ) );

                bytes.AddRange( new[]
                                {
                                    (byte) xAccordions, (byte) yAccordions,
                                    xAccordionExpansion, activeYExpansion,
                                    yAccordionExpansion, xSize, ySize
                                } );
            }
            else
            {
                throw new ArgumentException( "Expected Reactoid object" , "obj");
            }

            return bytes.ToArray();
        }
    }
}
