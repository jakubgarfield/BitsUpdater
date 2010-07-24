using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitsUpdater.Behavior;

namespace BitsUpdater.Example
{
    class Program
    {
        public const String PublicToken = "ad4a61e338c93fde";

        static void Main(string[] args)
        {
            BitsUpdater updater = new BitsUpdater("http://chodounsky.net/files/temp/UpdateManifest.xml");
            RegisterUpdaterEvents(updater);
            updater.ResumePreviousDownload();

            updater.CheckUpdateAsync();

            Console.ReadLine();
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
