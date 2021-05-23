using System;




namespace RI.Framework.Utilities.Windows
{
    [Serializable]
    public enum WindowsMessageLoopShutdownMode
    {
        None = 0,

        DiscardPending = 1,

        FinishPending = 2,
    }
}
