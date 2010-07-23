using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpBits.Base;

namespace BitsUpdater
{
    public sealed class UpdateProgressEventArgs : EventArgs
    {
        public ulong BytesTranferred
        {
            get;
            private set;
        }

        public ulong BytesTotal
        {
            get;
            private set;
        }

        public UpdateProgressEventArgs(ulong bytesTransferred, ulong bytesTotal)
        {
            BytesTranferred = bytesTransferred;
            BytesTotal = bytesTotal;
        }
    }
}
