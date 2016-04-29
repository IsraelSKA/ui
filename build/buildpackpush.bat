@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=nuget.exe

CALL buildpack %VERSION%
%NUGET% push .\..\artifacts\Itinero.ui.%VERSION%-alpha.nupkg 
git commit -m %VERSION% -a
git tag %VERSION%
git push

