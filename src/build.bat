@..\packages\NuGet.CommandLine.2.7.1\tools\NuGet.exe restore packages.config -PackagesDirectory "..\packages"
@msbuild TrailingStopLoss.msbuild /t:Build;Test;Package