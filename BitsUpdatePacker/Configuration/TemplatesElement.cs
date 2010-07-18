using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace BitsUpdatePacker.Configuration
{
    internal sealed class TemplatesElement : ConfigurationElement
    {
        [ConfigurationProperty("id", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Id
        {
            get
            {
                return ((string)(base["id"]));
            }
            set
            {
                base["id"] = value;
            }
        }

        [ConfigurationProperty("directory", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Directory
        {
            get
            {
                return ((string)(base["directory"]));
            }
            set
            {
                base["directory"] = value;
            }
        }

        [ConfigurationProperty("pattern", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Pattern
        {
            get
            {
                return ((string)(base["pattern"]));
            }
            set
            {
                base["pattern"] = value;
            }
        }

        [ConfigurationProperty("searchOption", DefaultValue = "TopDirectoryOnly", IsKey = false, IsRequired = false)]
        public SearchOption SearchOption
        {
            get
            {
                return (SearchOption)base["searchOption"];
            }
            set
            {
                base["searchOption"] = value;
            }
        }
    }
}
