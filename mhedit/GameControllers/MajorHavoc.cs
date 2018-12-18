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



        public bool WriteFiles()
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




        public bool SerializeObjects(Maze maze)
        {
            bool success = false;
            
            /////////////////////////////
            // Start building ROM here //
            /////////////////////////////
            //first is the level selection
            Write("levelst", (byte)maze.MazeType, 1);
            
            //next hint text
            Write("mzh0", GetText(maze.Hint), 0);

            //write reactor
            int offset = 0;
            Reactoid reactoid = maze.MazeObjects.OfType<Reactoid>().First();
            offset += Write("mzsc0", reactoid.ToBytes(reactoid.Position), offset);
            //build pyroids and perkoids now...
            foreach (Pyroid pyroid in maze.MazeObjects.OfType<Pyroid>())
            {
                offset += Write("mzsc0", pyroid.ToBytes(), offset);
            }
            if (maze.MazeObjects.OfType<Perkoid>().Count() > 0)
            {
                offset += Write("mzsc0", (byte)0xfe, offset);
                foreach (Perkoid perkoid in maze.MazeObjects.OfType<Perkoid>())
                {
                    offset += Write("mzsc0", perkoid.ToBytes(), offset);
                }
            }
            //end tag for CORE maze objects, ALWAYS!
            offset += Write("mzsc0", (byte)0xff, offset);

            //reactor timer, we will write all 4 entries for now...
            int reactorTimerOffset = 0;
            reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);
            reactorTimerOffset += Write("outime", reactoid.ToBytes(reactoid.Timer), reactorTimerOffset);

            //do oxygens now
            offset = 0;
            foreach (Oxoid oxoid in maze.MazeObjects.OfType<Oxoid>())
            {
                offset += Write("mzdc0", oxoid.ToBytes(), offset);
            }
            Write("mzdc0", 0, offset);

            //do lightning (Force Fields)
            offset = 0;
            foreach (LightningH lightning in maze.MazeObjects.OfType<LightningH>())
            {
                offset += Write("mzlg0", lightning.ToBytes(), offset);
            }
            //end horizontal with 0xff
            offset += Write("mzlg0", (byte)0xff, offset);
            foreach (LightningV lightning in maze.MazeObjects.OfType<LightningV>())
            {
                offset += Write("mzlg0", lightning.ToBytes(), offset);
            }
            //end all with 0x00
            Write("mzlg0", (byte)0, offset);

            //build arrows now
            offset = 0;
            foreach (Arrow arrow in maze.MazeObjects.OfType<Arrow>())
            {
                offset += Write("mzar0", arrow.ToBytes(), offset);
            }
            Write("mzar0", (byte)0, offset);

            //maze walls
            //static first
            offset = 0;
            
            foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w=>!w.IsDynamicWall))
            {
                offset += Write("mzta0", wall.ToBytes(maze), offset);
            }
            Write("mzta0", (byte)0, offset);

            //then dynamic
            offset = 0;
            foreach (MazeWall wall in maze.MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
            {
                offset += Write("mztd0", wall.ToBytes(maze), offset);
            }
            Write("mztd0", (byte)0, offset);

            //one way walls
            offset = 0;
            if (maze.MazeObjects.OfType<OneWay>().Where(o=>o.Direction == OneWayDirection.Right).Count() > 0)
            {
                foreach (OneWay oneway in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
                {
                    offset += Write("mone0", oneway.ToBytes(), offset);
                }
            }
            foreach (OneWay oneway in maze.MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
            {
                offset += Write("mone0", (byte)0xff, offset);
                offset += Write("mone0", oneway.ToBytes(), offset);
            }
            Write("mone0", (byte)0, offset);

            //build spikes now
            offset = 0;
            foreach (Spikes spike in maze.MazeObjects.OfType<Spikes>())
            {
                offset += Write("tite0", spike.ToBytes(), offset);
            }
            Write("tite0", (byte)0, offset);

            //locks and keys, for now, there has to be an even number of locks and keys
            offset = 0;
            foreach (Lock lock_ in maze.MazeObjects.OfType<Lock>())
            {
                Key thisKey = maze.MazeObjects.OfType<Key>().Where(k => k.KeyColor == lock_.LockColor).FirstOrDefault();
                if (thisKey != null)
                {
                    offset += Write("lock0", (byte)lock_.LockColor, offset);
                    offset += Write("lock0", Context.PointToByteArrayPacked(thisKey.Position), offset);
                    offset += Write("lock0", Context.PointToByteArrayPacked(new Point(lock_.Position.X, lock_.Position.Y + 64)), offset);
                }
            }
            Write("lock0", (byte)0, offset);

            //Escape pod
            EscapePod pod = maze.MazeObjects.OfType<EscapePod>().FirstOrDefault();
            if (pod != null)
            {
                Write("mpod", pod.ToBytes(), 0);
            }

            //clock & boots
            Clock clock = maze.MazeObjects.OfType<Clock>().FirstOrDefault();
            if (clock != null)
            {
                //write these on all 4 level options
                Write("mclock", clock.ToBytes(), 0);
                Write("mclock", clock.ToBytes(), 1);
                Write("mclock", clock.ToBytes(), 2);
                Write("mclock", clock.ToBytes(), 3);
            }

            Boots boots = maze.MazeObjects.OfType<Boots>().FirstOrDefault();
            if (boots != null)
            {
                //write these on all 4 level options
                Write("mboots", boots.ToBytes(), 0);
                Write("mboots", boots.ToBytes(), 1);
                Write("mboots", boots.ToBytes(), 2);
                Write("mboots", boots.ToBytes(), 3);
            }

            //transporters
            offset = 0;
            var transporterGroups = maze.MazeObjects.OfType<Transporter>().GroupBy(t => t.Color).Select(group => new { Key = group.Key, Count = group.Count() });

            foreach (var transporterPair in transporterGroups)
            {
                List<Transporter> coloredTranporterMatches = maze.MazeObjects.OfType<Transporter>().Where(t => t.Color == transporterPair.Key).ToList();
                foreach (Transporter t in coloredTranporterMatches)
                {
                    offset += Write("tran0", t.ToBytes(), offset);
                }
            }
            //write end of transports
            offset += Write("tran0", 0, offset);
            //write transportability data
            offset += Write("tran0", new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, offset);

            //Laser Cannon
            offset = 0;
            int pointer = 0;
            if (maze.MazeObjects.OfType<Cannon>().Count() > 0)
            {
                Write("mcan", new byte[] { 0x02, 0x02, 0x02, 0x02 }, 0);
            }
            foreach (Cannon cannon in maze.MazeObjects.OfType<Cannon>())
            {
                pointer += Write("mcan0", (UInt16)(GetAddress("mcand").Item1 + offset), pointer);
                //cannon location first...
                offset += Write("mcan0", cannon.ToBytes(), offset);
            }
            //build trips now
            offset = 0;
            int tripoffset = 0;
            foreach (TripPad trip in maze.MazeObjects.OfType<TripPad>())
            {
                offset += Write("mztr0", trip.ToBytes(), offset);
                //trip pad pyroid
                Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x18);
                Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x30);
                Write("trtbl", trip.Pyroid.ToBytes(), tripoffset + 0x48);
                tripoffset += Write("trtbl", trip.Pyroid.ToBytes(), tripoffset);
            }
            Write("mztr0", (byte)0, offset);

            //de hand finally
            offset = 0;
            if (maze.MazeObjects.OfType<Hand>().First() != null)
            {
                offset += Write("hand0", maze.MazeObjects.OfType<Hand>().First().ToBytes(), offset);
            }

            success = true;
            return success;
        }
    }
}
