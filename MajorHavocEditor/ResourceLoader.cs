using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MajorHavocEditor
{

    /// <summary>
    /// Utility class that can be used to find and load embedded resources into memory.
    ///
    /// TODO: Add CultureInfo to the resource searching algo that uses subfolders akin to the platforms.
    /// 
    /// </summary>
    /// <remarks>
    /// https://github.com/xamarin/mobile-samples/blob/master/EmbeddedResources/SharedLib/ResourceLoader.cs
    /// </remarks>
    public static class ResourceLoader
    {
        //BUG? Not sure that all assemblies will be loaded when lazy evaluated...
        private static readonly IList<Assembly> EditorAssemblies =
            new Lazy<IList<Assembly>>(
                () =>
                {
                    return AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .Where( a => a.GetName().Name.Contains( "mhedit" ) ||
                                                 a.GetName().Name.Contains( "MajorHavocEditor" ) )
                                    .ToList();
                } ).Value;

        public static Uri ApplicationUri = new Uri("application:///");

        public static Image GetEmbeddedImage( Uri uri )
        {
            Image bmp = null;

            using ( Stream imgStream = GetEmbeddedResourceStream( uri ) )
            {
                if (imgStream != null)
                {
                    bmp = new Bitmap(imgStream);
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
            }

            return bmp;
        }

        /// <summary>
        /// Load an embedded resource using a Pack URI.
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?view=netframeworkdesktop-4.8"/>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Stream GetEmbeddedResourceStream(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("Uri should be absolute.");
            }

            if (uri.Scheme != PackUriHelper.UriSchemePack)
            {
                throw new ArgumentException("UriShouldBePackScheme");
            }

            ///TODO: this is total crap. Clean this up.
            string assemblyName = uri.Segments[1].Split(';')[0];

            Assembly assembly =
                EditorAssemblies.FirstOrDefault( a => a.GetName().Name.Equals( assemblyName ) );

            if (assembly != null)
            {
                string resourcePath = uri.Segments
                                         .Skip(2)
                                         .Aggregate(new StringBuilder(),
                                             (sb, x) => sb.Append(x))
                                         .ToString();

                return ResourceLoader.GetEmbeddedResourceStream(assembly, resourcePath);
            }

            throw new Exception($"Resource {uri} not found.");
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static string GetEmbeddedResourceString(Uri uri)
        {
            using (var stream = ResourceLoader.GetEmbeddedResourceStream(uri))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource stream.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
        {
            /// TODO: leading / char implies that it's an absolute path. Validate this.
            /// Embedded resources should all be relative paths so no leading '/'.
            resourceFileName = resourceFileName.Replace('/', '.');

            return assembly.GetManifestResourceStream(
                resourceFileName.StartsWith(assembly.GetName().Name) ?
                    resourceFileName :
                    resourceFileName.Insert(0, $"{assembly.GetName().Name}."));
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a byte array.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static byte[] GetEmbeddedResourceBytes(Assembly assembly, string resourceFileName)
        {
            var stream = ResourceLoader.GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static string GetEmbeddedResourceString(Assembly assembly, string resourceFileName)
        {
            using (var stream =
                ResourceLoader.GetEmbeddedResourceStream(assembly, resourceFileName))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }

}