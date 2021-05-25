using System;
using System.Security.Principal;
using System.Threading.Tasks;




namespace RI.DesktopServices.Windows.Users
{
    /// <summary>
    ///     Delegate to call code as impersonated user.
    /// </summary>
    /// <param name="token"> The token of the impersonated user. </param>
    /// <param name="identity"> The identity of the impersonated user. </param>
    /// <param name="profile"> The profile of the impersonated user or null if no profile was loaded. </param>
    public delegate void ImpersonatedAction (IntPtr token, WindowsIdentity identity, WindowsUserProfile profile);

    /// <summary>
    ///     Delegate to call code as impersonated user.
    /// </summary>
    /// <param name="token"> The token of the impersonated user. </param>
    /// <param name="identity"> The identity of the impersonated user. </param>
    /// <param name="profile"> The profile of the impersonated user or null if no profile was loaded. </param>
    public delegate Task ImpersonatedActionAsync (IntPtr token, WindowsIdentity identity, WindowsUserProfile profile);
}
