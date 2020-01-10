housework author src/*.csproj -s src/StorAmp/build.ini -r -v

housework author src/*.appxmanifest -s src/StorAmp/build.ini -r -v

dotnet restore StorAmp.sln

dotnet build -c release StorAmp.sln

/p:AppxBundlePlatforms="$(BuildPlatform)" /p:AppxPackageDir="$(Build.ArtifactStagingDirectory)\AppxPackages\\"  /p:UapAppxPackageBuildMode=StoreUpload 