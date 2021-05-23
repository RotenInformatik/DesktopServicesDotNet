using System;
using System.IO;




namespace RI.Framework.IO.Paths
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="DirectoryPath" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsDirectoryPathExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets the access control information for the directory.
        /// </summary>
        /// <param name="directory"> The directory. </param>
        /// <param name="sections"> The access control sections required for the directory. </param>
        /// <returns>
        ///     The access control information for the directory.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="directory" /> has wildcards. </exception>
        public static DirectorySecurity GetAccessControl (this DirectoryPath directory, AccessControlSections sections)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            directory.VerifyRealDirectory();

            return Directory.GetAccessControl(directory, sections);
        }

        /// <summary>
        ///     Sets the access control information for the directory.
        /// </summary>
        /// <param name="directory"> The directory. </param>
        /// <param name="accessControl"> The access control information to apply to the directory. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="directory" /> or <paramref name="accessControl" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="directory" /> has wildcards. </exception>
        public static void SetAccessControl (this DirectoryPath directory, DirectorySecurity accessControl)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (accessControl == null)
            {
                throw new ArgumentNullException(nameof(accessControl));
            }

            directory.VerifyRealDirectory();

            Directory.SetAccessControl(directory, accessControl);
        }

        #endregion
    }
}
