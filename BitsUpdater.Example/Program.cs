using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using BitsUpdater.Extensions;

namespace BitsUpdater.Example
{
    class Program
    {
        public const String PublicToken = "e9aa88526a6ef0e2";

        static void Main(string[] args)
        {
            var assemblyDirectory = Assembly.GetExecutingAssembly().GetDirectory();
            Console.WriteLine("Starting BitsUpdater.Example version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString()  +" from location " + assemblyDirectory);
            Console.WriteLine("Press key to continue...");
            Console.ReadKey();

            BitsUpdater updater = new BitsUpdater("http://chodounsky.net/projects/BitsUpdater.Example/UpdateManifest.xml", Path.GetDirectoryName(assemblyDirectory));
            RegisterUpdaterEvents(updater);
            updater.ResumePreviousDownload();

            updater.CheckUpdateAsync();

            Console.ReadKey();
        }

        private static void RegisterUpdaterEvents(BitsUpdater updater)
        {
            updater.UpdateDownloaded += (s, e) =>
                {
                    Console.WriteLine("Update package is downloaded. Starting to apply update.");
                    updater.Update(PublicToken);
                };

            updater.UpdateDownloadError += (s, e) =>
                {
                    Console.WriteLine("Download error(" + e.Code + ") occured: " + e.Description);
                };

            updater.UpdateDownloadProgressChanged += (s, e) =>
                {
                    Console.WriteLine("Downloaded " + e.BytesTranferred/1024 + "/" + e.BytesTotal/1024);
                };

            updater.UpdateChecked += (s, e) =>
                {
                    if (e.UpdatesAvailable)
                    {
                        Console.WriteLine("New updates are ready to download!");
                        updater.Download();
                    }
                    else
                    {
                        Console.WriteLine("There are no new updates.");
                    }
                };
        }
    }
}
