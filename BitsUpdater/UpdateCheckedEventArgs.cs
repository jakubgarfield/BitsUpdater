using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitsUpdater
{
    /// <summary>
    /// Event args used by CheckUpdate method in BitsUpdater.
    /// </summary>
    public sealed class UpdateCheckedEventArgs : EventArgs
    {
        /// <summary>
        /// True if new updates are available, false in there are no new updates.
        /// </summary>
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
