using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
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
        private Dictionary<string, ushort> _exports = new Dictionary<string, ushort>();
        private string _page2367ROM = "mhpe.1np";
        private string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:";

        private int _page6Base = 0x4000;
        private int _page7Base = 0x6000;
        private string _lastError = "";

        #endregion


        public MajorHavocPromisedEnd(string templatePath)
        {
            _templatePath = templatePath;
            LoadROMS(_templatePath);
        }

        public string LastError
        {
            get { return _lastError; }
        }

        private void LoadROMS(string templatePath)
        {
            //load our exports
            if (File.Exists(templatePath + "mhavocpe.exp"))
            {
                string[] exportLines = File.ReadAllLines(templatePath + "mhavocpe.exp");
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

            //load up our roms for now...
            try
            {
                _page2367 = File.ReadAllBytes(templatePath + _page2367ROM);
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Page0/1: " + Exception.Message);
            }
        }

        public MazeCollection LoadMazes(string sourceROMFilePath, List<string> loadMessages)
        {

            MazeCollection mazeCollection = new MazeCollection("Promised End Mazes");
            mazeCollection.AuthorEmail = "Jess Askey";

            for (int i = 0; i < 28; i++)
            {

                byte mazeType = (byte)(i & 0x03);

                Maze maze = new Maze((MazeType)mazeType, "Level " + (i + 1).ToString());

                //hint text - only first 12 mazes tho
                byte messageIndex = ReadByte(_exports["mazehint"], i, 6);
                maze.Hint = GetMessage(messageIndex);
                //build reactor
                ushort mazeInitIndex = ReadWord(_exports["mzinit"], i * 2, 6);
                Reactoid reactor = new Reactoid();
                reactor.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                mazeInitIndex += 4;
                int timer = FromDecimal((int)ReadByte(_exports["outime"], i, 6));
                reactor.Timer = timer;
                maze.AddObject(reactor);

                //pyroids
                byte firstValue = ReadByte(mazeInitIndex, 0, 7);
                while (firstValue != 0xff)
                {
                    Pyroid pyroid = new Pyroid();
                    pyroid.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(pyroid.Velocity, pyroid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(pyroid);
                    firstValue = ReadByte(mazeInitIndex, 0, 7);

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
                    perkoid.LoadPosition(ReadBytes(mazeInitIndex, 4, 7));
                    mazeInitIndex += 4;
                    mazeInitIndex += (byte)LoadIncrementingVelocity(perkoid.Velocity, perkoid.IncrementingVelocity, mazeInitIndex);
                    maze.AddObject(perkoid);
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
                    arrow.ArrowDirection = (Containers.MazeObjects.ArrowDirection)arrowValue;
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
                    arrow.ArrowDirection = (Containers.MazeObjects.ArrowDirection)outArrowValue;
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

                //Laser Cannon
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
                        Cannon cannon = new Cannon();
                        cannon.LoadPosition(ReadBytes(cannonPointerAddress, 4, 6));
                        //now read data until we hit a cannon_end byte ($00)
                        int cannonCommandOffset = 4;
                        byte commandStartByte = commandStartByte = ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                        bool hasData = true;
                        while (hasData)
                        {
                            int cannonCommand = commandStartByte >> 6;
                            switch (cannonCommand)
                            {
                                case 0:     //loop
                                    cannon.Movements.Add(new CannonMovementReturn());
                                    hasData = false;
                                    break;
                                case 1:     //Move Gun
                                    CannonMovementPosition cannonPosition = new CannonMovementPosition();
                                    int gunAngle = (commandStartByte & 0x38) >> 3;
                                    cannonPosition.Position = (CannonGunPosition)gunAngle;
                                    int rotationSpeed = (commandStartByte & 0x06) >> 1;
                                    cannonPosition.Speed = (CannonGunSpeed)rotationSpeed;
                                    int fireBit = (commandStartByte & 0x01);
                                    if (fireBit > 0)
                                    {
                                        cannonCommandOffset++;
                                        cannonPosition.ShotSpeed = (byte)ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                                    }
                                    cannon.Movements.Add(cannonPosition);
                                    break;
                                case 2:     //Move Position
                                    CannonMovementMove cannonMovement = new CannonMovementMove();
                                    int waitFrames = (commandStartByte & 0x3F) << 2;
                                    cannonMovement.WaitFrames = waitFrames;
                                    if (waitFrames > 0)
                                    {
                                        cannonCommandOffset++;
                                        cannonMovement.Velocity.X = (sbyte)ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                                        cannonCommandOffset++;
                                        cannonMovement.Velocity.Y = (sbyte)ReadByte(cannonPointerAddress, cannonCommandOffset, 6);
                                    }
                                    //cannonMovement.
                                    cannon.Movements.Add(cannonMovement);
                                    break;
                                case 3:     //Pause
                                    CannonMovementPause cannonPause = new CannonMovementPause();
                                    cannonPause.WaitFrames = (commandStartByte & 0x3F) << 2;
                                    cannon.Movements.Add(cannonPause);
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
                        velocity = velocity * -1;
                        longBytes[0] = 0x40;
                    }

                    longBytes[1] = (byte)((xh & 0x1f));
                    longBytes[2] = 0x80;
                    longBytes[3] = yh;

                    TripPadPyroid tpp = new TripPadPyroid();
                    tpp.LoadPosition(longBytes);
                    tpp.Velocity = velocity;
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
                mazeCollection.AddMaze(maze);
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

        private byte[] ReadROM(ushort address, int offset, int length, int page)
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



        private int WriteROM(ushort address, byte[] bytes, int offset, int page)
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


        public int Write(string location, byte data, int offset)
        {
            Tuple<ushort, int> addressInfo = GetAddress(location);
            return WriteROM(addressInfo.Item1, new byte[] { data }, offset, addressInfo.Item2);
        }

        public int Write(string location, UInt16 data, int offset)
        {
            Tuple<ushort, int> addressInfo = GetAddress(location);
            byte datahigh = (byte)(data >> 8);
            byte datalow = (byte)(data & 0xff);
            return WriteROM(addressInfo.Item1, new byte[] { datalow, datahigh }, offset, addressInfo.Item2);
        }

        public int Write(string location, byte[] data, int offset)
        {
            Tuple<ushort, int> addressInfo = GetAddress(location);
            return WriteROM(addressInfo.Item1, data, offset, addressInfo.Item2);
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
            string page67FileNameMame = mamePath + _page2367ROM;

            //save each
            File.WriteAllBytes(page67FileNameMame, _page2367);

            //copy others 
            List<string> otherROMs = new List<string>();
            otherROMs.Add("mhpe.1mn");
            otherROMs.Add("mhpe.1l");
            otherROMs.Add("mhpe.1q");
            otherROMs.Add("mhpe.6kl");
            otherROMs.Add("mhpe.6h");
            otherROMs.Add("mhpe.6jk");
            otherROMs.Add("mhpe.9s");

            foreach (string rom in otherROMs)
            {
                File.Copy(_templatePath + rom, mamePath + rom, true);
            }

            return true;
        }

        public byte ReadByte(ushort address, int offset, int page)
        {
            return ReadROM(address, offset, 1, page)[0];
        }

        public byte[] ReadBytes(ushort address, int length, int page)
        {
            return ReadROM(address, 0, length, page);
        }

        public ushort ReadWord(ushort address, int offset, int page)
        {
            byte[] bytes = ReadROM(address, offset, 2, page);
            ushort wordH = bytes[1];
            return (ushort)(((ushort)wordH << 8) + (ushort)bytes[0]);
        }


        //returns a text value for the given message index.
        public string GetMessage(byte index)
        {
            ushort messageTableBase = 0xe48b;
            ushort messageBase = ReadWord(messageTableBase, index, 7);
            //get real index 
            return GetText(messageBase);
        }

        private string GetText(ushort textBase)
        {
            StringBuilder sb = new StringBuilder();
            //start @ 1 because first byte is color?
            int index = 1;
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

        public int ToDecimal(int value)
        {
            return Convert.ToInt16(("0x" + value.ToString()), 16);
        }

        public int FromDecimal(int value)
        {
            return Convert.ToInt16(value.ToString("X2"), 10);
        }

        public bool SerializeObjects(Maze maze)
        {
            throw new Exception("Major Havoc - The Promised End does not support serializing of single mazes.");
        }

        public bool SerializeObjects(MazeCollection mazeCollection, Maze maze)
        {
            bool success = false;
            int numMazes = 28;

            if (mazeCollection.MazeCount > numMazes)
            {
                _lastError = "Maze collection contained more than 24 mazes.";
                return false;
            }
            //serialization in The Promised End is a little more fluid... in general the steps are
            // 1. Iterate the Major Object Pointer tables
            //    a. For each Level (24 in MHTPE)
            //       i.  Write the current pointer (object data start)
            //       ii. Write the data
            // 2. Verify no strange boundaries were crossed
            // 3. Update the CSUM's on each ROM that was written.

            int index7Data = _exports["mzsc00"];
            Reactoid reactoid = null;

            int pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i] != null)
                {
                    //Reactoid, Pyroid, Perkoids and Maxoids
                    //Write Table Pointer
                    pointerIndex += WriteROM((ushort)_exports["mzinit"], WordToByteArray(index7Data), pointerIndex, 6);
                    //Reactoid
                    reactoid = mazeCollection.Mazes[i].MazeObjects.OfType<Reactoid>().FirstOrDefault();
                    if (reactoid != null)
                    {
                        index7Data += WriteROM((ushort)index7Data, reactoid.ToBytes(reactoid.Position), 0, 7);
                        foreach (Pyroid pyroid in mazeCollection.Mazes[i].MazeObjects.OfType<Pyroid>())
                        {
                            index7Data += WriteROM((ushort)index7Data, pyroid.ToBytes(), 0, 7);
                        }
                        //Perkoids
                        if (mazeCollection.Mazes[i].MazeObjects.OfType<Perkoid>().Count() > 0)
                        {
                            index7Data += WriteROM((ushort)index7Data, new byte[] { 0xfe }, 0, 7);
                            foreach (Perkoid perkoid in mazeCollection.Mazes[i].MazeObjects.OfType<Perkoid>())
                            {
                                index7Data += WriteROM((ushort)index7Data, perkoid.ToBytes(), 0, 7);
                            }
                        }
                        //Maxoids
                        if (mazeCollection.Mazes[i].MazeObjects.OfType<Maxoid>().Count() > 0)
                        {
                            index7Data += WriteROM((ushort)index7Data, new byte[] { 0xfe }, 0, 7);
                            foreach (Maxoid max in mazeCollection.Mazes[i].MazeObjects.OfType<Maxoid>())
                            {
                                index7Data += WriteROM((ushort)index7Data, max.ToBytes(), 0, 7);
                            }
                        }
                    }
                }
                //Done Flag
                index7Data += WriteROM((ushort)index7Data, new byte[] { 0xff }, 0, 7);
            }

            //Maze Hints
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                string message = " ";   //default to a single blank space
                if (mazeCollection.Mazes[i] != null)
                {
                    //Write Table Pointer
                    pointerIndex += WriteROM((ushort)_exports["mazehint"], WordToByteArray(index7Data), pointerIndex, 6);
                    //Maze Data
                    if (!String.IsNullOrEmpty(mazeCollection.Mazes[i].Hint.Replace(" ", "")))
                    {
                        message = mazeCollection.Mazes[i].Hint;
                    }
                }
                index7Data += WriteROM((ushort)index7Data, GetBytesFromString(message), 0, 7);
            }

            //********************************
            // Page 6 Data Now
            //********************************
            int index6Data = _exports["dynamic_base"];
            Dictionary<byte, ushort> existingOxoids = new Dictionary<byte, ushort>();
            //Oxygen Discs
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Oxoid data
                List<byte> oxoidBytes = new List<byte>();
                foreach (Oxoid oxoid in mazeCollection.Mazes[i].MazeObjects.OfType<Oxoid>())
                {
                    oxoidBytes.AddRange(oxoid.ToBytes());
                }

                byte byteHash = GetByteHash(oxoidBytes.ToArray());
                if (existingOxoids.ContainsKey(byteHash))
                {
                    //already defined, use this pointer
                    pointerIndex += WriteROM((ushort)_exports["mzdc"], WordToByteArray(existingOxoids[byteHash]), pointerIndex, 6);
                }
                else //didn't exist, write it out
                {
                    //Write Table Pointer
                    pointerIndex += WriteROM((ushort)_exports["mzdc"], WordToByteArray(index6Data), pointerIndex, 6);
                    index6Data += WriteROM((ushort)index6Data, oxoidBytes.ToArray(), 0, 6);
                    index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
                    //save to dictionary
                    existingOxoids.Add(byteHash, (ushort)index6Data);
                }
            }
            //Lightning
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mzlg"], WordToByteArray(index6Data), pointerIndex, 6);
                if ((mazeCollection.Mazes[i].MazeObjects.OfType<LightningH>().Count() > 0) || 
                    (mazeCollection.Mazes[i].MazeObjects.OfType<LightningV>().Count() > 0 ))
                {
                    //Oxoid data
                    foreach (LightningH lightningH in mazeCollection.Mazes[i].MazeObjects.OfType<LightningH>())
                    {
                        index6Data += WriteROM((ushort)index6Data, lightningH.ToBytes(), 0, 6);
                    }
                    if (mazeCollection.Mazes[i].MazeObjects.OfType<LightningV>().Count() > 0)
                    {
                        index6Data += WriteROM((ushort)index6Data, new byte[] { 0xff }, 0, 6);
                        foreach (LightningV lightningV in mazeCollection.Mazes[i].MazeObjects.OfType<LightningV>())
                        {
                            index6Data += WriteROM((ushort)index6Data, lightningV.ToBytes(), 0, 6);
                        }
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Arrows
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mzar"], WordToByteArray(index6Data), pointerIndex, 6);
                //Arrow data
                foreach (Arrow arrow in mazeCollection.Mazes[i].MazeObjects.OfType<Arrow>())
                {
                    index6Data += WriteROM((ushort)index6Data, arrow.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Exit Arrows
            pointerIndex = 0;
            //create a placeholder pointer which is for blank levels
            int blankOutArrowsPointer = index6Data;
            pointerIndex += WriteROM((ushort)_exports["mzor"], WordToByteArray(index6Data), pointerIndex, 6);
            index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);

            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<ArrowOut>().Count() > 0) {
                    //Write Table Pointer
                    pointerIndex += WriteROM((ushort)_exports["mzor"], WordToByteArray(index6Data), pointerIndex, 6);
                    //ArrowOut data
                    foreach (ArrowOut arrow in mazeCollection.Mazes[i].MazeObjects.OfType<ArrowOut>())
                    {
                        index6Data += WriteROM((ushort)index6Data, arrow.ToBytes(), 0, 6);
                    }
                    index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
                }
                else
                {
                    //no out arrows, point to the blank entry defined at the top
                    pointerIndex += WriteROM((ushort)_exports["mzor"], WordToByteArray(blankOutArrowsPointer), pointerIndex, 6);
                }
            }
            //Trip Points
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mztr"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (TripPad trip in mazeCollection.Mazes[i].MazeObjects.OfType<TripPad>())
                {
                    index6Data += WriteROM((ushort)index6Data, trip.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Trip Actions
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["trtbll"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Action Data
                foreach (TripPad trip in mazeCollection.Mazes[i].MazeObjects.OfType<TripPad>())
                {
                    index6Data += WriteROM((ushort)index6Data, trip.Pyroid.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00, 0x00, 0x00 }, 0, 6);
            }
            //Static Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mztdal"], WordToByteArray(index6Data), pointerIndex, 6);
                //Wall data, all walls in maze
                foreach (MazeWall wall in mazeCollection.Mazes[i].MazeObjects.OfType<MazeWall>().Where(w =>!w.IsDynamicWall))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Dynamic Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mztd"], WordToByteArray(index6Data), pointerIndex, 6);
                //wall data, only dynamic walls
                foreach (MazeWall wall in mazeCollection.Mazes[i].MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //One Way Walls
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mone"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (OneWay wall in mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                if (mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left).Count() > 0) {
                    foreach (OneWay wall in mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
                    {
                        index6Data += WriteROM((ushort)index6Data, new byte[] { 0xff }, 0, 6);
                        index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }

            //Ion Cannon, warning, this is very messy due to data compaction techniques
            //Three levels of data pointers
            // 1. Byte: Index into Pointers - mcan - Static Data Area - 28 Bytes - Written Last
            // 2. Word: Pointers to Data - mcanst - Dynamic Data Area - Written Second
            // 3. Byte: Data Stream - Dynamic Data Area - Written First
            Dictionary<int, int> cannonLevelPointers = new Dictionary<int, int>();
            Dictionary<Guid, int> cannonDataPointers = new Dictionary<Guid, int>();

            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>().Count() > 0)
                {
                    cannonLevelPointers.Add(i, index6Data);
                    foreach (Cannon cannon in mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>())
                    {
                        cannonDataPointers.Add(cannon.ObjectId, index6Data);
                        index6Data += WriteROM((ushort)index6Data, cannon.ToBytes(), 0, 6);
                    }
                }
            }
            //now build Indexes and Pointers
            pointerIndex = 0;
            int cannonIndexValue = 0;
            //empty data word for levels with no Cannons, pointerIndex = 0
            int cannonPointerEmpty = index6Data;
            index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00, 0x00 }, 0, 6);

            for (int i = 0; i < numMazes; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>().Count() == 0)
                {
                    //set empty pointers and index
                    //index6Data += WriteROM((ushort)index6Data, WordToByteArray(cannonPointerEmpty), 0, 6); 
                    pointerIndex += WriteROM((ushort)_exports["mcan"], new byte[] { 0x00 }, pointerIndex, 6);
                    cannonIndexValue += 2;
                }
                else
                {
                    //set this ponter index value and increment
                    pointerIndex += WriteROM((ushort)_exports["mcan"], new byte[] { (byte)cannonIndexValue }, pointerIndex, 6);
                    cannonIndexValue += 2;
                    foreach(Cannon cannon in mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>())
                    {
                        index6Data += WriteROM((ushort)index6Data, WordToByteArray(cannonDataPointers[cannon.ObjectId]), 0, 6);
                    }
                    index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00, 0x00 }, 0, 6);
                }
            }

            //Spikes
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mtite"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (Spikes spike in mazeCollection.Mazes[i].MazeObjects.OfType<Spikes>())
                {
                    index6Data += WriteROM((ushort)index6Data, spike.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }

            //locks and keys, for now, there has to be an even number of locks and keys
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mlock"], WordToByteArray(index6Data), pointerIndex, 6);

                foreach (Lock lock_ in mazeCollection.Mazes[i].MazeObjects.OfType<Lock>())
                {
                    Key thisKey = mazeCollection.Mazes[i].MazeObjects.OfType<Key>().Where(k => k.KeyColor == lock_.LockColor).FirstOrDefault();
                    if (thisKey != null)
                    {
                        index6Data += WriteROM((ushort)index6Data, lock_.ToBytes(thisKey), 0, 6);
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }

            //Transporters
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mtran"], WordToByteArray(index6Data), pointerIndex, 6);

                var transporterGroups = mazeCollection.Mazes[i].MazeObjects.OfType<Transporter>().GroupBy(t => t.Color).Select(group => new { Key = group.Key, Count = group.Count() });

                foreach (var transporterPair in transporterGroups)
                {
                    List<Transporter> coloredTranporterMatches = mazeCollection.Mazes[i].MazeObjects.OfType<Transporter>().Where(t => t.Color == transporterPair.Key).ToList();
                    foreach (Transporter t in coloredTranporterMatches)
                    {
                        index6Data += WriteROM((ushort)index6Data, t.ToBytes(), 0, 6);
                    }
                }
                //write end of transports
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
                //write transportability data
                if (mazeCollection.Mazes[i].TransportabilityFlags.Count > 0)
                {
                    int flagCount = 0;
                    int flagValue = 0;
                    for(int f = 0; f < mazeCollection.Mazes[i].TransportabilityFlags.Count; f++)
                    {
                        flagValue = flagValue << 1;
                        if (f < mazeCollection.Mazes[i].TransportabilityFlags.Count)
                        {
                            if (mazeCollection.Mazes[i].TransportabilityFlags[f])
                            {
                                flagValue += 1;
                            }
                        }
                        
                        flagCount++;
                        if (flagCount > 7)
                        {
                            index6Data += WriteROM((ushort)index6Data, new byte[] { (byte)flagValue }, 0, 6);
                            flagCount = 0;
                            flagValue = 0;
                        }
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0xee }, 0,6);
            }
            //De Hand
            pointerIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mhand"], WordToByteArray(index6Data), pointerIndex, 6);
                //Hand Data
                Hand hand = mazeCollection.Mazes[i].MazeObjects.OfType<Hand>().FirstOrDefault();
                if (hand != null)
                {
                    Reactoid r = mazeCollection.Mazes[i].MazeObjects.OfType<Reactoid>().FirstOrDefault();
                    if (r != null)
                    {
                        index6Data += WriteROM((ushort)index6Data, hand.ToBytes(r.Position), 0, 6);
                    }
                }
                else
                {
                    index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
                }
            }
            //Clock
            int clockIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                //Write Table Pointer
                //pointerIndex += WriteROM((ushort)_exports["mclock"], WordToByteArray(index6Data), pointerIndex, 6);
                //Clock Data
                Clock clock = mazeCollection.Mazes[i].MazeObjects.OfType<Clock>().FirstOrDefault();
                if (clock != null)
                {
                    clockIndex += WriteROM((ushort)_exports["mclock"], clock.ToBytes(), clockIndex, 6);
                }
                else
                {
                    clockIndex += WriteROM((ushort)_exports["mclock"], new byte[] { 0x00 }, clockIndex, 6);
                }
            }
            //Boots
            int bootsIndex = 0;
            for (int i = 0; i < numMazes; i++)
            {
                Boots boots = mazeCollection.Mazes[i].MazeObjects.OfType<Boots>().FirstOrDefault();
                //Boots Data
                if (boots != null)
                {
                    bootsIndex += WriteROM((ushort)_exports["mboots"], boots.ToBytes(), bootsIndex, 6);
                }
                else
                {
                    bootsIndex += WriteROM((ushort)_exports["mboots"], new byte[] { 0x00 }, bootsIndex, 6);
                }
            }

            int mpodAddressBase = _exports["mpod"];
            //Escape Pod
            for (int i = 1; i < numMazes; i+=4)
            {
                //Pod Data
                EscapePod pod = mazeCollection.Mazes[i].MazeObjects.OfType<EscapePod>().FirstOrDefault();
                if (pod != null)
                {
                    mpodAddressBase += WriteROM((ushort)mpodAddressBase, pod.ToBytes(), 0, 6);
                }
                else
                {
                    mpodAddressBase += WriteROM((ushort)mpodAddressBase, new byte[] { 0x00 }, 0, 6);
                }
            }
            //Out Time
            int outAddressBase = _exports["outime"];
            for (int i = 0; i < numMazes; i++)
            {
                //Pod Data
                byte reactorTimer = 0;
                if (reactoid != null)
                {
                    reactorTimer = (byte)reactoid.Timer;
                }
                outAddressBase += WriteROM((ushort)outAddressBase, new byte[] { reactorTimer }, 0, 6);
            }
            //OxygenReward
            int oxyAddressBase = _exports["oxybonus"];
            for (int i = 0; i < numMazes; i++)
            {
                oxyAddressBase += WriteROM((ushort)oxyAddressBase, new byte[] { (byte)mazeCollection.Mazes[i].OxygenReward }, 0, 6);
            }

            if (index6Data >= 0x2000)
            {
                //this is bad

            }
            if (index7Data >= 0x2000 )
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

        private int WriteObjectsGeneric(byte[] bytes, int tablePointer, int tablePointerPageBase, int dataPointer, int dataPointerPageBase, List<MazeObject> objects)
        {
            int currentIndex = 0;

            foreach (MazeObject obj in objects)
            {
                currentIndex += bytes[dataPointer + dataPointerPageBase];
            }

            return currentIndex;
        }
    }
}
