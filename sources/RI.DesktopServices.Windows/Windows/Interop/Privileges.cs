using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

using RI.DesktopServices.Windows.Runtime;




namespace RI.DesktopServices.Windows.Interop
{
    internal static class Privileges
    {
        #region Constants

        private const int SePrivilegeEnabled = 0x00000002;

        private const uint StandardRightsRead = 0x00020000;

        private const uint StandardRightsRequired = 0x000F0000;

        private const uint TokenAdjustDefault = 0x0080;

        private const uint TokenAdjustGroups = 0x0040;

        private const uint TokenAdjustPrivileges = 0x0020;

        private const uint TokenAdjustSessionid = 0x0100;

        private const uint TokenAllAccess = (Privileges.StandardRightsRequired | Privileges.TokenAssignPrimary |
                                             Privileges.TokenDuplicate | Privileges.TokenImpersonate |
                                             Privileges.TokenQuery | Privileges.TokenQuerySource |
                                             Privileges.TokenAdjustPrivileges | Privileges.TokenAdjustGroups |
                                             Privileges.TokenAdjustDefault | Privileges.TokenAdjustSessionid);

        private const uint TokenAssignPrimary = 0x0001;

        private const uint TokenDuplicate = 0x0002;

        private const uint TokenImpersonate = 0x0004;

        private const uint TokenQuery = 0x0008;

        private const uint TokenQuerySource = 0x0010;

        private const uint TokenRead = (Privileges.StandardRightsRead | Privileges.TokenQuery);

        #endregion




        #region Static Methods

        public static void EnablePrivilege (SecurityEntity securityEntity)
        {
            if (!Enum.IsDefined(typeof(SecurityEntity), securityEntity))
            {
                throw new InvalidEnumArgumentException(nameof(securityEntity), (int)securityEntity,
                                                       typeof(SecurityEntity));
            }

            string securityEntityValue = Privileges.GetSecurityEntityValue(securityEntity);

            try
            {
                LUID locallyUniqueIdentifier = new LUID();

                if (Privileges.LookupPrivilegeValue(null, securityEntityValue, ref locallyUniqueIdentifier))
                {
                    TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                    tokenPrivileges.PrivilegeCount = 1;
                    tokenPrivileges.Attributes = Privileges.SePrivilegeEnabled;
                    tokenPrivileges.Luid = locallyUniqueIdentifier;

                    IntPtr tokenHandle = IntPtr.Zero;

                    try
                    {
                        IntPtr currentProcess = Privileges.GetCurrentProcess();

                        if (Privileges.OpenProcessToken(currentProcess,
                                                        Privileges.TokenAdjustPrivileges | Privileges.TokenQuery,
                                                        ref tokenHandle))
                        {
                            if (Privileges.AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024,
                                                                 IntPtr.Zero, IntPtr.Zero))
                            {
                                int lastError = WindowsApi.GetLastErrorCode();

                                if (lastError == (int)WindowsError.ErrorNotAllAssigned)
                                {
                                    Win32Exception win32Exception =
                                        new Win32Exception(lastError, WindowsApi.GetErrorMessage(lastError));

                                    throw new InvalidOperationException("AdjustTokenPrivileges failed.",
                                                                        win32Exception);
                                }
                            }
                            else
                            {
                                int lastError = WindowsApi.GetLastErrorCode();

                                Win32Exception win32Exception =
                                    new Win32Exception(lastError, WindowsApi.GetErrorMessage(lastError));

                                throw new InvalidOperationException("AdjustTokenPrivileges failed.", win32Exception);
                            }
                        }
                        else
                        {
                            int lastError = WindowsApi.GetLastErrorCode();

                            Win32Exception win32Exception =
                                new Win32Exception(lastError, WindowsApi.GetErrorMessage(lastError));

                            string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                                                                    "OpenProcessToken failed. CurrentProcess: {0}",
                                                                    currentProcess.ToInt32());

                            throw new InvalidOperationException(exceptionMessage, win32Exception);
                        }
                    }
                    finally
                    {
                        if (tokenHandle != IntPtr.Zero)
                        {
                            Privileges.CloseHandle(tokenHandle);
                        }
                    }
                }
                else
                {
                    int lastError = WindowsApi.GetLastErrorCode();

                    Win32Exception win32Exception =
                        new Win32Exception(lastError, WindowsApi.GetErrorMessage(lastError));

                    string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                                                            "LookupPrivilegeValue failed. SecurityEntityValue: {0}",
                                                            securityEntityValue);

                    throw new InvalidOperationException(exceptionMessage, win32Exception);
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                                                        "EnablePrivilege failed. SecurityEntity: {0}", securityEntity);

