using mhedit.Containers;
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

    public class MajorHavocPromisedEnd : GameController, IGameController
    {

        #region Private Variables

        private string _sourceRomPath = String.Empty;
        //private string _mamePath = String.Empty;
        private Rom _page2367;
        private byte[] _alphaHigh = new byte[0x4000];
        private ExportsFile _exports;
        private string _page2367ROM = "mhpe100.1np";
        private string _alphaHighROM = "mhpe100.1l";
        private string _lastError = String.Empty;
        private readonly string _name;

        #endregion


        public MajorHavocPromisedEnd()
            :this( "Major Havoc Promised End" )
        { }

        public MajorHavocPromisedEnd( string name )
        {
            this._name = name;
        }

        public MajorHavocPromisedEnd( Rom rom, ExportsFile exports )
        {
            this._page2367 = rom;

            this._exports = exports;
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

        public Version GetROMVersion()
        {
            byte[] versionBytes = ReadPagedROM(0x2002, 0, 2, 6);
            return new Version( FromBCD( versionBytes[0]), FromBCD( versionBytes[1]),0,0);
        }

        private static int FromBCD( byte bcd )
        {
            int result = 0;
            result += ( 10 * ( bcd >> 4 ) );
            result += bcd & 0xf;

            return result;
        }

        public bool LoadTemplate(string sourceRomPath)
        {
            bool success = false;

            try
            {
                _sourceRomPath = sourceRomPath;

                //load up our roms for now...
                try
                {
                    _page2367 = new Rom( 0x8000, sourceRomPath );
                    _page2367.Load();
                    _alphaHigh = File.ReadAllBytes( Path.Combine( sourceRomPath, _alphaHighROM ) );
                }
                catch ( Exception Exception )
                {
                    throw new Exception( "ROM Load Error - Page6/7: " + Exception.Message );
                }

                Version romVersion = GetROMVersion();
                if ( romVersion.CompareTo( new Version( 0, 22 ) ) >= 0 )
                {
                    //load our exports
                    this._exports = new ExportsFile( sourceRomPath );

                    this._exports.Load();
                }
                else
                {
                    throw new Exception( "ROM Version has to be 0.22 or higher." );
                }

                success = true;
            }
            catch ( Exception ex )
            {
                _lastError = ex.Message;
            }

            return success;
        }

        public MazeCollection LoadMazes(List<string> loadMessages)
        {
            MazeCollection mazeCollection = new MazeCollection( this.Name );
            mazeCollection.AuthorEmail = "jess@askey.org";
            mazeCollection.AuthorName = "Jess Askey";

            for ( int i = 0; i < 28; i++)
            {
                byte mazeType = ReadByte(_exports["mzty"], i, 6);
                //byte mazeType = (byte)(i & 0x03);
                Maze maze = new Maze((MazeType)mazeType, "Level " + (i + 1).ToString());

                //hint text
                byte hintIndex = ReadByte(_exports["mazehints"], i * 2, 7);
                byte hintIndex2 = ReadByte(_exports["mazehints"], (i * 2) + 1, 7);
                if (hintIndex != 0xff)
                {
                    maze.Hint = GetMessage(hintIndex);
                }
                if (hintIndex2 != 0xff)
                {
                    maze.Hint2 = GetMessage(hintIndex2);
                }
                //build reactor
                ushort mazeInitIndex = ReadWord(_exports["mzinit"], i * 2, 7);
                Reactoid reactor = new Reactoid();
                reactor.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                mazeInitIndex += 4;
                int timer = DataConverter.FromDecimal((int)ReadByte(_exports["outime"], i, 6));
                reactor.Timer = timer;
                int reactorSize = DataConverter.FromDecimal((int)ReadByte(_exports["reacsz"], i, 6));
                reactor.MegaReactoid = reactorSize != 0 ? true : false;
                maze.AddObject(reactor);

                //pyroids
                byte firstValue = ReadByte(mazeInitIndex, 0, 7);
                while (firstValue != 0xff && firstValue != 0xfe)
                {
                    Pyroid pyroid = new Pyroid();
                    pyroid.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(pyroid.Velocity, pyroid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(pyroid);
                    firstValue = ReadByte(mazeInitIndex, 0, 7);
                }
                if (firstValue == 0xfe)
                {
                    mazeInitIndex++;
                    //signals pyroids were done/skipped, get next value
                    firstValue = ReadByte(mazeInitIndex, 0, 7);
                }

                // Perkoids 
                while (firstValue != 0xff && firstValue != 0xfe)
                {
                    Perkoid perkoid = new Perkoid();
                    perkoid.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(perkoid.Velocity, perkoid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(perkoid);
                    firstValue = ReadByte(mazeInitIndex, 0, 7);
                }
                if (firstValue == 0xfe)
                {
                    mazeInitIndex++;
                    //signals perkoids were done/skipped, get next value
                    firstValue = ReadByte(mazeInitIndex, 0, 7);
                }

                while (firstValue != 0xff)
                {
                    Maxoid maxoid = new Maxoid();
                    maxoid.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                    mazeInitIndex += 4;
                    byte[] maxData = ReadBytes(mazeInitIndex, 1, 7);
                    mazeInitIndex++;
                    maxoid.TriggerDistance = (maxData[0] & 0x0f);
                    maxoid.Speed = (MaxSpeed)((maxData[0] >> 4) & 0x3);
                    maxoid.HitsToKill =  1+((~(maxData[0] >> 6)) & 0x3);
                    maze.AddObject(maxoid);
                    firstValue = ReadByte(mazeInitIndex, 0, 7);
                }

                //do oxygens now
                ushort oxygenBaseAddress = ReadWord(_exports["mzdc"], i * 2, 6);

                byte oxoidValue = ReadByte(oxygenBaseAddress, 0, 6);
                while (oxoidValue != 0x00)
                {
                    Oxoid oxoid = new Oxoid();
                    oxoid.LoadPosition(oxoidValue);
                    maze.AddObject(oxoid);

                    oxygenBaseAddress++;
                    oxoidValue = ReadByte(oxygenBaseAddress, 0, 6);
                }

                //Oxygen reward
                maze.OxygenReward = ReadByte(_exports["oxybonus"], i, 6);

                //do lightning (Force Fields)
                ushort lightningBaseAddress = ReadWord(_exports["mzlg"], i * 2, 6);

                byte lightningValue = ReadByte(lightningBaseAddress, 0, 6);
                bool isHorizontal = true;

                if (lightningValue == 0xff)
                {
                    isHorizontal = false;
                    lightningBaseAddress++;
                    lightningValue = ReadByte(lightningBaseAddress, 0, 6);
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
                    lightningValue = ReadByte(lightningBaseAddress, 0, 6);
                    if (lightningValue == 0xff)
                    {
                        isHorizontal = false;
                        lightningBaseAddress++;
                    }
                    lightningValue = ReadByte(lightningBaseAddress, 0, 6);
                }

                //build arrows now
                ushort arrowBaseAddress = ReadWord(_exports["mzar"], i * 2, 6);
                byte arrowValue = ReadByte(arrowBaseAddress, 0, 6);

                while (arrowValue != 0x00)
                {
                    Arrow arrow = new Arrow();
                    arrow.LoadPosition(arrowValue);
                    arrowBaseAddress++;
                    arrowValue = ReadByte(arrowBaseAddress, 0, 6);
                    arrow.ArrowDirection = (Containers.ArrowDirection)arrowValue;
                    maze.AddObject(arrow);
                    arrowBaseAddress++;
                    arrowValue = ReadByte(arrowBaseAddress, 0, 6);
                }

                //build Out arrows now
                ushort outArrowBaseAddress = ReadWord(_exports["mzor"], i * 2, 6);
                byte outArrowValue = ReadByte(outArrowBaseAddress, 0, 6);

                while (outArrowValue != 0x00)
                {
                    ArrowOut arrow = new ArrowOut();
                    arrow.LoadPosition(outArrowValue);
                    outArrowBaseAddress++;
                    outArrowValue = ReadByte(outArrowBaseAddress, 0, 6);
                    arrow.ArrowDirection = (ArrowOutDirection)outArrowValue-9;
                    maze.AddObject(arrow);
                    outArrowBaseAddress++;
                    outArrowValue = ReadByte(outArrowBaseAddress, 0, 6);
                }

                //maze walls
                //static first
                ushort wallBaseAddress = ReadWord(_exports["mztdal"], i * 2, 6);
                byte wallValue = ReadByte(wallBaseAddress, 0, 6);

                while (wallValue != 0x00)
                {
                    int relativeWallValue = GetRelativeWallIndex(maze.MazeType, wallValue);
                    Point stampPoint = maze.PointFromStamp(relativeWallValue);
                    wallBaseAddress++;
                    wallValue = ReadByte(wallBaseAddress, 0, 6);
                    MazeWall wall = new MazeWall((MazeWallType)(wallValue & 0x07), stampPoint, relativeWallValue);
                    wall.UserWall = true;
                    maze.AddObject(wall);
                    wallBaseAddress++;
                    wallValue = ReadByte(wallBaseAddress, 0, 6);
                }

                //then dynamic walls
                ushort dynamicWallBaseAddress = ReadWord(_exports["mztd"], i * 2, 6);
                byte dynamicWallIndex = ReadByte(dynamicWallBaseAddress, 0, 6);
                while (dynamicWallIndex != 0x00)
                {
                    int relativeWallIndex = GetRelativeWallIndex(maze.MazeType, dynamicWallIndex);
                    int baseTimer = ReadByte(dynamicWallBaseAddress, 1, 6);
                    int altTimer = ReadByte(dynamicWallBaseAddress, 2, 6);
                    MazeWallType baseType = (MazeWallType)ReadByte(dynamicWallBaseAddress, 3, 6);
                    MazeWallType altType = (MazeWallType)ReadByte(dynamicWallBaseAddress, 4, 6);
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
                        loadMessages.Add("Level " + (i + 1).ToString() + ": Dynamic wall definition at " + dynamicWallIndex.ToString("X2"));
                    }
                    dynamicWallBaseAddress += 5;
                    dynamicWallIndex = ReadByte(dynamicWallBaseAddress, 0, 6);
                }

                //one way walls
                ushort onewayBaseAddress = ReadWord(_exports["mone"], i * 2, 6);
                byte onewayValue = ReadByte(onewayBaseAddress, 0, 6);

                while (onewayValue != 0x00)
                {
                    OneWay oneway = new OneWay();
                    if (onewayValue == 0xff)
                    {
                        oneway.Direction = OneWayDirection.Left;
                        onewayBaseAddress++;
                        onewayValue = ReadByte(onewayBaseAddress, 0, 6);
                    }
                    else
                    {
                        oneway.Direction = OneWayDirection.Right;
                    }
                    oneway.LoadPosition(onewayValue);
                    maze.AddObject(oneway);

                    onewayBaseAddress++;
                    onewayValue = ReadByte(onewayBaseAddress, 0, 6);
                }

                // Stalactites
                ushort stalactiteBaseAddress = ReadWord(_exports["mtite"], i * 2, 6);
                byte stalactiteValue = ReadByte(stalactiteBaseAddress, 0, 6);

                while (stalactiteValue != 0x00)
                {
                    Spikes spikes = new Spikes();
                    spikes.LoadPosition(stalactiteValue);
                    maze.AddObject(spikes);

                    stalactiteBaseAddress++;
                    stalactiteValue = ReadByte(stalactiteBaseAddress, 0, 6);
                }

                //locks and keys
                ushort lockBaseAddress = ReadWord(_exports["mlock"], i * 2, 6);
                byte lockValue = ReadByte(lockBaseAddress, 0, 6);

                while (lockValue != 0x00)
                {
                    byte lockColor = lockValue;
                    lockBaseAddress++;

                    Key key = new Key();
                    key.LoadPosition(ReadByte(lockBaseAddress, 0, 6));
                    key.KeyColor = (ObjectColor)lockColor;
                    maze.AddObject(key);

                    lockBaseAddress++;

                    Lock keylock = new Lock();
                    keylock.LoadPosition(ReadByte(lockBaseAddress, 0, 6));
                    keylock.LockColor = (ObjectColor)lockColor;
                    maze.AddObject(keylock);

                    lockBaseAddress++;
                    lockValue = ReadByte(lockBaseAddress, 0, 6);
                }

                //Escape pod
                // Levels 2,6,10,14 only
                if (mazeType == (int)MazeType.TypeB)
                {
                    ushort podBaseAddress = _exports["mpod"];
                    byte podValue = ReadByte(podBaseAddress, i >> 2, 6);
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
                byte clockData = ReadByte(_exports["mclock"], i, 6);
                if (clockData != 0)
                {
                    Clock clock = new Clock();
                    clock.LoadPosition(clockData);
                    maze.AddObject(clock);
                }

                byte bootsData = ReadByte(_exports["mboots"], i, 6);
                if (bootsData != 0)
                {
                    Boots boots = new Boots();
                    boots.LoadPosition(bootsData);
                    maze.AddObject(boots);
                }

                byte pouchData = ReadByte(_exports["mkeyp"], i, 6);
                if ( pouchData != 0)
                {
                    KeyPouch keyPouch = new KeyPouch();
                    keyPouch.LoadPosition(pouchData);
                    maze.AddObject(keyPouch);
                }

                //Laser Cannon
                for (int c = 0; c < 4; c++)
                {
                    ushort cannonPointerAddress = ReadWord(_exports["mcan"], (i * 8) + (c * 2), 6);

                    if (cannonPointerAddress == 0)
                    {
                        break;
                    }
                    else
                    {
                        IonCannon cannon = new IonCannon();
                        cannon.LoadPosition(ReadBytes(cannonPointerAddress, 4, 6));
                        //now read data until we hit a cannon_end byte ($00)
                        int cannonCommandOffset = 4;
                        byte commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
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
                                        cannonPosition.ShotSpeed = (byte)ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                                    }
                                    cannon.Program.Add(cannonPosition);
                                    break;
                                case Commands.Move:     //Move Position
                                    Move cannonMovement = new Move();
                                    int waitFrames = commandStartByte & 0x3F;
                                    cannonMovement.WaitFrames = waitFrames;
                                    if (waitFrames > 0)
                                    {
                                        cannonMovement.Velocity.X = (sbyte)ReadByte(cannonPointerAddress, ++cannonCommandOffset, 6);
                                        cannonMovement.Velocity.Y = (sbyte)ReadByte(cannonPointerAddress, ++cannonCommandOffset, 6);
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
                            commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                        }
                        maze.AddObject(cannon);
                    }
                }

                //transporters - this has to be de-serialized after Pyroids, Perkoids and Ion Cannons 
                //               due to the transportability bits
                ushort transporterBaseAddress = ReadWord(_exports["mtran"], i * 2, 6);
                byte colorValue = ReadByte(transporterBaseAddress, 0, 6);
                while (colorValue != 0x00)
                {
                    transporterBaseAddress++;
                    Transporter transporter = new Transporter();
                    transporter.LoadPosition(ReadByte(transporterBaseAddress, 0, 6));
                    transporter.Direction = TransporterDirection.Left;
                    if ((colorValue & 0x10) > 0)
                    {
                        transporter.Direction = TransporterDirection.Right;
                    }
                    transporter.Color = (ObjectColor)(colorValue & 0x0F);
                    transporter.IsSpecial = ((colorValue & 0x40) > 0);
                    transporter.IsHidden = ((colorValue & 0x80) > 0);
                    maze.AddObject(transporter);
                    transporterBaseAddress++;
                    colorValue = ReadByte(transporterBaseAddress, 0, 6);
                }
                transporterBaseAddress++;

                //transportability rules follow for the entire level...
                int transportabilityValue = ReadByte(transporterBaseAddress, 0, 6);
                List<bool> transportabilityData = new List<bool>();
                while (transportabilityValue != 0xEE)
                {
                    for(int b = 0; b < 8; b++)
                    {
                        //transportabilityValue = transportabilityValue << 1;
                        transportabilityData.Add(((transportabilityValue >> b) & 0x1) != 0);
                    }
                    transporterBaseAddress++;
                    transportabilityValue = ReadByte(transporterBaseAddress, 0, 6);
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
                // The max number of trips in a maze is 7. Trips are stored in a list
                // that is null terminated. 
                ushort tripBaseAddress = ReadWord((ushort)_exports["mztr"], (i * 2), 6);
                ushort tripPyroidBaseAddress = (ushort)ReadWord(_exports["trtbll"], (i * 2), 6);

                byte tripX = ReadByte(tripBaseAddress, 0, 6);

                while (tripX != 0)
                {
                    TripPad trip = new TripPad();
                    trip.LoadPosition(tripX);
                    maze.AddObject(trip);

                    tripBaseAddress++;
                    tripX = ReadByte(tripBaseAddress, 0, 6);

                    // level 8 has 2 pyroids per trip pad.
                    //trip pyroid too
                    byte xdata = ReadByte(tripPyroidBaseAddress++, 0, 6);
                    byte xh = (byte)(0x7f & xdata);
                    byte styleFlag = (byte)(0x80 & xdata);
                    byte yh = ReadByte(tripPyroidBaseAddress++, 0, 6);
                    byte vdata = ReadByte(tripPyroidBaseAddress++, 0, 6);

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

                //finally... de hand
                ushort handBaseAddress = ReadWord(_exports["mhand"], (i*2), 6);
                byte handData = ReadByte(handBaseAddress, 0, 6);
                if (handData != 0)
                {
                    handBaseAddress++;
                    //position is long format, but with no lsb
                    byte[] position = new byte[4];
                    position[0] = 0;        //X LSB
                    position[1] = handData; //X MSB
                    position[2] = 0;        //Y LSB
                    position[3] = ReadByte(handBaseAddress, 0, 6); //Y MSB
                    Hand hand = new Hand();
                    hand.LoadPosition(position);
                    maze.AddObject(hand);
                }

                mazeCollection.Mazes.Add(maze);
            }

            /// Extract and add HiddenLevelTokens
            ushort hiddenLevelTokenIndex = _exports[ "mtok" ];

            for ( int tokenIndex = 0; tokenIndex < Constants.MAXOBJECTS_TOKEN; tokenIndex++ )
            {
                sbyte level = (sbyte)ReadByte( hiddenLevelTokenIndex, 0, 6 );

                if ( level < 0 )
                {
                    continue;
                }

                HiddenLevelToken hiddenLevelToken = new HiddenLevelToken();
                hiddenLevelToken.TokenStyle = (TokenStyle)tokenIndex;
                hiddenLevelToken.LoadPosition(ReadBytes( (ushort)( hiddenLevelTokenIndex + 1 ), 4, 6 ) );
                hiddenLevelToken.TargetLevel = ( ReadByte( hiddenLevelTokenIndex, 5, 6 ) + 1 );
                hiddenLevelToken.ReturnLevel = ( ReadByte( hiddenLevelTokenIndex, 6, 6 ) + 1 );
                hiddenLevelToken.VisibleDistance = ( ReadByte( hiddenLevelTokenIndex, 7, 6 ) );
                mazeCollection.Mazes[ level ].AddObject( hiddenLevelToken );
                hiddenLevelTokenIndex += 8;
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

        protected int LoadIncrementingVelocity(SignedVelocity velocity, SignedVelocity incrementingVeloctiy, ushort mazeInitIndex)
        {
            int offset = 0;
            byte velX = ReadByte(mazeInitIndex, offset, 7);
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
                velX = ReadByte(mazeInitIndex, offset, 7);
            }
            velocity.X = (sbyte)velX;

            offset++;
            byte velY = ReadByte(mazeInitIndex, offset, 7);
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
                velY = ReadByte(mazeInitIndex, offset, 7);
            }
            velocity.Y = (sbyte)velY;
            offset++;
            return offset;
        }

        public byte[] GetBytesFromString(string text)
        {
            text = text.ToUpper();
            List<byte> bytes = new List<byte>();

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

        private byte[] ReadPagedROM(ushort address, int offset, int length, int page)
        {
            return this._page2367.ReadPagedROM( address, offset, length, page );
        }

        private byte[] ReadAlphaHigh(ushort address, int length)
        {
            byte[] bytes = new byte[length];
            int alphaHighBase = 0xC000; //since addresses come in with a base of 0xC000 already, we just need to add this amount

            if (address >= 0xC000 && address <= 0xffff)
            {
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _alphaHigh[address - alphaHighBase + i];
                }
            }
            return bytes;
        }
        
        private static readonly ushort alphaHighBase = 0xc000;

        private int WriteAlphaHigh(ushort address, byte data)
        {
            address -= alphaHighBase;
            _alphaHigh[address] = data;
            return 1;
        }


        private int WritePagedROM(ushort address, byte[] bytes, int offset, int page)
        {
           return this._page2367.WritePagedROM( address, bytes, offset, page );
        }

        private byte[] WordToByteArray(int word)
        {
            byte datahigh = (byte)(word >> 8);
            byte datalow = (byte)(word & 0xff);
            return new byte[] { datalow, datahigh };
        }

        private ushort BytesToWord(byte lowByte, byte highByte)
        {
            return (ushort)(((ushort)highByte << 8) + (ushort)lowByte);
        }


        private void WritePagedChecksum(int lowerBounds, int length, int page, byte csum)
        {
            byte calculatedCsum = _page2367.CalculateChecksum(
                lowerBounds, length - 1, page );

            //ROM needs to equal csum when it is all said and done
            byte finalCsum = (byte)((csum ^ calculatedCsum) & 0xff);

            WritePagedROM((ushort)(0x2000 + length - 1), new byte[] { finalCsum }, 0, page);
        }

        private void WriteAlphaHighChecksum()
        {
            ushort csumAddress = (ushort) ( _exports[ "chka2" ] - alphaHighBase );
            byte calculatedCsum = 0;

            for ( int i = 0x0000; i < 0x4000; i++ )
            {
                if ( i == csumAddress )
                    continue;

                calculatedCsum ^= _alphaHigh[ i ];
            }

            //ROM needs to equal csum when it is all said and done
            byte finalCsum = (byte) ( ( 0x01 ^ calculatedCsum ) & 0xff );
            _alphaHigh[ csumAddress ] = finalCsum;
        }

        private void MarkPagedROM(int page)
        {
            byte[] currentMajorVersion = ReadPagedROM(0x2002, 0, 1, page);
            WritePagedROM(0x2002, new byte[] { (byte)(currentMajorVersion[0] | 0xE0) }, 0, page);
        }

        private void MarkAlphaHighROM()
        {
            ushort alphaHighCsumAddress = 0xC002;
            byte[] currentMajorVersion = ReadAlphaHigh(alphaHighCsumAddress, 1);
            WriteAlphaHigh(alphaHighCsumAddress, (byte)(currentMajorVersion[0] | 0xE0));
        }

        public bool WriteFiles(string destinationPath, string driverName)
        {
            bool success = false;
            try
            {
                MarkPagedROM(6);
                MarkPagedROM(7);
                MarkAlphaHighROM();

                //fix csums...
                WritePagedChecksum(0x4000, 0x2000, 6, 0x08);
                WritePagedChecksum(0x6000, 0x2000, 7, 0x09);
                WriteAlphaHighChecksum();

                string page67FileNameMame = Path.Combine(destinationPath, _page2367ROM );
                string alphaHighFileNameMane = Path.Combine(destinationPath, _alphaHighROM );

                //save each
                File.WriteAllBytes(page67FileNameMame, _page2367.GetBuffer());
                File.WriteAllBytes(alphaHighFileNameMane, _alphaHigh);

                //copy others 
                List<string> otherROMs = new List<string>();
                otherROMs.Add("mhpe100.1mn");
                otherROMs.Add("mhpe100.1q");
                otherROMs.Add("mhpe100.6kl");
                otherROMs.Add("mhpe100.6h");
                otherROMs.Add("mhpe100.6jk");
                otherROMs.Add("mhpe100.9s");
                otherROMs.Add("136002-125.6c");
                otherROMs.Add("mhpex089.x1");

                foreach (string rom in otherROMs)
                {
                    File.Copy(Path.Combine(_sourceRomPath, rom), Path.Combine(destinationPath, rom), true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
            }
            return success;
        }

        public byte ReadByte(ushort address, int offset, int page)
        {
            return this._page2367.ReadByte(address, offset, page);
        }

        public byte[] ReadBytes(ushort address, int length, int page)
        {
            return this._page2367.ReadBytes(address, length, page);
        }

        public ushort ReadWord(ushort address, int offset, int page)
        {
            return this._page2367.ReadWord(address, offset, page);
        }

        //returns a text value for the given message index.
        public string GetMessage(byte index)
        {
            byte messageTableLow = ReadByte(_exports["zmessptrl"], index, 7);
            byte messageTableHigh = ReadByte(_exports["zmessptrh"], index, 7);
            int messagePointer = (messageTableHigh << 8) + messageTableLow;
            //get real index 
            return GetText((ushort)messagePointer);
        }

        private string GetText(ushort textBase)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            byte charValue = ReadByte(textBase, index, 7);
            while (index < 255)
            {
                sb.Append(GetCharacter(charValue));
                if (charValue > 0x7f) break;
                index++;
                charValue = ReadByte(textBase, index, 7);
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

        
        /// <summary>
        /// Returns a negative byte offset from center screen to get text centered
        /// Assumes letters are 6 units wide
        /// </summary>
        /// <param name="text">The text to use for calculations</param>
        /// <returns>Signed byte value to center align text</returns>
        public byte GetTextPosition(string text)
        {
            int len = text.Length * 6;
            byte offset = (byte)((-len / 2) & 0xFF);
            return offset;
        }

        public String ExtractSource(List<Tuple<Maze, int>> selectedMazes, SourceFile sourceFile)
        {
            int dataPosition = 8;
            int commentPosition = 60;
            string commentLine = ";********************************************************************";

            StringBuilder page6Source = new StringBuilder();
            StringBuilder page7Source = new StringBuilder();
            StringBuilder cannonSource = new StringBuilder();
            StringBuilder mazeMessageSource = new StringBuilder();

            List<String> mazeLetters = new List<string>() { "A", "B", "C", "D" };
            KeyValuePair<int,HiddenLevelToken>[] tokens = new KeyValuePair<int, HiddenLevelToken>[4];

            foreach (Tuple<Maze, int> selectedMaze in selectedMazes)
            {
                Maze maze = selectedMaze.Item1;
                int level = selectedMaze.Item2;

                page7Source.AppendLine(commentLine);
                page7Source.Append("; ");
                page7Source.AppendLine(" Dif " + (level / 4).ToString() + " - Maze " + mazeLetters[((level - 1) % 4)] + " - Level " + level.ToString() + " - " + maze.Name);
                page7Source.AppendLine(commentLine);

                //Maze Hints
                AddHints(maze.Hint, maze.Hint2, mazeMessageSource, dataPosition, level);
                page7Source.AppendLine("");

                //Reactoid.Pyroids.Perkoids.Max
                page7Source.AppendLine(DumpBytes("mzsc" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.ReactoidPyroidPerkoidMax).ObjectEncodings));


                page6Source.AppendLine(commentLine);
                page6Source.Append("; ");
                page6Source.AppendLine(" Dif " + (level / 4).ToString() + " - Maze " + mazeLetters[((level - 1) % 4)] + " - Level " + level.ToString() + " - " + maze.Name);
                page6Source.AppendLine(commentLine);
                //Oxygen
                page6Source.AppendLine(DumpBytes("mzdc" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Oxoids).ObjectEncodings));
                //Lightning
                page6Source.AppendLine(DumpBytes("mzlg" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Lightning).ObjectEncodings));
                //Arrows
                page6Source.AppendLine(DumpBytes("mzar" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.Arrows).ObjectEncodings));
                //Exit Arrows
                page6Source.AppendLine(DumpBytes("mzor" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.ArrowsOut).ObjectEncodings));
                //Trip Pads
                page6Source.AppendLine(DumpBytes("mztr" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.TripPoints).ObjectEncodings));
                //Trip Pad Actions
                page6Source.AppendLine(DumpBytes("trpa" + GetMazeCode(level), dataPosition, commentPosition, 3, EncodeObjects(maze, EncodingGroup.TripActions).ObjectEncodings));
                //Static Walls
                page6Source.AppendLine(DumpBytes("mzta" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.StaticWalls).ObjectEncodings));
                //Dynamic Walls
                page6Source.AppendLine(DumpBytes("mztd" + GetMazeCode(level), dataPosition, commentPosition, 5, EncodeObjects(maze, EncodingGroup.DynamicWalls).ObjectEncodings));
                //One Way Walls
                page6Source.AppendLine(DumpBytes("mone" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.OneWay).ObjectEncodings));
                //Ion Cannons
                var cannonGroupEncodings = from e in EncodeObjects(maze, EncodingGroup.IonCannon).ObjectEncodings
                                           group e by e.Group into g
                                           select new { Id = g.Key, Encodings = g.ToList() };
                int ionIndexer = 0;
                List<char> suffix = new List<char>() { 'a', 'b', 'c', 'd' };
                List<string> cannonLabels = new List<string>();
                StringBuilder levelCannonSource = new StringBuilder();
                foreach (var g in cannonGroupEncodings)
                {
                    string cannonLabel = "mcp" + GetMazeCode(level) + suffix[ionIndexer++];
                    cannonLabels.Add(cannonLabel);
                    page6Source.AppendLine(DumpBytes(cannonLabel, dataPosition, commentPosition, 16, g.Encodings)); 
                }
                levelCannonSource.Append("mcan" + GetMazeCode(level));
                Tabify(' ', dataPosition ,levelCannonSource);
                levelCannonSource.Append(".word ");
                levelCannonSource.Append(String.Join(",", cannonLabels.ToArray()));
                for(int i = cannonLabels.Count; i < 4; i++)
                {
                    if (i > 0)
                    {
                        levelCannonSource.Append(",");
                    }
                    levelCannonSource.Append("0");
                }
                cannonSource.AppendLine(levelCannonSource.ToString());

                //Stalactites
                page6Source.AppendLine(DumpBytes("tite" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Spikes).ObjectEncodings));
                //Locks and Keys
                page6Source.AppendLine(DumpBytes("lock" + GetMazeCode(level), dataPosition, commentPosition, 3, EncodeObjects(maze, EncodingGroup.LocksKeys).ObjectEncodings));
                //Transporters
                page6Source.AppendLine(DumpBytes("tran" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Transporters).ObjectEncodings));
                //DeHand
                page6Source.AppendLine(DumpBytes("hand" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Hand).ObjectEncodings));

                //misc stuff, defined as vars not tables
                page6Source.AppendLine(DumpScalar("mzty" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.MazeType).ObjectEncodings));
                page6Source.AppendLine(DumpScalar("clock" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.Clock).ObjectEncodings));
                page6Source.AppendLine(DumpScalar("boot" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.Boots).ObjectEncodings));
                page6Source.AppendLine(DumpScalar("keyp" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.KeyPouch).ObjectEncodings));
                if ((level - 1) % 4 == 1)
                {
                    page6Source.AppendLine(DumpScalar("mpod" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.EscapePod).ObjectEncodings));
                }
                page6Source.AppendLine(DumpScalar("outi" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.OutTime).ObjectEncodings));
                page6Source.AppendLine(DumpScalar("reaz" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.ReactorSize).ObjectEncodings));
                page6Source.AppendLine(DumpScalar("oxyb" + GetMazeCode(level), dataPosition, commentPosition, EncodeObjects(maze, EncodingGroup.OxygenReward).ObjectEncodings));

                List<HiddenLevelToken> tokensInMaze = maze.MazeObjects
                                                          .OfType<HiddenLevelToken>()
                                                          .ToList();

                foreach ( HiddenLevelToken token in tokensInMaze )
                {
                    tokens[ (int)token.TokenStyle ] =
                        new KeyValuePair<int, HiddenLevelToken>( level - 1, token );
                }
            }

            /// Pull out Hidden Level Token info that isn't on every level.
            StringBuilder tokenSource = new StringBuilder();
            List<ObjectEncoding> tokenEncodings = new List<ObjectEncoding>();
            for (int i = 0; i < tokens.Length; i++)
            {
                KeyValuePair<int,HiddenLevelToken> tokenInfo = tokens[i];
                if (tokenInfo.Value == null)
                {
                    tokenEncodings.Add(new ObjectEncoding(HiddenLevelToken.EmptyBytes.ToList()));
                }
                else
                {
                    List<byte> tokenBytes = new List<byte>();
                    tokenBytes.Add((byte)tokenInfo.Key);
                    tokenBytes.AddRange(tokenInfo.Value.ToBytes());
                    tokenEncodings.Add(new ObjectEncoding(tokenBytes));
                }
            }

            tokenSource.AppendLine( DumpBytes( "mtok" + "", dataPosition, commentPosition, 8, tokenEncodings) );

            switch (sourceFile)
            {
                case SourceFile.Page6:
                    return page6Source.ToString();
                case SourceFile.Page7:
                    return page7Source.ToString();
                case SourceFile.Token:
                    return tokenSource.ToString();
                case SourceFile.Cannon:
                    return cannonSource.ToString();
                case SourceFile.MazeMessages:
                    return mazeMessageSource.ToString();
            }
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(cannonSource.ToString());
            //sb.AppendLine(tokenSource.ToString());
            //sb.AppendLine(page6Source.ToString());
            //sb.AppendLine(page7Source.ToString());
            //return sb.ToString();
            return null;
        }

        private void AddHints(string hint1, string hint2, StringBuilder sb, int dataPosition, int level)
        {
            string prefix = "mzh";
            int hint1YPosition = 50;
            int hint2YPosition = 48;
            string label = prefix + GetMazeCode(level);
            AddHint(hint1, label + "a", hint1YPosition, sb, dataPosition);
            AddHint(hint2, label + "b", hint2YPosition, sb, dataPosition);
        }

        private void AddHint(string hint, string label, int yPosition, StringBuilder sb, int dataPosition)
        {
            if (!String.IsNullOrWhiteSpace(hint))
            {
                int position = 0 - (hint.Length * 3);
                string xPositionHex = position.ToString("X8").Substring(6, 2);
                if (position < -128)
                {
                    //cant go too far off left side of screen
                    xPositionHex = "80";
                }
                StringBuilder mb = new StringBuilder();
                mb.Append(label);
                Tabify(' ', dataPosition, mb);
                mb.Append(".ctext \"");
                mb.Append(hint);
                mb.Append("\"");
                sb.AppendLine(mb.ToString());
                //new line
                mb.Clear();
                Tabify(' ', dataPosition, mb);
                mb.Append("czmess(" + label + ",$" + yPosition.ToString() + "," + label + "_)");
                sb.AppendLine(mb.ToString());
            }
        }

        private string DumpScalar(string label, int dataPosition, int commentPosition, List<ObjectEncoding> encodings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(label);
            Tabify(' ', dataPosition, sb);
            sb.Append("= $");
            if (encodings.Count > 0)
            {
                sb.Append(encodings[0].Bytes[0].ToString("X2"));
            }
            return sb.ToString();
        }

        private void Tabify(char c, int len, StringBuilder sb)
        {
            if (sb.Length < len)
            {
                sb.Append(new String(c, len - sb.Length));
            }
        }

        private string GetMazeCode(int level)
        {
            int difficulty = (level - 1) / 4;
            int mazeNumber = (level - 1) % 4;
            return difficulty.ToString() + mazeNumber.ToString();
        }

        private string DumpBytes(string label, int dataPosition, int commentPosition, int dataWidth, List<ObjectEncoding> encodings)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < encodings.Count; i++)
            {
                ObjectEncoding encoding = encodings[i];
                StringBuilder eb = new StringBuilder();

                if (!String.IsNullOrEmpty(label) && i == 0)
                {
                    eb.Append(label);
                    eb.Append(" ");
                }

                if (eb.Length < dataPosition)
                {
                    eb.Append(new string(' ', dataPosition - eb.Length));
                }

                if (!String.IsNullOrEmpty(encoding.SourceMacro))
                {
                    eb.Append(encoding.SourceMacro);
                    if (eb.Length < commentPosition)
                    {
                        eb.Append(new string(' ', commentPosition - eb.Length));
                    }
                    eb.Append(";");
                    eb.Append(encoding.Comment);
                }
                else
                {
                    int skip = 0;
                    byte[] bytes = encoding.Bytes.ToArray();
                    while (skip < bytes.Length)
                    {
                        string[] rowBytes = bytes.Skip(skip).Take(16).Select(b => ("$" + b.ToString("X2"))).ToArray();
                        if (eb.Length < dataPosition)
                        {
                            eb.Append(new string(' ', dataPosition - eb.Length));
                        }
                        eb.Append(".db ");
                        eb.Append(String.Join(",", rowBytes));
                        skip += dataWidth;
                        if (skip >= bytes.Length)
                        {
                            //this is the last line of bytes... add the comment
                            if (eb.Length < commentPosition)
                            {
                                eb.Append(new string(' ', commentPosition - eb.Length));
                            }
                            if (!String.IsNullOrEmpty(encoding.Comment))
                            {
                                eb.Append(";");
                                eb.Append(encoding.Comment);
                            }
                        }
                    }
                }
                sb.AppendLine(eb.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mazeToStartOn">If specified, will make this the starting level when ROMs are generated</param>
        public void SetStartingMaze( int mazeToStartOn = 0 )
        {
            //****************
            //set up starting level
            //****************
            WriteAlphaHigh((ushort)(_exports["levelst"] + 1), (byte)mazeToStartOn);
        }

        /// <summary>
        /// Encodes all mazes in passed collection into EncodingObjects and sets the starting level to the passed maze object. 
        /// </summary>
        /// <param name="mazeCollection">The collection to encode</param>
        /// <returns></returns>
        public bool EncodeObjects(MazeCollection mazeCollection)
        {
            int numMazes = 28;

            if (mazeCollection.Mazes.Count > numMazes)
            {
                _lastError = "Maze collection contained more than " + numMazes.ToString() + " mazes.";
                return false;
            }
            //serialization in The Promised End is a little more fluid... in general the steps are
            // 1. Iterate the Major Object Pointer tables
            //    a. For each Level (24 in MHTPE)
            //       i.  Write the current pointer (object data start)
            //       ii. Write the data
            // 2. Verify no strange boundaries were crossed
            // 3. Update the CSUM's on each ROM that was written.

            int currentAddressPage7 = _exports["messagesbase"];
            //Maze Hints
            byte messageIndexer = 0;
            KeyValuePair<int, HiddenLevelToken>[] tokens = new KeyValuePair<int, HiddenLevelToken>[4];
            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i] != null)
                {
                    //Write Table Pointer - First Hint
                    if (!String.IsNullOrEmpty(mazeCollection.Mazes[i].Hint))
                    {
                        //update pointers and locations
                        WritePagedROM((ushort)_exports["zmessptrl"], new byte[] { (byte)(currentAddressPage7 & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessptrh"], new byte[] { (byte)((currentAddressPage7 >> 8) & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessypos"], new byte[] { 0x50 }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessxpos"], new byte[] { GetTextPosition(mazeCollection.Mazes[i].Hint) }, messageIndexer, 7);
                        //write the data stream
                        currentAddressPage7 += WritePagedROM((ushort)currentAddressPage7, GetBytesFromString(mazeCollection.Mazes[i].Hint), 0, 7);
                        //finally, book the index and increment
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { messageIndexer }, (i * 2) , 7);
                        messageIndexer++;
                    }
                    else
                    {
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { 0xff }, (i * 2), 7);
                    }
                    //Write Table Pointer - Second Hint
                    if (!String.IsNullOrEmpty(mazeCollection.Mazes[i].Hint2))
                    {
                        //update pointers and locations
                        WritePagedROM((ushort)_exports["zmessptrl"], new byte[] { (byte)(currentAddressPage7 & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessptrh"], new byte[] { (byte)((currentAddressPage7 >> 8) & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessypos"], new byte[] { 0x48 }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessxpos"], new byte[] { GetTextPosition(mazeCollection.Mazes[i].Hint2) }, messageIndexer, 7);
                        //write the data stream
                        currentAddressPage7 += WritePagedROM((ushort)currentAddressPage7, GetBytesFromString(mazeCollection.Mazes[i].Hint2), 0, 7);
                        //finally, book the index and increment
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { messageIndexer }, (i * 2)+ 1, 7);
                        messageIndexer++;
                    }
                    else
                    {
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { 0xff }, (i * 2)+ 1, 7);
                    }

                    List<HiddenLevelToken> tokensInMaze = mazeCollection.Mazes[ i ].MazeObjects
                                                                  .OfType<HiddenLevelToken>()
                                                                  .ToList();

                    foreach ( HiddenLevelToken token in tokensInMaze )
                    {
                        tokens[ (int)token.TokenStyle ] =
                            new KeyValuePair<int, HiddenLevelToken>( i, token );
                    }
                }
            }

            int pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i] != null)
                {
                    //Reactoid, Pyroid, Perkoids and Maxoids
                    pointerIndex += WritePagedROM((ushort)_exports["mzinit"], WordToByteArray(currentAddressPage7), pointerIndex, 7);
                    currentAddressPage7 += WritePagedROM((ushort)currentAddressPage7, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.ReactoidPyroidPerkoidMax).GetAllBytes().ToArray(), 0, 7);
                }
            }

            //********************************
            // Page 6 Dynamic Now
            //********************************
            int currentAddressPage6 = _exports["dynamic_base"];
            Dictionary<byte, ushort> existingOxoids = new Dictionary<byte, ushort>();
            pointerIndex = 0;
            //****************
            //Maze Type
            //****************
            int typeIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                typeIndex += WritePagedROM((ushort)_exports["mzty"], EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.MazeType).GetAllBytes().ToArray(), typeIndex, 6);
            }
            //****************
            //Hidden Level Token
            //****************
            List<byte> allTokenBytes = new List<byte>();
            List<ObjectEncoding> tokenEncodings = new List<ObjectEncoding>();
            for (int i = 0; i < tokens.Length; i++)
            {
                KeyValuePair<int, HiddenLevelToken> tokenInfo = tokens[i];
                if (tokenInfo.Value == null)
                {
                    tokenEncodings.Add(new ObjectEncoding(HiddenLevelToken.EmptyBytes.ToList()));
                }
                else
                {
                    List<byte> tokenBytes = new List<byte>();
                    tokenBytes.Add((byte)tokenInfo.Key);
                    tokenBytes.AddRange(tokenInfo.Value.ToBytes());
                    tokenEncodings.Add(new ObjectEncoding(tokenBytes));
                }
            }
            foreach(ObjectEncoding encoding in tokenEncodings)
            {
                allTokenBytes.AddRange(encoding.Bytes);
            }
            WritePagedROM(_exports["mtok"], allTokenBytes.ToArray(), 0, 6); 

            //****************
            //Oxoid data
            //****************
            for (int i = 0; i < numMazes; i++)
            {
                byte[] oxoidBytes = EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Oxoids).GetAllBytes().ToArray();
                byte byteHash = GetByteHash(oxoidBytes);
                if (existingOxoids.ContainsKey(byteHash))
                {
                    //already defined, use this pointer
                    pointerIndex += WritePagedROM((ushort)_exports["mzdc"], WordToByteArray(existingOxoids[byteHash]), pointerIndex, 6);
                }
                else //didn't exist, write it out
                {
                    //Write Table Pointer
                    pointerIndex += WritePagedROM((ushort)_exports["mzdc"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                    //save to dictionary
                    existingOxoids.Add(byteHash, (ushort)currentAddressPage6);
                    currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, oxoidBytes, 0, 6);
                }
            }
            //****************
            //Lightning
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mzlg"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Lightning).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Arrows
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mzar"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                //Arrow data
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Arrows).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Exit Arrows
            //****************
            pointerIndex = 0;
            //create a placeholder pointer which is for blank levels
            int blankOutArrowsPointer = currentAddressPage6;
            pointerIndex += WritePagedROM((ushort)_exports["mzor"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
            currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, new byte[] { 0x00 }, 0, 6);
            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<ArrowOut>().Count() > 0) {
                    //Write Table Pointer
                    pointerIndex += WritePagedROM((ushort)_exports["mzor"], WordToByteArray(currentAddressPage6), (i * 2), 6);
                    currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.ArrowsOut).GetAllBytes().ToArray(), 0, 6);
                }
                else
                {
                    //no out arrows, point to the blank entry defined at the top
                    pointerIndex += WritePagedROM((ushort)_exports["mzor"], WordToByteArray(blankOutArrowsPointer), (i * 2), 6);
                }
            }
            //****************
            //Trip Points
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztr"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.TripPoints).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Trip Actions
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["trtbll"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.TripActions).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Static Maze Walls
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztdal"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.StaticWalls).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Dynamic Maze Walls
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztd"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.DynamicWalls).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //One Way Walls
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mone"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OneWay).GetAllBytes().ToArray(), 0, 6);
            }

            //****************
            //Cannon
            //****************
            Dictionary<int, int> cannonLevelPointers = new Dictionary<int, int>();
            Dictionary<Guid, int> cannonDataPointers = new Dictionary<Guid, int>();

            //first build up each cannon and mark the address for it
            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>().Count() > 0)
                {
                    cannonLevelPointers.Add(i, currentAddressPage6);
                    //Levels with multiple cannons will all return in this single collection, but they will be grouped by A,B,C or D
                    //to identify indivdual cannons
                    var cannonGroupEncodings = from e in EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.IonCannon).ObjectEncodings
                                               group e by e.Group into g
                                               select new { Id = g.Key, Encodings = g.ToList()};
                    
                    foreach (var g in cannonGroupEncodings)
                    {
                        List<byte> encodedBytes = new List<byte>();
                        //cannonDataPointers.Add(cannon.Id, currentAddressPage6);
                        foreach (var encodingGroup in g.Encodings)
                        {
                            encodedBytes.AddRange(encodingGroup.Bytes);
                        }
                        cannonDataPointers.Add(Guid.Parse(g.Id), currentAddressPage6);
                        currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, encodedBytes.ToArray(), 0, 6);
                    }
                }
            }
            //now build Indexes and Pointers
            for (int i = 0; i < numMazes; i++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (c < mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>().Count())
                    {
                        IonCannon cannon = mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>().ToList()[c];
                        //valid cannon found here, write it
                        currentAddressPage6 += WritePagedROM((ushort)_exports["mcan"], WordToByteArray(cannonDataPointers[cannon.Id]), (i * 8) + (c * 2), 6);
                    }
                    else
                    {
                        //no cannon defined in this slot, write 
                        WritePagedROM((ushort)_exports["mcan"], new byte[] { 0x00, 0x00}, (i * 8) + (c * 2), 6);
                    }
                }
            }
            //****************
            //Spikes
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mtite"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Spikes).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //locks and keys, for now, there has to be an even number of locks and keys
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mlock"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.LocksKeys).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Transporters
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mtran"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Transporters).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //De Hand
            //****************
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mhand"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Hand).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Clock
            //****************
            int clockIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                clockIndex += WritePagedROM((ushort)_exports["mclock"], EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Clock).GetAllBytes().ToArray(), clockIndex, 6);
            }
            //****************
            //Boots
            //****************
            int bootsIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                bootsIndex += WritePagedROM((ushort)_exports["mboots"], EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Boots).GetAllBytes().ToArray(), bootsIndex, 6);
            }
            //****************
            //KeyPouch
            //****************
            int pouchIndex = 0;
            for ( int i = 0; i < numMazes; i++ )
            {
                pouchIndex += WritePagedROM((ushort)_exports["mkeyp"], EncodeObjects( mazeCollection.Mazes[i], EncodingGroup.KeyPouch).GetAllBytes().ToArray(), pouchIndex, 6);
            }
            //****************
            //Escape Pod
            //****************
            int mpodAddressBase = _exports["mpod"];
            for (int i = 1; i < numMazes; i+=4)
            {
                mpodAddressBase += WritePagedROM((ushort)mpodAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.EscapePod).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Out Time
            //****************
            int outAddressBase = _exports["outime"];
            for (int i = 0; i < numMazes; i++)
            {
                outAddressBase += WritePagedROM((ushort)outAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OutTime).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //Reactor Size
            //****************
            int rsizeAddressBase = _exports["reacsz"];
            for (int i = 0; i < numMazes; i++)
            {
                rsizeAddressBase += WritePagedROM((ushort)rsizeAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.ReactorSize).GetAllBytes().ToArray(), 0, 6);
            }
            //****************
            //OxygenReward
            //****************
            int oxyAddressBase = _exports["oxybonus"];
            for (int i = 0; i < numMazes; i++)
            {
                oxyAddressBase += WritePagedROM((ushort)oxyAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OxygenReward).GetAllBytes().ToArray(), 0, 6);
            }
            //*******************
            // Quality Checking
            //*******************
            if (currentAddressPage6 >= 0x4000)
            {
                //this is bad
                return false;
            }
            if (currentAddressPage7 >= 0x4000 )
            {
                //this is bad, it means we have overflowed our Paged ROM end boundary
                return false;
            }
            return true;
        }

        private byte GetByteHash(byte[] bytes)
        {
            byte hash = 0;
            foreach(byte b in bytes)
            {
                hash ^= b;
            }
            return hash;
        }

        public enum EncodingGroup : int
        {
            ReactoidPyroidPerkoidMax,
            Oxoids,
            Lightning,
            Arrows,
            ArrowsOut,
            TripPoints,
            TripActions,
            StaticWalls,
            DynamicWalls,
            OneWay,
            IonCannon,
            Spikes,
            LocksKeys,
            Transporters,
            Hand,
            Clock,
            Boots,
            EscapePod,
            OutTime,
            OxygenReward,
            MazeType,
            KeyPouch,
            ReactorSize,
            HiddenLevelToken
        }

        /// <summary>
        /// Used to encode individual maze object types, I had to abstract it so that it was easy to
        /// pull out serialized data one type at a time so it was easy to get data from the designed mazes
        /// into the source code of the main mhavocpe project.
        /// </summary>
        /// <param name="maze">The target maze</param>
        /// <param name="group">The target group to encode</param>
        /// <returns></returns>
        public ObjectEncodingCollection EncodeObjects(Maze maze, EncodingGroup group)
        {
            ObjectEncodingCollection encodings = new ObjectEncodingCollection();

            Reactoid reactoid = null;
            if (maze != null)
            {
                reactoid = maze.MazeObjects.OfType<Reactoid>().FirstOrDefault();
            }

            switch (group)
            {
                case EncodingGroup.ReactoidPyroidPerkoidMax:
                    //Reactoid
                    if (reactoid != null)
                    {
                        encodings.Add(reactoid.ToBytes(reactoid.Position), reactoid.Name);
                        foreach (Pyroid pyroid in maze.MazeObjects.OfType<Pyroid>())
                        {
                            encodings.Add(pyroid.ToBytes(), pyroid.Name);
                        }
                        //Perkoids
                        if (maze.MazeObjects.OfType<Perkoid>().Count() > 0)
                        {
                            encodings.Add(0xfe);
                            foreach (Perkoid perkoid in maze.MazeObjects.OfType<Perkoid>())
                            {
                                encodings.Add(perkoid.ToBytes(), perkoid.Name);
                            }
                        }
                        //Maxoids
                        if (maze.MazeObjects.OfType<Maxoid>().Count() > 0)
                        {
                            //make sure we did perkoids already
                            if (maze.MazeObjects.OfType<Perkoid>().Count() == 0)
                            {
                                encodings.Add(0xfe, "No Robots");
                            }
                            encodings.Add(0xfe, "End Robots");
                            foreach (Maxoid max in maze.MazeObjects.OfType<Maxoid>())
                            {
                                encodings.Add(max.ToBytes(), max.Name);
                            }
                        }
                        encodings.Add(0xff); //Data End Flag
                    }
                    break;
                case EncodingGroup.MazeType:
                    encodings.Add((byte)maze.MazeType, Enum.GetName(typeof(MazeType),maze.MazeType));
                    break;
                case EncodingGroup.Oxoids:
                    List<byte> oxygenBytes = new List<byte>();
                    foreach (Oxoid oxoid in maze.MazeObjects.OfType<Oxoid>())
                    {
                        oxygenBytes.AddRange(oxoid.ToBytes());
                    }
                    encodings.Add(oxygenBytes.ToArray(), "Oxygen");
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.Lightning:
                    if ((maze.MazeObjects.OfType<LightningH>().Count() > 0) || (maze.MazeObjects.OfType<LightningV>().Count() > 0))
                    {
                        foreach (LightningH lightningH in maze.MazeObjects.OfType<LightningH>())
                        {
                            encodings.Add(lightningH.ToBytes(), "H" + lightningH.Name);
                        }
                        if (maze.MazeObjects.OfType<LightningV>().Count() > 0)
                        {
                            encodings.Add(0xff);
                            foreach (LightningV lightningV in maze.MazeObjects.OfType<LightningV>())
                            {
                                encodings.Add(lightningV.ToBytes(), "V" + lightningV.Name);
                            }
                        }
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.Arrows:
                    foreach (Arrow arrow in maze.MazeObjects.OfType<Arrow>())
                    {
                        encodings.Add(arrow.ToBytes(), arrow.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.ArrowsOut:
                    foreach (ArrowOut arrow in maze.MazeObjects.OfType<ArrowOut>())
                    {
                        encodings.Add(arrow.ToBytes(), arrow.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.TripPoints:
                    //Trip Point data
                    foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
                    {
                        encodings.Add(trip.ToBytes(), trip.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.TripActions:
                    //Trip Action Data
                    foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
                    {
                        encodings.Add(trip.Pyroid.ToBytes(), trip.Name + "(pyroid)");
                    }
                    encodings.Add(new byte[] { 0x00, 0x00, 0x00 });
                    break;
                case EncodingGroup.StaticWalls:
                    //Wall data, all walls in maze
                    foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => !w.IsDynamicWall))
                    {
                        encodings.Add(wall.ToBytes(maze), wall.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.DynamicWalls:
                    //wall data, only dynamic walls
                    foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
                    {
                        encodings.Add(wall.ToBytes(maze), wall.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.OneWay:
                    foreach (OneWay wall in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
                    {
                        encodings.Add(wall.ToBytes(maze), wall.Name + "(right)");
                    }
                    if (maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left).Count() > 0)
                    {
                        foreach (OneWay wall in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
                        {
                            encodings.Add(0xff);
                            encodings.Add(wall.ToBytes(maze), wall.Name + "(left)");
                        }
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.IonCannon:
                    for (int i = 0; i < maze.MazeObjects.OfType<IonCannon>().Count(); i++)
                    {
                        IonCannon cannon = maze.MazeObjects.OfType<IonCannon>().ToArray()[i];
                        //Position first
                        byte[] positionBytes = (DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(cannon.Position)));
                        encodings.Add(positionBytes, "Position", cannon.Id.ToString(), ".dw $" + BytesToWord(positionBytes[0], positionBytes[1]).ToString("X4") + ",$" + BytesToWord(positionBytes[2], positionBytes[3]).ToString("X4"));
                        foreach (IonCannonInstruction instruction in cannon.Program)
                        {
                            Tuple<string, string> commentMacro = GetCannonCommentMacro(instruction);
                            List<byte> instructionBytes = new List<byte>();
                            instruction.GetObjectData(instructionBytes);
                            encodings.Add(instructionBytes.ToArray(), commentMacro.Item1, cannon.Id.ToString(), commentMacro.Item2);
                        }
                    }
                    break;
                case EncodingGroup.Spikes:
                    foreach (Spikes spike in maze.MazeObjects.OfType<Spikes>())
                    {
                        encodings.Add(spike.ToBytes(), spike.Name);
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.LocksKeys:
                    foreach (Lock lock_ in maze.MazeObjects.OfType<Lock>())
                    {
                        Key thisKey = maze.MazeObjects.OfType<Key>().Where(k => k.KeyColor == lock_.LockColor).FirstOrDefault();
                        if (thisKey != null)
                        {
                            encodings.Add(lock_.ToBytes(thisKey), Enum.GetName(typeof(ObjectColor), lock_.LockColor));
                        }
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.Transporters:

                    var transporterGroups = maze.MazeObjects.OfType<Transporter>().GroupBy(t => t.Color).Select(tg => new { Key = tg.Key, Count = tg.Count() });

                    foreach (var transporterPair in transporterGroups)
                    {
                        List<Transporter> coloredTranporterMatches = maze.MazeObjects.OfType<Transporter>().Where(t => t.Color == transporterPair.Key).ToList();
                        foreach (Transporter t in coloredTranporterMatches)
                        {
                            List<string> flags = new List<string>();
                            flags.Add("col" + Enum.GetName(typeof(ObjectColor), t.Color).ToLower());
                            if (t.Direction == TransporterDirection.Right)
                            {
                                flags.Add("tr_right");
                            }
                            else
                            {
                                flags.Add("tr_left");
                            }
                            if (t.IsHidden)
                            {
                                flags.Add("tr_hidden");
                            }
                            if (t.IsSpecial)
                            {
                                flags.Add("tr_special");
                            }
                            StringBuilder tMacro = new StringBuilder();
                            tMacro.Append(".db (");
                            tMacro.Append(String.Join("+", flags.ToArray()));
                            tMacro.Append("),$");
                            tMacro.Append(t.ToBytes()[1].ToString("X2"));
                            encodings.Add(t.ToBytes(), Enum.GetName(typeof(ObjectColor), t.Color), "", tMacro.ToString());
                        }
                    }
                    //write end of transports
                    encodings.Add(0x00);
                    //write transportability data
                    if (maze.MazeObjects.OfType<Pyroid>().Where(p => p.IsTransportable).Count() > 0
                        || maze.MazeObjects.OfType<IonCannon>().Where(c => c.IsShotTransportable).Count() > 0
                        || maze.MazeObjects.OfType<Perkoid>().Where(p => p.IsTransportable || p.IsShotTransportable).Count() > 0)
                    {
                        ulong bitValues = 0;
                        //something needs to be serialized, do Pyroids first
                        List<Pyroid> pyroids = maze.MazeObjects.OfType<Pyroid>().ToList();
                        for( int p = 0; p < pyroids.Count; p++)
                        {
                            bitValues |= ((pyroids[p].IsTransportable ? (ulong)1 : 0) << (p + 0x02));
                        }
                        //Ion Cannon Shots
                        List<IonCannon> cannons = maze.MazeObjects.OfType<IonCannon>().Where(c => c.IsShotTransportable).ToList();
                        //Ion Cannon shots are all or none
                        if (cannons.Count > 0)
                        {
                            for (int i = 0x12; i <= 0x19; i++)
                            {
                                bitValues |=(((ulong)1) << (i));
                            }
                        }
                        //Perkoids
                        List<Perkoid> perkoids = maze.MazeObjects.OfType<Perkoid>().ToList();
                        for (int p = 0; p < perkoids.Count; p++)
                        {
                            bitValues |= ((perkoids[p].IsTransportable ? (ulong)1 : 0) << (p + 0x1e));
                            bitValues |= ((perkoids[p].IsShotTransportable ? (ulong)1 : 0) << (p + 0x28));
                        }
                        //Bits are defined, now convert into bytes...
                        List<byte> transportabilityBytes = new List<byte>();
                        for(int i = 0; i < 7; i++)
                        {
                            if (bitValues == 0)
                            {
                                break;
                            }
                            byte tb = (byte)((bitValues & ((ulong)0x0000000FF)));
                            transportabilityBytes.Add(tb);
                            bitValues = bitValues >> 8;
                        }
                        encodings.Add(transportabilityBytes.ToArray(), "Transportability Flags");
                    }
                    encodings.Add(0xee, "Transportability Flags");
                    break;
                case EncodingGroup.Hand:
                    Hand hand = maze.MazeObjects.OfType<Hand>().FirstOrDefault();
                    if (hand != null)
                    {
                        Reactoid r = maze.MazeObjects.OfType<Reactoid>().FirstOrDefault();
                        if (r != null)
                        {
                            encodings.Add(hand.ToBytes(r), hand.Name);
                        }
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.Clock:
                    //Clock Data
                    Clock clock = maze.MazeObjects.OfType<Clock>().FirstOrDefault();
                    if (clock != null)
                    {
                        encodings.Add(clock.ToBytes(), clock.Name);
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.Boots:
                    //Boots Data
                    Boots boots = maze.MazeObjects.OfType<Boots>().FirstOrDefault();
                    if (boots != null)
                    {
                        encodings.Add(boots.ToBytes(), boots.Name);
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.KeyPouch:
                    //KeyPouch Data
                    KeyPouch keyPouch = maze.MazeObjects.OfType<KeyPouch>().FirstOrDefault();
                    if ( keyPouch != null)
                    {
                        encodings.Add( keyPouch.ToBytes(), keyPouch.Name );
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.EscapePod:
                    //Pod Data
                    EscapePod pod = maze.MazeObjects.OfType<EscapePod>().FirstOrDefault();
                    if (pod != null)
                    {
                        encodings.Add(pod.ToBytes(), pod.Name);
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.OutTime:
                    //Maze Escape Time
                    int reactorTimer = 0;
                    if (reactoid != null)
                    {
                        reactorTimer = DataConverter.ToDecimal(reactoid.Timer);
                    }
                    encodings.Add((byte)reactorTimer, "MazeTime");
                    break;
                case EncodingGroup.ReactorSize:
                    //Reactor Size
                    int reactorSize = 0;
                    if (reactoid != null)
                    {
                        if (reactoid.MegaReactoid)
                        {
                            reactorSize = 1;
                        }
                    }
                    encodings.Add((byte)reactorSize, "ReactorSize");
                    break;
                case EncodingGroup.OxygenReward:
                    encodings.Add((byte)maze.OxygenReward, "O2 Reward");
                    break;
                case EncodingGroup.HiddenLevelToken:
                    foreach ( var token in maze.MazeObjects.OfType<HiddenLevelToken>() )
                    {
                        encodings.Add( token.ToBytes(), $"{token.TokenStyle}" );
                    }
                    break;
            }

            return encodings;
        }

        private Tuple<string,string> GetCannonCommentMacro(IonCannonInstruction instruction)
        {
            StringBuilder mb = new StringBuilder();
            StringBuilder cb = new StringBuilder();

            switch (instruction.Command)
            {
                case Commands.ReturnToStart:
                    mb.Append("cann_end");
                    cb.Append("Loop It");
                    break;
                case Commands.OrientAndFire:
                    string positionText = "";
                    string positionMacro = "";
                    switch (((OrientAndFire)instruction).Orientation)
                    {
                        case Orientation.UpRight:
                            positionText = "TopRight";
                            positionMacro = "canp_tr";
                            break;
                        case Orientation.Right:
                            positionText = "MidRight";
                            positionMacro = "canp_mr";
                            break;
                        case Orientation.DownRight:
                            positionText = "BotRight";
                            positionMacro = "canp_br";
                            break;
                        case Orientation.Down:
                            positionText = "Down";
                            positionMacro = "canp_dn";
                            break;
                        case Orientation.UpLeft:
                            positionText = "TopLeft";
                            positionMacro = "canp_tl";
                            break;
                        case Orientation.Left:
                            positionText = "MidLeft";
                            positionMacro = "canp_ml";
                            break;
                        case Orientation.DownLeft:
                            positionText = "BotLeft";
                            positionMacro = "canp_bl";
                            break;
                    }


                    mb.Append("cann_pos(");
                    mb.Append(positionMacro);
                    mb.Append(",");
                    mb.Append(((int)((OrientAndFire)instruction).RotateSpeed).ToString());
                    mb.Append(",");
                    if (((OrientAndFire)instruction).ShotSpeed > 0)
                    {
                        mb.Append("1,");
                        mb.Append(((OrientAndFire)instruction).ShotSpeed.ToString());
                    }
                    else
                    {
                        mb.Append("0,0");
                    }
                    mb.Append(")");

                    cb.Append("GunPos - ");
                    cb.Append(positionText);
                    cb.Append(" Speed: ");
                    cb.Append(((int)((OrientAndFire)instruction).RotateSpeed).ToString());
                    cb.Append(" Shot: ");
                    cb.Append(((OrientAndFire)instruction).ShotSpeed > 0 ? "Yes" : "No");
                    if (((OrientAndFire)instruction).ShotSpeed > 0)
                    {
                        cb.Append(" ShotVel: $");
                        cb.Append(((OrientAndFire)instruction).ShotSpeed.ToString("X2"));
                    }
                    break;
                case Commands.Move:
  
                    mb.Append("cann_loc(");
                    mb.Append(((Move)instruction).WaitFrames.ToString());
                    mb.Append(",");
                    mb.Append(((Move)instruction).Velocity.X.ToString());
                    mb.Append(",");
                    mb.Append(((Move)instruction).Velocity.Y.ToString());
                    mb.Append(")");

                    cb.Append("GunLoc - Frames: ");
                    cb.Append(((Move)instruction).WaitFrames.ToString());
                    cb.Append(" XVel: ");
                    cb.Append(((Move)instruction).Velocity.X.ToString());
                    cb.Append(" YVel: ");
                    cb.Append(((Move)instruction).Velocity.Y.ToString());
                    //sb.AppendLine(prefix + "cann_loc(" + frames.ToString() + "," + xVel.ToString() + "," + yVel.ToString() + ")\t;GunLoc - Frames: " + frames.ToString() + " XVel: " + xVel.ToString() + " YVel: " + yVel.ToString());
                    break;
                case Commands.Pause:
                    mb.Append("cann_pau(" + ((Pause)instruction).WaitFrames.ToString() + ")");
                    cb.Append("Pause = " + ((Pause)instruction).WaitFrames.ToString() + " frames");
                    break;
            }
            return new Tuple<string, string>( cb.ToString(), mb.ToString());
        }
        
    }
}
