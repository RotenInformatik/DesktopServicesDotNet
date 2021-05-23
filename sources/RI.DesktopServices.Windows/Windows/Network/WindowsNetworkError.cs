using System;




namespace RI.Framework.Windows.Network
{
    /// <summary>
    ///     Describes the result of a network resource operation performed by <see cref="WindowsNetwork" />.
    /// </summary>
    [Serializable]
    public enum WindowsNetworkError
    {
        /// <summary>
        ///     The operation succeeded.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Invalid parameters were provided.
        /// </summary>
        InvalidParameters = 1,

        /// <summary>
        ///     The specified password is invalid.
        /// </summary>
        InvalidPassword = 2,

        /// <summary>
        ///     The operation was canceled by the user.
        /// </summary>
        CanceledByUser = 3,

        /// <summary>
        ///     The remote resource is available but currently busy.
        /// </summary>
        Busy = 4,

        /// <summary>
        ///     The remote resource or the network is currently not available.
        /// </summary>
        Unavailable = 5,
    }
}
