using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BitsUpdater
{
    [XmlRootAttribute("UpdateStatus", Namespace = "", IsNullable = false)]
    public sealed class XmlUpdateStatus
    {
        [XmlElementAttribute("NextVersion")]
        public String NextVersion;
        [XmlElementAttribute("CurrentBITS")]
        public String BitsJob;
    }
}
