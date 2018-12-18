using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeObjects;
using mhedit.GameControllers;
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
            MajorHavoc mh = new MajorHavoc(romPath, romPath, romPath);

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
                    byte messageIndex = mh.ReadByte(0x93FE, i);
                    maze.Hint = mh.GetMessage(messageIndex);
                }
                //build reactor
                ushort mazeInitIndex = mh.ReadWord(0x2581, i * 2);
                Reactoid reactor = new Reactoid();
                reactor.LoadPosition(mh.ReadBytes(mazeInitIndex, 4));
                mazeInitIndex += 4;
                int timer = mh.FromDecimal((int)mh.ReadByte(0x3355, i));
                reactor.Timer = timer;
                maze.AddObject(reactor);

                //pyroids
                byte firstValue = mh.ReadByte(mazeInitIndex, 0);
                while (firstValue != 0xff)
                {
                    Pyroid pyroid = new Pyroid();
                    pyroid.LoadPosition(mh.ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    byte fireballVelX = mh.ReadByte(mazeInitIndex, 0);
                    if (fireballVelX > 0x70 && fireballVelX < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelXIncrement = fireballVelX;
                        pyroid.IncrementingVelocity.X = fireballVelXIncrement;
                        fireballVelX = mh.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;
                    byte fireballVelY = mh.ReadByte(mazeInitIndex, 0);
                    if (fireballVelY > 0x70 && fireballVelY < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte fireballVelYIncrement = fireballVelY;
                        pyroid.IncrementingVelocity.Y = fireballVelYIncrement;
                        fireballVelY = mh.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;

                    pyroid.Velocity.X = fireballVelX;
                    pyroid.Velocity.Y = fireballVelY;
                    maze.AddObject(pyroid);

                    firstValue = mh.ReadByte(mazeInitIndex, 0);

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
                    perkoid.LoadPosition(mh.ReadBytes(mazeInitIndex, 4));
                    mazeInitIndex += 4;
                    byte perkoidVelX = mh.ReadByte(mazeInitIndex, 0);
                    if (perkoidVelX > 0x70 && perkoidVelX < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte perkoidVelXIncrement = perkoidVelX;
                        perkoidVelX = mh.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;
                    byte perkoidVelY = mh.ReadByte(mazeInitIndex, 0);
                    if (perkoidVelY > 0x70 && perkoidVelY < 0x90)
                    {
                        //incrementing velocity
                        mazeInitIndex++;
                        byte perkoidVelYIncrement = perkoidVelY;
                        perkoidVelY = mh.ReadByte(mazeInitIndex, 0);
                    }
                    mazeInitIndex++;

                    perkoid.Velocity.X = perkoidVelX;
                    perkoid.Velocity.Y = perkoidVelY;
                    maze.AddObject(perkoid);

                    firstValue = mh.ReadByte(mazeInitIndex, 0);
                }

                //do oxygens now
                ushort oxygenBaseAddress = mh.ReadWord(0x25A9, i * 2);

                byte oxoidValue = mh.ReadByte(oxygenBaseAddress, 0);
                while (oxoidValue != 0x00)
                {
                    Oxoid oxoid = new Oxoid();
                    oxoid.LoadPosition(oxoidValue);
                    maze.AddObject(oxoid);

                    oxygenBaseAddress++;
                    oxoidValue = mh.ReadByte(oxygenBaseAddress, 0);
                }

                //do lightning (Force Fields)
                ushort lightningBaseAddress = mh.ReadWord(0x25D1, i * 2);

                byte lightningValue = mh.ReadByte(lightningBaseAddress, 0);
                bool isHorizontal = true;

                if (lightningValue == 0xff)
                {
                    isHorizontal = false;
                    lightningBaseAddress++;
                    lightningValue = mh.ReadByte(lightningBaseAddress, 0);
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
                    lightningValue = mh.ReadByte(lightningBaseAddress, 0);
                    if (lightningValue == 0xff)
                    {
                        isHorizontal = false;
                        lightningBaseAddress++;
                    }
                    lightningValue = mh.ReadByte(lightningBaseAddress, 0);
                }

                //build arrows now
                ushort arrowBaseAddress = mh.ReadWord(0x25F9, i * 2);
                byte arrowValue = mh.ReadByte(arrowBaseAddress, 0);

                while (arrowValue != 0x00)
                {
                    Arrow arrow = new Arrow();
                    arrow.LoadPosition(arrowValue);
                    arrowBaseAddress++;
                    arrowValue = mh.ReadByte(arrowBaseAddress, 0);
                    arrow.ArrowDirection = (Containers.MazeObjects.ArrowDirection)arrowValue;
                    maze.AddObject(arrow);
                    arrowBaseAddress++;
                    arrowValue = mh.ReadByte(arrowBaseAddress, 0);
                }

                //maze walls
                //static first
                ushort wallBaseAddress = mh.ReadWord(0x2647, i * 2);
                byte wallValue = mh.ReadByte(wallBaseAddress, 0);

                while (wallValue != 0x00)
                {
                    int relativeWallValue = GetRelativeWallIndex(maze.MazeType, wallValue);
                    Point stampPoint = maze.PointFromStamp(relativeWallValue);
                    wallBaseAddress++;
                    wallValue = mh.ReadByte(wallBaseAddress, 0);
                    MazeWall wall = new MazeWall((MazeWallType)(wallValue & 0x07), stampPoint, relativeWallValue);
                    wall.UserWall = true;
                    maze.AddObject(wall);
                    wallBaseAddress++;
                    wallValue = mh.ReadByte(wallBaseAddress, 0);
                }

                //then dynamic walls
                //TODO: Dyanamic Walls loading from ROM
                //only level 9 and higher
                if (i > 7)
                {
                    ushort dynamicWallBaseAddress = mh.ReadWord(0x2667, (i-8) * 2);
                    byte dynamicWallIndex = mh.ReadByte(dynamicWallBaseAddress, 0);
                    while(dynamicWallIndex != 0x00)
                    {
                        int relativeWallIndex = GetRelativeWallIndex(maze.MazeType, dynamicWallIndex);
                        int baseTimer = mh.ReadByte(dynamicWallBaseAddress, 1);
                        int altTimer = mh.ReadByte(dynamicWallBaseAddress, 2);
                        MazeWallType baseType = (MazeWallType)mh.ReadByte(dynamicWallBaseAddress, 3);
                        MazeWallType altType = (MazeWallType)mh.ReadByte(dynamicWallBaseAddress, 4);
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
                        dynamicWallIndex = mh.ReadByte(dynamicWallBaseAddress, 0);
                    }
                }

                //one way walls
                ushort onewayBaseAddress = mh.ReadWord(0x2677, i * 2);
                byte onewayValue = mh.ReadByte(onewayBaseAddress, 0);

                while (onewayValue != 0x00)
                {
                    OneWay oneway = new OneWay();
                    if (onewayValue == 0xff)
                    {
                        oneway.Direction = OneWayDirection.Left;
                        onewayBaseAddress++;
                        onewayValue = mh.ReadByte(onewayBaseAddress, 0);
                    }
                    else
                    {
                        oneway.Direction = OneWayDirection.Right;
                    }
                    oneway.LoadPosition(onewayValue);
                    maze.AddObject(oneway);

                    onewayBaseAddress++;
                    onewayValue = mh.ReadByte(onewayBaseAddress, 0);
                }

                // Stalactite Level 5 and up
                if (i > 4)
                {
                    ushort stalactiteBaseAddress = mh.ReadWord(0x26B3, (i - 5) * 2);
                    byte stalactiteValue = mh.ReadByte(stalactiteBaseAddress, 0);

                    while (stalactiteValue != 0x00)
                    {
                        Spikes spikes = new Spikes();
                        spikes.LoadPosition(stalactiteValue);
                        maze.AddObject(spikes);

                        stalactiteBaseAddress++;
                        stalactiteValue = mh.ReadByte(stalactiteBaseAddress, 0);
                    }
                }

                //locks and keys
                ushort lockBaseAddress = mh.ReadWord(0x26D1, i * 2);
                byte lockValue = mh.ReadByte(lockBaseAddress, 0);

                while (lockValue != 0x00)
                {
                    byte lockColor = lockValue;
                    lockBaseAddress++;

                    Key key = new Key();
                    key.LoadPosition(mh.ReadByte(lockBaseAddress, 0));
                    key.KeyColor = (ObjectColor)lockColor;
                    maze.AddObject(key);

                    lockBaseAddress++;

                    Lock keylock = new Lock();
                    keylock.LoadPosition(mh.ReadByte(lockBaseAddress, 0));
                    keylock.LockColor = (ObjectColor)lockColor;
                    maze.AddObject(keylock);

                    lockBaseAddress++;
                    lockValue = mh.ReadByte(lockBaseAddress, 0);
                }

                //Escape pod
                // Levels 2,6,10,14 only
                if (mazeType == 0x01)
                {
                    ushort podBaseAddress = 0x32FF;
                    byte podValue = mh.ReadByte(podBaseAddress, i >> 2);

                    if (podValue > 0)
                    {
                        EscapePod pod = new EscapePod();
                        maze.AddObject(pod);
                    }
                }

                //clock & boots
                byte clockData = mh.ReadByte(0x3290, i);
                byte bootsData = mh.ReadByte(0x3290, i + 0x10);

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
                ushort transporterBaseAddress = mh.ReadWord(0x26F9, i * 2);
                byte colorValue = mh.ReadByte(transporterBaseAddress, 0);

                while (colorValue != 0x00)
                {
                    transporterBaseAddress++;
                    Transporter transporter = new Transporter();
                    transporter.LoadPosition(mh.ReadByte(transporterBaseAddress, 0));
                    transporter.Direction = TransporterDirection.Left;
                    if ((colorValue & 0x10) > 0)
                    {
                        transporter.Direction = TransporterDirection.Right;
                    }
                    transporter.Color = (ObjectColor)(colorValue & 0x07);
                    maze.AddObject(transporter);
                    transporterBaseAddress++;
                    colorValue = mh.ReadByte(transporterBaseAddress, 0);
                }

                //Laser Cannon
                // Ok, So looking at why the cannons are shifted down on level 16.
                // The issue is that the cannon goes up and down. The key is where
                // the cannon starts with respect to the ceiling. Cannons 2 and 3
                // start low (closer to the floor) than all others.
                // I need to figure out how that's encoded.
                byte cannonAddressOffset = mh.ReadByte(0x269F, i);
                if (cannonAddressOffset != 0)
                {
                    ushort cannonBaseAddress = (ushort)(0x30B1 + cannonAddressOffset);
                    ushort cannonPointerAddress = mh.ReadWord(cannonBaseAddress, 0);

                    while (cannonPointerAddress != 0)
                    {
                        Cannon cannon = new Cannon();
                        cannon.LoadPosition(mh.ReadBytes(cannonPointerAddress, 4));
                        //now read data until we hit a cannon_end byte ($00)
                        int cannonCommandOffset = 4;
                        byte commandStartByte = commandStartByte = mh.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                                        cannonPosition.ShotSpeed = (byte)mh.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                                        cannonMovement.Velocity.X = (sbyte)mh.ReadByte(cannonPointerAddress, cannonCommandOffset);
                                        cannonCommandOffset++;
                                        cannonMovement.Velocity.Y = (sbyte)mh.ReadByte(cannonPointerAddress, cannonCommandOffset);
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
                            commandStartByte = commandStartByte = mh.ReadByte(cannonPointerAddress, cannonCommandOffset);
                        }
                        maze.AddObject(cannon);

                        cannonBaseAddress += 2;
                        cannonPointerAddress = mh.ReadWord(cannonBaseAddress, 0);
                    }
                }

                //build trips now
                // Level 5 and up
                if (i > 3)
                {
                    // The max number of trips in a maze is 7. Trips are stored in a list
                    // that is null terminated. Trips start on level 5 and exist on every
                    // level to 16. 12 total levels.
                    ushort tripBaseAddress = mh.ReadWord((ushort)0x2627, ((i - 4) * 2));
                    // Trip Pyroids are a 1 to 1 relationship to a trip. Trip Pyroids are
                    // described in 3 bytes. Each level worth of trip pyroids are stored in
                    // an array 8 pyroids long (7 + null) even if there are less than 7
                    // trips in a level.
                    ushort tripPyroidBaseAddress = (ushort)(0x2D36 + ((i - 4) * 3 * 8));

                    byte tripX = mh.ReadByte(tripBaseAddress, 0);

                    while (tripX != 0)
                    {
                        TripPad trip = new TripPad();
                        trip.LoadPosition(tripX);
                        maze.AddObject(trip);

                        tripBaseAddress++;
                        tripX = mh.ReadByte(tripBaseAddress, 0);

                        // level 8 has 2 pyroids per trip pad.
                        //trip pyroid too

                        byte xdata = mh.ReadByte(tripPyroidBaseAddress++, 0);
                        byte xh = (byte)(0x7f & xdata);
                        byte styleFlag = (byte)(0x80 & xdata);
                        byte yh = mh.ReadByte(tripPyroidBaseAddress++, 0);
                        byte vdata = mh.ReadByte(tripPyroidBaseAddress++, 0);

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
                }

                //finally... de hand
                // Level 7 and up.
                if (i > 5)
                {
                    byte[] longBytes = new byte[4];

                    //longBytes[ 0 ] = 0;
                    //longBytes[ 2 ] = 0;

                    ushort handBaseAddress = mh.ReadWord((ushort)(0x2721 + ((i - 6) * 2)), 0);
                    longBytes[1] = mh.ReadByte(handBaseAddress, 0);
                    if (longBytes[1] != 0)
                    {
                        handBaseAddress++;
                        longBytes[3] = mh.ReadByte(handBaseAddress, 0);

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

        public static bool SaveROM(MazeCollection collection, Maze maze)
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
            if (Directory.Exists(backupPath))
            {
                Directory.Delete(backupPath, true);
            }

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

            collection.Validate();

            if (collection.IsValid)
            {
                //we will always serialize to target 'The Promised End' here in this editor.
                IGameController controller = new MajorHavocPromisedEnd(templatePath, mamePath, templatePath);
                bool serializeSuccess = controller.SerializeObjects(collection, maze);
                if (serializeSuccess)
                {
                    success = controller.WriteFiles();
                }
                else
                {
                    MessageBox.Show("There was an issue serializing the maze objects to binary.", "Serialization Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(collection.ValidationMessage, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                success = false;
            }
            return success;
        }


        //public static byte GetDecimalByte(int value)
        //{
        //    byte outValue = 0;
        //    string valueString = value.ToString();
        //    for (int i = 0; i < valueString.Length; i++)
        //    {
        //        if (i > 0) outValue = (byte)(outValue << 4);
        //        outValue += (byte)(((byte)0xF) & byte.Parse(valueString[i].ToString()));
        //    }
        //    return outValue;
        //}
    }
}
