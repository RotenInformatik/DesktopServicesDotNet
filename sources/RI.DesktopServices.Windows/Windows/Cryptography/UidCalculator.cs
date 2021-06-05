using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Win32;

using RI.DesktopServices.Windows.Users;




namespace RI.DesktopServices.Windows.Cryptography
{
    /// <summary>
    /// Implements a calculator for unique IDs based on specified sources.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class UidCalculator
    {
        /// <summary>
        /// Gets the unique ID as a byte array.
        /// </summary>
        /// <param name="source">Combination (flags) of the used sources.</param>
        /// <returns>
        /// The byte array representing the unique ID.
        /// The array size is always 64 bytes.
        /// </returns>
        public static byte[] GetUniqueIdBytes (UidSource source)
        {
            if ((int)source == 0x000)
            {
                throw new ArgumentException("Invalid UID source (0x000).", nameof(source));
            }

            if ((int)source > 0x1000)
            {
                throw new ArgumentException("Invalid UID source (greater than 0x1000).", nameof(source));
            }

            List<string> sourceValues = new List<string>();

            if ((source & UidSource.UserName) == UidSource.UserName)
            {
                sourceValues.Add(UidCalculator.GetUserName());
            }

            if ((source & UidSource.MachineName) == UidSource.MachineName)
            {
                sourceValues.Add(UidCalculator.GetMachineName());
            }

            if ((source & UidSource.UserDomainName) == UidSource.UserDomainName)
            {
                sourceValues.Add(UidCalculator.GetUserDomainName());
            }

            if ((source & UidSource.MachineDomainName) == UidSource.MachineDomainName)
            {
                sourceValues.Add(UidCalculator.GetMachineDomainName());
            }

            if ((source & UidSource.OsVersion) == UidSource.OsVersion)
            {
                sourceValues.Add(UidCalculator.GetOsVersion());
            }

            if ((source & UidSource.OsInstallationId) == UidSource.OsInstallationId)
            {
                sourceValues.Add(UidCalculator.GetOsInstallationId());
            }

            if ((source & UidSource.SystemId) == UidSource.SystemId)
            {
                sourceValues.Add(UidCalculator.GetSystemId());
            }

            if ((source & UidSource.MacAddress) == UidSource.MacAddress)
            {
                sourceValues.Add(UidCalculator.GetMacAddress(source));
            }

            if ((source & UidSource.ProcessorId) == UidSource.ProcessorId)
            {
                sourceValues.Add(UidCalculator.GetProcessorId());
            }

            if ((source & UidSource.MotherboardId) == UidSource.MotherboardId)
            {
                sourceValues.Add(UidCalculator.GetMotherboardId());
            }

            if ((source & UidSource.DriveId) == UidSource.DriveId)
            {
                sourceValues.Add(UidCalculator.GetDriveId());
            }

            List<byte[]> byteCollection = sourceValues.Select(x => Encoding.UTF8.GetBytes(x)).ToList();

            byte[] bytes = CombineBytes(byteCollection);

            using (SHA512 algorithm = SHA512.Create())
            {
                byte[] hash = algorithm.ComputeHash(bytes);
                return hash;
            }
        }

        /// <summary>
        /// Gets the unique ID as a hex string.
        /// </summary>
        /// <param name="source">Combination (flags) of the used sources.</param>
        /// <returns>
        /// The string representing the unique ID.
        /// The string length is always 128 characters.
        /// </returns>
        public static string GetUniqueIdHex (UidSource source)
        {
            byte[] bytes = GetUniqueIdBytes(source);

            StringBuilder str = new StringBuilder(bytes.Length * 2, bytes.Length * 2);

            for (int i1 = 0; i1 < bytes.Length; i1++)
            {
                str.Append(bytes[i1].ToString("X2", CultureInfo.InvariantCulture));
            }

            return str.ToString();
        }

        /// <summary>
        /// Gets the unique ID as a Base64 string.
        /// </summary>
        /// <param name="source">Combination (flags) of the used sources.</param>
        /// <returns>
        /// The string representing the unique ID.
        /// The string length is always 80 characters (no padding).
        /// </returns>
        public static string GetUniqueIdBase64 (UidSource source)
        {
            byte[] bytes = GetUniqueIdBytes(source);

            string str = Convert.ToBase64String(bytes, Base64FormattingOptions.None);

            return str;
        }

        /// <summary>
        /// Gets the unique ID as a GUID.
        /// </summary>
        /// <param name="source">Combination (flags) of the used sources.</param>
        /// <returns>
        /// The GUID representing the unique ID.
        /// </returns>
        public static Guid GetUniqueIdGuid (UidSource source)
        {
            byte[] bytes = GetUniqueIdBytes(source);

            using (MD5 algorithm = MD5.Create())
            {
                byte[] guidBytes = algorithm.ComputeHash(bytes);
                Guid guid = new Guid(guidBytes);
                return guid;
            }
        }

        private static byte[] CombineBytes (List<byte[]> bytes)
        {
            int length = 0;

            foreach (byte[] array in bytes)
            {
                length += array.Length;
            }

            byte[] result = new byte[length];
            int position = 0;

            foreach (byte[] array in bytes)
            {
                Array.Copy(array, 0, result, position, array.Length);
                position += array.Length;
            }

            return result;
        }

        private static string GetUserName () => WindowsUser.GetCurrentUser() ?? string.Empty;

        private static string GetMachineName () => WindowsUser.GetLocalDomain() ?? string.Empty;

        private static string GetUserDomainName () => WindowsUser.GetCurrentDomain() ?? string.Empty;

        private static string GetMachineDomainName () => WindowsUser.GetNetworkDomain() ?? string.Empty;

        private static string GetOsVersion () => Environment.OSVersion.ToString();

        private static string GetOsInstallationId () => GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography", "MachineGuid");

        private static string GetMacAddress (UidSource source)
        {
            bool includeWireless = (source & UidSource.MacAddressIncludeWireless) == UidSource.MacAddressIncludeWireless;
            bool includeNonPhysical = (source & UidSource.MacAddressIncludeNonPhysical) == UidSource.MacAddressIncludeNonPhysical;

            List<string> values = new List<string>();

            try
            {
                values = NetworkInterface.GetAllNetworkInterfaces()
                                         .Where(x => includeWireless ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Wireless80211))
                                         .Where(x => includeWireless ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Wman))
                                         .Where(x => includeWireless ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Wwanpp))
                                         .Where(x => includeWireless ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Wwanpp2))
                                         .Where(x => includeNonPhysical ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Tunnel))
                                         .Where(x => includeNonPhysical ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                                         .Where(x => includeNonPhysical ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Ppp))
                                         .Where(x => includeNonPhysical ||
                                                     (x.NetworkInterfaceType != NetworkInterfaceType.Slip))
                                         .Where(x => x.NetworkInterfaceType != NetworkInterfaceType.Unknown)
                                         .Select(x => x.GetPhysicalAddress()
                                                       .ToString())
                                         .Where(x => x.Trim()
                                                      .Replace("0", string.Empty)
                                                      .Replace(":", string.Empty)
                                                      .Replace(".", string.Empty)
                                                      .Length > 0)
                                         .ToList();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Network enumeration failed in {nameof(UidCalculator)}: {ex}");
            }

            values.Sort();

            return values.Count > 0
                       ? string.Join(",", values.ToArray())
                       : string.Empty;
        }

        private static string GetSystemId () => ExecuteWmiQuery("Win32_ComputerSystemProduct", "UUID") ?? string.Empty;

        private static string GetProcessorId () => ExecuteWmiQuery("Win32_Processor", "ProcessorId") ?? string.Empty;

        private static string GetMotherboardId () => ExecuteWmiQuery("Win32_BaseBoard", "SerialNumber") ?? string.Empty;

        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        private static string GetDriveId ()
        {
            string systemLogicalDiskDeviceId = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2);
            string queryString = $"SELECT * FROM Win32_LogicalDisk where DeviceId = '{systemLogicalDiskDeviceId}'";

            try
            {
                using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString);

                foreach (ManagementObject disk in managementObjectSearcher.Get())
                {
                    foreach (ManagementObject partition in disk.GetRelated("Win32_DiskPartition"))
                    {
                        foreach (ManagementObject drive in partition.GetRelated("Win32_DiskDrive"))
                        {
                            if (drive["SerialNumber"] is string value)
                            {
                                return value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"WMI execution of ({queryString}) failed in {nameof(UidCalculator)}: {ex}");
            }

            return string.Empty;
        }

        private static string ExecuteWmiQuery (string wmiClass, string wmiProperty)
        {
            List<string> values = new List<string>();

            string queryString = $"SELECT {wmiProperty} FROM {wmiClass}";

            try
            {
                using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString);
                using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
                
                foreach (ManagementBaseObject managementObject in managementObjectCollection)
                {
                    using (managementObject)
                    {
                        if (managementObject[wmiProperty] is string value)
                        {
                            values.Add(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"WMI execution of ({queryString}) failed in {nameof(UidCalculator)}: {ex}");
            }

            values.Sort();

            return values.Count > 0
                       ? string.Join(",", values.ToArray())
                       : string.Empty;
        }

        private static string GetRegistryValue (string path, string key)
        {
            object value = Registry.GetValue(path, key, null);
            return value?.ToString() ?? string.Empty;
        }
    }
}
