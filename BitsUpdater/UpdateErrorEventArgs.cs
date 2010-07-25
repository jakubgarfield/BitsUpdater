using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpBits.Base;

namespace BitsUpdater
{
    /// <summary>
    /// Event args used to notify if error occured while using BITS download.
    /// </summary>
    public sealed class UpdateErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Error Description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Error Code.
        /// </summary>
        public int Code
        {
            get;
            set;
        }

        public UpdateErrorEventArgs(BitsError error)
        {
            Description = error.Description;
            Code = error.ErrorCode;
        }      
    }
}
