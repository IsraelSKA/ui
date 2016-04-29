@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=nuget.exe

msbuild updateversionnumber.proj /p:AsmVersion=%VERSION%
msbuild build.proj 
%NUGET% pack itinero.ui.nuspec -Version %VERSION% -outputdirectory .\..\artifacts
