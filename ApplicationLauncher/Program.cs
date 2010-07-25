using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ApplicationLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            string versionDirectory = "Version";
            string applicationName = "BitsUpdater.Example.exe";
            var latestVersion = GetLatestVersionDirectory(versionDirectory);

            string processPath = Path.Combine(versionDirectory, Path.Combine(latestVersion.ToString(), applicationName));
            Process.Start(new ProcessStartInfo(processPath));
        }

        private static Version GetLatestVersionDirectory(string versionDirectory)
        {
            Version max = new Version();
            foreach (var item in Directory.GetDirectories(versionDirectory))
            {
                try
                {
                    var current = new Version(Path.GetFileName(item));
                    if (current > max)
                    {
                        max = current;
                    }
                }
                catch (ArgumentException) { /* Version try parse */ }
                catch (OverflowException) { /* Version try parse */ }
                catch (FormatException) { /* Version try parse */ }
            }

            return max;
        }
    }
}
