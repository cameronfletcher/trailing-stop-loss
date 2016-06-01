@..\packages\NuGet.CommandLine.2.8.5\tools\NuGet.exe restore packages.config -PackagesDirectory "..\packages"
@msbuild Skeleton.msbuild /t:Build;Test;Package