class SimpleDllLoader {
    string path;
    string ns;
    string className;
    string method;
    string[] cctorArgs;
    string[] methodArgs;
    public SimpleDllLoader(
        string path,
        string className,
        string ns = "", 
        string method = "", 
        string[] cctorArgs = null,
        string[] methodArgs = null        
    ) {
        this.path = path;
        this.className = className;
        this.ns = ns;
        this.method = method;
        this.cctorArgs = cctorArgs;
        this.methodArgs = methodArgs;
    }
}    