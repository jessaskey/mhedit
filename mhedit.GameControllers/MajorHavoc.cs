﻿using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.MazeObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.GameControllers
{
    public class MajorHavoc : GameController, IGameController
    {
        private static string Production = "Major Havoc Production";
        private static string ReturnToVax = "Major Havoc: Return to Vax";

        #region Private Variables

        private Dictionary<string, ushort> _exports = new Dictionary<string, ushort>();
        private string _sourceRomPath = String.Empty;
        private byte[] _alphaHigh = new byte[0x4000];
        private byte[] _alphaLow = new byte[0x4000];
        private byte[] _page01 = new byte[0x4000];
        private string _alphaHighROM = "136025.217";
        private string _alphaLowROM = "136025.216";
        private string _page01ROM = "136025.215";
        private string _lastError = String.Empty;
        private bool _isReturnToVaxx = false;
        private int _numberOfLevels = 20;
        private readonly string _name;

        #endregion

        public MajorHavoc()
            : this( false )
        { }

        public MajorHavoc(bool isReturnToVaxx)
            : this( isReturnToVaxx, isReturnToVaxx ? ReturnToVax : Production )
        { }

        public MajorHavoc( bool isReturnToVaxx, string name )
        {
            _isReturnToVaxx = isReturnToVaxx;
            if ( _isReturnToVaxx )
            {
                _numberOfLevels = 24;
            }

            this._name = name;

            InitializeExports();
        }

        public string Name
        {
            get { return this._name; }
            set { }
        }

        public string LastError
        {
            get { return _lastError; }
        }

        private void InitializeExports()
        {
            //these are stored internally since we dont want to have a file out there that people can use for DASM of the original ROMs
            _exports.Add("mzinit", 0x2581);
            _exports.Add("mpod", 0x32FF);
            _exports.Add("trtbl", 0x2D36);

            if (_isReturnToVaxx)
            {
                //Return to Vaxx
                _exports.Add("mazehint", 0xF7C0);
                _exports.Add("mztdal", 0x26F9);
                _exports.Add("mztd", 0x2617);
                _exports.Add("outime", 0x2681);
                _exports.Add("mzdc", 0x2699);
                _exports.Add("mzar", 0x26C9);
                _exports.Add("mztr", 0x25ED);
                _exports.Add("mzlg", 0x25B3);
                _exports.Add("mone", 0x3C4A);
                _exports.Add("mtite", 0x3C7A);
                _exports.Add("mlock", 0x264F);
                _exports.Add("mtran", 0x3CA0);
                _exports.Add("mhand", 0x3CD0);
                _exports.Add("mclock", 0x3CF4);
                _exports.Add("mboots", 0x3D14);
                _exports.Add("mcanst", 0x3D2C);
                _exports.Add("mzor", 0x25E7);
                _exports.Add("mcan", 0x2637);
            }
            else
            {
                //Atari ROMs
                _exports.Add("mazehint", 0x93FE);
                _exports.Add("mztdal", 0x2647);
                _exports.Add("mztd", 0x2667);
                _exports.Add("outime", 0x3355);
                _exports.Add("mzdc", 0x25A9);
                _exports.Add("mzar", 0x25F9);
                _exports.Add("mztr", 0x2627);
                _exports.Add("mzlg", 0x25D1);
                _exports.Add("mone", 0x2677);
                _exports.Add("mtite", 0x26B3);
                _exports.Add("mlock", 0x26D1);
                _exports.Add("mtran", 0x26F9);
                _exports.Add("mhand", 0x2721);
                _exports.Add("mclock", 0x3290);
                _exports.Add("mboots", 0x32A0);
                _exports.Add("mcanst", 0x30B1);
                _exports.Add("mzor", 0x2621);
                _exports.Add("mcan", 0x269F);
            }
        }

        public bool LoadTemplate(string sourceRomPath)
        {
            bool success = false;
            try
            {
                _sourceRomPath = sourceRomPath;

                if (_isReturnToVaxx)
                {
                    _alphaHighROM = "136025.917";
                    _alphaLowROM = "136025.916";
                    _page01ROM = "136025.915";
                }

                //load up our roms for now...
                string alphaHighFileName = Path.Combine(sourceRomPath, _alphaHighROM);
                string alphaLowFileName = Path.Combine(sourceRomPath, _alphaLowROM);
                string page01FileName = Path.Combine(sourceRomPath, _page01ROM);

                try
                {
                    _alphaHigh = File.ReadAllBytes(alphaHighFileName);
                }
                catch (Exception Exception)
                {
                    throw new Exception("ROM Load Error - Alpha High: " + Exception.Message);
                }

                try
                {
                    _alphaLow = File.ReadAllBytes(alphaLowFileName);
                }
                catch (Exception Exception)
                {
                    throw new Exception("ROM Load Error - Alpha Low: " + Exception.Message);
                }

                try
                {
                    _page01 = File.ReadAllBytes(page01FileName);
                }
                catch (Exception Exception)
                {
                    throw new Exception("ROM Load Error - Page0/1: " + Exception.Message);
                }
                success = true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
            }
            return success;
        }

        private int FixTopLimit(int level)
        {
            if (!_isReturnToVaxx)
            {
                if (level >= 16)
                {
                    return level - 4;
                }
            }
            return level;
        }

        public MazeCollection LoadMazes(List<string> loadMessages)
        {
            
            MazeCollection mazeCollection = new MazeCollection( this.Name );
            mazeCollection.AuthorEmail =  this._isReturnToVaxx ? "Jess@maynard.vax": "owen@maynard.vax";
            mazeCollection.AuthorName = this._isReturnToVaxx ? "Jess Askey" : "Owen Rubin";

            for ( int i = 0; i < _numberOfLevels; i++)
            {

                byte mazeType = (byte)(i & 0x03);
                Maze maze = new Maze((MazeType)mazeType, "Level " + (i + 1).ToString());
                
                //hint text - only first 12 mazes tho
                if (i < 12)
                {
                    byte messageIndex = ReadByte(_exports["mazehint"], i);
                    maze.Hint = GetMessage(messageIndex);
                }
                //build reactor
                ushort mazeInitIndex = ReadWord(_exports["mzinit"], i * 2);
                Reactoid reactor = new Reactoid();
                reactor.LoadPosition(ReadBytes(mazeInitIndex, 4));
                mazeInitIndex += 4;
                int timer = FromDecimal((int)ReadByte(_exports["outime"], FixTopLimit(i)));
                reactor.Timer = timer;
                maze.AddObject(reactor);

                //pyroids
                byte firstValue = ReadByte(mazeInitIndex, 0);
                while (firstValue != 0xff)
                {
                    Pyroid pyroid = new Pyroid();
                    pyroid.LoadPosition(ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(pyroid.Velocity, pyroid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(pyroid);
                    firstValue = ReadByte(mazeInitIndex, 0);

                    if (firstValue == 0xfe)
                    {
                        mazeInitIndex++;
                        //perkoids now...
                        break;
                    }
                }

                // Perkoids 
                while (firstValue != 0xff)
                {
                    Perkoid perkoid = new Perkoid();
                    perkoid.LoadPosition(ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(perkoid.Velocity, perkoid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(perkoid);
                    firstValue = ReadByte(mazeInitIndex, 0);
                }

                //do oxygens now
                ushort oxygenBaseAddress = ReadWord(_exports["mzdc"], i * 2);

                byte oxoidValue = ReadByte(oxygenBaseAddress, 0);
                while (oxoidValue != 0x00)
                {
                    Oxoid oxoid = new Oxoid();
                    oxoid.LoadPosition(oxoidValue);
                    maze.AddObject(oxoid);

                    oxygenBaseAddress++;
                    oxoidValue = ReadByte(oxygenBaseAddress, 0);
                }

                //do lightning (Force Fields)
                ushort lightningBaseAddress = ReadWord(_exports["mzlg"], i * 2);

                byte lightningValue = ReadByte(lightningBaseAddress, 0);
                bool isHorizontal = true;

                if (lightningValue == 0xff)
                {
                    isHorizontal = false;
                    lightningBaseAddress++;
                    lightningValue = ReadByte(lightningBaseAddress, 0);
                }

                while (lightningValue != 0x00)
                {
                    if (isHorizontal)
                    {
                        LightningH lightningh = new LightningH();
                        lightningh.LoadPosition(lightningValue);
                        maze.AddObject(lightningh);
                    }
                    else
                    {
                        LightningV lightningv = new LightningV();
                        lightningv.LoadPosition(lightningValue);
                        maze.AddObject(lightningv);

                    }

                    lightningBaseAddress++;
                    lightningValue = ReadByte(lightningBaseAddress, 0);
                    if (lightningValue == 0xff)
                    {
                        isHorizontal = false;
                        lightningBaseAddress++;
                    }
                    lightningValue = ReadByte(lightningBaseAddress, 0);
                }

                //build arrows now
                ushort arrowBaseAddress = ReadWord(_exports["mzar"], i * 2);
                byte arrowValue = ReadByte(arrowBaseAddress, 0);

                while (arrowValue != 0x00)
                {
                    Arrow arrow = new Arrow();
                    arrow.LoadPosition(arrowValue);
                    arrowBaseAddress++;
                    arrowValue = ReadByte(arrowBaseAddress, 0);
                    arrow.ArrowDirection = (Containers.ArrowDirection)arrowValue;
                    maze.AddObject(arrow);
                    arrowBaseAddress++;
                    arrowValue = ReadByte(arrowBaseAddress, 0);
                }

                //build output arrows now, levels 1 -3 only.
                if (i < 3)
                {
                    ushort outputArrowBaseAddress = ReadWord(_exports["mzor"], i * 2);
                    if (outputArrowBaseAddress != 0)
                    {
                        byte outputArrowValue = ReadByte(outputArrowBaseAddress, 0);

                        while (outputArrowValue != 0x00)
                        {
                            ArrowOut arrow = new ArrowOut();
                            arrow.LoadPosition(outputArrowValue);
                            outputArrowBaseAddress++;
                            outputArrowValue = ReadByte(outputArrowBaseAddress, 0);
                            arrow.ArrowDirection = (ArrowOutDirection)outputArrowValue - 9;
                            maze.AddObject(arrow);
                            outputArrowBaseAddress++;
                            outputArrowValue = ReadByte(outputArrowBaseAddress, 0);
                        }
                    }
                }

                //maze walls
                //static first
                ushort wallBaseAddress = ReadWord(_exports["mztdal"], FixTopLimit(i) * 2);
                byte wallValue = ReadByte(wallBaseAddress, 0);

                while (wallValue != 0x00)
                {
                    int relativeWallValue = GetRelativeWallIndex(maze.MazeType, wallValue);
                    Point stampPoint = maze.PointFromStamp(relativeWallValue);
                    wallBaseAddress++;
                    wallValue = ReadByte(wallBaseAddress, 0);
                    MazeWall wall = new MazeWall((MazeWallType)(wallValue & 0x07), stampPoint, relativeWallValue);
                    wall.UserWall = true;
                    maze.AddObject(wall);
                    wallBaseAddress++;
                    wallValue = ReadByte(wallBaseAddress, 0);
                }

                //then dynamic walls
                //TODO: Dyanamic Walls loading from ROM
                //only level 9 and higher
                if (i > 7)
                {
                    ushort dynamicWallBaseAddress = ReadWord(_exports["mztd"], (FixTopLimit(i) - 8) * 2);
                    byte dynamicWallIndex = ReadByte(dynamicWallBaseAddress, 0);
                    while (dynamicWallIndex != 0x00)
                    {
                        int relativeWallIndex = GetRelativeWallIndex(maze.MazeType, dynamicWallIndex);
                        int baseTimer = ReadByte(dynamicWallBaseAddress, 1);
                        int altTimer = ReadByte(dynamicWallBaseAddress, 2);
                        MazeWallType baseType = (MazeWallType)ReadByte(dynamicWallBaseAddress, 3);
                        MazeWallType altType = (MazeWallType)ReadByte(dynamicWallBaseAddress, 4);
                        MazeWall wall = maze.MazeObjects.Where(o => o.GetType() == typeof(MazeWall) && ((MazeWall)o).WallIndex == relativeWallIndex).FirstOrDefault() as MazeWall;
                        if (wall == null)
                        {
                            //make a new one
                            wall = new MazeWall(baseType, maze.PointFromStamp(relativeWallIndex), relativeWallIndex);
                            wall.DynamicWallTimout = baseTimer;
                            wall.AlternateWallTimeout = altTimer;
                            wall.WallType = baseType;
                            wall.AlternateWallType = altType;
                            wall.IsDynamicWall = true;
                            maze.AddObject(wall);
                        }
                        else
                        {
                            //we found a wall at this position, fill in the details...
                            wall.DynamicWallTimout = baseTimer;
                            wall.AlternateWallTimeout = altTimer;
                            wall.WallType = baseType;
                            wall.AlternateWallType = altType;
                            wall.IsDynamicWall = true;
                        }
                        dynamicWallBaseAddress += 5;
                        dynamicWallIndex = ReadByte(dynamicWallBaseAddress, 0);
                    }
                }

                //one way walls
                ushort onewayBaseAddress = ReadWord(_exports["mone"], i * 2);
                byte onewayValue = ReadByte(onewayBaseAddress, 0);

                while (onewayValue != 0x00)
                {
                    OneWay oneway = new OneWay();
                    if (onewayValue == 0xff)
                    {
                        oneway.Direction = OneWayDirection.Left;
                        onewayBaseAddress++;
                        onewayValue = ReadByte(onewayBaseAddress, 0);
                    }
                    else
                    {
                        oneway.Direction = OneWayDirection.Right;
                    }
                    oneway.LoadPosition(onewayValue);
                    maze.AddObject(oneway);

                    onewayBaseAddress++;
                    onewayValue = ReadByte(onewayBaseAddress, 0);
                }

                // Stalactite Level 5 and up
                if (i > 4)
                {
                    ushort stalactiteBaseAddress = ReadWord(_exports["mtite"], (i - 5) * 2);
                    byte stalactiteValue = ReadByte(stalactiteBaseAddress, 0);

                    while (stalactiteValue != 0x00)
                    {
                        Spikes spikes = new Spikes();
                        spikes.LoadPosition(stalactiteValue);
                        maze.AddObject(spikes);

                        stalactiteBaseAddress++;
                        stalactiteValue = ReadByte(stalactiteBaseAddress, 0);
                    }
                }

                //locks and keys
                ushort lockBaseAddress = ReadWord(_exports["mlock"], i * 2);
                byte lockValue = ReadByte(lockBaseAddress, 0);

                while (lockValue != 0x00)
                {
                    byte lockColor = lockValue;
                    lockBaseAddress++;

                    Key key = new Key();
                    key.LoadPosition(ReadByte(lockBaseAddress, 0));
                    key.KeyColor = (ObjectColor)lockColor;
                    maze.AddObject(key);

                    lockBaseAddress++;

                    Lock keylock = new Lock();
                    keylock.LoadPosition(ReadByte(lockBaseAddress, 0));
                    keylock.LockColor = (ObjectColor)lockColor;
                    maze.AddObject(keylock);

                    lockBaseAddress++;
                    lockValue = ReadByte(lockBaseAddress, 0);
                }

                //Escape pod
                // Levels 2,6,10,14 only
                if (mazeType == 0x01)
                {
                    ushort podBaseAddress = _exports["mpod"];
                    byte podValue = ReadByte(podBaseAddress, i >> 2);
                    if (podValue > 0)
                    {
                        EscapePod pod = new EscapePod();
                        if ((podValue & 0x02) > 0)
                        {
                            pod.Option = EscapePodOption.Required;
                        }
                        maze.AddObject(pod);
                    }
                }

                //clock & boots
                byte clockData = ReadByte(_exports["mclock"], FixTopLimit(i) );
                byte bootsData = ReadByte(_exports["mboots"], FixTopLimit(i) );

                if (clockData != 0)
                {
                    Clock clock = new Clock();
                    clock.LoadPosition(clockData);
                    maze.AddObject(clock);
                }

                if (bootsData != 0)
                {
                    Boots boots = new Boots();
                    boots.LoadPosition(bootsData);
                    maze.AddObject(boots);
                }

                //Laser IonCannon
                byte cannonAddressOffset = ReadByte(_exports["mcan"], i);
                if (cannonAddressOffset != 0)
                {
                    ushort cannonBaseAddress = (ushort)(_exports["mcanst"] + cannonAddressOffset);
                    ushort cannonPointerAddress = ReadWord(cannonBaseAddress, 0);

                    while (cannonPointerAddress != 0)
                    {
                        IonCannon cannon = new IonCannon();
                        cannon.LoadPosition(ReadBytes(cannonPointerAddress, 4));
                        //now read data until we hit a cannon_end byte ($00)
                        int cannonCommandOffset = 4;
                        byte commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset);
                        bool hasData = true;
                        while (hasData)
                        {
                            Commands cannonCommand = (Commands)(commandStartByte >> 6);
                            switch (cannonCommand)
                            {
                                case Commands.ReturnToStart:     //loop
                                    cannon.Program.Add(new ReturnToStart());
                                    hasData = false;
                                    break;
                                case Commands.OrientAndFire:     //Move Gun
                                    OrientAndFire cannonPosition = new OrientAndFire();
                                    int gunAngle = (commandStartByte & 0x38) >> 3;
                                    cannonPosition.Orientation = (Orientation)gunAngle;
                                    int rotationSpeed = (commandStartByte & 0x06) >> 1;
                                    cannonPosition.RotateSpeed = (RotateSpeed)rotationSpeed;
                                    int fireBit = commandStartByte & 0x01;
                                    if (fireBit > 0)
                                    {
                                        cannonCommandOffset++;
                                        cannonPosition.ShotSpeed = (byte)ReadByte(cannonPointerAddress, cannonCommandOffset);
                                    }
                                    cannon.Program.Add(cannonPosition);
                                    break;
                                case Commands.Move:     //Move Position
                                    Move cannonMovement = new Move();
                                    int waitFrames = commandStartByte & 0x3F;
                                    cannonMovement.WaitFrames = waitFrames;
                                    if (waitFrames > 0)
                                    {
                                        cannonMovement.Velocity.X = (sbyte)ReadByte(cannonPointerAddress, ++cannonCommandOffset);
                                        cannonMovement.Velocity.Y = (sbyte)ReadByte(cannonPointerAddress, ++cannonCommandOffset);
                                    }
                                    //cannonMovement.
                                    cannon.Program.Add(cannonMovement);
                                    break;
                                case Commands.Pause:     //Pause
                                    Pause cannonPause = new Pause();
                                    cannonPause.WaitFrames = commandStartByte & 0x3F;
                                    cannon.Program.Add(cannonPause);
                                    break;
                            }
                            cannonCommandOffset++;
                            commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset);
                        }
                        maze.AddObject(cannon);

                        cannonBaseAddress += 2;
                        cannonPointerAddress = ReadWord(cannonBaseAddress, 0);
                    }
                }

                //transporters
                ushort transporterBaseAddress = ReadWord(_exports["mtran"], i * 2);
                byte colorValue = ReadByte(transporterBaseAddress, 0);

                while (colorValue != 0x00)
                {
                    transporterBaseAddress++;
                    Transporter transporter = new Transporter();
                    transporter.LoadPosition(ReadByte(transporterBaseAddress, 0));
                    transporter.Direction = TransporterDirection.Left;
                    if ((colorValue & 0x10) > 0)
                    {
                        transporter.Direction = TransporterDirection.Right;
                    }
                    transporter.Color = (ObjectColor)(colorValue & 0x0F);
                    maze.AddObject(transporter);
                    transporterBaseAddress++;
                    colorValue = ReadByte(transporterBaseAddress, 0);
                }
                transporterBaseAddress++;
                //transportability rules follow for the entire level...
                int transportabilityValue = ReadByte(transporterBaseAddress, 0);
                List<bool> transportabilityData = new List<bool>();
                while (transportabilityValue != 0xEE)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        //transportabilityValue = transportabilityValue << 1;
                        transportabilityData.Add(((transportabilityValue >> b) & 0x1) != 0);
                    }
                    transporterBaseAddress++;
                    transportabilityValue = ReadByte(transporterBaseAddress, 0);
                }
                if (transportabilityData.Where(f => f).Count() > 0)
                {
                    //now set transportability flags on objects based upon the bit indexes
                    //0x02-0x11 -> Pyroid/Fireball
                    for (int t = 2; t <= 0x11; t++)
                    {
                        if (t < transportabilityData.Count && transportabilityData[t])
                        {
                            //set this pyroid (if it exists)
                            int pyroidIndex = t - 2;
                            var pyroids = maze.MazeObjects.OfType<Pyroid>().ToList();
                            if (pyroidIndex < pyroids.Count())
                            {
                                pyroids[pyroidIndex].IsTransportable = true;
                            }
                        }
                    }
                    //0x12-0x19 -> Laser Cannon Shots - All or none on these
                    bool ionCannonShotsTransportable = false;
                    for (int t = 0x12; t <= 0x19; t++)
                    {
                        if (t < transportabilityData.Count && transportabilityData[t])
                        {
                            ionCannonShotsTransportable = true;
                        }
                    }
                    if (ionCannonShotsTransportable)
                    {
                        foreach (IonCannon cannon in maze.MazeObjects.OfType<IonCannon>())
                        {
                            cannon.IsShotTransportable = true;
                        }
                    }
                    //0x1e - 0x27 -> Perkoids
                    for (int t = 0x1e; t <= 0x27; t++)
                    {
                        if (t < transportabilityData.Count && transportabilityData[t])
                        {
                            //set this perkoid (if it exists)
                            int perkoidIndex = t - 0x1e;
                            var perkoids = maze.MazeObjects.OfType<Perkoid>().ToList();
                            if (perkoidIndex < perkoids.Count())
                            {
                                perkoids[perkoidIndex].IsTransportable = true;
                            }
                        }
                    }
                    //0x28-0x31 -> Perkoid Shots
                    for (int t = 0x28; t <= 0x31; t++)
                    {
                        if (t < transportabilityData.Count && transportabilityData[t])
                        {
                            //set this perkoid (if it exists)
                            int perkoidIndex = t - 0x28;
                            var perkoids = maze.MazeObjects.OfType<Perkoid>().ToList();
                            if (perkoidIndex < perkoids.Count())
                            {
                                perkoids[perkoidIndex].IsShotTransportable = true;
                            }
                        }
                    }
                }

                //build trips now
                // Level 5 and up
                if (i > 3)
                {
                    // The max number of trips in a maze is 7. Trips are stored in a list
                    // that is null terminated. Trips start on level 5 and exist on every
                    // level to 16. 12 total levels.
                    ushort tripBaseAddress = ReadWord(_exports["mztr"], ((i - 4) * 2));
                    // Trip Pyroids are a 1 to 1 relationship to a trip. Trip Pyroids are
                    // described in 3 bytes. Each level worth of trip pyroids are stored in
                    // an array 8 pyroids long (7 + null) even if there are less than 7
                    // trips in a level.
                    ushort tripPyroidBaseAddress = (ushort)(_exports["trtbl"] + ((i - 4) * 3 * 8));

                    byte tripX = ReadByte(tripBaseAddress, 0);

                    while (tripX != 0)
                    {
                        TripPad trip = new TripPad();
                        trip.LoadPosition(tripX);
                        maze.AddObject(trip);

                        tripBaseAddress++;
                        tripX = ReadByte(tripBaseAddress, 0);

                        // level 8 has 2 pyroids per trip pad.
                        //trip pyroid too

                        byte xdata = ReadByte(tripPyroidBaseAddress++, 0);
                        byte xh = (byte)(0x7f & xdata);
                        byte styleFlag = (byte)(0x80 & xdata);
                        byte yh = ReadByte(tripPyroidBaseAddress++, 0);
                        byte vdata = ReadByte(tripPyroidBaseAddress++, 0);

                        byte[] longBytes = new byte[4];

                        int speedIndex = (byte)(vdata & 0x7f);
                        if ((vdata & 0x80) == 0)
                        {
                            longBytes[0] = 0x80;
                        }
                        else
                        {
                            longBytes[0] = 0x40;
                        }

                        longBytes[1] = (byte)((xh & 0x1f));
                        longBytes[2] = 0x80;
                        longBytes[3] = yh;

                        TripPadPyroid tpp = new TripPadPyroid();
                        tpp.LoadPosition(longBytes);
                        tpp.SpeedIndex = (TripPyroidSpeedIndex)speedIndex;
                        tpp.Direction = (TripPyroidDirection)( vdata & 0x80 );
                        if (styleFlag != 0)
                        {
                            tpp.PyroidStyle = PyroidStyle.Single;
                        }
                        maze.AddObject(tpp);

                        trip.Pyroid = tpp;
                    }
                }

                //finally... de hand
                // Level 7 and up.
                if (i > 5)
                {
                    ushort handBaseAddress = ReadWord((ushort)(_exports["mhand"] + ((i - 6) * 2)), 0);
                    byte handXData = ReadByte(handBaseAddress, 0);
                    if (handXData != 0)
                    {
                        handBaseAddress++;
                        //position is long format, but with no lsb
                        byte[] position = new byte[4];
                        position[0] = 0;        //X LSB
                        position[1] = handXData; //X MSB
                        position[2] = 0;        //Y LSB
                        position[3] = ReadByte(handBaseAddress, 0); //Y MSB
                        Hand hand = new Hand();
                        hand.LoadPosition(position);
                        maze.AddObject(hand);
                    }
                }

                mazeCollection.Mazes.Add(maze);
            }
            return mazeCollection;
        }


        private static int GetRelativeWallIndex(MazeType mazeType, int absoluteWallIndex)
        {
            int relativeWallIndex = absoluteWallIndex - 1;
            if (mazeType == MazeType.TypeB)
            {
                relativeWallIndex = absoluteWallIndex + 4;
            }
            else if (mazeType == MazeType.TypeC)
            {
                relativeWallIndex = absoluteWallIndex + 4;
            }
            else if (mazeType == MazeType.TypeD)
            {
                relativeWallIndex = absoluteWallIndex + 2;
            }
            return relativeWallIndex;
        }

        public byte[] GetBytesFromString(string text)
        {
            text = text.ToUpper();
            List<byte> bytes = new List<byte>();
            bytes.Add(0xa3);

            for (int i = 0; i < text.Length; i++)
            {
                int index = _validText.IndexOf(text[i]);
                if (index >= 0)
                {
                    bytes.Add((byte)(index * 2));
                }
            }
            //last one needs special flag set...
            if (bytes.Count > 0)
            {
                bytes[bytes.Count - 1] |= (byte)0x80;
            }
            return bytes.ToArray();
        }


        public ushort GetAddress(string location)
        {
            //search the export list for this address...
            if (!_exports.ContainsKey(location))
            {
                throw new Exception("Address not found: " + location.ToString());
            }
            return _exports[location];
        }

        private byte[] ReadROM(ushort address, int offset, int length)
        {
            byte[] bytes = new byte[length];
            if (address < 0x8000)
            {
                //must be paged rom
                if (address < 0x2000 || address > 0x3fff)
                {
                    throw new Exception("Address '" + address.ToString() + "' is not in range of ROM");
                }
            }

            if (address >= 0xc000)
            {
                address -= 0xc000;
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _alphaHigh[address + i + offset];
                }
            }
            else if (address >= 0x8000)
            {
                address -= 0x8000;
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _alphaLow[address + i + offset];
                }
            }
            else
            {
                //address -= 0x2000;
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _page01[address + i + offset];
                }
            }
            return bytes;
        }

        protected int LoadIncrementingVelocity(SignedVelocity velocity, SignedVelocity incrementingVeloctiy, ushort mazeInitIndex)
        {
            int offset = 0;
            byte velX = ReadByte(mazeInitIndex, offset);
            if (velX > 0x70 && velX < 0x90)
            {
                offset++;
                //incrementing velocity, is it positive or negative
                if ((velX & 0x80) > 0)
                {
                    //positive
                    incrementingVeloctiy.X = (sbyte)(velX & 0x7F);
                }
                else
                {
                    //negative
                    incrementingVeloctiy.X = (sbyte)(velX | 0x80);
                }
                //base velocity
                velX = ReadByte(mazeInitIndex, offset);
            }
            velocity.X = (sbyte)velX;

            offset++;
            byte velY = ReadByte(mazeInitIndex, offset);
            if (velY > 0x70 && velY < 0x90)
            {
                offset++;
                //incrementing velocity, is it positive or negative
                if ((velY & 0x80) > 0)
                {
                    //positive
                    incrementingVeloctiy.Y = (sbyte)(velY & 0x7F);
                }
                else
                {
                    //negative
                    incrementingVeloctiy.Y = (sbyte)(velY | 0x80);
                }
                //base velocity
                velY = ReadByte(mazeInitIndex, offset);
            }
            velocity.Y = (sbyte)velY;
            offset++;
            return offset;
        }

        public byte ReadByte(ushort address, int offset)
        {
            return ReadROM(address, offset, 1)[0];
        }

        public byte[] ReadBytes(ushort address, int length)
        {
            return ReadROM(address, 0, length);
        }

        public ushort ReadWord(ushort address, int offset)
        {
            byte[] bytes = ReadROM(address, offset, 2);
            ushort wordH = bytes[1];
            return (ushort)(((ushort)wordH << 8) + (ushort)bytes[0]);
        }

        private int WriteROM(ushort address, byte[] bytes, int offset)
        {
            if (address < 0x8000)
            {
                //must be paged rom
                if (address < 0x2000 || address > 0x3fff)
                {
                    throw new Exception("Address '" + address.ToString() + "' is not in range of ROM");
                }
            }

            if (address >= 0xc000)
            {
                address -= 0xc000;
                for (int i = 0; i < bytes.Length; i++)
                {
                    _alphaHigh[address + i + offset] = bytes[i];
                }
            }
            else if (address >= 0x8000)
            {
                address -= 0x8000;
                for (int i = 0; i < bytes.Length; i++)
                {
                    _alphaLow[address + i + offset] = bytes[i];
                }
            }
            else
            {
                //address -= 0x2000;
                for (int i = 0; i < bytes.Length; i++)
                {
                    _page01[address + i + offset] = bytes[i];
                }
            }
            return bytes.Length;
        }


        public int Write(string location, byte data, int offset)
        {
            ushort address = GetAddress(location);
            return WriteROM(address, new byte[] { data }, offset);
        }

        public int Write(string location, UInt16 data, int offset)
        {
            ushort address = GetAddress(location);
            byte datahigh = (byte)(data >> 8);
            byte datalow = (byte)(data & 0xff);
            return WriteROM(address, new byte[] { datalow, datahigh }, offset);
        }

        public int Write(string location, byte[] data, int offset)
        {
            ushort address = GetAddress(location);
            return WriteROM(address, data, offset);
        }



        public bool WriteFiles(string mamePath)
        {

            //fix csums...
            //Alpha High first
            //int csumAlphaHigh = 0;
            //foreach (byte b in _alphaHigh)
            //{
            //    csumAlphaHigh += b;
            //}
            //Write(ROMAddress.cksumah, 0x00, 0);

            //Crc32 crc32 = new Crc32();
            //String hash = String.Empty;
            //foreach (byte b in crc32.ComputeHash(_alphaHigh)) hash += b.ToString("x2").ToLower();

            string alphaHighFileNameMame = mamePath + _alphaHighROM;
            string alphaLowFileNameMame = mamePath + _alphaLowROM;
            string page01FileNameMame = mamePath + _page01ROM;

            //save each
            File.WriteAllBytes(alphaHighFileNameMame, _alphaHigh);
            File.WriteAllBytes(alphaLowFileNameMame, _alphaLow);
            File.WriteAllBytes(page01FileNameMame, _page01);

            //copy others 
            List<string> otherROMs = new List<string>();
            otherROMs.Add("136025.106");
            otherROMs.Add("136025.107");
            otherROMs.Add("136025.108");
            otherROMs.Add("136025.210");
            otherROMs.Add("136025.318");

            foreach (string rom in otherROMs)
            {
                File.Copy(_sourceRomPath + rom, mamePath + rom, true);
            }

            return true;
        }


        //returns a text value for the given message index.
        public string GetMessage(byte index)
        {
            ushort messageTableBase = 0xe48b;
            ushort messageBase = ReadWord(messageTableBase, index);
            //get real index 
            return GetText(messageBase);
        }

        public byte[] GetText(string text)
        {
            text = text.ToUpper();
            List<byte> bytes = new List<byte>();
            bytes.Add(0xa3);

            for (int i = 0; i < text.Length; i++)
            {
                int index = _validText.IndexOf(text[i]);
                if (index >= 0)
                {
                    bytes.Add((byte)(index * 2));
                }
            }
            //last one needs special flag set...
            bytes.Add(0x80);
            return bytes.ToArray();
        }

        private string GetText(ushort textBase)
        {
            StringBuilder sb = new StringBuilder();
            //start @ 1 because first byte is color?
            int index = 1;
            byte charValue = ReadByte(textBase, index);
            while (index < 255)
            {
                sb.Append(GetCharacter(charValue));
                if (charValue > 0x7f) break;
                index++;
                charValue = ReadByte(textBase, index);
            }
            return sb.ToString();
        }

        private string GetCharacter(byte charValue)
        {
            //we have to ignore the sign bit, that signals the end of the string
            //which we don't care about here.
            byte charindex = (byte)((charValue & 0x7f) >> 1);
            if (charindex < _validText.Length)
            {
                return _validText.Substring(charindex, 1);
            }
            return "";
        }

        public int ToDecimal(int value)
        {
            return Convert.ToInt16(("0x" + value.ToString()), 16);
        }

        public int FromDecimal(int value)
        {
            return Convert.ToInt16(value.ToString("X2"), 10);
        }


        public bool EncodeObjects(MazeCollection collection, Maze maze)
        {
            throw new Exception("Writing of ROM files is not supported in this driver.");

            //bool success = false;
            
            ///////////////////////////////
            //// Start building ROM here //
            ///////////////////////////////
            ////first is the level selection
            //Write("levelst", (byte)maze.MazeType, 1);
            
            ////next hint text
            //Write("mzh0", GetText(maze.Hint), 0);

            ////write reactor
            //int offset = 0;
            //Reactoid reactoid = maze.MazeObjects.OfType<Reactoid>().First();
            //offset += Write("mzsc0", reactoid.ToBytes(reactoid.Position), offset);
            ////build pyroids and perkoids now...
            //foreach (Pyroid pyroid in maze.MazeObjects.OfType<Pyroid>())
            //{
            //    offset += Write("mzsc0", pyroid.ToBytes(), offset);
            //}
            //if (maze.MazeObjects.OfType<Perkoid>().Count() > 0)
            //{
            //    offset += Write("mzsc0", (byte)0xfe, offset);
            //    foreach (Perkoid perkoid in maze.MazeObjects.OfType<Perkoid>())
            //    {
            //        offset += Write("mzsc0", perkoid.ToBytes(), offset);
            //    }
            //}
            ////end tag for CORE maze objects, ALWAYS!
            //offset += Write("mzsc0", (byte)0xff, offset);

            ////reactor timer, we will write all 4 entries for now...
            //int reactorTimerOffset = 0;
            //reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            //reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            //reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            //reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);

            ////do oxygens now
            //offset = 0;
            //foreach (Oxoid oxoid in maze.MazeObjects.OfType<Oxoid>())
            //{
            //    offset += Write("mzdc0", oxoid.ToBytes(), offset);
            //}
            //Write("mzdc0", 0, offset);

            ////do lightning (Force Fields)
            //offset = 0;
            //if ((maze.MazeObjects.OfType<LightningH>().Count() > 0) ||
            //    (maze.MazeObjects.OfType<LightningV>().Count() > 0))
            //{
            //    {
            //        foreach (LightningH lightning in maze.MazeObjects.OfType<LightningH>())
            //        {
            //            offset += Write("mzlg0", lightning.ToBytes(), offset);
            //        }
            //        if (maze.MazeObjects.OfType<LightningV>().Count() > 0)
            //        {
            //            //end horizontal with 0xff
            //            offset += Write("mzlg0", (byte)0xff, offset);
            //            foreach (LightningV lightning in maze.MazeObjects.OfType<LightningV>())
            //            {
            //                offset += Write("mzlg0", lightning.ToBytes(), offset);
            //            }
            //        }
            //    }
            //}
            ////end all with 0x00
            //Write("mzlg0", (byte)0, offset);

            ////build arrows now
            //offset = 0;
            //foreach (Arrow arrow in maze.MazeObjects.OfType<Arrow>())
            //{
            //    offset += Write("mzar0", arrow.ToBytes(), offset);
            //}
            //Write("mzar0", (byte)0, offset);

            ////build output arrows now
            //offset = 0;
            //foreach (ArrowOut arrowOut in maze.MazeObjects.OfType<ArrowOut>())
            //{
            //    offset += Write("mzor0", arrowOut.ToBytes(), offset);
            //}
            //Write("mzor0", (byte)0, offset);

            ////maze walls - static first
            //offset = 0;
            //foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w=>!w.IsDynamicWall))
            //{
            //    offset += Write("mzta0", wall.ToBytes(maze), offset);
            //}
            //Write("mzta0", (byte)0, offset);

            ////then dynamic
            //offset = 0;
            //foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
            //{
            //    offset += Write("mztd0", wall.ToBytes(maze), offset);
            //}
            //Write("mztd0", (byte)0, offset);

            ////one way walls
            //offset = 0;
            //if (maze.MazeObjects.OfType<OneWay>().Where(o=>o.Direction == OneWayDirection.Right).Count() > 0)
            //{
            //    foreach (OneWay oneway in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
            //    {
            //        offset += Write("mone0", oneway.ToBytes(), offset);
            //    }
            //}
            //foreach (OneWay oneway in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
            //{
            //    offset += Write("mone0", (byte)0xff, offset);
            //    offset += Write("mone0", oneway.ToBytes(), offset);
            //}
            //Write("mone0", (byte)0, offset);

            ////build spikes now
            //offset = 0;
            //foreach (Spikes spike in maze.MazeObjects.OfType<Spikes>())
            //{
            //    offset += Write("tite0", spike.ToBytes(), offset);
            //}
            //Write("tite0", (byte)0, offset);

            ////locks and keys, for now, there has to be an even number of locks and keys
            //offset = 0;
            //foreach (Lock lock_ in maze.MazeObjects.OfType<Lock>())
            //{
            //    Key thisKey = maze.MazeObjects.OfType<Key>().Where(k => k.KeyColor == lock_.LockColor).FirstOrDefault();
            //    if (thisKey != null)
            //    {

            //        offset += Write("lock0", lock_.ToBytes(thisKey), offset);
            //    }
            //}
            //Write("lock0", (byte)0, offset);

            ////Escape pod
            //EscapePod pod = maze.MazeObjects.OfType<EscapePod>().FirstOrDefault();
            //if (pod != null)
            //{
            //    Write("mpod", pod.ToBytes(), 0);
            //}

            ////clock & boots
            //Clock clock = maze.MazeObjects.OfType<Clock>().FirstOrDefault();
            //if (clock != null)
            //{
            //    //write these on all 4 level options
            //    Write("mclock", clock.ToBytes(), 0);
            //    Write("mclock", clock.ToBytes(), 1);
            //    Write("mclock", clock.ToBytes(), 2);
            //    Write("mclock", clock.ToBytes(), 3);
            //}

            //Boots boots = maze.MazeObjects.OfType<Boots>().FirstOrDefault();
            //if (boots != null)
            //{
            //    //write these on all 4 level options
            //    Write("mboots", boots.ToBytes(), 0);
            //    Write("mboots", boots.ToBytes(), 1);
            //    Write("mboots", boots.ToBytes(), 2);
            //    Write("mboots", boots.ToBytes(), 3);
            //}

            ////transporters
            //offset = 0;
            //var transporterGroups = maze.MazeObjects.OfType<Transporter>().GroupBy(t => t.Color).Select(group => new { Key = group.Key, Count = group.Count() });

            //foreach (var transporterPair in transporterGroups)
            //{
            //    List<Transporter> coloredTranporterMatches = maze.MazeObjects.OfType<Transporter>().Where(t => t.Color == transporterPair.Key).ToList();
            //    foreach (Transporter t in coloredTranporterMatches)
            //    {
            //        offset += Write("tran0", t.ToBytes(), offset);
            //    }
            //}
            ////write end of transports
            //offset += Write("tran0", 0, offset);
            ////write transportability data
            //offset += Write("tran0", new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, offset);

            ////Laser IonCannon
            //offset = 0;
            //int pointer = 0;
            //if (maze.MazeObjects.OfType<IonCannon>().Count() > 0)
            //{
            //    Write("mcan", new byte[] { 0x02, 0x02, 0x02, 0x02 }, 0);
            //}
            //foreach (IonCannon cannon in maze.MazeObjects.OfType<IonCannon>())
            //{
            //    pointer += Write("mcan0", (UInt16)(GetAddress("mcand").Item1 + offset), pointer);
            //    //cannon location first...
            //    offset += Write("mcan0", cannon.ToBytes(), offset);
            //}
            ////build trips now
            //offset = 0;
            //int tripoffset = 0;
            //foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
            //{
            //    offset += Write("mztr0", trip.ToBytes(), offset);
            //    //trip pad pyroid
            //    Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x18);
            //    Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x30);
            //    Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x48);
            //    tripoffset += Write("trtbl", trip.Pyroid.ToBytes(), tripoffset);
            //}
            //Write("mztr0", (byte)0, offset);

            ////de hand finally
            //offset = 0;
            //Hand hand = maze.MazeObjects.OfType<Hand>().FirstOrDefault();
            //if (hand != null)
            //{
            //    offset += Write("hand0", hand.ToBytes(), offset);
            //}
            //else
            //{
            //    offset += Write("hand0", new byte[] { 0x00 }, offset);
            //}

            //success = true;
            //return success;
        }
    }
}
