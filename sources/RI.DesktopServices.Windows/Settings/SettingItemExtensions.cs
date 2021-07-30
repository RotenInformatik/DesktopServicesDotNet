using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using RI.DesktopServices.Windows.Cryptography;




namespace RI.DesktopServices.Settings
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="SettingItem{T}" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class SettingItemExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets the first protected setting value.
        /// </summary>
        /// <param name="settingService"> The setting service. </param>
        /// <param name="safeName"> The name of the protected values. </param>
        /// <param name="userScope">
        ///     Specifies whether the values should be readable by all users on a machine (false) or only by
        ///     the user protecting them (true).
        /// </param>
        /// <param name="additionalEntropy"> Additional custom strings which increase the entropy of the encrypted value. </param>
        /// <returns>
        ///     The setting value or null if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="settingService" /> or <paramref name="safeName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="safeName" /> is an empty string. </exception>
        /// <exception cref="CryptographicException">
        ///     The value could not be decrypted. This may be cause when trying to decrypt a
        ///     value which belongs to another user.
        /// </exception>
        public static string GetProtectedValue (this ISettingService settingService, string safeName, bool userScope,
                                                params string[] additionalEntropy)
        {
            if (settingService == null)
            {
                throw new ArgumentNullException(nameof(settingService));
            }

            if (safeName == null)
            {
                throw new ArgumentNullException(nameof(safeName));
            }

            if (string.IsNullOrWhiteSpace(safeName))
            {
                throw new ArgumentException("The string is empty.", nameof(safeName));
            }

            string safeValue = settingService.GetValue<string>(safeName);

            if (safeValue == null)
            {
                return null;
            }

            string finalAdditionalEntropy =
                SettingItemExtensions.BuildAdditionalEntropy(safeName, userScope, additionalEntropy);

            string unsafeValue = LocalEncryption.Decrypt(userScope, safeValue, finalAdditionalEntropy);
            return unsafeValue;
        }

        /// <summary>
        ///     Gets all protected setting values.
        /// </summary>
        /// <param name="settingService"> The setting service. </param>
        /// <param name="safeName"> The name of the protected values. </param>
        /// <param name="userScope">
        ///     Specifies whether the values should be readable by all users on a machine (false) or only by
        ///     the user protecting them (true).
        /// </param>
        /// <param name="additionalEntropy"> Additional custom strings which increase the entropy of the encrypted value. </param>
        /// <returns>
        ///     The setting values or an empty list if the setting is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="settingService" /> or <paramref name="safeName" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="safeName" /> is an empty string. </exception>
        /// <exception cref="CryptographicException">
        ///     The value could not be decrypted. This may be cause when trying to decrypt a
        ///     value which belongs to another user.
        /// </exception>
        public static List<string> GetProtectedValues (this ISettingService settingService, string safeName,
                                                       bool userScope, params string[] additionalEntropy)
        {
            if (settingService == null)
            {
                throw new ArgumentNullException(nameof(settingService));
            }

            if (safeName == null)
            {
                throw new ArgumentNullException(nameof(safeName));
            }

            if (string.IsNullOrWhiteSpace(safeName))
            {
                throw new ArgumentException("The string is empty.", nameof(safeName));
            }

            List<string> safeValues = settingService.GetValues<string>(safeName);

            string finalAdditionalEntropy =
                SettingItemExtensions.BuildAdditionalEntropy(safeName, userScope, additionalEntropy);

            List<string> unsafeValues = (from x in safeValues
                                         select LocalEncryption.Decrypt(userScope, x, finalAdditionalEntropy)).ToList();

            return unsafeValues;
        }

        /// <summary>
        ///     Protects values by encrypting them and storing under a separate name, deleting the unprotected values.
        /// </summary>
        /// <param name="settingService"> The setting service. </param>
        /// <param name="safeName"> The name of the protected values. </param>
        /// <param name="unsafeName"> The name of the unprotected values. </param>
        /// <param name="userScope">
        ///     Specifies whether the values should be readable by all users on a machine (false) or only by
        ///     the user protecting them (true).
        /// </param>
        /// <param name="additionalEntropy"> Additional custom strings which increase the entropy of the encrypted value. </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="settingService" />, <paramref name="safeName" />, or
        ///     <paramref name="unsafeName" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException"> <paramref name="safeName" /> or <paramref name="unsafeName" /> is an empty string. </exception>
        public static void ProtectValues (this ISettingService settingService, string safeName, string unsafeName,
                                          bool userScope, params string[] additionalEntropy)
        {
            if (settingService == null)
            {
                throw new ArgumentNullException(nameof(settingService));
            }

            if (safeName == null)
            {
                throw new ArgumentNullException(nameof(safeName));
            }

            if (string.IsNullOrWhiteSpace(safeName))
            {
                throw new ArgumentException("The string is empty.", nameof(safeName));
            }

            if (unsafeName == null)
            {
                throw new ArgumentNullException(nameof(unsafeName));
            }

            if (string.IsNullOrWhiteSpace(unsafeName))
            {
                throw new ArgumentException("The string is empty.", nameof(unsafeName));
            }

            string finalAdditionalEntropy =
                SettingItemExtensions.BuildAdditionalEntropy(safeName, userScope, additionalEntropy);

            List<string> unsafeValues = settingService.GetValues<string>(unsafeName);
            List<string> safeValues = settingService.GetValues<string>(safeName);

            safeValues.AddRange(from x in unsafeValues
                                select LocalEncryption.Encrypt(userScope, x, finalAdditionalEntropy));

            settingService.SetValues(safeName, safeValues);
            settingService.DeleteValues(unsafeName);

            unsafeValues.Clear();
            safeValues.Clear();
        }

        private static string BuildAdditionalEntropy (string safeName, bool userScope, string[] additionalEntropy) =>
            safeName + userScope + string.Join(' ', (additionalEntropy ?? new string[0]));

        #endregion
    }
}
