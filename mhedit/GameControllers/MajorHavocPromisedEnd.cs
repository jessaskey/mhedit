using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.GameControllers
{
    public class MajorHavocPromisedEnd
    {

        #region Private Variables

        private string _templatePath = String.Empty;
        private string _mamePath = String.Empty;
        private byte[] _page2367 = new byte[0x8000];
        private Dictionary<string, int> _exports6 = new Dictionary<string, int>();
        private Dictionary<string, int> _exports7 = new Dictionary<string, int>();
        private string _page2367ROM = "mhpe.1np";
        private string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:";

        #endregion


        public MajorHavocPromisedEnd(string templatePath, string mamePath, string exportPath)
        {
            _templatePath = templatePath;
            _mamePath = mamePath;

            //load our exports
            if (File.Exists(exportPath + "havocpe6.exp"))
            {
                string[] exportLines = File.ReadAllLines(exportPath + "havocpe6.exp");
                foreach(string exportLine in exportLines)
                {
                    string[] def = exportLine.Split(',');
                    if (def.Length == 2)
                    {
                        int value = int.Parse(def[1].Replace("$", "0x"), System.Globalization.NumberStyles.HexNumber);
                        _exports6.Add(def[0], value);
                    }
                }
            }

            if (File.Exists(exportPath + "havocpe7.exp"))
            {
                string[] exportLines = File.ReadAllLines(exportPath + "havocpe7.exp");
                foreach (string exportLine in exportLines)
                {
                    string[] def = exportLine.Split(',');
                    if (def.Length == 2)
                    {
                        int value = int.Parse(def[1].Replace("$", "0x"), System.Globalization.NumberStyles.HexNumber);
                        _exports7.Add(def[0], value);
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

        public byte[] GetText(string text)
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
            bytes.Add(0x80);
            return bytes.ToArray();
        }


        public Tuple<ushort,int> GetAddress(string label)
        {
            Tuple<ushort, int> address = null;
            //search the export list for this address...

            if (_exports6.ContainsKey(label.ToLower()))
            {
                address = new Tuple<ushort, int>((ushort)_exports6[label],6);
            }
            else {
                if (_exports7.ContainsKey(label.ToLower()))
                {
                    address = new Tuple<ushort, int>((ushort)_exports7[label], 6);
                }
                else
                {
                    throw new Exception("Address not found: " + label.ToString());
                }
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

        private int WriteROM(ushort address, byte[] bytes, int offset, int page)
        {
            int page67Base = 0x4000;
            if (page == 7) page67Base += 0x2000;

            if (address >= 0x2000 && address <= 0x3fff)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _page2367[page67Base + address + i + offset] = bytes[i];
                }
            }
            return bytes.Length;
        }


        public int Write(string location, byte data, int offset)
        {
            Tuple<ushort,int> addressInfo = GetAddress(location);
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


    }
}
