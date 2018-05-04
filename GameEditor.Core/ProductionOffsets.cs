using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core
{

    /// <summary>
    /// Notes:
    /// Not sure how to represent the complete data table yet.
    /// 
    /// I think MazeType refers to the 4 different sizes of mazes in MH. 
    /// </summary>
    struct MazeData
    {
        ReactoidData TheReactoid;
        /// List of bytes for each Pyroid terminated by the 0x0ff character.
        List<MovingOid> MovingOids;

    }

    /// <summary>
    /// All things have a Maze Location, This is a Hi Resolution maze
    /// position based upon Ushort x/y positions.
    /// </summary>
    struct HiRezMazePosition
    {
        short X; // in atari vector units
        short Y; // in atari vector units
    }

    /// <summary>
    /// All things have a Maze Location, This is a Low Resolution maze
    /// position based upon Ushort x/y positions.
    /// 
    /// A byte provides x/y as encoded position data. X is the LSNibble and Y is the
    /// MSNibble. Each x/y nibble is cast to a byte, incremented by 1 for x and negated
    /// for y (upper nibble ORd with 0xf0), and shifted into the LSNibble of the MSB of
    /// a word. (got that? The underscore shows => 0x0_00)
    /// 
    /// Right now the LSB values for the encoding are set to 0,0???
    /// </summary>
    struct LowRezMazePosition
    {
        short X; // in atari vector units
        short Y; // in atari vector units
    }

    /// <summary>
    /// Mazes are constructed on a very coarse grid system where you plug in Wall "sprites"
    /// into a specific cell based upon Column/Row. Not sure how this is represented in 
    /// the actual code because Jess has made adjustments to the raw data based upon Maze Type
    /// and then uses Remainder/Quotient (column/row) to determine x/y.
    /// 
    /// I think this might be using the LowRes position but is converted to a grid column/row
    /// to help with display in the program.
    /// </summary>
    struct WallCellPosition // ?? : LowRezMazePosition
    {
        short Column; //x
        short Row; //y
    }

    /// <summary>
    /// There is a Reactoid in every maze. It's odd that this has a hi rez position. Seems
    /// like it could get a way with a low res one.
    /// </summary>
    struct ReactoidData : HiRezMazePosition
    {
    }

    /// <summary>
    /// Moving Oids add velocity data. Note that if 0x70 > Velocity > 0x90 then it's moving
    /// in that dir and the following byte actually holds the initial Vel??
    /// </summary>
    struct MovingOid : HiRezMazePosition
    {
        byte XVelocity; // starting velocity for Oid.
        byte XVelocityIncrement; // how much to move each update for Oid??
        byte YVelocity; // starting velocity for Oid.
        byte YVelocityIncrement; // starting velocity for Oid.
    }

    /// <summary>
    /// According to the object loading code there must be at least ONE Pyroid
    /// in every maze. 
    /// </summary>
    struct PyroidData : MovingOid
    {
    }

    /// <summary>
    /// Perkoids are detailed after Pyroids
    /// </summary>
    struct PerkoidData : MovingOid
    {

    }
    
    /// <summary>
    /// 
    /// </summary>
    struct Oxoid : LowRezMazePosition
    {
        byte Value;
    }

    struct Lightning : LowRezMazePosition
    {
        // Defaults to true but 
        bool IsHorizontal;
    }

    struct Arrow : LowRezMazePosition
    {
        // Defaults to true but 
        MazeObjects.ArrowDirection Direction;
    }

    /// <summary>
    /// Maze walls are adjusted by the Maze Type.
    /// </summary>
    struct MazeWall : WallCellPosition
    {
        MazeWallType Type;
    }

    /// <summary>
    /// This is positioned less as a "Wall" and more like an "Oid"
    /// </summary>
    struct OneWayMazeWall : LowRezMazePosition
    {
        MazeObjects.OneWayDirection Direction;
    }

    /// <summary>
    /// These are listed as a pair with the Color first, followed by the Key and then
    /// the Locked door position. Need to think about how to make these 2 distinct objects
    /// that are linked but distinct maze objects. 
    /// </summary>
    struct Key : LowRezMazePosition
    {
        MazeObjects.ObjectColor Color;
        Lock Lock;
    }
    struct Lock : LowRezMazePosition
    {
        MazeObjects.ObjectColor Color;
        Key Key;
    }

    /// <summary>
    ///  Escape pods seem to have a fixed location in Maze type 1. So that limits the
    ///  things that creators can modify when it comes to maze design.
    /// </summary>
    struct EscapePod //: LowRezMazePosition
    {

    }

    /// <summary>
    /// Just position
    /// </summary>
    struct Clock : LowRezMazePosition
    { }

    /// <summary>
    /// Just Position
    /// </summary>
    struct Boots : LowRezMazePosition
    { }

    /// <summary>
    /// 
    /// </summary>
    struct Transporter : LowRezMazePosition
    {
        MazeObjects.OneWayDirection Direction;
        MazeObjects.ObjectColor Color;
    }

    /// <summary>
    /// THE ION CANNON ... lol
    /// How do we choose how many shots it makes?? IIRC the game has 2/3 shots..
    /// </summary>
    struct IonCannon : HiRezMazePosition
    {
        
    }

    /// <summary>
    /// This is linked to a Pyroid. The Pyroid is stored in low res coordinates but Pyroids
    /// are usually Hi Res Position. Only one Pyroid per trip pad?
    /// </summary>
    struct TripPad : LowRezMazePosition
    {
        PyroidData Pyroid;
    }

    /// <summary>
    /// I would have thought this would need a hi res position.
    /// </summary>
    struct Hand : LowRezMazePosition
    {

    }

    /// These offsets are from the base 0x8000 address I believe. Nope...
    /// Some of these fall into paged ROM. The upper chunk of paged ROM
    /// so there is no adjustment to the addrs.

    public enum ProductionOffsets
    {
        /// TODO:: find the following offsets:
        /// Maze type,
        /// Out Arrows,
        /// 
     
        /// <summary>
        ///  Array of bytes with size 12. Contains index into the MazeHintMessagePointerArray
        ///  which contains * to message. levels 1-12 ONLY.
        /// </summary>
        MazeHintMessageIndexArray = 0x93FE,

        /// <summary>
        /// Array of word *'s 12 elements long to Static Strings stored using custom encoding.
        /// Index for this array is stored via the MazeHintMessageIndexArray. This always filled with 
        /// 12 strings. I believe you can put in an empty string by using the custom termination
        /// character as the 1st one.
        /// </summary>
        MazeHintMessagePointerArray = 0xE48b,

        /// <summary>
        /// Array of bytes with size 16. Provides the Reactoid timer value. Every level has
        /// a Reactoid so it's always filled to capacity. 
        /// </summary>
        ReactoidTimerValueArray = 0x3355,

        /// <summary>
        /// Array of word *'s 16 elements long to Reactoid and Pyroid Data. Every level has a
        /// Reactoid so it's always filled with 16 pointers. The Pyroids list is after the 
        /// Reactoid and is terminated by a value of 0xFF/-1. 
        /// </summary>
        ReactoidAndPyroidInformationPointerArray = 0x2581,

        /// <summary>
        /// Array of word *'s 16 elements long to Oxoid Data. Data is a Null terminated list.
        /// </summary>
        OxoidInformationPointerArray = 0x25A9,

        /// <summary>
        /// Array of word *'s 16 elements long to Lightning Data. Data is a Null terminated list.
        /// </summary>
        LightningInformationPointerArray = 0x25D1,

        /// <summary>
        /// Array of word *'s 16 elements long to Arrow Data. Data is a Null terminated list.
        /// </summary>
        ArrowInformationPointerArray = 0x25F9,

        /// <summary>
        /// Array of word *'s 16 elements long to Wall Data. Data is a Null terminated list.
        /// </summary>
        WallInformationPointerArray = 0x2647,

        /// <summary>
        /// Array of word *'s 16 elements long to OneWay Wall Data. Data is a Null terminated list.
        /// </summary>
        OneWayWallInformationPointerArray = 0x2677,

        /// <summary>
        /// Array of word *'s 16 elements long to Lock/Key Data. Data is a Null terminated list.
        /// </summary>
        LockAndKeyInformationPointerArray = 0x26D1,

        /// <summary>
        /// Array of word *'s 16 elements long to Stalactite Data. Data is a Null terminated list.
        /// </summary>
        StalactiteInformationPointerArray = 0x26B3,

        /// <summary>
        /// Array of byte sized bool - Escape Pod Data with size 4??. Only mazes of
        /// type 1, looks like every 4th maze have a pod.
        /// </summary>
        EscapePodEnableArray = 0x32FF,

        /// <summary>
        /// Array of byte sized bool with size 16.
        /// </summary>
        ClockEnabledArray = 0x3290,

        /// <summary>
        /// Array of byte sized bool with size 16.
        /// </summary>
        BootsEnabledArray = ClockEnabledArray + 0x10,

        /// <summary>
        /// Array of word *'s 16 elements long to Transporter Data. Data is a Null terminated list.
        /// </summary>
        TransporterInformationPointerArray = 0x26F9,

        /// <summary>
        /// Array of byte sized offsets with size 16. If non-zero this value is an offset
        /// into a stream of word pointers to IonCannonInformation.
        /// See IonCannonInformationPointerStream.  Zero is not a valid offset. So the
        /// first pointer in the IonCannonInformationPointerStream is NULL and unused.
        /// </summary>
        IonCannonOffsetArray = 0x269F,

        /// <summary>
        /// Stream of word *'s to IonCannon Data. Each Cannon has a pointer, multiple cannons
        /// on a level are represented by multiple word pointers in a Null terminated list.
        /// The next pointer list would be after the NULL terminator of the previous.
        /// Obviously limited to a stream FF bytes long.
        /// </summary>
        IonCannonInformationPointerStream = 0x30B1,

        /// <summary>
        /// Array of word *'s 12 elements long to Trip Pad Data. Only Levels 5 and up.
        /// Trip Pads and the Trip Pyroids go together. Data is a Null terminated list.
        /// </summary>
        TripPadInformationPointerArray = 0x2627,

        /// <summary>
        /// Array of bytes of Trip Pad Pyroid data. Seems that there is only one Pyroid per trip pad.
        /// Trip Pads and the Trip Pyroid go together.
        /// </summary>
        TripPyroidInformationPointerArray = 0x2D36,

        /// <summary>
        /// Array of word *'s 10 elements long to Hand Data.. Only Levels 7 and up. 
        /// Data is a Null terminated list.
        /// </summary>
        TheHandInformationPointerArray = 0x2721,

    }
}
