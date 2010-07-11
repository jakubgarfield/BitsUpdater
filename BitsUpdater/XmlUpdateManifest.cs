using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BitsUpdater
{
    [XmlRootAttribute("UpdateManifest", Namespace = "", IsNullable = false)]
    public sealed class XmlUpdateManifest
    {
        [XmlElementAttribute("Version")]
        public string Version;
        [XmlElementAttribute("Url")]
        public String Url;
    }
}
