using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitsUpdater
{
    public sealed class UpdateCheckedEventArgs : EventArgs
    {
        public bool UpdatesAvailable
        {
            get;
            private set;
        }

        public UpdateCheckedEventArgs(bool updatesAvailable)
        {
            UpdatesAvailable = updatesAvailable;
        }
    }
}
