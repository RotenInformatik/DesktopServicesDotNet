using System;
using System.Security.Cryptography;
using System.Text;




namespace RI.DesktopServices.Windows.Cryptography
{
    /// <summary>
    ///     Provides transparent encryption functionality with a machine or user local scope.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="LocalEncryption" /> allows you to encrypt strings so that they can only be decrypted on the machine
    ///         and/or by the user which encrypted them.
    ///         The used encryption key is managed by the operating system.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public static class LocalEncryption
    {
        #region Static Fields

        private static readonly Encoding StringEncoding = Encoding.UTF8;

        #endregion




        #region Static Methods

        /// <summary>
        ///     Decrypts a string.
        /// </summary>
        /// <param name="userScope">
        ///     true if the string should only be decryptable by the current user, false if the string should
        ///     only be decryptable by any user on the local machine.
        /// </param>
        /// <param name="cipherText"> The string to decrypt (binary data in BASE64 format). </param>
        /// <param name="additionalEntropy"> Any used additional entropy or salt (can be null). </param>
        /// <returns>
        ///     The decrypted plain text string or null if <paramref name="cipherText" /> is null.
        /// </returns>
        /// <exception cref="CryptographicException"> The decryption failed. </exception>
        /// <exception cref="ArgumentException"> <paramref name="cipherText" /> is not in BASE64 format. </exception>
        public static string Decrypt (bool userScope, string cipherText, string additionalEntropy = null)
        {
            if (cipherText == null)
            {
                return null;
            }

            byte[] encryptedBytes;

            try
            {
                encryptedBytes = Convert.FromBase64String(cipherText);
            }
            catch (FormatException exception)
            {
                throw new ArgumentException("The cipher text is not in BASE64.", nameof(cipherText), exception);
            }

            byte[] additionalEntropyBytes = LocalEncryption.GetAdditionalEntropyBytes(additionalEntropy);

            byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, additionalEntropyBytes,
                                                        userScope ? DataProtectionScope.CurrentUser
                                                            : DataProtectionScope.LocalMachine);

            string plainText = LocalEncryption.StringEncoding.GetString(plainBytes);

            return plainText;
        }

        /// <summary>
        ///     Encrypts a string.
        /// </summary>
        /// <param name="userScope">
        ///     true if the string should only be decryptable by the current user, false if the string should
        ///     only be decryptable by any user on the local machine.
        /// </param>
        /// <param name="plainText"> The plain text string to encrypt. </param>
        /// <param name="additionalEntropy"> Any used additional entropy or salt (can be null). </param>
        /// <returns>
        ///     The encrypted string (binary data in BASE64 format) or null if <paramref name="plainText" /> is null.
        /// </returns>
        /// <exception cref="CryptographicException"> The encryption failed. </exception>
        public static string Encrypt (bool userScope, string plainText, string additionalEntropy = null)
        {
            if (plainText == null)
            {
                return null;
            }

            byte[] additionalEntropyBytes = LocalEncryption.GetAdditionalEntropyBytes(additionalEntropy);
            byte[] plainBytes = LocalEncryption.StringEncoding.GetBytes(plainText);

            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, additionalEntropyBytes,
                                                          userScope ? DataProtectionScope.CurrentUser
                                                              : DataProtectionScope.LocalMachine);

            string encryptedText = Convert.ToBase64String(encryptedBytes);

            return encryptedText;
        }

        private static byte[] GetAdditionalEntropyBytes (string additionalEntropy)
        {
            return additionalEntropy == null ? null : (additionalEntropy.Length == 0 ? null
                                                           : LocalEncryption.StringEncoding
                                                                            .GetBytes(additionalEntropy));
        }

        #endregion
    }
}
