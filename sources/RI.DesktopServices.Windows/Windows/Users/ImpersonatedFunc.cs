using System;
using System.Security.Principal;
using System.Threading.Tasks;




namespace RI.DesktopServices.Windows.Users
{
    /// <summary>
    ///     Delegate to call code as impersonated user.
    /// </summary>
    /// <typeparam name="TResult"> The type of the return value. </typeparam>
    /// <param name="token"> The token of the impersonated user. </param>
    /// <param name="identity"> The identity of the impersonated user. </param>
    /// <param name="profile"> The profile of the impersonated user or null if no profile was loaded. </param>
    /// <returns>
    ///     The return value.
    /// </returns>
    public delegate TResult ImpersonatedFunc <out TResult> (IntPtr token, WindowsIdentity identity,
                                                            WindowsUserProfile profile);

    /// <summary>
    ///     Delegate to call code as impersonated user.
    /// </summary>
    /// <typeparam name="TResult"> The type of the return value. </typeparam>
    /// <param name="token"> The token of the impersonated user. </param>
    /// <param name="identity"> The identity of the impersonated user. </param>
    /// <param name="profile"> The profile of the impersonated user or null if no profile was loaded. </param>
    /// <returns>
    ///     The return value.
    /// </returns>
    public delegate Task<TResult> ImpersonatedFuncAsync <TResult> (IntPtr token, WindowsIdentity identity,
                                                                   WindowsUserProfile profile);
}
