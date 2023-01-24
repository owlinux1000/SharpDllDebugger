using System;
using System.Collections.Generic;

internal class ParsedArg {
    public Dictionary<string, string> Args;
    public Dictionary<string, List<string>> VArgs;
    public Dictionary<string, bool> Flags;
    public ParsedArg() {
        this.Args = new Dictionary<string, string>();
        this.Flags = new Dictionary<string, bool>();
        this.VArgs = new Dictionary<string, List<string>>();
    }
    public override string ToString()
    {
        List<string> s = new List<string>();
        foreach(var kv in Args) {
            s.Add($"{kv.Key}: {kv.Value}");
        }
        return string.Join("\n", s.ToArray());
    }
}

internal class ArgumentParser {
    private string _appName;
    private string _version;
    private string _description;

    private List<Option> _options;

    public ArgumentParser(string appName, string version, string description) {
        this._appName = appName;
        this._version = version;
        this._description = description;
        this._options = new List<Option>();
        // TODO: OptionFlagクラスを作成したらヘルプ出力用のオプションをデフォルトで追加しておく
        this.AddFlag("-h", "--help", "Show this help message and exit");
    }

    private Option GetOptionFromName(string name) {
        foreach(var option in this._options) {
            if(option.ShortName == name || option.LongName == name) {                
                return option;
            }
        }
        return null;
    }

    private Option[] GetRequiredOptions() {
        List<Option> options = new List<Option>();
        foreach(var option in this._options) {            
            if(option is OptionArg) {
                if(((OptionArg)option).Required) {
                    options.Add(option);
                }
            }
        }
        return options.ToArray();
    }

    private Option[] GetOptionalOptions() {
        List<Option> options = new List<Option>();
        foreach(var option in this._options) {
            if(option is OptionArg ) {
                if(!((OptionArg)option).Required)
                    options.Add(option);
            }
            if(option is OptionFlag)
                options.Add(option);
        }
        return options.ToArray();
    }

    public void AddArgument(string shortName, string metaVar, string longName = "", string help = "", bool required = true, bool requiredVArgs = false) {
        OptionArg option = new OptionArg(shortName, metaVar, longName, help, required, requiredVArgs);
        if(this.GetOptionFromName(shortName) != null) {
            throw new ArgumentException("hoge");
        }
        if(longName != "" && this.GetOptionFromName(longName) != null) {
            throw new ArgumentException("fuga");
        }
        this._options.Add(option);
    }

    public void AddFlag(string shortName, string longName = "", string help = "") {
        OptionFlag option = new OptionFlag(shortName, longName, help);
        if(this.GetOptionFromName(shortName) != null) {
            throw new ArgumentException("hoge");
        }
        if(longName != "" && this.GetOptionFromName(longName) != null) {
            throw new ArgumentException("fuga");
        }
        this._options.Add(option);
    }
    public string Help() {
        string message = $"usage: {this._appName} v{this._version}\n\n{this._description}\n";
        if(this._options.Count == 1) {            
            return message;
        }
        
        Option[] requiredOptions = this.GetRequiredOptions();
        if(requiredOptions.Length > 0) {
            message += "\nrequired arguments:\n";
            foreach(var option in requiredOptions) {
                message += $"  {option.FormattedHelp()}\n";
            }
        }

        Option[] optionalOptions = this.GetOptionalOptions();
        if(optionalOptions.Length > 0) {
            message += "\noptional arguments:\n";
            foreach(var option in optionalOptions) {
                message += $"  {option.FormattedHelp()}\n";
            }
        }
        
        return message;
    }

    public ParsedArg ParseArgs(string[] args) {
        ParsedArg pa = new ParsedArg();
        pa.Flags["-h"] = false;
        pa.Flags["--help"] = false;
        bool isArgumentValue = false;
        bool isArgumentValues = false;
        string currentOptionName = "";
        Option currentOption = null;
        List<string> vargs = new List<string>();        
        // もしかしたら末尾から解析していったほうが良いかもしれない...
        // 末尾から検査して、オプションではない値があったら保持しつづけて、
        // オプションがでたら、そのオプションの値だと解釈するなど
        foreach(var arg in args) {
            if(arg.StartsWith("--") || arg.StartsWith("-")) {

                if(isArgumentValues) {
                    if(vargs.Count == 0) {
                        Console.WriteLine($"{currentOptionName} requires arguments");
                        Environment.Exit(0);
                    }
                    pa.VArgs[currentOptionName] = vargs;
                    isArgumentValues = false;
                    vargs = new List<string>();
                }
                // -- もしくは -から始まるオプションに該当する引数設定がなければnullを返す
                currentOption = GetOptionFromName(arg);                
                currentOptionName = arg;
                if(currentOption == null) {
                    Console.WriteLine($"Invalid argument: {currentOptionName}");
                    return null;         
                }
                if(currentOption is OptionFlag) {                                        
                    pa.Flags[currentOptionName] = true;
                    if(currentOption.HasLongName) {
                        pa.Flags[currentOptionName] = true;
                    }
                } else if(currentOption is OptionArg) {                    
                    if(((OptionArg)currentOption).RequiredVArgs){
                        isArgumentValues = true;
                    } else {
                        isArgumentValue = true;
                    }
                }
                continue;
            }
            if(isArgumentValue) {

                pa.Args[currentOption.ShortName] = arg;
                if(currentOption.HasLongName) {
                    pa.Args[currentOption.LongName] = arg;
                }
                isArgumentValue = false;
                continue;
            }
            if(isArgumentValues) {
                vargs.Add(arg);
                continue;
            }
        }
        // 引数ありのオプション引数を受け取ったにも関わらず、
        // その引数をパースし切れていないケースを想定
        if(isArgumentValue) {
            Console.WriteLine($"{currentOptionName} requires argument");
            return null;
        }
        // 最後が可変長引数だった場合に一時変数vargsをparsedArgumentに格納する
        if(isArgumentValues) {
            if(vargs.Count == 0) {
                Console.WriteLine($"{currentOptionName} requires arguments");
                Environment.Exit(0);
            }
            pa.VArgs[currentOptionName] = vargs;
            isArgumentValues = false;
        }
        return pa;
    }
}