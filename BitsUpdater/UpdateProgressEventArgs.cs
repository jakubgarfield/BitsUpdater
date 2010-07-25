using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpBits.Base;

namespace BitsUpdater
{
    /// <summary>
    /// Update progress event args used in UpdateProgressChanged event in BitsUpdater while BITS is downloading.
    /// </summary>
    public sealed class UpdateProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Current bytes transferred by BITS.
        /// </summary>
        public ulong BytesTranferred
        {
            get;
            private set;
        }

        /// <summary>
        /// Total size of update.
        /// </summary>
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
