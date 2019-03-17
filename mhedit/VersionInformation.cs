using System;
using System.IO;
using mhedit.GameControllers;

namespace mhedit
{
    public static class VersionInformation
    {
        public static readonly Version RomVersion;

        static VersionInformation()
        {
            string fullTemplatePath = Path.GetFullPath( Properties.Settings.Default.TemplatesLocation );

            RomVersion = new MajorHavocPromisedEnd( fullTemplatePath ).GetROMVersion();
        }
    }
}
