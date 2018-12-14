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
    public class MajorHavoc : GameController, IGameController
    {
        #region Enums
        //public enum ROMAddress
        //{
        //    levelst,
        //    mzsc0,
        //    mzdc0,
        //    mzlg0,
        //    mzar0,
        //    mzta0,
        //    mztd0,
        //    mone0,
        //    tite0,
        //    lock0,
        //    outime,
        //    mpod,
        //    cksumal,
        //    cksumah,
        //    cksump01,
        //    mclock,
        //    mboots,
        //    tran0,
        //    mzh0,
        //    mcan,
        //    mcan0,
        //    mcand,
        //    mztr0,
        //    trtbl,
        //    hand0
        //}

        //public enum ROMPhysical
        //{
        //    AlphaHigh,
        //    AlphaLow,
        //    Page0,
        //    Page1,
        //    Page2,
        //    Page3
        //}

        #endregion

        #region Private Variables

        private string _templatePath = String.Empty;
        private string _mamePath = String.Empty;
        private byte[] _alphaHigh = new byte[0x4000];
        private byte[] _alphaLow = new byte[0x4000];
        private byte[] _page01 = new byte[0x4000];
        private string[] _exports;
        private string _alphaHighROM = "136025.217";
        private string _alphaLowROM = "136025.216";
        private string _page01ROM = "136025.215";

        #endregion


        public MajorHavoc(string templatePath, string mamePath, string exportPath)
        {
            _templatePath = templatePath;
            _mamePath = mamePath;

            //load our exports
            string exportFile = exportPath + "havoc.exp";
            if (File.Exists(exportFile))
            {
                _exports = File.ReadAllLines(exportFile);
            }

            //load up our roms for now...
            string alphaHighFileName = _templatePath + _alphaHighROM;
            string alphaLowFileName = _templatePath + _alphaLowROM;
            string page01FileName = _templatePath + _page01ROM;

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
            bytes.Add(0x80);
            return bytes.ToArray();
        }


        public Tuple<ushort, int> GetAddress(string location)
        {
            ushort address = 0;
            //search the export list for this address...

            bool found = false;
            foreach (string line in _exports)
            {
                //this is an MHEDIT export
                string[] split = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 3)
                {
                    if (split[0].ToLower() == location.ToString().ToLower())
                    {
                        address = ushort.Parse(split[2].Replace("$", ""), System.Globalization.NumberStyles.HexNumber);
                        found = true;
                    }
                }
            }

            if (!found)
            {
                throw new Exception("Address not found: " + location.ToString());
            }

            return new Tuple<ushort, int>(address, 2);
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
            ushort address = GetAddress(location).Item1;
            return WriteROM(address, new byte[] { data }, offset);
        }

        public int Write(string location, UInt16 data, int offset)
        {
            ushort address = GetAddress(location).Item1;
            byte datahigh = (byte)(data >> 8);
            byte datalow = (byte)(data & 0xff);
            return WriteROM(address, new byte[] { datalow, datahigh }, offset);
        }

        public int Write(string location, byte[] data, int offset)
        {
            ushort address = GetAddress(location).Item1;
            return WriteROM(address, data, offset);
        }



        public bool Save()
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

            string alphaHighFileNameMame = _mamePath + _alphaHighROM;
            string alphaLowFileNameMame = _mamePath + _alphaLowROM;
            string page01FileNameMame = _mamePath + _page01ROM;

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
                File.Copy(_templatePath + rom, _mamePath + rom, true);
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




        public bool CreateMAMERom(Maze maze)
        {
            bool success = false;

            //ROMDump rom = new ROMDump(_templatePath, _mamePath, _templatePath);

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
            Write("levelst", (byte)maze.MazeType, 1);
            
            //next hint text
            Write("mzh0", GetText(maze.Hint), 0);

            //build reactor, pyroids and perkoids now...
            //write reactor
            int offset = 0;
            offset += Write("mzsc0", Context.PointToByteArrayLong(Context.ConvertPixelsToVector(reactor.Position)), offset);
            foreach (Pyroid pyroid in pyroids)
            {
                offset += Write("mzsc0", Context.PointToByteArrayLong(Context.ConvertPixelsToVector(pyroid.Position)), offset);

                if (pyroid.IncrementingVelocity.X != 0)
                {
                    offset += Write("mzsc0", new byte[] { (byte)(0x80 | pyroid.IncrementingVelocity.X) }, offset);
                }
                offset += Write("mzsc0", new byte[] { (byte)pyroid.Velocity.X }, offset);

                if (pyroid.IncrementingVelocity.Y != 0)
                {
                    offset += Write("mzsc0", new byte[] { (byte)(0x80 | pyroid.IncrementingVelocity.Y) }, offset);
                }
                offset += Write("mzsc0", new byte[] { (byte)pyroid.Velocity.Y }, offset);
            }
            if (perkoids.Count > 0)
            {
                offset += Write("mzsc0", (byte)0xfe, offset);
                foreach (Perkoid perkoid in perkoids)
                {
                    offset += Write("mzsc0", Context.PointToByteArrayLong(Context.ConvertPixelsToVector(perkoid.Position)), offset);
                    if (perkoid.IncrementingVelocity.X != 0)
                    {
                        offset += Write("mzsc0", new byte[] { (byte)(0x80 | perkoid.IncrementingVelocity.X) }, offset);
                    }
                    offset += Write("mzsc0", new byte[] { (byte)perkoid.Velocity.X }, offset);

                    if (perkoid.IncrementingVelocity.Y != 0)
                    {
                        offset += Write("mzsc0", new byte[] { (byte)(0x80 | perkoid.IncrementingVelocity.Y) }, offset);
                    }
                    offset += Write("mzsc0", new byte[] { (byte)perkoid.Velocity.Y }, offset);
                }
            }
            Write("mzsc0", (byte)0xff, offset);
            //reactor timer, we will write all 4 entries for now...
            Write("outime", new byte[] { (byte)ToDecimal(reactor.Timer), (byte)ToDecimal(reactor.Timer), (byte)ToDecimal(reactor.Timer), (byte)ToDecimal(reactor.Timer) }, 0);

            //do oxygens now
            offset = 0;
            foreach (Oxoid oxoid in oxoids)
            {
                byte[] oxoidPositionBytes = Context.PointToByteArrayPacked(oxoid.Position);
                offset += Write("mzdc0", oxoidPositionBytes, offset);
            }
            Write("mzdc0", 0, offset);

            //do lightning (Force Fields)
            offset = 0;
            foreach (LightningH lightning in lightningHorizontal)
            {
                offset += Write("mzlg0", Context.PointToByteArrayPacked(lightning.Position), offset);
            }
            //end horizontal with 0xff
            offset += Write("mzlg0", (byte)0xff, offset);
            foreach (LightningV lightning in lightningVertical)
            {
                offset += Write("mzlg0", Context.PointToByteArrayPacked(lightning.Position), offset);
            }
            //end all with 0x00
            Write("mzlg0", (byte)0, offset);

            //build arrows now
            offset = 0;
            foreach (Arrow arrow in arrows)
            {
                offset += Write("mzar0", Context.PointToByteArrayPacked(arrow.Position), offset);
                offset += Write("mzar0", (byte)arrow.ArrowDirection, offset);
            }
            Write("mzar0", (byte)0, offset);

            //maze walls
            //static first
            offset = 0;
            int wallDataOffset = 18; //this is a set of blank data offsets defined in the mhavoc source for some reason
            foreach (MazeWall wall in staticWalls)
            {
                offset += Write("mzta0", (byte)(wallDataOffset + (maze.PointToStamp(wall.Position))), offset);
                offset += Write("mzta0", (byte)wall.WallType, offset);
            }
            Write("mzta0", (byte)0, offset);

            //then dynamic
            offset = 0;
            foreach (MazeWall wall in dynamicWalls)
            {
                offset += Write("mztd0", (byte)(wallDataOffset + (maze.PointToStamp(wall.Position))), offset);
                offset += Write("mztd0", (byte)wall.DynamicWallTimout, offset);
                offset += Write("mztd0", (byte)wall.AlternateWallTimeout, offset);
                offset += Write("mztd0", (byte)wall.WallType, offset);
                offset += Write("mztd0", (byte)wall.AlternateWallType, offset);
            }
            Write("mztd0", (byte)0, offset);

            //one way walls
            offset = 0;
            if (oneWayRights.Count > 0)
            {
                foreach (OneWay oneway in oneWayRights)
                {
                    offset += Write("mone0", Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
                }
            }
            foreach (OneWay oneway in oneWayLefts)
            {
                offset += Write("mone0", (byte)0xff, offset);
                offset += Write("mone0", Context.PointToByteArrayPacked(new Point(oneway.Position.X, oneway.Position.Y + 64)), offset);
            }
            Write("mone0", (byte)0, offset);

            //build spikes now
            offset = 0;
            foreach (Spikes spike in spikes)
            {
                offset += Write("tite0", Context.PointToByteArrayPacked(spike.Position), offset);
            }
            Write("tite0", (byte)0, offset);

            //locks and keys, for now, there has to be an even number of locks and keys
            offset = 0;
            for (int i = 0; i < locks.Count; i++)
            {
                Lock thisLock = locks[i];
                Key thisKey = keys.Where(k => k.KeyColor == thisLock.LockColor).FirstOrDefault();
                if (thisKey != null)
                {
                    offset += Write("lock0", (byte)thisLock.LockColor, offset);
                    offset += Write("lock0", Context.PointToByteArrayPacked(thisKey.Position), offset);
                    offset += Write("lock0", Context.PointToByteArrayPacked(new Point(thisLock.Position.X, thisLock.Position.Y + 64)), offset);
                }
            }
            Write("lock0", (byte)0, offset);

            //Escape pod
            if (pod != null)
            {
                Write("mpod", (byte)pod.Option, 0);
            }

            //clock & boots
            if (clock != null)
            {
                //write these on all 4 level options
                Write("mclock", Context.PointToByteArrayPacked(clock.Position), 0);
                Write("mclock", Context.PointToByteArrayPacked(clock.Position), 1);
                Write("mclock", Context.PointToByteArrayPacked(clock.Position), 2);
                Write("mclock", Context.PointToByteArrayPacked(clock.Position), 3);
            }
            if (boots != null)
            {
                //write these on all 4 level options
                Write("mboots", Context.PointToByteArrayPacked(boots.Position), 0);
                Write("mboots", Context.PointToByteArrayPacked(boots.Position), 1);
                Write("mboots", Context.PointToByteArrayPacked(boots.Position), 2);
                Write("mboots", Context.PointToByteArrayPacked(boots.Position), 3);
            }

            //transporters
            offset = 0;
            var transporterPairs = transporters.GroupBy(t => t.Color).Select(group => new { Key = group.Key, Count = group.Count() });

            foreach (var transporterPair in transporterPairs)
            {
                List<Transporter> coloredTranporterMatches = transporters.Where(t => t.Color == transporterPair.Key).ToList();
                foreach (Transporter t in coloredTranporterMatches)
                {
                    byte colorByte = (byte)(((byte)t.Color) & 0x0F);
                    if (t.Direction == OneWayDirection.Right)
                    {
                        colorByte += 0x10;
                    }
                    offset += Write("tran0", colorByte, offset);
                    offset += Write("tran0", Context.PointToByteArrayPacked(new Point(t.Position.X, t.Position.Y + 64)), offset);
                }
            }
            //write end of transports
            offset += Write("tran0", 0, offset);
            //write transportability data
            offset += Write("tran0", new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, offset);

            //Laser Cannon
            offset = 0;
            int pointer = 0;
            if (cannons.Count > 0)
            {
                Write("mcan", new byte[] { 0x02, 0x02, 0x02, 0x02 }, 0);
            }
            for (int i = 0; i < cannons.Count; i++)
            {
                Cannon cannon = cannons[i];
                pointer += Write("mcan0", (UInt16)(GetAddress("mcand").Item1 + offset), pointer);
                //cannon location first...
                offset += Write("mcand", Context.PointToByteArrayLong(Context.ConvertPixelsToVector(cannon.Position)), offset);
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
                            offset += Write("mcand", command, offset);
                        }
                        else
                        {
                            command += (byte)(0x3F & ((move.WaitFrames) >> 2));
                            offset += Write("mcand", command, offset);
                            //write velocities
                            if (move.Velocity.X >= 0)
                            {
                                offset += Write("mcand", (byte)(move.Velocity.X & 0x3F), offset);
                            }
                            else
                            {
                                offset += Write("mcand", (byte)(move.Velocity.X | 0xc0), offset);
                            }
                            if (move.Velocity.Y >= 0)
                            {
                                offset += Write("mcand", (byte)(move.Velocity.Y & 0x3F), offset);
                            }
                            else
                            {
                                offset += Write("mcand", (byte)(move.Velocity.Y | 0xc0), offset);
                            }
                        }
                    }
                    else if (movement is CannonMovementPause)
                    {
                        CannonMovementPause pause = (CannonMovementPause)movement;
                        command = 0xc0;
                        command += (byte)(pause.WaitFrames & 0x3F);
                        offset += Write("mcand", command, offset);
                    }
                    else if (movement is CannonMovementReturn)
                    {
                        CannonMovementReturn ret = (CannonMovementReturn)movement;
                        command = 0x00;
                        offset += Write("mcand", 0, offset);
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
                        offset += Write("mcand", command, offset);
                        if (position.ShotSpeed > 0)
                        {
                            //write velocity now too
                            offset += Write("mcand", position.ShotSpeed, offset);
                        }
                    }
                }
            }
            //build trips now
            offset = 0;
            int tripoffset = 0;
            foreach (TripPad trip in tripPads)
            {
                offset += Write("mztr0", Context.PointToByteArrayPacked(trip.Position), offset);
                byte[] position = Context.PointToByteArrayShort(new Point(trip.Pyroid.Position.X, trip.Pyroid.Position.Y + 64));
                if (trip.Pyroid.PyroidStyle == PyroidStyle.Single)
                {
                    position[0] |= 0x80;
                }
                Write("trtbl", position, tripoffset + 0x18);
                Write("trtbl", position, tripoffset + 0x30);
                Write("trtbl", position, tripoffset + 0x48);
                tripoffset += Write("trtbl", position, tripoffset);

                byte velocity = (byte)Math.Abs(trip.Pyroid.Velocity);
                if (trip.Pyroid.Velocity < 0)
                {
                    velocity |= 0x80;
                }

                Write("trtbl", velocity, tripoffset + 0x18);
                Write("trtbl", velocity, tripoffset + 0x30);
                Write("trtbl", velocity, tripoffset + 0x48);
                tripoffset += Write("trtbl", velocity, tripoffset);

            }
            Write("mztr0", (byte)0, offset);

            //de hand finally
            offset = 0;
            if (hand != null)
            {
                byte[] handLocation = Context.PointToByteArrayShort(hand.Position);
                offset += Write("hand0", handLocation, offset);
                byte[] reactoidLocation = Context.PointToByteArrayShort(reactor.Position);
                int xAccordians = Math.Abs(reactoidLocation[0] - handLocation[0]);
                int yAccordians = Math.Abs(handLocation[1] - reactoidLocation[1]);
                offset += Write("hand0", new byte[] { (byte)((xAccordians * 2) + 1), (byte)(yAccordians * 2), 0x3F, 0x0B, 0x1F, 0x05, 0x03 }, offset);
            }

            //write it BABY!!!!
            if (Save())
            {
                success = true;
            }


            return success;
        }
    }
}
