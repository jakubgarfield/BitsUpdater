using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpBits.Base;

namespace BitsUpdater
{
    public sealed class UpdateErrorEventArgs : EventArgs
    {
        public string Description
        {
            get;
            set;
        }

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
