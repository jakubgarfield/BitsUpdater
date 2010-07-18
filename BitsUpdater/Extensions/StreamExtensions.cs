using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BitsUpdater.Extensions
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream input, Stream destination, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            int read = 0;
            do
            {
                read = input.Read(buffer, 0, bufferSize);
                destination.Write(buffer, 0, read);
            } while (read > 0);
        }

        public static void CopyTo(this Stream input, Stream destination)
        {
            var bufferSize = 1048576;
            CopyTo(input, destination, bufferSize);
        }

        public static bool AreEqual(this Stream input, Stream other)
        {
            int buffer = sizeof(Int64);

            if (input.Length != other.Length)
                return false;

            int iterations = (int)Math.Ceiling((double)input.Length / buffer);

            byte[] one = new byte[buffer];
            byte[] two = new byte[buffer];

            for (int i = 0; i < iterations; i++)
            {
                input.Read(one, 0, buffer);
                other.Read(two, 0, buffer);

                if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }

            return true;
        }
    }
}
