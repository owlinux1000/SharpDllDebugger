// See https://aka.ms/new-console-template for more information
using System;
using System.Reflection;
using System.Collections.Generic;
class Program {
    static void Main(string[] args) {
        SimpleDllLoader option = InitArgs(args);
        Type type = GetTypeFromAssembly(option.path, option.@namespace, option.className);
        var constructorInfo= GetConstructorInfo(type, option.cctorTypes);
        object[] param = MapParams(option.cctorArgs, constructorInfo);
        var instance = constructorInfo.Invoke(param);
        var methodInfo = GetMethodInfo(type, option.methodName, option.methodTypes);
        param = MapParams(option.methodArgs, methodInfo);
        methodInfo.Invoke(instance, param);
    }
    public static SimpleDllLoader InitArgs(string[] args) {
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
        argparser.AddArgument("-mt", "TYPES", "--method-types", "Specify method argument types", false, true);
        ParsedArg parsedArgs = argparser.ParseArgs(args);
        if(parsedArgs == null) {
            Environment.Exit(0);
        }        
        if(parsedArgs.Flags["-h"] || args.Length == 0) {
            Console.WriteLine(argparser.Help());
            Environment.Exit(0);
        }
        string ns = null;
        string method = null;
        string[] methodArgs = new string[]{};
        string[] methodTypes = new string[]{};
        string[] cctorArgs = new string[]{};
        string[] cctorTypes = new string[]{};
        parsedArgs.Args.TryGetValue("-n", out ns);
        parsedArgs.Args.TryGetValue("-m", out method);
        string dllPath = parsedArgs.Args["-d"];
        string className = parsedArgs.Args["-c"];
        if(parsedArgs.VArgs.ContainsKey("-ma"))
            methodArgs = parsedArgs.VArgs["-ma"].ToArray();
        if(parsedArgs.VArgs.ContainsKey("-mt"))
            methodTypes = parsedArgs.VArgs["-mt"].ToArray();           
        if(parsedArgs.VArgs.ContainsKey("-ca"))
            cctorArgs = parsedArgs.VArgs["-ca"].ToArray();
        if(parsedArgs.VArgs.ContainsKey("-ct"))
            cctorTypes = parsedArgs.VArgs["-ct"].ToArray();
        if(parsedArgs.VArgs.ContainsKey("-ma") && !parsedArgs.VArgs.ContainsKey("-mt")) {
            if(!parsedArgs.Args.ContainsKey("-m"))
                Console.WriteLine("-ma option requires -m and -mt options");
            else
                Console.WriteLine("-ma option requires -mt option");
            Environment.Exit(0);
        } else if(!parsedArgs.VArgs.ContainsKey("-ma") && parsedArgs.VArgs.ContainsKey("-mt")) {
            if(!parsedArgs.Args.ContainsKey("-m"))
                Console.WriteLine("-mt option requires -m and -ma options");
            else
                Console.WriteLine("-mt option requires -ma option");
            Environment.Exit(0);
        }
        if(parsedArgs.VArgs.ContainsKey("-ca") && !parsedArgs.VArgs.ContainsKey("-ct")) {
            if(!parsedArgs.Args.ContainsKey("-c"))
                Console.WriteLine("-ca option requires -c and -ct options");
            else
                Console.WriteLine("-ca option requires -ct option");
            Environment.Exit(0);
        } else if(!parsedArgs.VArgs.ContainsKey("-ca") && parsedArgs.VArgs.ContainsKey("-ct")) {
            if(!parsedArgs.Args.ContainsKey("-c"))
                Console.WriteLine("-ct option requires -c and -ca options");
            else
                Console.WriteLine("-ct option requires -ca option");
            Environment.Exit(0);
        }
        SimpleDllLoader instance = new SimpleDllLoader(
            dllPath,
            className,
            ns,
            method,
            cctorArgs,
            cctorTypes,
            methodArgs,
            methodTypes
        );        
        return instance;
    }
    private static Type GetTypeFromAssembly(string path, string ns, string cls) {
        Assembly asm = Assembly.LoadFile(path);
        string fqdn = ns == null ? cls : ns + "." + cls;
        Type type = asm.GetType(fqdn);
        if(type == null) {
            Console.WriteLine($"[Error] {fqdn} is not found in the assembly");
            Environment.Exit(0);
        }
        return type;
    }
    private static ConstructorInfo GetConstructorInfo(Type type, string[] cctorTypes) {
        if(cctorTypes.Length == 0)
            return type.GetConstructor(new Type[]{});

        List<Type> types = new List<Type>();
        foreach(var cctorType in cctorTypes) {
            types.Add(Type.GetType(cctorType));            
        }
        return type.GetConstructor(types.ToArray());
    }
    private static MethodInfo GetMethodInfo(Type type, string methodName, string[] methodTypes) {
        if(methodName == null) {
            Environment.Exit(0);
        }
        if(methodTypes.Length == 0)
            return type.GetMethod(methodName, new Type[]{});
        List<Type> types = new List<Type>();
        foreach(var mt in methodTypes) {
            types.Add(Type.GetType(mt));
        }
        return type.GetMethod(methodName, types.ToArray());
    }
    private static object[] MapParams(object[] param, MethodBase mbf) {
        if(param.Length == 0)
            return null;
        List<object> result = new List<object>();
        ParameterInfo[] declaredParams = mbf.GetParameters();
        Console.WriteLine(declaredParams.Length);
        if(declaredParams.Length != param.Length) {
            Console.WriteLine("Argument count is not same.");
            Environment.Exit(0);
        }
        for(int i = 0; i < param.Length; i++) {
            if(declaredParams[i].ParameterType.ToString() == "System.Int32") {
                int num;
                if(Int32.TryParse((string)param[i], out num)) {
                    result.Add(num);
                } else {
                    Console.WriteLine($"{param[i]} cannot cast to System.Int32");
                    Environment.Exit(0);
                }
            } else {
                result.Add(param[i]);
            }
        }
        return result.ToArray();
    }
}

