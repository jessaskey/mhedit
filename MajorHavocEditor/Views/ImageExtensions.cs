using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public static class ImageExtensions
    {
        private const string ImageList = nameof( ImageList );
        private const string FileNames = nameof( FileNames );
        private const string AssemblyName = nameof( AssemblyName );
        private const string ResourcePath = nameof( ResourcePath );

        public static IDictionary<string, object> AddImages( this ImageList il,
            IEnumerable<string> fileNames )
        {
            IDictionary<string, object> cfg = new Dictionary<string, object>();

            cfg[ ImageList ] = il;
            cfg[ FileNames ] = fileNames;

            return cfg;
        }

        public static IDictionary<string, object> InAssembly(
            this IDictionary<string, object> config, Assembly assembly )
        {
            IDictionary<string, object> cfg = config ?? new Dictionary<string, object>();

            cfg[ AssemblyName ] = assembly;

            return cfg;
        }

        public static IDictionary<string, object> WithResourcePath( this IDictionary<string, object> config,
            string path )
        {
            IDictionary<string, object> cfg = config ?? new Dictionary<string, object>();

            cfg[ ResourcePath ] = path;

            return cfg;
        }

        public static ImageList Load( this IDictionary<string, object> config )
        {
            Assembly assembly = config.ContainsKey( AssemblyName ) ?
                                    (Assembly) config[ AssemblyName ] :
                                    null;

            string path = config.ContainsKey( ResourcePath ) ?
                              (string) config[ ResourcePath ] :
                              string.Empty;

            ImageList imageList = (ImageList) config[ ImageList ];

            foreach ( string filename in (IEnumerable<string>)config[FileNames])
            {
                imageList.Images.Add(
                    ResourceLoader.GetEmbeddedImage(
                        Path.Combine( path, filename ).CreateResourceUri( assembly ) ) );
            }

            return imageList;
        }
    }

    public static class UriExtensions
    {
        public static Uri CreateResourceUri( this string pathToResource, Assembly assembly = null )
        {
            string assemblyName = (assembly ?? Assembly.GetCallingAssembly()).GetName().Name;

            // Resource URIs want /
            string resourcePath = pathToResource.Replace( '\\', '/' ).TrimStart('/');

            return PackUriHelper.Create( ResourceLoader.ApplicationUri,
                new Uri( $"/{assemblyName};component/{resourcePath}",
                    UriKind.Relative ) );
        }
    }
}
