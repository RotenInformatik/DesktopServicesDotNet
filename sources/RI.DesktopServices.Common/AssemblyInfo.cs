using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DesktopServices.Common"),]
[assembly: AssemblyDescription("Desktop application services for .NET"),]
[assembly: Guid("5EE586C8-0850-42E4-9F07-A145D21BB41F"),]

[assembly: AssemblyProduct("RotenInformatik/DesktopServicesDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2021 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(true),]
[assembly: AssemblyVersion("1.1.0.0"),]
[assembly: AssemblyFileVersion("1.1.0.0"),]
[assembly: AssemblyInformationalVersion("1.1.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
