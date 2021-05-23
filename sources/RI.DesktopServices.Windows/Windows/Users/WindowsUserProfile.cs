namespace RI.Framework.Windows.Users
{
    /// <summary>
    ///     Contains the user profile of a Windows user.
    /// </summary>
    public sealed class WindowsUserProfile
    {
        #region Instance Constructor/Destructor

        internal WindowsUserProfile (WindowsUser.USERPROFILE nativeUserProfile)
        {
            this.NativeUserProfile = nativeUserProfile;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the path to the default user profile.
        /// </summary>
        /// <value>
        ///     The path to the default user profile or null if not available.
        /// </value>
        public string DefaultPath
        {
            get
            {
                return this.NativeUserProfile.lpDefaultPath;
            }
        }

        /// <summary>
        ///     Gets the path to the roaming user profile.
        /// </summary>
        /// <value>
        ///     The path to the roaming user profile or null if not available.
        /// </value>
        public string ProfilePath
        {
            get
            {
                return this.NativeUserProfile.lpProfilePath;
            }
        }

        /// <summary>
        ///     Gets the name of the domain controller.
        /// </summary>
        /// <value>
        ///     The name of the domain controller or null if not used.
        /// </value>
        public string ServerName
        {
            get
            {
                return this.NativeUserProfile.lpServerName;
            }
        }

        /// <summary>
        ///     Gets the user name.
        /// </summary>
        /// <value>
        ///     The user name.
        /// </value>
        public string UserName
        {
            get
            {
                return this.NativeUserProfile.lpUserName;
            }
        }

        internal WindowsUser.USERPROFILE NativeUserProfile { get; private set; }

        #endregion
    }
}