                throw new InvalidOperationException(exceptionMessage, e);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges (IntPtr tokenhandle,
                                                          [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges,
                                                          ref TOKEN_PRIVILEGES newstate, uint bufferlength,
                                                          IntPtr previousState, IntPtr returnlength);

        [DllImport("kernel32.dll", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle (IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern IntPtr GetCurrentProcess ();

        private static string GetSecurityEntityValue (SecurityEntity securityEntity)
        {
            switch (securityEntity)
            {
                case SecurityEntity.SeAssignprimarytokenName:
                    return "SeAssignPrimaryTokenPrivilege";

                case SecurityEntity.SeAuditName:
                    return "SeAuditPrivilege";

                case SecurityEntity.SeBackupName:
                    return "SeBackupPrivilege";

                case SecurityEntity.SeChangeNotifyName:
                    return "SeChangeNotifyPrivilege";

                case SecurityEntity.SeCreateGlobalName:
                    return "SeCreateGlobalPrivilege";

                case SecurityEntity.SeCreatePagefileName:
                    return "SeCreatePagefilePrivilege";

                case SecurityEntity.SeCreatePermanentName:
                    return "SeCreatePermanentPrivilege";

                case SecurityEntity.SeCreateSymbolicLinkName:
                    return "SeCreateSymbolicLinkPrivilege";

                case SecurityEntity.SeCreateTokenName:
                    return "SeCreateTokenPrivilege";

                case SecurityEntity.SeDebugName:
                    return "SeDebugPrivilege";

                case SecurityEntity.SeEnableDelegationName:
                    return "SeEnableDelegationPrivilege";

                case SecurityEntity.SeImpersonateName:
                    return "SeImpersonatePrivilege";

                case SecurityEntity.SeIncBasePriorityName:
                    return "SeIncreaseBasePriorityPrivilege";

                case SecurityEntity.SeIncreaseQuotaName:
                    return "SeIncreaseQuotaPrivilege";

                case SecurityEntity.SeIncWorkingSetName:
                    return "SeIncreaseWorkingSetPrivilege";

                case SecurityEntity.SeLoadDriverName:
                    return "SeLoadDriverPrivilege";

                case SecurityEntity.SeLockMemoryName:
                    return "SeLockMemoryPrivilege";

                case SecurityEntity.SeMachineAccountName:
                    return "SeMachineAccountPrivilege";

                case SecurityEntity.SeManageVolumeName:
                    return "SeManageVolumePrivilege";

                case SecurityEntity.SeProfSingleProcessName:
                    return "SeProfileSingleProcessPrivilege";

                case SecurityEntity.SeRelabelName:
                    return "SeRelabelPrivilege";

                case SecurityEntity.SeRemoteShutdownName:
                    return "SeRemoteShutdownPrivilege";

                case SecurityEntity.SeRestoreName:
                    return "SeRestorePrivilege";

                case SecurityEntity.SeSecurityName:
                    return "SeSecurityPrivilege";

                case SecurityEntity.SeShutdownName:
                    return "SeShutdownPrivilege";

                case SecurityEntity.SeSyncAgentName:
                    return "SeSyncAgentPrivilege";

                case SecurityEntity.SeSystemEnvironmentName:
                    return "SeSystemEnvironmentPrivilege";

                case SecurityEntity.SeSystemProfileName:
                    return "SeSystemProfilePrivilege";

                case SecurityEntity.SeSystemtimeName:
                    return "SeSystemtimePrivilege";

                case SecurityEntity.SeTakeOwnershipName:
                    return "SeTakeOwnershipPrivilege";

                case SecurityEntity.SeTcbName:
                    return "SeTcbPrivilege";

                case SecurityEntity.SeTimeZoneName:
                    return "SeTimeZonePrivilege";

                case SecurityEntity.SeTrustedCredmanAccessName:
                    return "SeTrustedCredManAccessPrivilege";

                case SecurityEntity.SeUndockName:
                    return "SeUndockPrivilege";

                default:
                    throw new ArgumentOutOfRangeException(typeof(SecurityEntity).Name);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue (string lpsystemname, string lpname, ref LUID lpLuid);

        [DllImport("Advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken (IntPtr processHandle, uint desiredAccesss, ref IntPtr tokenHandle);

        #endregion




        #region Type: LUID

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct LUID
        {
            internal int LowPart;

            internal uint HighPart;
        }

        #endregion




        #region Type: TOKEN_PRIVILEGES

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private struct TOKEN_PRIVILEGES
        {
            internal int PrivilegeCount;

            internal LUID Luid;

            internal int Attributes;
        }

        #endregion
    }
}
