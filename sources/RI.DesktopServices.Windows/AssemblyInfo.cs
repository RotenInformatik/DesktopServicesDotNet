using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DesktopServices.Windows"),]
[assembly: AssemblyDescription("Desktop application services for .NET"),]
[assembly: Guid("998D01B0-9293-47B0-B169-4F043B150B27"),]

[assembly: AssemblyProduct("RotenInformatik/DesktopServicesDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2021 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(true),]
[assembly: AssemblyVersion("0.0.0.0"),]
[assembly: AssemblyFileVersion("0.0.0.0"),]
[assembly: AssemblyInformationalVersion("0.0.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
