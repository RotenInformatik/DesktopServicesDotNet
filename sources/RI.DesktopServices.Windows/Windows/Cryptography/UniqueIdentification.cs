using System;
using System.Security.Cryptography;
using System.Text;

using RI.Framework.Windows.Users;




namespace RI.Framework.Windows.Cryptography
{
    /// <summary>
    ///     Provides functionality to obtain various unique IDs.
    /// </summary>
    /// TODO: Add additional entropy option
    public static class UniqueIdentification
    {
        #region Constants

        private const string InnerGuid = "D2810CA2E2B74A1CB859644FD5BE32C2";

        #endregion




        #region Static Methods

        /// <summary>
        ///     Gets an anonymous ID identifying the current network domain.
        /// </summary>
        /// <returns>
        ///     The ID as a GUID.
        /// </returns>
        public static Guid GetDomainId ()
        {
            string cipher = WindowsUser.GetNetworkDomain() + "\\" + UniqueIdentification.InnerGuid;
            Guid guid = UniqueIdentification.CreateGuidFromString(cipher);
            return guid;
        }

        /// <summary>
        ///     Gets an anonymous ID identifying the current machine.
        /// </summary>
        /// <returns>
        ///     The ID as a GUID.
        /// </returns>
        public static Guid GetMachineId ()
        {
            string cipher = Environment.MachineName + "\\" + UniqueIdentification.InnerGuid;
            Guid guid = UniqueIdentification.CreateGuidFromString(cipher);
            return guid;
        }

        /// <summary>
        ///     Gets an anonymous ID identifying the current user.
        /// </summary>
        /// <returns>
        ///     The ID as a GUID.
        /// </returns>
        public static Guid GetUserId ()
        {
            string cipher = Environment.UserDomainName + "\\" + Environment.UserName + "\\" +
                            UniqueIdentification.InnerGuid;

            Guid guid = UniqueIdentification.CreateGuidFromString(cipher);
            return guid;
        }

        private static Guid CreateGuidFromString (string data)
        {
            byte[] guidBytes;

            using (MD5 hasher = MD5.Create())
            {
                guidBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
            }

            Guid guid = new Guid(guidBytes);
            return guid;
        }

        #endregion
    }
}
