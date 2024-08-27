# Arad.Net.Core.Informix Provider

  Starting with CSDK 4.50.xC4, Arad.Net.Core.Informix Provider compatible with .NET Core SDK/Runtime v3.1 is shipped for Windows x64 and Linux x86_64 platforms.

# Build Environment
##### The Arad.Net.Core.Informix Provider is built using below environment:
  
  ### Windows x64
    1- Microsoft Windows Server 2022 Standard or later
    2- .NET Core SDK Version : v8
    3- CMake
    4- Microsoft Visual Studio Enterprise 2022+

  ### Linux x86_64
    The Arad.Net.Core.Informix Provider is built on above Windows environment for Linux x86_64. 
    .NET Core SDK provides options to build cross platform binaries.

# CSDK requirement for .NET Core applications
  .NET Core applications using Arad.Net.Core.Informix Provider 8 need CSDK version 4.50.xC4 or later.
  The Informix .NET Core Provider is shipped with CSDK 4.50.xC4. The file/assembly name for Arad.Net.Core.Informix Provider is Informix.Net.Core.dll and the file is located at INFORMIXDIR\bin folder on both Windows and Linux CSDK installations.

# Dependencies on .NET Core Assemblies
  .NET Core applications using Arad.Net.Core.Informix Provider would need other .NET Core assemblies/libraries. 
  Such applications need to download/acquire the required assembly files (.NET Core SDK/Runtime) as suits the applications. 
  In order to run such applications, all the Informix related environment variables required i.e. INFORMIXDIR, INFORMIXSQLHOSTS, PATH etc must be set appropriately .
