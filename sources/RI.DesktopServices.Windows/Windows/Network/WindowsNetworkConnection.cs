using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Windows.Network
{
    /// <summary>
    ///     Implements a connection to a Windows network resource.
    /// </summary>
    public sealed class WindowsNetworkConnection : IDisposable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="WindowsNetworkConnection" />.
        /// </summary>
        /// <param name="resource"> The path of the resource (e.g. a UNC path to a file share). </param>
        /// <param name="username"> The logon name under which the connection is to be established (null for the current user). </param>
        /// <param name="password"> The password for the logon if <paramref name="username" /> is specified. </param>
        /// <param name="interactive">
        ///     Specifies whether an interactive logon can be performed if necessary (e.g. the user might be
        ///     asked to enter credentials).
        /// </param>
        /// <remarks>
        ///     <para>
        ///         See <see cref="WindowsNetwork.OpenConnection" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="resource" /> is null. </exception>
        /// <exception cref="Win32Exception">
        ///     An unknown error occurred which could not be translated to
        ///     <see cref="WindowsNetworkError" />.
        /// </exception>
        public WindowsNetworkConnection (string resource, string username, string password, bool interactive)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            this.Resource = resource;
            this.Username = username;
            this.Password = password;
            this.Interactive = interactive;

            this.Error = WindowsNetworkError.None;

            this.Connect();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the error which ocurred during establishing the connection.
        /// </summary>
        /// <value>
        ///     The error which ocurred during establishing the connection.
        /// </value>
        public WindowsNetworkError Error { get; private set; }

        /// <summary>
        ///     Gets whether an interactive logon can be performed.
        /// </summary>
        /// <value>
        ///     true if an interactive logon can be performed, false otherwise.
        /// </value>
        public bool Interactive { get; private set; }

        /// <summary>
        ///     Gets whether the connection to the resource is open.
        /// </summary>
        /// <value>
        ///     true if the connection is open, false otherwise.
        /// </value>
        public bool IsOpen { get; private set; }

        /// <summary>
        ///     Gets the password for the logon
        /// </summary>
        /// <value>
        ///     The password for the logon.
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        ///     Gets the path of the resource.
        /// </summary>
        /// <value>
        ///     The path of the resource.
        /// </value>
        public string Resource { get; private set; }

        /// <summary>
        ///     Gets the logon name under which the connection is to be established.
        /// </summary>
        /// <value>
        ///     The logon name under which the connection is to be established.
        /// </value>
        public string Username { get; private set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Closes the connection to the network resource.
        /// </summary>
        public void Close ()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Connect ()
        {
            this.IsOpen = false;
            this.Error = WindowsNetwork.OpenConnection(this.Resource, this.Username, this.Password, this.Interactive);
            this.IsOpen = true;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void Dispose (bool disposing)
        {
            this.IsOpen = false;
            WindowsNetwork.CloseConnection(this.Resource, true);
        }

        #endregion




        #region Overrides

        /// <summary>
        ///     Garbage collects this instance of <see cref="WindowsNetworkConnection" />.
        /// </summary>
        ~WindowsNetworkConnection ()
        {
            this.Dispose(false);
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose ()
        {
            this.Close();
        }

        #endregion
    }
}
