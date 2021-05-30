using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

using RI.DesktopServices.Windows.Interop;
using RI.DesktopServices.Windows.Runtime;




namespace RI.DesktopServices.Windows.Users
{
    /// <summary>
    ///     Provides utilities for working with Windows user contexts.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsUser
    {
        #region Constants

        private const int Logon32LogonInteractive = 2;

        private const int Logon32ProviderDefault = 0;

        #endregion




        #region Static Methods

        /// <summary>
        ///     Closes a logon token.
        /// </summary>
        /// <param name="token"> The logon token. </param>
        public static void CloseLogonToken (IntPtr token)
        {
            WindowsUser.CloseHandle(token);
        }

        /// <summary>
        ///     Performs a user logon and creates a logon token for that user.
        /// </summary>
        /// <param name="domain"> The domain (null or an empty string to use the local machine). </param>
        /// <param name="user"> The user name (null or an empty string to use the current user name). </param>
        /// <param name="password"> The optional password (null or empty string if not used). </param>
        /// <param name="token"> The created logon token. </param>
        /// <remarks>
        ///     <para>
        ///         Before being used for the logon, <paramref name="domain" /> and <paramref name="user" /> are processed by
        ///         <see cref="ResolveDomain" /> and <see cref="ResolveUser" /> respectively.
        ///     </para>
        /// </remarks>
        /// <exception cref="Win32Exception"> The current user does not have sufficient permissions or the logon failed. </exception>
        public static void CreateLogonToken (string domain, string user, string password, out IntPtr token)
        {
            domain ??= string.Empty;
            user ??= string.Empty;
            password = string.IsNullOrEmpty(password) ? null : password;

            domain = WindowsUser.ResolveDomain(domain);
            user = WindowsUser.ResolveUser(user);

            token = IntPtr.Zero;

            bool returnValue = WindowsUser.LogonUser(user, domain, password, WindowsUser.Logon32LogonInteractive,
                                                     WindowsUser.Logon32ProviderDefault, ref token);

            if ((!returnValue) || (token == IntPtr.Zero))
            {
                int errorCode = WindowsApi.GetLastErrorCode();
                string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                throw new Win32Exception(errorCode, errorMessage);
            }
        }

        /// <summary>
        ///     Extracts domain and user name from logon information.
        /// </summary>
        /// <param name="logon"> The logon information (either user or domain\user). </param>
        /// <param name="resolve"> Specifies whether the extracted domain and user should be resolved. </param>
        /// <param name="domain"> The extracted domain. Null if no domain information is available. </param>
        /// <param name="user"> The extracted user name. Null if no user information is available. </param>
        /// <remarks>
        ///     <para>
        ///         If <paramref name="resolve" /> is true, <paramref name="domain" /> and <paramref name="user" /> are first
        ///         processed by <see cref="ResolveDomain" /> and <see cref="ResolveUser" /> respectively before being returned.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="logon" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="logon" /> is an empty string. </exception>
        public static void ExtractDomainAndUser (string logon, bool resolve, out string domain, out string user)
        {
            if (logon == null)
            {
                throw new ArgumentNullException(nameof(logon));
            }

            if (string.IsNullOrWhiteSpace(logon))
            {
                throw new ArgumentException("The string is empty.", nameof(logon));
            }

            int index = logon.IndexOf('\\');

            if (index == -1)
            {
                domain = null;
                user = logon;
            }
            else
            {
                if (index == 0)
                {
                    domain = null;
                }
                else
                {
                    domain = logon.Substring(0, index);
                }

                if (index >= (logon.Length - 1))
                {
                    user = null;
                }
                else
                {
                    user = logon.Substring(index + 1);
                }
            }

            user ??= string.Empty;
            domain ??= string.Empty;

            if (resolve)
            {
                user = WindowsUser.ResolveUser(user);
                domain = WindowsUser.ResolveDomain(domain);
            }
        }

        /// <summary>
        ///     Gets the current users default culture.
        /// </summary>
        /// <returns>
        ///     The culture of the current user.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="CultureInfo" /> specifies the formatting culture of the current user.
        ///         It is not necessarily the same as for example <see cref="CultureInfo.CurrentCulture" /> as this might have been
        ///         modified by your or some other code.
        ///         <see cref="GetCurrentDefaultCulture" /> retrieves the default culture of the current user.
        ///     </para>
        /// </remarks>
        public static CultureInfo GetCurrentDefaultCulture ()
        {
            return new CultureInfo(WindowsUser.GetUserDefaultUILanguage(), true);
        }

        /// <summary>
        ///     Gets the domain of the current user.
        /// </summary>
        /// <returns>
        ///     The domain of the current user.
        /// </returns>
        public static string GetCurrentDomain ()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent(false))
            {
                WindowsUser.ExtractDomainAndUser(identity.Name, true, out string domain, out _);
                return domain;
            }
        }

        /// <summary>
        ///     Gets the logon token of the current user.
        /// </summary>
        /// <returns>
        ///     The logon token of the current user.
        /// </returns>
        public static IntPtr GetCurrentToken ()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent(false))
            {
                return identity.Token;
            }
        }

        /// <summary>
        ///     Gets the user name of the current user.
        /// </summary>
        /// <returns>
        ///     The user name of the current user.
        /// </returns>
        public static string GetCurrentUser ()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent(false))
            {
                WindowsUser.ExtractDomainAndUser(identity.Name, true, out _, out string username);
                return username;
            }
        }

        /// <summary>
        ///     Gets the localized username for the "Everyone" user.
        /// </summary>
        /// <returns>
        ///     The localized username for the "Everyone" user.
        /// </returns>
        public static string GetEveryoneUserName ()
        {
            WindowsUser.GetUserFromSid(new SecurityIdentifier(WellKnownSidType.WorldSid, null), out _, out string user);
            return user;
        }

        /// <summary>
        ///     Gets the local domain or the local computer name respectively.
        /// </summary>
        /// <returns>
        ///     The local computer name.
        /// </returns>
        public static string GetLocalDomain ()
        {
            return Environment.MachineName;
        }

        /// <summary>
        ///     Gets the logon domain, or the domain the local computer has joined respectively.
        /// </summary>
        /// <returns>
        ///     The domain the local computer has joined.
        /// </returns>
        public static string GetNetworkDomain ()
        {
            using (ManagementObject mgmtObj =
                new ManagementObject("Win32_ComputerSystem.Name='" + WindowsUser.GetLocalDomain() + "'"))
            {
                mgmtObj.Get();

                string networkDomain = mgmtObj["domain"]
                    .ToString();

                networkDomain = WindowsUser.ResolveDomain(networkDomain ?? string.Empty);

                return networkDomain;
            }
        }

        /// <summary>
        ///     Resolves the domain and username of a user as specified by a SID.
        /// </summary>
        /// <param name="sid"> The SID to resolve. </param>
        /// <param name="domain"> The domain the resolved user belongs to. </param>
        /// <param name="user"> The username of the resolved user. </param>
        /// <returns>
        ///     true if the SID was successfully resolved, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <value>
        ///         For well-known-users/SIDs, the resolved username depends on the system language.
        ///     </value>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="sid" /> is null. </exception>
        /// <exception cref="Win32Exception"> The resolve failed. </exception>
        public static bool GetUserFromSid (SecurityIdentifier sid, out string domain, out string user)
        {
            if (sid == null)
            {
                throw new ArgumentNullException(nameof(sid));
            }

            domain = null;
            user = null;

            byte[] sidBytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidBytes, 0);

            uint capacity = 1024;

            StringBuilder domainBuilder = new StringBuilder((int)capacity);
            StringBuilder nameBuilder = new StringBuilder((int)capacity);

            bool success =
                WindowsUser.LookupAccountSid(null, sidBytes, nameBuilder, ref capacity, domainBuilder, ref capacity,
                                             out _);

            if (!success)
            {
                int errorCode = WindowsApi.GetLastErrorCode();

                if (errorCode != (int)WindowsError.ErrorNoneMapped)
                {
                    string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                    throw new Win32Exception(errorCode, errorMessage);
                }

                return false;
            }

            domain = domainBuilder.ToString();
            user = nameBuilder.ToString();

            return true;
        }

        /// <summary>
        ///     Determines whether the current user has administrator privileges.
        /// </summary>
        /// <returns>
        ///     true if the current user has administrator privileges, false otherwise.
        /// </returns>
        public static bool IsCurrentAdministrator ()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent(false))
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        ///     Determines whether the current user is a system user.
        /// </summary>
        /// <returns>
        ///     true if the current user is a system user.
        /// </returns>
        public static bool IsCurrentSystem ()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent(false))
            {
                return identity.IsSystem;
            }
        }

        /// <summary>
        ///     Resolves a domain.
        /// </summary>
        /// <param name="domain"> The domain to resolve. </param>
        /// <returns>
        ///     Either the domain name specified by <paramref name="domain" /> or the local domain (<see cref="GetLocalDomain" />)
        ///     if <paramref name="domain" /> is an empty string or a &quot;.&quot;.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="domain" /> is null. </exception>
        public static string ResolveDomain (string domain)
        {
            if (domain == null)
            {
                throw new ArgumentNullException(nameof(domain));
            }

            if (string.IsNullOrWhiteSpace(domain) ||
                string.Equals(domain.Trim(), ".", StringComparison.InvariantCultureIgnoreCase))
            {
                return WindowsUser.GetLocalDomain();
            }

            return domain;
        }

        /// <summary>
        ///     Resolves a user.
        /// </summary>
        /// <param name="user"> The user to resolve. </param>
        /// <returns>
        ///     Either the user name specified by <paramref name="user" /> or the current user (<see cref="GetCurrentUser" />) if
        ///     <paramref name="user" /> is an empty string or a &quot;.&quot;.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="user" /> is null. </exception>
        public static string ResolveUser (string user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user) ||
                string.Equals(user.Trim(), ".", StringComparison.InvariantCultureIgnoreCase))
            {
                //TODO #24: Do not use CurrentUser because it would call ResolveUser again
                return WindowsUser.GetCurrentUser();
            }

            return user;
        }

        /// <summary>
        ///     Performs a user impersonation based on a logon token.
        /// </summary>
        /// <param name="token"> The logon token. </param>
        /// <param name="loadUserProfile"> Specifies whether the users profile is to be loaded. </param>
        /// <param name="action"> The code to run under the impersonated user. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="token" /> is zero. </exception>
        /// <exception cref="SecurityException"> The current user does not have sufficient permissions. </exception>
        /// <exception cref="Win32Exception">
        ///     The current user does not have sufficient permissions or the impersonation could not
        ///     be completed.
        /// </exception>
        public static void RunImpersonated (IntPtr token, bool loadUserProfile,
                                            ImpersonatedAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (token == IntPtr.Zero)
            {
                throw new ArgumentException("The token is zero.", nameof(token));
            }

            WindowsUser.RunImpersonated<object>(token, loadUserProfile, (ptr, identity, profile) =>
            {
                action(ptr, identity, profile);
                return null;
            });
        }

        /// <summary>
        ///     Performs a user impersonation based on a logon token.
        /// </summary>
        /// <param name="token"> The logon token. </param>
        /// <param name="loadUserProfile"> Specifies whether the users profile is to be loaded. </param>
        /// <param name="func"> The code to run under the impersonated user. </param>
        /// <returns>
        ///     The return value from <paramref name="func" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="token" /> is zero. </exception>
        /// <exception cref="SecurityException"> The current user does not have sufficient permissions. </exception>
        /// <exception cref="Win32Exception">
        ///     The current user does not have sufficient permissions or the impersonation could not
        ///     be completed.
        /// </exception>
        public static TResult RunImpersonated <TResult> (IntPtr token, bool loadUserProfile,
                                                         ImpersonatedFunc<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (token == IntPtr.Zero)
            {
                throw new ArgumentException("The token is zero.", nameof(token));
            }

            TResult result = WindowsIdentity.RunImpersonated(new SafeAccessTokenHandle(token), () =>
            {
                WindowsIdentity identity = null;
                WindowsUserProfile profile = null;

                try
                {
                    identity = new WindowsIdentity(token);

                    if (loadUserProfile)
                    {
                        USERPROFILE profileInfo = new USERPROFILE();
                        profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                        profileInfo.lpUserName = identity.Name;
                        profileInfo.dwFlags = 1;

                        bool loadSuccess = WindowsUser.LoadUserProfile(token, ref profileInfo);

                        if (!loadSuccess)
                        {
                            int errorCode = WindowsApi.GetLastErrorCode();
                            string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                            throw new Win32Exception(errorCode, errorMessage);
                        }

                        profile = new WindowsUserProfile(profileInfo);
                    }

                    return func(token, identity, profile);
                }
                finally
                {
                    if (profile != null)
                    {
                        WindowsUser.UnloadUserProfile(token, profile.NativeUserProfile.hProfile);
                    }

                    if (identity != null)
                    {
                        identity.Dispose();
                    }
                }
            });

            return result;
        }

        /// <summary>
        ///     Performs a user impersonation based on a logon token.
        /// </summary>
        /// <param name="token"> The logon token. </param>
        /// <param name="loadUserProfile"> Specifies whether the users profile is to be loaded. </param>
        /// <param name="action"> The code to run under the impersonated user. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="token" /> is zero. </exception>
        /// <exception cref="SecurityException"> The current user does not have sufficient permissions. </exception>
        /// <exception cref="Win32Exception">
        ///     The current user does not have sufficient permissions or the impersonation could not
        ///     be completed.
        /// </exception>
        public static Task RunImpersonatedAsync (IntPtr token, bool loadUserProfile,
                                                 ImpersonatedActionAsync action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (token == IntPtr.Zero)
            {
                throw new ArgumentException("The token is zero.", nameof(token));
            }

            return WindowsUser.RunImpersonatedAsync<object>(token, loadUserProfile, async (ptr, identity, profile) =>
            {
                await action(ptr, identity, profile);
                return null;
            });
        }

        /// <summary>
        ///     Performs a user impersonation based on a logon token.
        /// </summary>
        /// <param name="token"> The logon token. </param>
        /// <param name="loadUserProfile"> Specifies whether the users profile is to be loaded. </param>
        /// <param name="func"> The code to run under the impersonated user. </param>
        /// <returns>
        ///     The return value from <paramref name="func" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="func" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="token" /> is zero. </exception>
        /// <exception cref="SecurityException"> The current user does not have sufficient permissions. </exception>
        /// <exception cref="Win32Exception">
        ///     The current user does not have sufficient permissions or the impersonation could not
        ///     be completed.
        /// </exception>
        public static Task<TResult> RunImpersonatedAsync <TResult> (IntPtr token, bool loadUserProfile,
                                                                    ImpersonatedFuncAsync<TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (token == IntPtr.Zero)
            {
                throw new ArgumentException("The token is zero.", nameof(token));
            }

            Task<TResult> result = WindowsIdentity.RunImpersonatedAsync(new SafeAccessTokenHandle(token), async () =>
            {
                WindowsIdentity identity = null;
                WindowsUserProfile profile = null;

                try
                {
                    identity = new WindowsIdentity(token);

                    if (loadUserProfile)
                    {
                        USERPROFILE profileInfo = new USERPROFILE();
                        profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                        profileInfo.lpUserName = identity.Name;
                        profileInfo.dwFlags = 1;

                        bool loadSuccess = WindowsUser.LoadUserProfile(token, ref profileInfo);

                        if (!loadSuccess)
                        {
                            int errorCode = WindowsApi.GetLastErrorCode();
                            string errorMessage = WindowsApi.GetErrorMessage(errorCode);
                            throw new Win32Exception(errorCode, errorMessage);
                        }

                        profile = new WindowsUserProfile(profileInfo);
                    }

                    return await func(token, identity, profile);
                }
                finally
                {
                    if (profile != null)
                    {
                        WindowsUser.UnloadUserProfile(token, profile.NativeUserProfile.hProfile);
                    }

                    if (identity != null)
                    {
                        identity.Dispose();
                    }
                }
            });

            return result;
        }

        [DllImport("kernel32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle (IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = false)]
        private static extern ushort GetUserDefaultUILanguage ();

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LoadUserProfile (IntPtr hToken, ref USERPROFILE lpProfileInfo);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LogonUser (string lpszUsername, string lpszDomain, string lpszPassword,
                                              int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupAccountSid (string lpSystemName, [MarshalAs(UnmanagedType.LPArray)] byte[] sid,
                                                     StringBuilder lpName, ref uint cchName,
                                                     StringBuilder lpReferencedDomainName,
                                                     ref uint cchReferencedDomainName, out SID_NAME_USE peUse);

        [DllImport("userenv.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnloadUserProfile (IntPtr hToken, IntPtr hProfileInfo);

        #endregion




        #region Type: SID_NAME_USE

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal enum SID_NAME_USE
        {
            SidTypeUser = 1,

            SidTypeGroup,

            SidTypeDomain,

            SidTypeAlias,

            SidTypeWellKnownGroup,

            SidTypeDeletedAccount,

            SidTypeInvalid,

            SidTypeUnknown,

            SidTypeComputer,
        }

        #endregion




        #region Type: USERPROFILE

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal struct USERPROFILE
        {
            public int dwSize;

            public int dwFlags;

            public string lpUserName;

            public string lpProfilePath;

            public string lpDefaultPath;

            public string lpServerName;

            public string lpPolicyPath;

            public IntPtr hProfile;
        }

        #endregion
    }
}
