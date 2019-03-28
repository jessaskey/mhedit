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
    public class MajorHavocPromisedEnd : GameController, IGameController
    {

        #region Private Variables

        private string _templatePath = String.Empty;
        //private string _mamePath = String.Empty;
        private byte[] _page2367 = new byte[0x8000];
        private byte[] _alphaHigh = new byte[0x4000];
        private Dictionary<string, ushort> _exports = new Dictionary<string, ushort>();
        private string _page2367ROM = "mhpe.1np";
        private string _alphaHighROM = "mhpe.1l";

        private string _lastError = "";

        #endregion


        public MajorHavocPromisedEnd(string templatePath)
        {
            _templatePath = templatePath;
            LoadTemplate(_templatePath);
        }

        public string Name
        {
            get { return "Major Havoc Promised End"; }
            set { }
        }

        public string LastError
        {
            get { return _lastError; }
        }

        public Version GetROMVersion()
        {
            byte[] versionBytes = ReadPagedROM(0x2002, 0, 2, 6);
            return new Version(versionBytes[0], versionBytes[1],0,0);
        }

        private void LoadTemplate(string templatePath)
        {
            //load up our roms for now...
            try
            {
                _page2367 = File.ReadAllBytes(Path.Combine(templatePath,_page2367ROM));
                _alphaHigh = File.ReadAllBytes(Path.Combine(templatePath,_alphaHighROM));
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Page6/7: " + Exception.Message);
            }

            Version romVersion = GetROMVersion(); 
            if (romVersion.Major >= 0)
            {
                if (romVersion.Minor >= 0x22)
                {
                    //load our exports
                    string exportFile = Path.Combine(templatePath, "mhavocpe.exp");
                    if (File.Exists(exportFile))
                    {
                        string[] exportLines = File.ReadAllLines(exportFile);
                        foreach (string exportLine in exportLines)
                        {
                            string[] def = exportLine.Replace(" ", "").Replace("\t", "").Replace(".EQU", "|").Split('|');
                            if (def.Length == 2)
                            {
                                int value = int.Parse(def[1].Replace("$", ""), System.Globalization.NumberStyles.HexNumber);
                                _exports.Add(def[0], (ushort)value);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("ROM Version has to be 0.22 or higher.");
                }
            }

        }

        public MazeCollection LoadMazes(List<string> loadMessages)
        {

            MazeCollection mazeCollection = new MazeCollection("Promised End Mazes");
            mazeCollection.AuthorEmail = "Jess@maynard.vax";
            mazeCollection.AuthorName = "Jess Askey";

            for ( int i = 0; i < 28; i++)
            {

                byte mazeType = (byte)(i & 0x03);

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

                //transporters
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
                    transporter.IsBroken = ((colorValue & 0x40) > 0);
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
                        transportabilityValue = transportabilityValue << 1;
                        transportabilityData.Add((transportabilityValue & 0x100) != 0);
                    }
                    transporterBaseAddress++;
                    transportabilityValue = ReadByte(transporterBaseAddress, 0, 6);
                }
                if (transportabilityData.Count > 0)
                {
                    maze.TransportabilityFlags = transportabilityData;
                }

                //Laser IonCannon
                // Ok, So looking at why the cannons are shifted down on level 16.
                // The issue is that the cannon goes up and down. The key is where
                // the cannon starts with respect to the ceiling. Cannons 2 and 3
                // start low (closer to the floor) than all others.
                // I need to figure out how that's encoded.
                byte cannonAddressOffset = ReadByte(_exports["mcan"], i, 6);
                if (cannonAddressOffset != 0)
                {
                    ushort cannonBaseAddress = (ushort)(_exports["mcanst"] + cannonAddressOffset);
                    ushort cannonPointerAddress = ReadWord(cannonBaseAddress, 0, 6);

                    while (cannonPointerAddress != 0)
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
                                    int fireBit = (commandStartByte & 0x01);
                                    if (fireBit > 0)
                                    {
                                        cannonCommandOffset++;
                                        cannonPosition.ShotSpeed = (byte)ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                                    }
                                    cannon.Program.Add(cannonPosition);
                                    break;
                                case Commands.Move:     //Move Position
                                    Move cannonMovement = new Move();
                                    int waitFrames = (commandStartByte & 0x3F) << 2;
                                    cannonMovement.WaitFrames = waitFrames;
                                    if (waitFrames > 0)
                                    {
                                        cannonMovement.Velocity.X = (sbyte)ReadByte( cannonPointerAddress, ++cannonCommandOffset, 6 );
                                        cannonMovement.Velocity.Y = (sbyte)ReadByte( cannonPointerAddress, ++cannonCommandOffset, 6 );
                                    }
                                    //cannonMovement.
                                    cannon.Program.Add(cannonMovement);
                                    break;
                                case Commands.Pause:     //Pause
                                    Pause cannonPause = new Pause();
                                    cannonPause.WaitFrames = (commandStartByte & 0x3F) << 2;
                                    cannon.Program.Add(cannonPause);
                                    break;
                            }
                            cannonCommandOffset++;
                            commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                        }
                        maze.AddObject(cannon);

                        cannonBaseAddress += 2;
                        cannonPointerAddress = ReadWord(cannonBaseAddress, 0, 6);
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

                    int velocity = (byte)(vdata & 0x7f);
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
                    tpp.Velocity = velocity;
                    tpp.Direction = (TripPyroidDirection)( vdata & 0x80 );
                    if (styleFlag != 0)
                    {
                        tpp.PyroidStyle = PyroidStyle.Single;
                    }
                    maze.AddObject(tpp);

                    trip.Pyroid = tpp;
                    tpp.TripPad = trip;
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

        public byte ReadByte(ushort address, int offset)
        {
            //throw new Exception("Not implemented.");
            return 0;
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


        public Tuple<ushort, int> GetAddress(string label)
        {
            Tuple<ushort, int> address = null;
            //search the export list for this address...

            if (_exports.ContainsKey(label.ToLower()))
            {
                address = new Tuple<ushort, int>((ushort)_exports[label], 6);
            }
            else
            {
                throw new Exception("Address not found: " + label.ToString());
            }
            return address;
        }

        private byte[] ReadPagedROM(ushort address, int offset, int length, int page)
        {
            byte[] bytes = new byte[length];
            int page67Base = 0x4000; //since addresses come in with a base of 0x2000 already, we just need to add this amount
            if (page == 7) page67Base = 0x6000;

            if (address >= 0x2000 && address <= 0x3fff)
            {
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _page2367[page67Base - 0x2000 + address + i + offset];
                }
            }
            return bytes;
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

        private int WriteAlphaHigh(ushort address, byte data)
        {
            ushort alphaHighBase = 0xc000;
            address -= alphaHighBase;
            _alphaHigh[address] = data;
            return 1;
        }


        private int WritePagedROM(ushort address, byte[] bytes, int offset, int page)
        {
            int page67Base = 0x4000;
            if (page == 7) page67Base = 0x6000;

            if (address >= 0x2000 && address <= 0x3fff)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _page2367[page67Base + address - 0x2000 + i + offset] = bytes[i];
                }
            }
            return bytes.Length;
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
            byte calculatedCsum = 0;
            for (int i = lowerBounds; i < (lowerBounds + length - 1); i++)
            {
                calculatedCsum += _page2367[i];
            }
            //ROM needs to equal csum when it is all said and done
            byte finalCsum = (byte)((csum - calculatedCsum) & 0xff);
            WritePagedROM((ushort)(0x2000 + length - 1), new byte[] { finalCsum }, 0, page);
        }

        private void WriteAlphaHighChecksum(int lowerBounds, int length, byte csum)
        {
            byte calculatedCsum = 0;
            for (int i = lowerBounds; i < (lowerBounds + length - 1); i++)
            {
                calculatedCsum += _alphaHigh[i];
            }
            //ROM needs to equal csum when it is all said and done
            byte finalCsum = (byte)((csum - calculatedCsum) & 0xff);
            WriteAlphaHigh((ushort)(_exports["chka2"]), finalCsum );
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

        public bool WriteFiles(string mamePath)
        {
            //fix csums...
            WritePagedChecksum(0x4000, 0x2000, 6, 0x08);
            WritePagedChecksum(0x6000, 0x2000, 7, 0x09);
            WriteAlphaHighChecksum(0x0000, 0x4000, 0x01);

            MarkPagedROM(6);
            MarkPagedROM(7);
            MarkAlphaHighROM();

            string page67FileNameMame = mamePath + _page2367ROM;
            string alphaHighFileNameMane = mamePath + _alphaHighROM;

            //save each
            File.WriteAllBytes(page67FileNameMame, _page2367);
            File.WriteAllBytes(alphaHighFileNameMane, _alphaHigh);

            //copy others 
            List<string> otherROMs = new List<string>();
            otherROMs.Add("mhpe.1mn");
            otherROMs.Add("mhpe.1q");
            otherROMs.Add("mhpe.6kl");
            otherROMs.Add("mhpe.6h");
            otherROMs.Add("mhpe.6jk");
            otherROMs.Add("mhpe.9s");
            otherROMs.Add("036408-01.b1");

            foreach (string rom in otherROMs)
            {
                File.Copy(_templatePath + rom, mamePath + rom, true);
            }
            return true;
        }

        public byte ReadByte(ushort address, int offset, int page)
        {
            return ReadPagedROM(address, offset, 1, page)[0];
        }

        public byte[] ReadBytes(ushort address, int length, int page)
        {
            return ReadPagedROM(address, 0, length, page);
        }

        public ushort ReadWord(ushort address, int offset, int page)
        {
            byte[] bytes = ReadPagedROM(address, offset, 2, page);
            ushort wordH = bytes[1];
            return (ushort)(((ushort)wordH << 8) + (ushort)bytes[0]);
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

        public String ExtractSource(Maze maze, int level)
        {
            int dataPosition = 10;
            int commentPosition = 60;

            string commentLine = ";********************************************************************";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(commentLine);
            sb.Append("; ");
            sb.AppendLine(GetMazeCode(level) + " - " + maze.Name);
            sb.AppendLine(commentLine);

            //Maze Hints
            if (!String.IsNullOrEmpty(maze.Hint))
            {
                StringBuilder mb = new StringBuilder();
                mb.Append("mhz");
                mb.Append(GetMazeCode(level));
                Tabify(' ', dataPosition, mb);
                mb.Append(".ctext \"");
                mb.Append(maze.Hint.ToUpper());
                mb.Append("\"");
                sb.AppendLine(mb.ToString());
            }
            if (!String.IsNullOrEmpty(maze.Hint2))
            {
                StringBuilder mb = new StringBuilder();
                mb.Append("mhz");
                mb.Append(GetMazeCode(level));
                mb.Append("a");
                Tabify(' ', dataPosition, mb);
                mb.Append(".ctext \"");
                mb.Append(maze.Hint2.ToUpper());
                mb.Append("\"");
                sb.AppendLine(mb.ToString());
            }
            sb.AppendLine("");

            //Reactoid.Pyroids.Perkoids.Max
            sb.AppendLine(DumpBytes("mzsc" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.ReactoidPyroidPerkoidMax).ObjectEncodings));
            //Oxygen
            sb.AppendLine(DumpBytes("mzdc" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Oxoids).ObjectEncodings));
            //Lightning
            sb.AppendLine(DumpBytes("mzlg" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.Lightning).ObjectEncodings));
            //Arrows
            sb.AppendLine(DumpBytes("mzar" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.Arrows).ObjectEncodings));
            //Exit Arrows
            sb.AppendLine(DumpBytes("mzor" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.ArrowsOut).ObjectEncodings));
            //Trip Pads
            sb.AppendLine(DumpBytes("mztr" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.TripPoints).ObjectEncodings));
            //Trip Pad Actions
            sb.AppendLine(DumpBytes("trpa" + GetMazeCode(level), dataPosition, commentPosition, 3, EncodeObjects(maze, EncodingGroup.TripActions).ObjectEncodings));
            //Static Walls
            sb.AppendLine(DumpBytes("mzta" + GetMazeCode(level), dataPosition, commentPosition, 2, EncodeObjects(maze, EncodingGroup.StaticWalls).ObjectEncodings));
            //Dynamic Walls
            sb.AppendLine(DumpBytes("mztd" + GetMazeCode(level), dataPosition, commentPosition, 5, EncodeObjects(maze, EncodingGroup.DynamicWalls).ObjectEncodings));
            //One Way Walls
            sb.AppendLine(DumpBytes("mone" + GetMazeCode(level), dataPosition, commentPosition, 16, EncodeObjects(maze, EncodingGroup.OneWay).ObjectEncodings));
            //Ion Cannons
            var cannonGroupEncodings = from e in EncodeObjects(maze, EncodingGroup.IonCannon).ObjectEncodings
                                       group e by e.Group into g select new { Id = g.Key, Encodings = g.ToList() };
            int ionIndexer = 0;
            List<char> suffix = new List<char>() { 'a', 'b', 'c', 'd' };
            foreach (var g in cannonGroupEncodings)
            {
                sb.AppendLine(DumpBytes("mcp" + GetMazeCode(level) + suffix[ionIndexer++], dataPosition, commentPosition, 16, g.Encodings));
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
            int difficulty = level / 4;
            int mazeNumber = level % 4;
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
                    while (skip <= bytes.Length)
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


        public bool EncodeObjects(MazeCollection mazeCollection, Maze maze)
        {
            bool success = false;
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
                        WritePagedROM((ushort)_exports["zmessypos"], new byte[] { 0x48 }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessxpos"], new byte[] { GetTextPosition(mazeCollection.Mazes[i].Hint) }, messageIndexer, 7);
                        //write the data stream
                        currentAddressPage7 += WritePagedROM((ushort)currentAddressPage7, GetBytesFromString(mazeCollection.Mazes[i].Hint), 0, 7);
                        //finally, book the index and increment
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { messageIndexer }, (i * 2) + 1, 7);
                        messageIndexer++;
                    }
                    else
                    {
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { 0xff }, i * 2, 7);
                    }
                    //Write Table Pointer - Second Hint
                    if (!String.IsNullOrEmpty(mazeCollection.Mazes[i].Hint2))
                    {
                        //update pointers and locations
                        WritePagedROM((ushort)_exports["zmessptrl"], new byte[] { (byte)(currentAddressPage7 & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessptrh"], new byte[] { (byte)((currentAddressPage7 >> 8) & 0xFF) }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessypos"], new byte[] { 0x50 }, messageIndexer, 7);
                        WritePagedROM((ushort)_exports["zmessxpos"], new byte[] { GetTextPosition(mazeCollection.Mazes[i].Hint2) }, messageIndexer, 7);
                        //write the data stream
                        currentAddressPage7 += WritePagedROM((ushort)currentAddressPage7, GetBytesFromString(mazeCollection.Mazes[i].Hint2), 0, 7);
                        //finally, book the index and increment
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { messageIndexer }, (i * 2)+1, 7);
                        messageIndexer++;
                    }
                    else
                    {
                        WritePagedROM((ushort)_exports["mazehints"], new byte[] { 0xff }, (i * 2)+1, 7);
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
            //Oxygen Discs
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Oxoid data
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
            //Lightning
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mzlg"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Lightning).GetAllBytes().ToArray(), 0, 6);
            }
            //Arrows
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mzar"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                //Arrow data
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Arrows).GetAllBytes().ToArray(), 0, 6);
            }
            //Exit Arrows
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
            //Trip Points
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztr"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.TripPoints).GetAllBytes().ToArray(), 0, 6);
            }
            //Trip Actions
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["trtbll"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.TripActions).GetAllBytes().ToArray(), 0, 6);
            }
            //Static Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztdal"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.StaticWalls).GetAllBytes().ToArray(), 0, 6);
            }
            //Dynamic Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mztd"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.DynamicWalls).GetAllBytes().ToArray(), 0, 6);
            }
            //One Way Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mone"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OneWay).GetAllBytes().ToArray(), 0, 6);
            }

            //Ion IonCannon, warning, this is very messy due to data compaction techniques
            //Three levels of data pointers
            // 1. Byte: Index into Pointers - mcan - Static Data Area - 28 Bytes - Written Last
            // 2. Word: Pointers to Data - mcanst - Dynamic Data Area - Written Second
            // 3. Byte: Data Stream - Dynamic Data Area - Written First
            Dictionary<int, int> cannonLevelPointers = new Dictionary<int, int>();
            Dictionary<Guid, int> cannonDataPointers = new Dictionary<Guid, int>();

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

                    //JMA - 03272019 - Old code, left in case there are troubles serializing with new code above
                    //foreach (IonCannon cannon in mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>())
                    //{
                    //    cannonDataPointers.Add(cannon.Id, currentAddressPage6);
                    //    currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, cannon.ToBytes(), 0, 6);
                    //}
                }
            }
            //now build Indexes and Pointers
            pointerIndex = 0;
            int cannonIndexValue = 0x02;
            //empty data word for levels with no Cannons, pointerIndex = 0
            int cannonPointerEmpty = currentAddressPage6;
            currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, new byte[] { 0x00, 0x00 }, 0, 6);

            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>().Count() == 0)
                {
                    //set empty pointers and index
                    pointerIndex += WritePagedROM((ushort)_exports["mcan"], new byte[] { 0x00 }, pointerIndex, 6);
                }
                else
                {
                    //set this ponter index value and increment
                    pointerIndex += WritePagedROM((ushort)_exports["mcan"], new byte[] { (byte)cannonIndexValue }, pointerIndex, 6);
                    cannonIndexValue += (mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>().Count() * 2) + 2;
                    foreach (IonCannon cannon in mazeCollection.Mazes[i].MazeObjects.OfType<IonCannon>())
                    {
                        currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, WordToByteArray(cannonDataPointers[cannon.Id]), 0, 6);
                    }
                    currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, new byte[] { 0x00, 0x00 }, 0, 6);
                }
            }

            //Spikes
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mtite"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Spikes).GetAllBytes().ToArray(), 0, 6);
            }

            //locks and keys, for now, there has to be an even number of locks and keys
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mlock"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.LocksKeys).GetAllBytes().ToArray(), 0, 6);
            }

            //Transporters
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mtran"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Transporters).GetAllBytes().ToArray(), 0, 6);
            }
            //De Hand
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WritePagedROM((ushort)_exports["mhand"], WordToByteArray(currentAddressPage6), pointerIndex, 6);
                currentAddressPage6 += WritePagedROM((ushort)currentAddressPage6, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Hand).GetAllBytes().ToArray(), 0, 6);
            }
            //Clock
            int clockIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                clockIndex += WritePagedROM((ushort)_exports["mclock"], EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Clock).GetAllBytes().ToArray(), clockIndex, 6);
            }
            //Boots
            int bootsIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                bootsIndex += WritePagedROM((ushort)_exports["mboots"], EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.Boots).GetAllBytes().ToArray(), bootsIndex, 6);
            }

            int mpodAddressBase = _exports["mpod"];
            //Escape Pod
            for (int i = 1; i < numMazes; i+=4)
            {
                mpodAddressBase += WritePagedROM((ushort)mpodAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.EscapePod).GetAllBytes().ToArray(), 0, 6);
            }
            //Out Time
            int outAddressBase = _exports["outime"];
            for (int i = 0; i < numMazes; i++)
            {
                outAddressBase += WritePagedROM((ushort)outAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OutTime).GetAllBytes().ToArray(), 0, 6);
            }
            //OxygenReward
            int oxyAddressBase = _exports["oxybonus"];
            for (int i = 0; i < numMazes; i++)
            {
                oxyAddressBase += WritePagedROM((ushort)oxyAddressBase, EncodeObjects(mazeCollection.Mazes[i], EncodingGroup.OxygenReward).GetAllBytes().ToArray(), 0, 6);
            }

            //set up starting level
            for(int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i] == maze)
                {
                    byte startLevel = (byte)i;
                    if (startLevel > 24) startLevel = 24;
                    WriteAlphaHigh((ushort)(_exports["levelst"]+1), startLevel);
                }
            }

            if (currentAddressPage6 >= 0x2000)
            {
                //this is bad

            }
            if (currentAddressPage7 >= 0x2000 )
            {
                //this is bad, it means we have overflowed our Paged ROM end boundary
            }

            success = true;
            return success;
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
            OxygenReward
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

            Reactoid reactoid = maze.MazeObjects.OfType<Reactoid>().FirstOrDefault();
            int counter = 0;

            switch (group)
            {
                case EncodingGroup.ReactoidPyroidPerkoidMax:
                    //Reactoid
                    if (reactoid != null)
                    {
                        encodings.Add(reactoid.ToBytes(reactoid.Position), "Reactor");
                        foreach (Pyroid pyroid in maze.MazeObjects.OfType<Pyroid>())
                        {
                            encodings.Add(pyroid.ToBytes(), "F" + counter++.ToString());
                        }
                        //Perkoids
                        counter = 0;
                        if (maze.MazeObjects.OfType<Perkoid>().Count() > 0)
                        {
                            encodings.Add(0xfe);
                            foreach (Perkoid perkoid in maze.MazeObjects.OfType<Perkoid>())
                            {
                                encodings.Add(perkoid.ToBytes(), "R" + counter++.ToString());
                            }
                        }
                        //Maxoids
                        counter = 0;
                        if (maze.MazeObjects.OfType<Maxoid>().Count() > 0)
                        {
                            //make sure we did perkoids already
                            if (maze.MazeObjects.OfType<Perkoid>().Count() == 0)
                            {
                                encodings.Add(0xfe, "No Robots");
                            }
                            encodings.Add(0xfe,"End Robots");
                            foreach (Maxoid max in maze.MazeObjects.OfType<Maxoid>())
                            {
                                encodings.Add(max.ToBytes(), "M" + counter++.ToString());
                            }
                        }
                        encodings.Add(0xff); //Data End Flag
                    }
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
                            encodings.Add(lightningH.ToBytes(), "Lightning-Horizontal");
                        }
                        if (maze.MazeObjects.OfType<LightningV>().Count() > 0)
                        {
                            encodings.Add(0xff);
                            foreach (LightningV lightningV in maze.MazeObjects.OfType<LightningV>())
                            {
                                encodings.Add(lightningV.ToBytes(),"Lightning-Vertical");
                            }
                        }
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.Arrows:
                    foreach (Arrow arrow in maze.MazeObjects.OfType<Arrow>())
                    {
                        encodings.Add(arrow.ToBytes(),"Arrows");
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.ArrowsOut:
                    foreach (ArrowOut arrow in maze.MazeObjects.OfType<ArrowOut>())
                    {
                        encodings.Add(arrow.ToBytes(),"Out Arrows");
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.TripPoints:
                    //Trip Point data
                    foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
                    {
                        encodings.Add(trip.ToBytes(),"Trip Pads");
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.TripActions:
                    //Trip Action Data
                    foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
                    {
                        encodings.Add(trip.Pyroid.ToBytes(),"Trip Pad Actions");
                    }
                    encodings.Add(new byte[] { 0x00, 0x00, 0x00 });
                    break;
                case EncodingGroup.StaticWalls:
                    //Wall data, all walls in maze
                    foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => !w.IsDynamicWall))
                    {
                        encodings.Add(wall.ToBytes(maze),"Static Walls");
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.DynamicWalls:
                    //wall data, only dynamic walls
                    foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
                    {
                        encodings.Add(wall.ToBytes(maze),"Dynamic Walls");
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.OneWay:
                    foreach (OneWay wall in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
                    {
                        encodings.Add(wall.ToBytes(maze), "OneWay Walls-Right");
                    }
                    if (maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left).Count() > 0)
                    {
                        foreach (OneWay wall in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
                        {
                            encodings.Add(0xff);
                            encodings.Add(wall.ToBytes(maze), "OneWay Walls-Left");
                        }
                    }
                    encodings.Add(0x00);
                    break;
                case EncodingGroup.IonCannon:
                    for(int i = 0; i < maze.MazeObjects.OfType<IonCannon>().Count(); i++)
                    {
                        IonCannon cannon = maze.MazeObjects.OfType<IonCannon>().ToArray()[i];
                        //Position first
                        byte[] positionBytes = (DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(cannon.Position)));
                        encodings.Add(positionBytes, "Position", cannon.Id.ToString(), ".dw $" + BytesToWord(positionBytes[0], positionBytes[1]).ToString("X4") + ",$" + BytesToWord(positionBytes[2], positionBytes[3]).ToString("X4"));
                        foreach (IonCannonInstruction instruction in cannon.Program)
                        {
                            Tuple<string,string> commentMacro = GetCannonCommentMacro(instruction);
                            List<byte> instructionBytes = new List<byte>();
                            instruction.GetObjectData(instructionBytes);
                            encodings.Add(instructionBytes.ToArray(), commentMacro.Item1, cannon.Id.ToString(), commentMacro.Item2);
                        }
                    }
                    break;
                case EncodingGroup.Spikes:
                    foreach (Spikes spike in maze.MazeObjects.OfType<Spikes>())
                    {
                        encodings.Add(spike.ToBytes(), "Stalactites");
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
                            encodings.Add(t.ToBytes(), Enum.GetName(typeof(ObjectColor), t.Color));
                        }
                    }
                    //write end of transports
                    encodings.Add(0x00);
                    //write transportability data
                    if (maze.TransportabilityFlags.Count > 0)
                    {
                        int flagCount = 0;
                        int flagValue = 0;
                        for (int f = 0; f < maze.TransportabilityFlags.Count; f++)
                        {
                            flagValue = flagValue << 1;
                            if (f < maze.TransportabilityFlags.Count)
                            {
                                if (maze.TransportabilityFlags[f])
                                {
                                    flagValue += 1;
                                }
                            }

                            flagCount++;
                            if (flagCount > 7)
                            {
                                encodings.Add((byte)flagValue, "Transportability Flags");
                                flagCount = 0;
                                flagValue = 0;
                            }
                        }
                    }
                    encodings.Add(0xee);
                    break;
                case EncodingGroup.Hand:
                    Hand hand = maze.MazeObjects.OfType<Hand>().FirstOrDefault();
                    if (hand != null)
                    {
                        Reactoid r = maze.MazeObjects.OfType<Reactoid>().FirstOrDefault();
                        if (r != null)
                        {
                            encodings.Add(hand.ToBytes(r.Position), "Hand");
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
                        encodings.Add(clock.ToBytes(),"Clock");
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
                        encodings.Add(boots.ToBytes(),"Boots");
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
                        encodings.Add(pod.ToBytes());
                    }
                    else
                    {
                        encodings.Add(0x00);
                    }
                    break;
                case EncodingGroup.OutTime:
                    //Pod Data
                    int reactorTimer = 0;
                    if (reactoid != null)
                    {
                        reactorTimer = DataConverter.ToDecimal(reactoid.Timer);
                    }
                    encodings.Add((byte)reactorTimer);
                    break;
                case EncodingGroup.OxygenReward:
                    encodings.Add((byte)maze.OxygenReward);
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
                    mb.AppendLine("cann_pau(" + ((Pause)instruction).WaitFrames.ToString() + ")");
                    cb.Append("Pause = " + ((Pause)instruction).WaitFrames.ToString() + " frames");
                    break;
            }
            return new Tuple<string, string>( cb.ToString(), mb.ToString());
        }
        
    }
}
