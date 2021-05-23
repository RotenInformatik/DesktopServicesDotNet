using System;
using System.IO;




namespace RI.Framework.IO.Paths
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="FilePath" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class WindowsFilePathExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Gets the access control information for the file.
        /// </summary>
        /// <param name="file"> The file. </param>
        /// <param name="sections"> The access control sections required for the file. </param>
        /// <returns>
        ///     The access control information for the file.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="file" /> has wildcards. </exception>
        public static FileSecurity GetAccessControl (this FilePath file, AccessControlSections sections)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            file.VerifyRealFile();

            return File.GetAccessControl(file, sections);
        }

        /// <summary>
        ///     Sets the access control information for the file.
        /// </summary>
        /// <param name="file"> The file. </param>
        /// <param name="accessControl"> The access control information to apply to the file. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="file" /> or <paramref name="accessControl" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="file" /> has wildcards. </exception>
        public static void SetAccessControl (this FilePath file, FileSecurity accessControl)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (accessControl == null)
            {
                throw new ArgumentNullException(nameof(accessControl));
            }

            file.VerifyRealFile();

            File.SetAccessControl(file, accessControl);
        }

        #endregion
    }
}
