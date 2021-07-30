# Building

---

1. Update year info
   1. AssemblyInfo.cs
   2. package.nuspec
   3. *.shfbproj
2. Update version info
   1. AssemblyInfo.cs
   2. package.nuspec
   3. *.shfbproj
3. Update version dependencies
   1. package.nuspec
4. Update documentation
   1. Synchronize Start.aml and README.md
   2. Update Assemblies.aml
   3. Update HISTORY.md
5. Build
   1. Build all code projects
   2. Build all documentation projects
   3. Create API documentation ZIP file (documentation-api.zip)
   4. Create binary ZIP files ([assembly name].zip)
6. Release
   1. Upload NuGet packages
   2. Commit and push
   3. Merge and delete branch
   4. Create GitHub release
      1. API documentation ZIP file
      2. Binary ZIP files
      3. NuGetPackages
   5. Create new branch for vNext (incl. local switch)

