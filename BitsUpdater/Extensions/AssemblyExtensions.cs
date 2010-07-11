using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace BitsUpdater.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetDirectory(this Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }
    }
}
