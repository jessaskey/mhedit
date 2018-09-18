using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{
    /// <summary>
    /// Base class for the GameProfile xml elements. At present this is an
    /// easy way to add utility methods rather than a shim base class and
    /// extension methods.
    /// </summary>
    public class ProfileElement
    {
        /// <summary>
        /// Quick way to print out this object for debug or any other
        /// need
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            XmlSerializer xsSubmit = new XmlSerializer( this.GetType() );

            using ( var stringWriter = new StringWriter() )
            {
                using ( XmlWriter xmlWriter = XmlWriter.Create( stringWriter ) )
                {
                    xsSubmit.Serialize( xmlWriter, this );

                    return stringWriter.ToString();
                }
            }
        }
    }
}
