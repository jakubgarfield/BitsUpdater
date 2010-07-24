using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using BitsUpdater.Extensions;

namespace BitsUpdater
{
    internal sealed class AssemblyProxy : MarshalByRefObject
    {
        public void ExtractUpdate(Version version, string outputDirectory, string publicToken)
        {
            Assembly update = Assembly.ReflectionOnlyLoad(String.Format("{0}, Version={1}, Culture=neutral, PublicKeyToken={2}", string.Format(UpdatePackage.AssemblyName, version), version, publicToken));

            foreach (string name in update.GetManifestResourceNames())
            {
                using (var fs = new FileStream(Path.Combine(outputDirectory, name), FileMode.Create))
                {
                    update.GetManifestResourceStream(name).CopyTo(fs);
                }
            }
        }
    }
}
