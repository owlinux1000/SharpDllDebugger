internal abstract class Option {
    public string ShortName;
    public string LongName;
    public string Help;
    public bool HasLongName;
    public bool Required;
    
    public Option(string shortName, string longName = "", string help = "", bool required = true) {
        this.ShortName = shortName;
        this.LongName = longName;
        this.Help = help;
        this.HasLongName = longName != "";
        this.Required = required;
    }
    public abstract string FormattedHelp();
}

internal class OptionArg : Option{
    string MetaVar;
    public bool RequiredVArgs {get;}
    public OptionArg (
        string shortName, 
        string metaVar,
        string longName = "", 
        string help = "", 
        bool required = true,
        bool requiredVArgs = false
    ) : base(shortName, longName, help) {
        this.Required = required;
        this.RequiredVArgs = requiredVArgs;
        this.MetaVar = metaVar;        
    }
    public override string FormattedHelp()
    {
        string s = $"{this.ShortName}";
        if(this.LongName != "")
            s += $", {this.LongName}";
        s += $" {MetaVar}";
        if(this.Help != "")
            s += $"\t{this.Help}";
        return s;
    }
}

internal class OptionFlag : Option {
    public OptionFlag (
        string shortName,
        string longName = "", 
        string help = "" 
    ) : base(shortName, longName, help, false) {}
    public override string FormattedHelp() {
        string s = $"{this.ShortName}";
        if(this.LongName != "")
            s += $", {this.LongName}";
        if(this.Help != "")
            s += $"\t{this.Help}";
        return s;
    }
}

/*
internal class PositionArgs : Option {
    string MetaVar;
    public PositionArgs(string metaVar, string help) : base("", "", help, true) {
        this.MetaVar = metaVar;
    }
    public override string FormattedHelp()
    {
        string s = $" {MetaVar}";
        if(this.Help != "")
            s += $"\t{this.Help}";
        return s;
    }
}
*/