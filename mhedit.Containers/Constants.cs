using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    public static class Constants
    {
        private static XmlQualifiedName xmlName = new XmlQualifiedName("MHEdit", "http://mhedit.askey.org");
        public static XmlSerializerNamespaces XmlNamespace = new XmlSerializerNamespaces(new XmlQualifiedName[] { xmlName });
    }
}
