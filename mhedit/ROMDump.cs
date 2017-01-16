using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{


    public enum ROMAddress
    {
        levelst,
        mzsc0,
        mzdc0, 
        mzlg0,
        mzar0,
        mzta0,
        mztd0,
        mone0,
        tite0,
        lock0, 
        outime,
        mpod,
        cksumal,
        cksumah,
        cksump01,
        mclock,
        tran0,
        mzh0,
        mcan,
        mcan0,
        mcand,
        mztr0,
        trtbl,
        hand0
    }

    public enum ROMPhysical
    {
        AlphaHigh,
        AlphaLow,
        Page0,
        Page1,
        Page2,
        Page3
    }


    public class ROMDump
    {
        private string _templatePath = String.Empty;
        private string _mamePath = String.Empty;
        private byte[] _alphaHigh = new byte[0x4000];
        private byte[] _alphaLow = new byte[0x4000];
        private byte[] _page01 = new byte[0x4000];
        private string _alphaHighFileNameMame = String.Empty;
        private string _alphaLowFileNameMame = String.Empty;
        private string _page01FileNameMame = String.Empty;
        private string[] _exports;

        private string _alphaHighROM = "136025.217";
        private string _alphaLowROM = "136025.216";
        private string _page01ROM = "136025.215";
        private string _validText = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ..!-,%:";

        public ROMDump(string templatePath, string mamePath, string exportPath)
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

            _alphaHighFileNameMame = _mamePath + _alphaHighROM;
            _alphaLowFileNameMame = _mamePath + _alphaLowROM;
            _page01FileNameMame = _mamePath + _page01ROM;


            FileStream alphaHighStream = null;
            try
            {
                alphaHighStream = File.Open(alphaHighFileName, FileMode.Open);
                if (alphaHighStream != null)
                {
                    alphaHighStream.Read(_alphaHigh, 0, 0x4000);
                }
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Alpha High: " + Exception.Message);
            }
            finally
            {
                alphaHighStream.Close();
                alphaHighStream.Dispose();
            }

            FileStream alphaLowStream = null;
            try
            {
                alphaLowStream = File.Open(alphaLowFileName, FileMode.Open);
                if (alphaLowStream != null)
                {
                    alphaLowStream.Read(_alphaLow, 0, 0x4000);
                }
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Alpha Low: " + Exception.Message);
            }
            finally
            {
                alphaLowStream.Close();
                alphaLowStream.Dispose();
            }

            FileStream page01Stream = null;
            try
            {
                page01Stream = File.Open(page01FileName, FileMode.Open);
                if (page01Stream != null)
                {
                    page01Stream.Read(_page01, 0, 0x4000);
                }
            }
            catch (Exception Exception)
            {
                throw new Exception("ROM Load Error - Page0/1: " + Exception.Message);
            }
            finally
            {
                page01Stream.Close();
                page01Stream.Dispose();
            }

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


        public ushort GetAddress(ROMAddress location)
        {
            ushort address = 0;
            //search the export list for this address...

            bool found = false;
            foreach (string line in _exports)
            {
                //this is an MHEDIT export
                string[] split = line.Split(new String[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 3)
                {
                    if (split[0].ToLower() == location.ToString().ToLower())
                    {
                        address = ushort.Parse(split[2].Replace("$",""),System.Globalization.NumberStyles.HexNumber);
                        found = true;
                    }
                }
            }

            if (!found)
            {
                throw new Exception("Address not found: " + location.ToString());
            }

            return address;
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
                    bytes[i] = _alphaHigh[address + i +offset];
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
            return (ushort) (((ushort)wordH << 8 ) + (ushort)bytes[0]);
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


        public int Write(ROMAddress location, byte data, int offset)
        {
            ushort address = GetAddress(location);
            return WriteROM(address, new byte[] { data }, offset);
        }

        public int Write(ROMAddress location, UInt16 data, int offset)
        {
            ushort address = GetAddress(location);
            byte datahigh = (byte)(data>>8);
            byte datalow = (byte)(data & 0xff);
            return WriteROM(address, new byte[] { datalow, datahigh }, offset);
        }

        public int Write(ROMAddress location, byte[] data, int offset)
        {
            ushort address = GetAddress(location);
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


            //save each
            File.WriteAllBytes(_alphaHighFileNameMame, _alphaHigh);
            File.WriteAllBytes(_alphaLowFileNameMame, _alphaLow);
            File.WriteAllBytes(_page01FileNameMame, _page01);

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

        public int HexToDecimal(int value)
        {
            return Convert.ToInt16(value.ToString(), 16);
        }

        public int DecimalToHex(int value)
        {
            return Convert.ToInt16(value.ToString(), 10);
        }
    }
}
