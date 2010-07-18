using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BitsUpdatePacker.Configuration
{
    internal sealed class TemplatesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("templates")]
        public TemplatesCollection Templates
        {
            get { return ((TemplatesCollection)(base["templates"])); }
        }
    }
}
