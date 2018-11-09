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
using System.Windows.Forms;

namespace mhedit.Controllers
{
    static class MAMEHelper
    {
        public static MazeCollection GetMazeCollectionFromROM(string romPath, MazePropertiesUpdated mazePropertiesUpdatedHandler)
        {
            ROMDump rom = new ROMDump(romPath, romPath, romPath);

            MazeCollection mazeCollection = new MazeCollection("Production Mazes");
            mazeCollection.AuthorEmail = "Owen Rubin";

            for (int i = 0; i < 16; i++)
            {

                byte mazeType = (byte)(i & 0x03);

                Maze maze = new Maze((MazeType)mazeType, "Level " + (i + 1).ToString());
                maze.OnMazePropertiesUpdated += new MazePropertiesUpdated(mazePropertiesUpdatedHandler);
                //hint text - only first 12 mazes tho
                if (i < 12)
                {
                    byte messageIndex = rom.ReadByte(0x93FE, i);
                    maze.Hint = rom.GetMessage(messageIndex);
                }
                //build reactor
                ushort mazeInitIndex = rom.ReadWord(0x2581, i * 2);
                Reactoid reactor = new Reactoid();
                reactor.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                mazeInitIndex += 4;
                int timer = rom.HexToDecimal((int)rom.ReadByte(0x3355, i));
                reactor.Timer = timer;
                maze.AddObject(reactor);



                //pyroids
                byte firstValue = rom.ReadByte(mazeInitIndex, 0);
                while (firstValue != 0xff)
                {
                    Pyroid pyroid = new Pyroid();
                    pyroid.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    byte fireballVelX = rom.ReadByte(mazeInitIndex, 0);
                    if (fireballVelX > 0x70 && fireballVelX < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelXIncrement = fireballVelX;
                        fireballVelX = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;
                    byte fireballVelY = rom.ReadByte(mazeInitIndex, 0);
                    if (fireballVelY > 0x70 && fireballVelY < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelYIncrement = fireballVelY;
                        fireballVelY = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;

                    pyroid.Velocity.X = fireballVelX;
                    pyroid.Velocity.Y = fireballVelY;
                    maze.AddObject(pyroid);

                    firstValue = rom.ReadByte(mazeInitIndex, 0);

                    if (firstValue == 0xfe)
                    {
                        mazeInitIndex++;
                        //perkoids now...
                        break;
                    }
                }

                /// Perkoids 
                while (firstValue != 0xff)
                {
                    Perkoid perkoid = new Perkoid();
                    perkoid.LoadPosition(rom.ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    byte perkoidVelX = rom.ReadByte(mazeInitIndex, 0);
                    if (perkoidVelX > 0x70 && perkoidVelX < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte perkoidVelXIncrement = perkoidVelX;
                        perkoidVelX = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;
                    byte perkoidVelY = rom.ReadByte(mazeInitIndex, 0);
                    if (perkoidVelY > 0x70 && perkoidVelY < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte perkoidVelYIncrement = perkoidVelY;
                        perkoidVelY = rom.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;

                    perkoid.Velocity.X = perkoidVelX;
                    perkoid.Velocity.Y = perkoidVelY;
                    maze.AddObject(perkoid);

                    firstValue = rom.ReadByte(mazeInitIndex, 0);
                }

                //do oxygens now
                ushort oxygenBaseAddress = rom.ReadWord(0x25A9, i * 2);

                byte oxoidValue = rom.ReadByte(oxygenBaseAddress, 0);
                while (oxoidValue != 0x00)
                {
                    Oxoid oxoid = new Oxoid();
                    oxoid.LoadPosition(oxoidValue);
                    maze.AddObject(oxoid);

                    oxygenBaseAddress++;
                    oxoidValue = rom.ReadByte(oxygenBaseAddress, 0);
                }


                //do lightning (Force Fields)
                ushort lightningBaseAddress = rom.ReadWord(0x25D1, i * 2);

                byte lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                bool isHorizontal = true;

                if (lightningValue == 0xff)
                {
                    isHorizontal = false;
                    lightningBaseAddress++;
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
                    lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                    if (lightningValue == 0xff)
                    {
                        isHorizontal = false;
                        lightningBaseAddress++;
                    }
                    lightningValue = rom.ReadByte(lightningBaseAddress, 0);
                }

                //build arrows now
                ushort arrowBaseAddress = rom.ReadWord(0x25F9, i * 2);
                byte arrowValue = rom.ReadByte(arrowBaseAddress, 0);

                while (arrowValue != 0x00)
                {
                    Arrow arrow = new Arrow();
                    arrow.LoadPosition(arrowValue);
                    arrowBaseAddress++;
                    arrowValue = rom.ReadByte(arrowBaseAddress, 0);
                    arrow.ArrowDirection = (Containers.MazeObjects.ArrowDirection)arrowValue;
                    maze.AddObject(arrow);
                    arrowBaseAddress++;
                    arrowValue = rom.ReadByte(arrowBaseAddress, 0);
                }

                //maze walls
                //static first
                ushort wallBaseAddress = rom.ReadWord(0x2647, i * 2);
                byte wallValue = rom.ReadByte(wallBaseAddress, 0);

                while (wallValue != 0x00)
                {
                    int relativeWallValue = GetRelativeWallIndex(maze.MazeType, wallValue);
                    Point stampPoint = maze.PointFromStamp(relativeWallValue);
                    wallBaseAddress++;
                    wallValue = rom.ReadByte(wallBaseAddress, 0);
                    MazeWall wall = new MazeWall((MazeWallType)(wallValue & 0x07), stampPoint, relativeWallValue);
                    wall.UserWall = true;
                    maze.AddObject(wall);
                    wallBaseAddress++;
                    wallValue = rom.ReadByte(wallBaseAddress, 0);
                }

                //then dynamic walls
                //TODO: Dyanamic Walls loading from ROM
                //only level 9 and higher
                if (i > 7)
                {
                    ushort dynamicWallBaseAddress = rom.ReadWord(0x2667, (i-8) * 2);
                    byte dynamicWallIndex = rom.ReadByte(dynamicWallBaseAddress, 0);
                    while(dynamicWallIndex != 0x00)
                    {
                        int relativeWallIndex = GetRelativeWallIndex(maze.MazeType, dynamicWallIndex);
                        int baseTimer = rom.ReadByte(dynamicWallBaseAddress, 1);
                        int altTimer = rom.ReadByte(dynamicWallBaseAddress, 2);
                        MazeWallType baseType = (MazeWallType)rom.ReadByte(dynamicWallBaseAddress, 3);
                        MazeWallType altType = (MazeWallType)rom.ReadByte(dynamicWallBaseAddress, 4);
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
                        dynamicWallIndex = rom.ReadByte(dynamicWallBaseAddress, 0);
                    }
                }

                //one way walls
                ushort onewayBaseAddress = rom.ReadWord(0x2677, i * 2);
                byte onewayValue = rom.ReadByte(onewayBaseAddress, 0);

                while (onewayValue != 0x00)
                {
                    OneWay oneway = new OneWay();
                    if (onewayValue == 0xff)
                    {
                        oneway.Direction = OneWayDirection.Left;
                        onewayBaseAddress++;
                        onewayValue = rom.ReadByte(onewayBaseAddress, 0);
                    }
                    else
                    {
                        oneway.Direction = OneWayDirection.Right;
                    }
                    oneway.LoadPosition(onewayValue);
                    maze.AddObject(oneway);

                    onewayBaseAddress++;
                    onewayValue = rom.ReadByte(onewayBaseAddress, 0);
                }

                // Stalactite Level 5 and up
                if (i > 4)
                {
                    ushort stalactiteBaseAddress = rom.ReadWord(0x26B3, (i - 5) * 2);
                    byte stalactiteValue = rom.ReadByte(stalactiteBaseAddress, 0);

                    while (stalactiteValue != 0x00)
                    {
                        Spikes spikes = new Spikes();
                        spikes.LoadPosition(stalactiteValue);
                        maze.AddObject(spikes);

                        stalactiteBaseAddress++;
                        stalactiteValue = rom.ReadByte(stalactiteBaseAddress, 0);
                    }
                }

                //locks and keys
                ushort lockBaseAddress = rom.ReadWord(0x26D1, i * 2);
                byte lockValue = rom.ReadByte(lockBaseAddress, 0);

                while (lockValue != 0x00)
                {
                    byte lockColor = lockValue;
                    lockBaseAddress++;

                    Key key = new Key();
                    key.LoadPosition(rom.ReadByte(lockBaseAddress, 0));
                    key.KeyColor = (ObjectColor)lockColor;
                    maze.AddObject(key);

                    lockBaseAddress++;

                    Lock keylock = new Lock();
                    keylock.LoadPosition(rom.ReadByte(lockBaseAddress, 0));
                    keylock.LockColor = (ObjectColor)lockColor;
                    maze.AddObject(keylock);

                    lockBaseAddress++;
                    lockValue = rom.ReadByte(lockBaseAddress, 0);
                }

                //Escape pod
                // Levels 2,6,10,14 only
                if (mazeType == 0x01)
                {
                    ushort podBaseAddress = 0x32FF;
                    byte podValue = rom.ReadByte(podBaseAddress, i >> 2);

                    if (podValue > 0)
                    {
                        EscapePod pod = new EscapePod();
                        maze.AddObject(pod);
                    }
                }

                //clock & boots
                byte clockData = rom.ReadByte(0x3290, i);
                byte bootsData = rom.ReadByte(0x3290, i + 0x10);

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

                //transporters
                ushort transporterBaseAddress = rom.ReadWord(0x26F9, i * 2);
                byte colorValue = rom.ReadByte(transporterBaseAddress, 0);

                while (colorValue != 0x00)
                {
                    transporterBaseAddress++;
                    Transporter transporter = new Transporter();
                    transporter.LoadPosition(rom.ReadByte(transporterBaseAddress, 0));
                    transporter.Direction = OneWayDirection.Left;
                    if ((colorValue & 0x10) > 0)
                    {
                        transporter.Direction = OneWayDirection.Right;
                    }
                    transporter.Color = (ObjectColor)(colorValue & 0x07);
                    maze.AddObject(transporter);
                    transporterBaseAddress++;
                    colorValue = rom.ReadByte(transporterBaseAddress, 0);
                }

                //Laser Cannon
                /// Ok, So looking at why the cannons are shifted down on level 16.
                /// The issue is that the cannon goes up and down. The key is where
                /// the cannon starts with respect to the ceiling. Cannons 2 and 3
                /// start low (closer to the floor) than all others.
                /// I need to figure out how that's encoded.
                byte cannonAddressOffset = rom.ReadByte(0x269F, i);
                if (cannonAddressOffset != 0)
                {
                    ushort cannonBaseAddress = (ushort)(0x30B1 + cannonAddressOffset);
                    ushort cannonPointerAddress = rom.ReadWord(cannonBaseAddress, 0);

                    while (cannonPointerAddress != 0)
                    {
                        Cannon cannon = new Cannon();
                        cannon.LoadPosition(rom.ReadBytes(cannonPointerAddress, 4));
                        //now read data until we hit a cannon_end byte ($00)
                        int cannonCommandOffset = 4;
                        byte commandStartByte = commandStartByte = rom.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                                        cannonPosition.ShotSpeed = (byte)rom.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                                        cannonMovement.Velocity.X = (sbyte)rom.ReadByte(cannonPointerAddress, cannonCommandOffset);
                                        cannonCommandOffset++;
                                        cannonMovement.Velocity.Y = (sbyte)rom.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                            commandStartByte = commandStartByte = rom.ReadByte(cannonPointerAddress, cannonCommandOffset);
                        }
                        maze.AddObject(cannon);

                        cannonBaseAddress += 2;
                        cannonPointerAddress = rom.ReadWord(cannonBaseAddress, 0);
                    }
                }

                //build trips now
                // Level 5 and up
                if (i > 3)
                {
                    /// The max number of trips in a maze is 7. Trips are stored in a list
                    /// that is null terminated. Trips start on level 5 and exist on every
                    /// level to 16. 12 total levels.
                    ushort tripBaseAddress = rom.ReadWord((ushort)0x2627, ((i - 4) * 2));
                    /// Trip Pyroids are a 1 to 1 relationship to a trip. Trip Pyroids are
                    /// described in 3 bytes. Each level worth of trip pyroids are stored in
                    /// an array 8 pyroids long (7 + null) even if there are less than 7
                    /// trips in a level.
                    ushort tripPyroidBaseAddress = (ushort)(0x2D36 + ((i - 4) * 3 * 8));

                    byte tripX = rom.ReadByte(tripBaseAddress, 0);

                    while (tripX != 0)
                    {
                        TripPad trip = new TripPad();
                        trip.LoadPosition(tripX);
                        maze.AddObject(trip);

                        tripBaseAddress++;
                        tripX = rom.ReadByte(tripBaseAddress, 0);

                        /// level 8 has 2 pyroids per trip pad.
                        //trip pyroid too
                        byte bx = (byte)(0x7f & rom.ReadByte(tripPyroidBaseAddress++, 0));
                        byte by = rom.ReadByte(tripPyroidBaseAddress++, 0);
                        byte bv = rom.ReadByte(tripPyroidBaseAddress++, 0);

                        byte[] longBytes = new byte[4];

                        longBytes[0] = 0;
                        longBytes[1] = (byte)((bx & 0x1f) + 1);
                        longBytes[2] = 0x80;
                        longBytes[3] = by;

                        TripPadPyroid tpp = new TripPadPyroid();
                        tpp.LoadPosition(longBytes);
                        maze.AddObject(tpp);

                        trip.Pyroid = tpp;
                    }
                }

                //finally... de hand
                // Level 7 and up.
                if (i > 5)
                {
                    byte[] longBytes = new byte[4];

                    //longBytes[ 0 ] = 0;
                    //longBytes[ 2 ] = 0;

                    ushort handBaseAddress = rom.ReadWord((ushort)(0x2721 + ((i - 6) * 2)), 0);
                    longBytes[1] = rom.ReadByte(handBaseAddress, 0);
                    if (longBytes[1] != 0)
                    {
                        handBaseAddress++;
                        longBytes[3] = rom.ReadByte(handBaseAddress, 0);

                        Hand hand = new Hand();

                        hand.LoadPosition(longBytes);
                        maze.AddObject(hand);
                    }
                }
                mazeCollection.InsertMaze(i, maze);
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

        public static bool CreateMAMERom(Maze maze)
        {
            bool success = false;

            string mamePath = Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + "\\";

            string templatePath = Path.GetDirectoryName(Application.ExecutablePath) + "\\template\\";

            if (!File.Exists(Properties.Settings.Default.MameExecutable))
            {
                MessageBox.Show("MAME Executable not found. Check path in Preferences", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            if (!Directory.Exists(mamePath))
            {
                Directory.CreateDirectory(mamePath);
            }

            string backupPath = mamePath + "\\_backup\\";

            //delete the current backup folder so we can make a fresh copy
            Directory.Delete(backupPath, true);

            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            if (File.Exists(Path.GetDirectoryName(Properties.Settings.Default.MameExecutable) + "\\roms\\" + Properties.Settings.Default.MameDriver + ".zip"))
            {
                MessageBox.Show("The MAME driver you have specified is using a zipped ROM archive. The level editor does not support zipped ROM's. Please extract your '" + Properties.Settings.Default.MameDriver + ".zip' file to a '" + Properties.Settings.Default.MameDriver + "' folder under the 'roms' folder and delete the .zip file. The level editor will then overwrite your mhavoc ROM's as needed in order to run the level you have created.", "MAME Configuration Issue", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            foreach (string file in Directory.GetFiles(mamePath))
            {
                //copy each file here into _backup folder
                File.Copy(file, backupPath + Path.GetFileName(file), true);
            }

            maze.Validate();

            if (maze.IsValid)
            {
                ROMDump rom = new ROMDump(templatePath, mamePath, templatePath);

                Reactoid reactor = null;
                EscapePod pod = null;
                Boots boots = null;
                Clock clock = null;
                List<Pyroid> pyroids = new List<Pyroid>();
                List<Perkoid> perkoids = new List<Perkoid>();
                List<Oxoid> oxoids = new List<Oxoid>();
                List<LightningH> lightningHorizontal = new List<LightningH>();
                List<LightningV> lightningVertical = new List<LightningV>();
                List<Arrow> arrows = new List<Arrow>();
                List<MazeWall> staticWalls = new List<MazeWall>();
                List<MazeWall> dynamicWalls = new List<MazeWall>();
                List<OneWay> oneWayRights = new List<OneWay>();
                List<OneWay> oneWayLefts = new List<OneWay>();
                List<Spikes> spikes = new List<Spikes>();
                List<Lock> locks = new List<Lock>();
                List<Key> keys = new List<Key>();
                List<Transporter> transporters = new List<Transporter>();
                List<Cannon> cannons = new List<Cannon>();
                List<TripPad> tripPads = new List<TripPad>();
                Hand hand = null;

                foreach (MazeObject obj in maze.MazeObjects)
                {
                    if (obj is Reactoid)
                    {
                        reactor = (Reactoid)obj;
                    }
                    else if (obj is Pyroid)
                    {
                        pyroids.Add((Pyroid)obj);
                    }
                    else if (obj is Perkoid)
                    {
                        perkoids.Add((Perkoid)obj);
                    }
                    else if (obj is Oxoid)
                    {
                        oxoids.Add((Oxoid)obj);
                    }
                    else if (obj is LightningH)
                    {
                        lightningHorizontal.Add((LightningH)obj);
                    }
                    else if (obj is LightningV)
                    {
                        lightningVertical.Add((LightningV)obj);
                    }
                    else if (obj is Arrow)
                    {
                        arrows.Add((Arrow)obj);
                    }
                    else if (obj is MazeWall)
                    {
                        if (((MazeWall)obj).IsDynamicWall)
                        {
                            dynamicWalls.Add((MazeWall)obj);
                        }
                        else
                        {
                            staticWalls.Add((MazeWall)obj);
                        }
                    }
                    else if (obj is OneWay)
                    {
                        if (((OneWay)obj).Direction == OneWayDirection.Right)
                        {
                            oneWayRights.Add((OneWay)obj);
                        }
                        else if (((OneWay)obj).Direction == OneWayDirection.Left)
                        {
                            oneWayLefts.Add((OneWay)obj);
                        }
                    }
                    else if (obj is Spikes)
                    {
                        spikes.Add((Spikes)obj);
                    }
                    else if (obj is Lock)
                    {
                        locks.Add((Lock)obj);
                    }
                    else if (obj is Key)
                    {
                        keys.Add((Key)obj);
                    }
                    else if (obj is EscapePod)
                    {
                        pod = (EscapePod)obj;
                    }
                    else if (obj is Boots)
                    {
                        boots = (Boots)obj;
                    }
                    else if (obj is Clock)
                    {
                        clock = (Clock)obj;
                    }
                    else if (obj is Transporter)
                    {
                        transporters.Add((Transporter)obj);
                    }
                    else if (obj is Cannon)
                    {
                        cannons.Add((Cannon)obj);
                    }
                    else if (obj is TripPad)
                    {
                        tripPads.Add((TripPad)obj);
                    }
                    else if (obj is Hand)
                    {
                        hand = (Hand)obj;
                    }
                }

                /////////////////////////////
                // Start building ROM here //
                /////////////////////////////
                //first is the level selection
                rom.Write(ROMAddress.levelst, (byte)maze.MazeType, 1);

                //next hint text
                rom.Write(ROMAddress.mzh0, rom.GetText(maze.Hint), 0);

                //build reactor, pyroids and perkoids now...
                //write reactor
                int offset = 0;
                offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(reactor.Position)), offset);
                foreach (Pyroid pyroid in pyroids)
                {
                    offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(pyroid.Position)), offset);
                    offset += rom.Write(ROMAddress.mzsc0, new byte[] { (byte)pyroid.Velocity.X, (byte)pyroid.Velocity.Y }, offset);
                }
                if (perkoids.Count > 0)
                {
                    offset += rom.Write(ROMAddress.mzsc0, (byte)0xfe, offset);
                    foreach (Perkoid perkoid in perkoids)
                    {
                        offset += rom.Write(ROMAddress.mzsc0, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(perkoid.Position)), offset);
                        offset += rom.Write(ROMAddress.mzsc0, new byte[] { (byte)perkoid.Velocity.X, (byte)perkoid.Velocity.Y }, offset);
                    }
                }
                rom.Write(ROMAddress.mzsc0, (byte)0xff, offset);
                //reactor timer, we will write all 4 entries for now...
                rom.Write(ROMAddress.outime, new byte[] { GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer), GetDecimalByte(reactor.Timer) }, 0);

                //do oxygens now
                offset = 0;
                foreach (Oxoid oxoid in oxoids)
                {
                    byte[] oxoidPositionBytes = Context.PointToByteArrayPacked(oxoid.Position);
                    offset += rom.Write(ROMAddress.mzdc0, oxoidPositionBytes, offset);
                }
                rom.Write(ROMAddress.mzdc0, 0, offset);

                //do lightning (Force Fields)
                offset = 0;
                if (lightningHorizontal.Count > 0)
                {
                    foreach (LightningH lightning in lightningHorizontal)
                    {
                        offset += rom.Write(ROMAddress.mzlg0, Context.PointToByteArrayPacked(lightning.Position), offset);
                    }
                    //end horizontal with 0xff
                    offset += rom.Write(ROMAddress.mzlg0, (byte)0xff, offset);
                }
                foreach (LightningV lightning in lightningVertical)
                {
                    offset += rom.Write(ROMAddress.mzlg0, Context.PointToByteArrayPacked(lightning.Position), offset);
                }
                //end all with 0x00
                rom.Write(ROMAddress.mzlg0, (byte)0, offset);

                //build arrows now
                offset = 0;
                foreach (Arrow arrow in arrows)
                {
                    offset += rom.Write(ROMAddress.mzar0, Context.PointToByteArrayPacked(arrow.Position), offset);
                    offset += rom.Write(ROMAddress.mzar0, (byte)arrow.ArrowDirection, offset);
                }
                rom.Write(ROMAddress.mzar0, (byte)0, offset);

                //maze walls
                //static first
                offset = 0;
                int wallDataOffset = 18; //this is a set of blank data offsets defined in the mhavoc source for some reason
                foreach (MazeWall wall in staticWalls)
                {
                    offset += rom.Write(ROMAddress.mzta0, (byte)(wallDataOffset + (maze.PointToStamp(wall.Position))), offset);
                    offset += rom.Write(ROMAddress.mzta0, (byte)wall.WallType, offset);
                }
                rom.Write(ROMAddress.mzta0, (byte)0, offset);

                //then dynamic
                offset = 0;
                foreach (MazeWall wall in dynamicWalls)
                {
                    offset += rom.Write(ROMAddress.mztd0, (byte)(wallDataOffset + (maze.PointToStamp(wall.Position))), offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.DynamicWallTimout, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.AlternateWallTimeout, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.WallType, offset);
                    offset += rom.Write(ROMAddress.mztd0, (byte)wall.AlternateWallType, offset);
                }
                rom.Write(ROMAddress.mztd0, (byte)0, offset);

                //one way walls
                offset = 0;
                if (oneWayRights.Count > 0)
                {
                    foreach (OneWay oneway in oneWayRights)
                    {
                        offset += rom.Write(ROMAddress.mone0, Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
                    }
                    offset += rom.Write(ROMAddress.mone0, (byte)0xff, offset);
                }
                foreach (OneWay oneway in oneWayLefts)
                {
                    offset += rom.Write(ROMAddress.mone0, Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
                }
                rom.Write(ROMAddress.mone0, (byte)0, offset);

                //build spikes now
                offset = 0;
                foreach (Spikes spike in spikes)
                {
                    offset += rom.Write(ROMAddress.tite0, Context.PointToByteArrayPacked(spike.Position), offset);
                }
                rom.Write(ROMAddress.tite0, (byte)0, offset);

                //locks and keys, for now, there has to be an even number of locks and keys
                offset = 0;
                for (int i = 0; i < locks.Count; i++)
                {
                    Lock thisLock = locks[i];
                    Key thisKey = keys.Where(k => k.KeyColor == thisLock.LockColor).FirstOrDefault();
                    if (thisKey != null)
                    {
                        offset += rom.Write(ROMAddress.lock0, (byte)thisLock.LockColor, offset);
                        offset += rom.Write(ROMAddress.lock0, Context.PointToByteArrayPacked(thisKey.Position), offset);
                        offset += rom.Write(ROMAddress.lock0, Context.PointToByteArrayPacked(thisLock.Position), offset);
                    }
                }
                rom.Write(ROMAddress.lock0, (byte)0, offset);

                //Escape pod
                if (pod != null)
                {
                    rom.Write(ROMAddress.mpod, (byte)pod.Option, 0);
                }

                //clock & boots
                if (clock != null)
                {
                    rom.Write(ROMAddress.mclock, Context.PointToByteArrayPacked(clock.Position), 0);
                }
                if (boots != null)
                {
                    rom.Write(ROMAddress.mclock, Context.PointToByteArrayPacked(boots.Position), 0x10);
                }

                //transporters
                offset = 0;
                var tpairs = transporters.GroupBy(t => t.Color).Select(group => new { Color = group.Key, Count = keys.Count() });
                if (tpairs.Count() > 0)
                {
                    foreach (var i in tpairs)
                    {
                        if (i.Count == 2)
                        {
                            //there are two... move ahead
                            List<Transporter> colorT = transporters.Where(t => t.Color == i.Color).ToList();
                            foreach (Transporter t in colorT)
                            {
                                byte colorByte = (byte)(((byte)t.Color) & 0x0F);
                                if (t.Direction == OneWayDirection.Right)
                                {
                                    colorByte += 0x10;
                                }
                                offset += rom.Write(ROMAddress.tran0, colorByte, offset);
                                offset += rom.Write(ROMAddress.tran0, Context.PointToByteArrayPacked(t.Position), offset);
                            }
                        }
                    }
                    //write end of transports
                    offset += rom.Write(ROMAddress.tran0, 0, offset);
                    //write transportability data
                    offset += rom.Write(ROMAddress.tran0, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, offset);
                }

                //Laser Cannon
                offset = 0;
                int pointer = 0;
                if (cannons.Count > 0)
                {
                    rom.Write(ROMAddress.mcan, new byte[] { 0x02, 0x02, 0x02, 0x02 }, 0);
                }
                for (int i = 0; i < cannons.Count; i++)
                {
                    Cannon cannon = cannons[i];
                    pointer += rom.Write(ROMAddress.mcan0, (UInt16)(rom.GetAddress(ROMAddress.mcand) + offset), pointer);
                    //cannon location first...
                    offset += rom.Write(ROMAddress.mcand, Context.PointToByteArrayLong(Context.ConvertPixelsToVector(cannon.Position)), offset);
                    //now cannon commands
                    foreach (iCannonMovement movement in cannon.Movements)
                    {
                        byte command = 0;
                        if (movement is CannonMovementMove)
                        {
                            CannonMovementMove move = (CannonMovementMove)movement;
                            command = 0x80;
                            if (move.Velocity.X == 0 && move.Velocity.Y == 0)
                            {
                                offset += rom.Write(ROMAddress.mcand, command, offset);
                            }
                            else
                            {
                                command += (byte)((move.WaitFrames & 0x3F)>>2);
                                offset += rom.Write(ROMAddress.mcand, command, offset);
                                //write velocities
                                if (move.Velocity.X >= 0)
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.X & 0x3F), offset);
                                }
                                else
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.X | 0xc0), offset);
                                }
                                if (move.Velocity.Y >= 0)
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.Y & 0x3F), offset);
                                }
                                else
                                {
                                    offset += rom.Write(ROMAddress.mcand, (byte)(move.Velocity.Y | 0xc0), offset);
                                }
                            }
                        }
                        else if (movement is CannonMovementPause)
                        {
                            CannonMovementPause pause = (CannonMovementPause)movement;
                            command = 0xc0;
                            command += (byte)(pause.WaitFrames & 0x3F);
                            offset += rom.Write(ROMAddress.mcand, command, offset);
                        }
                        else if (movement is CannonMovementReturn)
                        {
                            CannonMovementReturn ret = (CannonMovementReturn)movement;
                            command = 0x00;
                            offset += rom.Write(ROMAddress.mcand, 0, offset);
                        }
                        else if (movement is CannonMovementPosition)
                        {
                            CannonMovementPosition position = (CannonMovementPosition)movement;
                            command = 0x40;
                            command += (byte)(((int)position.Position) << 3);
                            command += (byte)(((int)position.Speed) << 1);
                            if (position.ShotSpeed > 0)
                            {
                                command += 0x01;
                            }
                            offset += rom.Write(ROMAddress.mcand, command, offset);
                            if (position.ShotSpeed > 0)
                            {
                                //write velocity now too
                                offset += rom.Write(ROMAddress.mcand, position.ShotSpeed, offset);
                            }
                        }
                    }
                }
                //build trips now
                offset = 0;
                int tripoffset = 0;
                foreach (TripPad trip in tripPads)
                {
                    offset += rom.Write(ROMAddress.mztr0, Context.PointToByteArrayPacked(trip.Position), offset);
                    byte[] position = Context.PointToByteArrayShort(trip.Pyroid.Position);
                    if (trip.Pyroid.PyroidStyle == PyroidStyle.Single)
                    {
                        position[0] += 0x80;
                    }
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x18);
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x30);
                    rom.Write(ROMAddress.trtbl, position, tripoffset + 0x48);
                    tripoffset += rom.Write(ROMAddress.trtbl, position, tripoffset);

                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x18);
                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x30);
                    rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset + 0x48);
                    tripoffset += rom.Write(ROMAddress.trtbl, new byte[] { (byte)trip.Pyroid.Velocity }, tripoffset);

                }
                rom.Write(ROMAddress.mztr0, (byte)0, offset);
                //de hand finally
                offset = 0;
                if (hand != null)
                {
                    byte[] handLocation = Context.PointToByteArrayShort(hand.Position);
                    offset += rom.Write(ROMAddress.hand0, handLocation, offset);
                    byte[] reactoidLocation = Context.PointToByteArrayShort(reactor.Position);
                    int xAccordians = Math.Abs(reactoidLocation[0] - handLocation[0]);
                    int yAccordians = Math.Abs(handLocation[1] - reactoidLocation[1]);
                    offset += rom.Write(ROMAddress.hand0, new byte[] { (byte)((xAccordians * 2) + 1), (byte)(yAccordians * 2), 0x3F, 0x0B, 0x1F, 0x05, 0x03 }, offset);
                }

                //write it BABY!!!!
                if (rom.Save())
                {
                    success = true;
                }
            }
            else
            {
                MessageBox.Show(String.Join("\r\n", maze.ValidationMessage.ToArray()), "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                success = false;
            }
            return success;
        }


        public static byte GetDecimalByte(int value)
        {
            byte outValue = 0;
            string valueString = value.ToString();
            for (int i = 0; i < valueString.Length; i++)
            {
                if (i > 0) outValue = (byte)(outValue << 4);
                outValue += (byte)(((byte)0xF) & byte.Parse(valueString[i].ToString()));
            }
            return outValue;
        }
    }
}
