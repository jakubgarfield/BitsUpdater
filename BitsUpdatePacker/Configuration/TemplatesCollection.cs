using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BitsUpdatePacker.Configuration
{
    [ConfigurationCollection(typeof(TemplatesElement))]
    internal sealed class TemplatesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplatesElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (((TemplatesElement)(element)).Directory + ((TemplatesElement)(element)).Pattern).GetHashCode();
        }

        public TemplatesElement this[int idx]
        {
            get
            {
                return (TemplatesElement)BaseGet(idx);
            }
        }
    }
}
