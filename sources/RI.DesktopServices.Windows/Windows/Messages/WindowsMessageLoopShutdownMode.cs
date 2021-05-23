using System;




namespace RI.DesktopServices.Windows.Messages
{
    [Serializable]
    public enum WindowsMessageLoopShutdownMode
    {
        None = 0,

        DiscardPending = 1,

        FinishPending = 2,
    }
}
