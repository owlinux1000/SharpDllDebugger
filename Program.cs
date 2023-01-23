// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;
using System.Collections.Generic;
class Program {
    static void Main(string[] args) {
        ParsedArg parsedArgs = InitArgs(args);
        Console.WriteLine(parsedArgs);
        string ns = "";
        string method = "";
        string[] methodArgs = new string[]{};
        string[] cctorArgs = new string[]{};
        if(parsedArgs.Args.ContainsKey("-n"))
            ns = parsedArgs.Args["-n"];
        if(parsedArgs.Args.ContainsKey("-m"))
            method = parsedArgs.Args["-m"];
        string dllPath = parsedArgs.Args["-d"];
        string className = parsedArgs.Args["-c"];
        if(parsedArgs.VArgs.ContainsKey("-ma"))
            methodArgs = parsedArgs.VArgs["-ma"].ToArray();
        if(parsedArgs.VArgs.ContainsKey("-ca"))
            cctorArgs = parsedArgs.VArgs["-ca"].ToArray();
        Assembly asm = Assembly.LoadFile(dllPath);
        
        //Type type = asm.GetType("FakeDll.FakeDll");
        Type type = GetTypeFromAssembly(asm, ns, className);
        Type[] types = GetConstructorParameterTypes(type, null);
        var constructorInfo= type.GetConstructor(types);
        var param = MapParams(cctorArgs, (MethodInfo)constructorInfo);
        var instance = constructorInfo.Invoke(param);
        if(method != "") {
            if(methodArgs.Length != 0) {
            }
            MethodInfo mf = type.GetMethod(method);
            if(mf != null) {
                mf.Invoke(instance, new[]{"hogehoge"});
            } else {
                Console.WriteLine($"{method} is not found");
            }
        }
        
    }
    public static ParsedArg InitArgs(string[] args) {
        var argparser = new ArgumentParser(
            "SimpleDllLoader",
            "1.0.0",
            "A Simple Loader for .NET DLL"
        );
        argparser.AddArgument("-d", "DLL_PATH", "--dll", "Specify a path for DLL");
        argparser.AddArgument("-c", "CLASS", "--class", "Specify a class");
        argparser.AddArgument("-ca", "ARGS", "--cctor-args", "Specify constructor arguments", false, true);
        argparser.AddArgument("-ct", "TYPES", "--cctor-types", "Specify constructor argument types", false, true);
        argparser.AddArgument("-n", "NAMESPACE", "--namespace", "Specify a namespace", false);
        argparser.AddArgument("-m", "METHOD", "--method", "Specify a method name", false);
        argparser.AddArgument("-ma", "ARGS", "--method-args", "Specify method arguments", false, true);
        ParsedArg parsedArgs = argparser.ParseArgs(args);
        if(parsedArgs == null || parsedArgs.Flags["-h"]) {
            Console.WriteLine(argparser.Help());
            Environment.Exit(0);
        }
        return parsedArgs;
    }
    private static Type GetTypeFromAssembly(Assembly asm, string ns, string cls) {
        string fqdn = ns == "" ? cls : ns + "." + cls;
        Type type = asm.GetType(fqdn);
        if(type == null) {
            Console.WriteLine($"{fqdn} is not found in the assembly");
            throw new ArgumentException();
        }
        return type;
    }
    private static Type[] GetConstructorParameterTypes(Type type, Type[] cctorTypes) {
        List<Type> types = new List<Type>();
        var cctor = type.GetConstructors()[0];
        foreach(var paramInfo in cctor.GetParameters()) {
            types.Add(Type.GetType(paramInfo.ParameterType.ToString()));
        }   
        return types.ToArray();
    }
    private static object[] MapParams(object[] param, MethodInfo mf) {
        if(param.Length == 0)
            return null;
        List<object> result = new List<object>();
        ParameterInfo declaredParams = mf.GetParameters();
        if(declaredParams.Length != param.Length) {
            Console.WriteLine("Argument count is not same.");
            Environment.Exit(0);
        }
        foreach(var v in param) {
            if(declaredParams.ParameterType.ToString() == "System.Int32") {
                int num;
                if(Int32.TryParse(v, out num)) {
                    result.Add(num);
                } else {
                    Console.WriteLine($"{v} cannot cast to System.Int32");
                    Environment.Exit(0);
                }
            } else {
                result.Add(v);
            }
        }
        return result;
    }
}

