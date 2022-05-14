using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using mhedit.GameControllers;

namespace mhedit
{

    public static class VersionInformation
    {
        private static Version _romVersion;

        static VersionInformation()
        {
            /// if someone changes the templates dir update the version.
            //Properties.Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private static void OnSettingsChanged( object sender, PropertyChangedEventArgs e )
        {
            _romVersion = null;
        }

        public static Version RomVersion
        {
            get
            {
                Version version = _romVersion;

                if ( version == null )
                {
                    //string fullTemplatePath = Path.GetFullPath( Properties.Settings.Default.TemplatesLocation );

                    //MajorHavocPromisedEnd controller = new MajorHavocPromisedEnd();
                    //if (controller.LoadTemplate(fullTemplatePath))
                    //{
                    //    _romVersion = controller.GetROMVersion();
                    //}

                    return new Version(1, 1); //_romVersion;
                }

                return version;
            }
        }
    }

}
