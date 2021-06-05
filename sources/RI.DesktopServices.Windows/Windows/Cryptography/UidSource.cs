using System;




namespace RI.DesktopServices.Windows.Cryptography
{
    /// <summary>
    /// Flags to define the sources used in a unique ID calculation by <see cref="UidCalculator"/>.
    /// </summary>
    /// <remarks>
    /// <note type="note">
    /// Name-based sources (<see cref="UserName"/>, <see cref="MachineName"/>, <see cref="UserDomainName"/>, <see cref="MachineDomainName"/>) can easily be changed by the user.
    /// </note>
    /// <note type="note">
    /// Hardware-based sources (<see cref="MacAddress"/>, <see cref="MotherboardId"/>, <see cref="ProcessorId"/>, <see cref="DriveId"/>) are not always available.
    /// </note>
    /// </remarks>
    [Flags]
    [Serializable]
    public enum UidSource
    {
        /// <summary>
        /// The currently logged in user.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// Note that the username can easily be changed by the user.
        /// </note>
        /// </remarks>
        UserName = 0x0001,

        /// <summary>
        /// The machine name.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// Note that the machine name can easily be changed by the user.
        /// </note>
        /// </remarks>
        MachineName = 0x0002,

        /// <summary>
        /// The domain name this user has joined.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// Note that the domain name can easily be changed by the user.
        /// </note>
        /// </remarks>
        UserDomainName = 0x0004,

        /// <summary>
        /// The domain name this machine has joined.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// Note that the domain name can easily be changed by the user.
        /// </note>
        /// </remarks>
        MachineDomainName = 0x0008,

        /// <summary>
        /// The version of the operating system.
        /// </summary>
        OsVersion = 0x010,

        /// <summary>
        /// The ID of the OS installation.
        /// </summary>
        OsInstallationId = 0x0020,

        /// <summary>
        /// The ID of the system.
        /// </summary>
        SystemId = 0x0040,

        /// <summary>
        /// The MAC addresses of the installed network interfaces.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// If multiple network adapters are available, all of them are used.
        /// </note>
        /// <note type="note">
        /// Note that the MAC addresses can easily be changed by the user.
        /// </note>
        /// </remarks>
        MacAddress = 0x0080,

        /// <summary>
        /// The ID of the processor (if available)
        /// </summary>
        ProcessorId = 0x0100,

        /// <summary>
        /// The ID of the motherboard (if available).
        /// </summary>
        MotherboardId = 0x0200,

        /// <summary>
        /// The ID of the system drive (if available)
        /// </summary>
        DriveId = 0x0400,

        /// <summary>
        /// Option for <see cref="MacAddress"/> to include also wireless network interfaces.
        /// </summary>
        MacAddressIncludeWireless = 0x0800,

        /// <summary>
        /// Option for <see cref="MacAddress"/> to include also non-physical network interfaces.
        /// </summary>
        MacAddressIncludeNonPhysical = 0x1000,

        /// <summary>
        /// Uses all possible sources.
        /// </summary>
        All = 0x1FFF,

        /// <summary>
        /// Uses all hardware-based sources (<see cref="MacAddress"/>, <see cref="MotherboardId"/>, <see cref="ProcessorId"/>, <see cref="DriveId"/>). Wireless and non-physical network interfaces are ignored.
        /// </summary>
        Hardware = UidSource.MacAddress | UidSource.MotherboardId | UidSource.ProcessorId | UidSource.DriveId,

        /// <summary>
        /// Uses all operation system based sources (<see cref="OsVersion"/>, <see cref="OsInstallationId"/>, <see cref="SystemId"/>).
        /// </summary>
        OperatingSystem = UidSource.OsVersion | UidSource.OsInstallationId | UidSource.SystemId,

        /// <summary>
        /// Uses all name-based sources (<see cref="UserName"/>, <see cref="MachineName"/>, <see cref="UserDomainName"/>, <see cref="MachineDomainName"/>)).
        /// </summary>
        Names = UidSource.UserName | UidSource.MachineName | UidSource.UserDomainName | UidSource.MachineDomainName,

        /// <summary>
        /// Uses all sources which usually changes the least (<see cref="MotherboardId"/>, <see cref="ProcessorId"/>, <see cref="DriveId"/>, <see cref="OsVersion"/>, <see cref="OsInstallationId"/>, <see cref="SystemId"/>).
        /// </summary>
        MostStatic = UidSource.MotherboardId | UidSource.ProcessorId | UidSource.DriveId | UidSource.OsVersion | UidSource.OsInstallationId | UidSource.SystemId,
    }
}
