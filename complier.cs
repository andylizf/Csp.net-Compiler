
static class More{
    static string toString(this string[] arrayStr){
        StringBuilder all = new StringBuilder();
        foreach(item in arrayStr)
            all += (item + '\n');
        return all;
    }
    static T run<T>(func<T> fun){
        return fun();
    }
    static bool Analysis(this string a, params string[] items){
        var aArray = a.Split(new char[]{' '}); 
        foreach (var item in items)
        {
            if(item != string())
                if(a != item)
                    return false;
            else
                item = a;
        }
        return true;
    }
}
class Namespace{}
class CCSFile : StreamReader, IDisposable
{
    private Namespace[] namespaces;
    private CCSFile[] translateInfoFiles;
    public void Dispose(){
        base.Dispose();
        foreach(var i in translateInfoFiles){
            File.Delete()
        }
    }
    
    private static void getClassInNamespace(Namespace aNamespace){// By Reflecting
        try{
            var newFileWithNamespace = translateInfoFiles();
            newFileWithNamespace.Add(
                run () => {
                    fileName + ".ComplOut"
                    string code = namespaces.toString() + @"
                        using System.Reflection;

                        class example{
                            void Main(){
                                Assembly()
                                getClass().ToFile()
                            }
                        }
                    "
                }
            )
            complier(newFileWithNamespace);
            newFileWithNamespace.delete();
        } catch(NoneNamespace err) {
            
        } finally {

        }
        if(namespaces.Contains(System))
            extern(varible("args", "Array<String>"))
    }

    public override String ReadLine(){
        var line = base.ReadLine();

        var NamespaceName = new StringBuilder();
        if(line.Analysis("using", NamespaceName)){
            var aNamespace = new Namespace(NamespaceName);
            getClassInNamespace(aNamespace);
            namespaces.Add(aNamespace);
            return line;
        }
        


        throw unTranslatedError(base.lineNum, line);
    }
}

using (resource)
{
    
}

var resource
try(){
    fun()
} finally {
    ~resource()
}