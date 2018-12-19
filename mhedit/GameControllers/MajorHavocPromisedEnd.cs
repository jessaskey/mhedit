using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeObjects;
using System;
using System.Collections.Generic;
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
        private string _mamePath = String.Empty;
        private byte[] _page2367 = new byte[0x8000];
        private Dictionary<string, int> _exports = new Dictionary<string, int>();
        private string _page2367ROM = "mhpe.1np";
        private string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:";

        private int _page6Base = 0x4000;
        private int _page7Base = 0x6000;

        private string _lastError = "";

        #endregion


        public MajorHavocPromisedEnd(string templatePath, string mamePath, string exportPath)
        {
            _templatePath = templatePath;
            _mamePath = mamePath;

            //load our exports
            if (File.Exists(exportPath + "mhavocpe.exp"))
            {
                string[] exportLines = File.ReadAllLines(exportPath + "mhavocpe.exp");
                foreach (string exportLine in exportLines)
                {
                    string[] def = exportLine.Replace(" ", "").Replace("\t","").Replace(".EQU", "|").Split('|');
                    if (def.Length == 2)
                    {
                        int value = int.Parse(def[1].Replace("$", ""), System.Globalization.NumberStyles.HexNumber);
                        _exports.Add(def[0], value);
                    }
                }
            }

            //load up our roms for now...
            try
            {
                _page2367 = File.ReadAllBytes(_templatePath + _page2367ROM);
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Page0/1: " + Exception.Message);
            }
        }

        public byte ReadByte(ushort address, int offset)
        {
            throw new Exception("Not implemented.");
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
            int page67Base = 0x4000;
            if (page == 7) page67Base += 0x2000;

            if (address >= 0x2000 && address <= 0x3fff)
            {
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = _page2367[page67Base + address + i + offset];
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
            string page67FileNameMame = _mamePath + _page2367ROM;

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
                File.Copy(_templatePath + rom, _mamePath + rom, true);
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

            if (mazeCollection.MazeCount > 24)
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
            for (int i = 0; i < 24; i++)
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
            for (int i = 0; i < 24; i++)
            {
                string message = " ";   //default to a single blank space
                if (mazeCollection.Mazes[i] != null)
                {
                    //Write Table Pointer
                    pointerIndex += WriteROM((ushort)_exports["mazehint"], WordToByteArray(index7Data), pointerIndex, 6);
                    //Maze Data
                    if (!String.IsNullOrEmpty(mazeCollection.Mazes[i].Hint))
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
            //Oxygen Discs
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mzdc"], WordToByteArray(index6Data), pointerIndex, 6);
                //Oxoid data
                foreach (Oxoid oxoid in mazeCollection.Mazes[i].MazeObjects.OfType<Oxoid>())
                {
                    index6Data += WriteROM((ushort)index6Data, oxoid.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Lightning
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mzlg"], WordToByteArray(index6Data), pointerIndex, 6);
                //Oxoid data
                foreach (LightningH lightningH in mazeCollection.Mazes[i].MazeObjects.OfType<LightningH>())
                {
                    index6Data += WriteROM((ushort)index6Data, lightningH.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0xff }, 0, 6);
                foreach (LightningV lightningV in mazeCollection.Mazes[i].MazeObjects.OfType<LightningV>())
                {
                    index6Data += WriteROM((ushort)index6Data, lightningV.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Arrows
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
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
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mzor"], WordToByteArray(index6Data), pointerIndex, 6);
                //Never Defined
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Trip Points
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
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
            //Static Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mztdal"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (MazeWall wall in mazeCollection.Mazes[i].MazeObjects.OfType<MazeWall>().Where(w=>!w.IsDynamicWall))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Dynamic Maze Walls
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mztd"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (MazeWall wall in mazeCollection.Mazes[i].MazeObjects.OfType<MazeWall>().Where(w => w.IsDynamicWall))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //One Way Walls
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mone"], WordToByteArray(index6Data), pointerIndex, 6);
                //Trip Point data
                foreach (OneWay wall in mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Right))
                {
                    index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                }
                if (mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left).Count() > 0) {
                    index6Data += WriteROM((ushort)index6Data, new byte[] { 0xff }, 0, 6);
                    foreach (OneWay wall in mazeCollection.Mazes[i].MazeObjects.OfType<OneWay>().Where(o => o.Direction == OneWayDirection.Left))
                    {
                        index6Data += WriteROM((ushort)index6Data, wall.ToBytes(mazeCollection.Mazes[i]), 0, 6);
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Spikes
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
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
            for (int i = 0; i < 24; i++)
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

            for (int i = 0; i < 24; i++)
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
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xee }, 0,6);
            }
            //De Hand
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mhand"], WordToByteArray(index6Data), pointerIndex, 6);
                //Hand Data
                foreach (Hand hand in mazeCollection.Mazes[i].MazeObjects.OfType<Hand>())
                {
                    if (reactoid != null)
                    {
                        index6Data += WriteROM((ushort)index6Data, hand.ToBytes(reactoid.Position), 0, 6);
                    }
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Clock
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mclock"], WordToByteArray(index6Data), pointerIndex, 6);
                //Clock Data
                foreach (Clock clock in mazeCollection.Mazes[i].MazeObjects.OfType<Clock>())
                {
                    index6Data += WriteROM((ushort)index6Data, clock.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }
            //Boots
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["mboots"], WordToByteArray(index6Data), pointerIndex, 6);
                //Boots Data
                foreach (Boots boots in mazeCollection.Mazes[i].MazeObjects.OfType<Boots>())
                {
                    index6Data += WriteROM((ushort)index6Data, boots.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00 }, 0, 6);
            }

            int mpodAddressBase = _exports["mpod"];
            //Escape Pod
            for (int i = 1; i < 24; i+=4)
            {
                //Pod Data
                EscapePod pod = mazeCollection.Mazes[i].MazeObjects.OfType<EscapePod>().FirstOrDefault();
                if (pod != null)
                {
                    mpodAddressBase += WriteROM((ushort)mpodAddressBase, pod.ToBytes(), 0, 6);
                }
                mpodAddressBase += WriteROM((ushort)mpodAddressBase, new byte[] { 0x00 }, 0, 6);
            }
            //Out Time
            int outAddressBase = _exports["outime"];
            for (int i = 0; i < 24; i++)
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
            for (int i = 0; i < 24; i++)
            {
                oxyAddressBase += WriteROM((ushort)oxyAddressBase, new byte[] { (byte)mazeCollection.Mazes[i].OxygenReward }, 0, 6);
            }
            //Trip Actions
            pointerIndex = 0;
            for (int i = 0; i < 24; i++)
            {
                //Write Table Pointer
                pointerIndex += WriteROM((ushort)_exports["trtbll"], WordToByteArray(index6Data), pointerIndex, 6);
                //Clock Data
                foreach (TripPad trip in mazeCollection.Mazes[i].MazeObjects.OfType<TripPad>())
                {
                    index6Data += WriteROM((ushort)index6Data, trip.Pyroid.ToBytes(), 0, 6);
                }
                index6Data += WriteROM((ushort)index6Data, new byte[] { 0x00, 0x00, 0x00 }, 0, 6);
            }

            //Ion Cannon, warning, this is very messy due to data compaction techniques
            //Three levels of data pointers
            // 1. Byte: Index into Pointers - mcan - Static Data Area - 28 Bytes
            // 2. Word: Pointers to Data - mcanst - Dynamic Data Area - This is of a calculated size
            // 3. Byte: Data Stream - Dynamic Data Area
            //
            pointerIndex = 0;
            int cannonIndexValue = 0;
            int mazesWithCannons = mazeCollection.Mazes.Where(m => m.MazeObjects.OfType<Cannon>().Count() > 0).Count();
            int totalCannons = mazeCollection.Mazes.SelectMany(m => m.MazeObjects.OfType<Cannon>()).Count();
            //allocate some memory... it needs to be equal to TotalCannons + MazesWithCannons + 1 (for empty levels)
            int allocationOffsetIndex = (totalCannons + mazesWithCannons + 1)*2; //Words

            //empty data word for levels with no Cannons, pointerIndex = 0
            int cannonPointersBase = index6Data;
            cannonPointersBase += WriteROM((ushort)cannonPointersBase, new byte[] { 0x00, 0x00 }, 0, 6);
            int cannonDataBase = cannonPointersBase + allocationOffsetIndex;

            for (int i = 0; i < 24; i++)
            {
                if (mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>().Count() == 0)
                {
                    pointerIndex += WriteROM((ushort)_exports["mcan"], new byte[] { 0x00 }, pointerIndex, 6);
                    cannonIndexValue += 2;
                }
                else
                {
                    //set this ponter index value and increment
                    pointerIndex += WriteROM((ushort)_exports["mcan"], new byte[] { (byte)cannonIndexValue }, pointerIndex, 6);
                    cannonIndexValue += 2;

                    foreach (Cannon cannon in mazeCollection.Mazes[i].MazeObjects.OfType<Cannon>())
                    {
                        cannonDataBase += WriteROM((ushort)cannonDataBase, cannon.ToBytes(), 0, 6);
                    }
                }

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
