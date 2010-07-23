using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitsUpdater.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            BitsUpdater updater = new BitsUpdater("http://chodounsky.net/files/temp/UpdateManifest.xml");
            updater.Download();
            
            Console.ReadLine();
        }
    }
}
