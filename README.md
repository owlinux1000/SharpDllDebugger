# SimpleDllLoader

## How to use

```
C:\>SimpleDllLoader.exe -h
usage: SimpleDllLoader v1.0.0

A Simple Loader for .NET DLL

required arguments:
  -d, --dll DLL_PATH    Specify a path for DLL
  -c, --class CLASS     Specify a class

optional arguments:
  -h, --help    Show this help message and exit
  -ca, --cctor-args ARGS        Specify constructor arguments
  -ct, --cctor-types TYPES      Specify constructor argument types
  -n, --namespace NAMESPACE     Specify a namespace
  -m, --method METHOD   Specify a method name
  -ma, --method-args ARGS       Specify method arguments
  -mt, --method-types TYPES     Specify method argument types
```

### Case1: Execute a constructor without any argument

```
C:\>SimpleDllLoader.exe -d C:\FakeDll.dll -c FakeDll
```

### Case2: Execute a constructor with argument

```
C:\>SimpleDllLoader.exe -d C:\FakeDll.dll -c FakeDll -ca Test -ct System.String
```
