using System;




namespace RI.Framework.Utilities.Windows
{
    [Serializable]
    public enum WindowsMessageLoopOperationState
    {
        Waiting = 0,

        Executing = 1,

        Finished = 2,

        Canceled = 3,

        Exception = 4,
    }
}
