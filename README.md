# .NET Worldpay Within wrapper

## Introduction
The .NET Worldpay Within wrapper provides a convenient entry point for application developers who wish to create applications using the Worldpay Within toolkit.

Currently, this should be built against .NET Framework 4.5.2.

## Prerequisites


## Building

This section is only relevant to you if you just want to use the pre-built DLLs for using Worldpay Within.

Use nuget to acquire the 0.10.0 Apache Thrift libraries using the following command from the nugen package console:

```Install-Package ApacheThrift -Version 0.10.0```

### Worldpay.Within.Rpc

This project contains nothing but the generated RPC wrappers for Thrift, you should not need to edit code in here.

You can rebuild this code by running:

```thrift-0.10.0.exe -r -out %GOPATH%\src\innovation.worldpay.com\worldpay-within-sdk\wrappers\dotnet\Worldpay.Within\Worldpay.Within.Rpc --gen csharp:nullable,union %GOPATH%\src\innovation.worldpay.com\worldpay-within-sdk\rpc\wpwithin.thrift```

> The -r is there just for safety, in case subdirectories are used in future for storing dependent Thrift IDL files.

Be sure to refresh the project source tree in Visual Studio to ensure that any newly generated files are included in the project.  If you fail to do this, expect compile errors for missing types to be thrown.

## Using

To use Worldpay Within, add the following DLLs to your project path:

1. ```Worldpay.Within.dll``` - this contains the wrapper code and Apache Thrift generated C# wrapper code. 
1. ```Thrift.dll``` - Apache Thrift library. 
1. ```Logging Framework``` - whatever logging framework we're going to use.



  