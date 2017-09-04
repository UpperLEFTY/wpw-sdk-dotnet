@echo off

rem To generate the documentation you must have Sandcastle and the Sandcastle Help File Builder installed.

setlocal

set DXROOT=c:\Program Files (x86)\Sandcastle
set SHFBROOT=c:\Program Files (x86)\EWSoftware\Sandcastle Help File Builder
set LANGUAGE=
"%SHFBROOT%\SandcastleBuilderGUI.exe"

endlocal