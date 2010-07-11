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

        public static bool CompareTo(this Stream input, Stream other)
        {
            var bufferSize = 1048576;
            return CompareTo(input, other, bufferSize);
        }

        public static bool CompareTo(this Stream input, Stream other, int bufferSize)
        {
            var buffer1 = new byte[bufferSize];
            var buffer2 = new byte[bufferSize];

            while (true)
            {
                int count1 = input.Read(buffer1, 0, bufferSize);
                int count2 = other.Read(buffer2, 0, bufferSize);

                if (count1 != count2)
                {
                    return false;
                }

                if (count1 == 0)
                {
                    return true;
                }

                int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
                for (int i = 0; i < iterations; i++)
                {
                    if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
                    {
                        return false;
                    }
                }
            }
        }
    }
}
