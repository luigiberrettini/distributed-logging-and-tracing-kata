@echo off


set scriptDir=%~dp0
set scriptDir=%scriptDir:~0,-1%
set rootDir=%scriptDir%\..

set NuGetExe="%scriptDir%\NuGet.exe"

set MSBuildPath=%ProgramFiles%\MSBuild\14.0\Bin
if exist "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" set MSBuildPath=%ProgramFiles(x86)%\MSBuild\14.0\Bin
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MsBuild.exe" set MSBuildPath=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin
set MSBuildExe="%MSBuildPath%\MSBuild.exe"
set MSBuildPath="%MSBuildPath%"

set softwareName=DistributedLoggingTracing
set srcDir=%rootDir%\src
set packagesDir="%srcDir%\packages"
set solutionFile="%srcDir%\%softwareName%.sln"
set toBeDeployed=%srcDir%\WebApi\bin\Release\*.*
set deployDir="%rootDir%\deploy"

set pauseMode=%1


if exist %deployDir% (
    @rmdir /S /Q %deployDir%
)
mkdir %deployDir%

if not exist %packagesDir% (
    set Configuration=Release
    set Platform=AnyCPU
    echo Restoring dependency packages...
    %NuGetExe% restore %solutionFile% -verbosity quiet -msBuildPath %MSBuildPath% -noninteractive
    echo.
)

echo Building...
%MSBuildExe% %solutionFile% /t:Build /p:Configuration=Release /property:"Platform=Any CPU" /v:minimal /nologo
echo.

echo Deploying...
xcopy %toBeDeployed% %deployDir% /H /Q /R /Y
echo.

if [%pauseMode%] == [] (
    pause
)