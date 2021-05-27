using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.DesktopServices.Wpf"),]
[assembly: AssemblyDescription("Desktop application services for .NET"),]
[assembly: Guid("E4DE95EB-51A3-4684-ABA6-C59259B6089B"),]

[assembly: AssemblyProduct("RotenInformatik/DesktopServicesDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2021 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(true),]
[assembly: AssemblyVersion("1.0.0.0"),]
[assembly: AssemblyFileVersion("1.0.0.0"),]
[assembly: AssemblyInformationalVersion("1.0.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
