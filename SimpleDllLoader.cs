class SimpleDllLoader {
    public string path;
    public string @namespace;
    public string className;
    public string methodName;
    public string[] cctorArgs;
    public string[] cctorTypes;
    public string[] methodArgs;
    public string[] methodTypes;
    public SimpleDllLoader(
        string path,
        string className,
        string @namespace,
        string methodName,
        string[] cctorArgs,
        string[] cctorTypes,
        string[] methodArgs,
        string[] methodTypes
    ) {
        this.path = path;
        this.className = className;
        this.@namespace = @namespace;
        this.methodName = methodName;
        this.cctorArgs = cctorArgs;
        this.cctorTypes = cctorTypes;
        this.methodArgs = methodArgs;
        this.methodTypes = methodTypes;
    }
}    