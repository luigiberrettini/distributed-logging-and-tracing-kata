@echo off


set scriptDir=%~dp0
set scriptDir=%scriptDir:~0,-1%
set rootDir=%scriptDir%\..

set deployDir="%rootDir%\deploy"
set buildBat="%scriptDir%\build.bat"
set toBeRun=%deployDir%\WebApi.exe


if not exist %deployDir% (
    call %buildBat% noPause
)

echo Starting...
%toBeRun%