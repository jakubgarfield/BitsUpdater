using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitsUpdater.Behavior
{
    /// <summary>
    /// Interface used for different behaviors, which can be passed and executed by BitsUpdater method Update
    /// </summary>
    public interface IUpdateBehavior
    {
        void Execute();
    }
}
