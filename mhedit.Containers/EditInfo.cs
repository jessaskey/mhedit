using System;

namespace mhedit.Containers
{

    [ Serializable ]
    public class RomInfo
    {
        public RomInfo()
            : this( string.Empty, new Version() )
        { }

        public RomInfo( string name, Version romVersion )
        {
            this.Name = name;

            this.Version = romVersion.ToString();
        }

        public string Name;
        public string Version;
    }

    [ Serializable ]
    public class EditInfo
    {
        public EditInfo()
            : this( DateTime.Now, new Version() )
        { }

        public EditInfo( DateTime timeStamp, Version editorVersion )
        {
            this.TimeStamp = timeStamp;

            this.EditorVersion = editorVersion.ToString();
        }

        public DateTime TimeStamp;
        public string EditorVersion;
        public RomInfo Rom;

        public override string ToString()
        {
            string str = $"MHEdit {this.EditorVersion}, {this.TimeStamp:yyyy-MM-dd hh:mm:ss tt\" GMT\"zzz}";

            if ( this.Rom != null )
            {
                str += $" {this.Rom.Name} ROMs:{this.Rom.Version}";
            }

            return str;

        }
    }

}