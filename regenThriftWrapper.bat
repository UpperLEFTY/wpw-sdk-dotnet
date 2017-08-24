@echo off

rem This simple batch file makes it easier to regenerate the dotnet Thrift code from the contract
rem defined in \rpc\wpwithin.thrift within this project.
rem
rem You must have set a THRIFT_HOME environment variable, or you can modify this batch file to hardcode
rem the full path to the thrift compiler.

setlocal

if NOT DEFINED THRIFT_HOME (
	echo Please set THRIFT_HOME environment variable to the root of your Apache Thrift 0.10.0 installation
	goto end
)

rem Assume that standard Go conventions have been used and the core Worldpay Within SDK is within the GOPATH structure.
set _WPWithinHome=%GOPATH%\src\github.com\wptechinnovation\worldpay-within-sdk

rem Assume that CWD is where this batch file is located (in the root of the solution).
set _OutputDirectory=Worldpay.Within.Rpc
echo Regenerating Thrift RPC classes in to %_OutputDirectory%

%THRIFT_HOME%\thrift-0.10.0.exe -r -out %_OutputDirectory% --gen csharp:nullable,union %_WPWithinHome%\rpc\wpwithin.thrift

:end

endlocal

