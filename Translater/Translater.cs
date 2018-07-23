using System;
using System.Collections.Generic;
using System.IO;
using Translation.RegexExt;
using Translation.Expression;
<<<<<<< HEAD
using Capture = System.Text.RegularExpressions.Capture;
using Translation.Element.Literal.RegexGrammar.Name;
=======
using Match = Translation.RegexExt.Match;
using Regex = Translation.RegexExt.Regex;
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930

namespace Translation
{
    public class Translater
    {
        Dictionary<int, (string, string)> errors = new Dictionary<int, (string, string)>();
<<<<<<< HEAD
        List<string> translatedLine = new List<string>();
        string cspFilePath;

        public Translater(string path)
=======
        List<String> translatedLine = new List<string>();
        String cspFilePath;
        public bool Success;

        public Translater(String path)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            if (Path.GetExtension(path) != ".csp")
            {
                throw new Exception("参数path的文件非.csp文件");
            }

            using (var reader = new StreamReader(path))
            {
                cspFilePath = path;
                var lineNum = 0;
                var fileStr = reader.ReadToEnd();
                var structLine = CspFile.Find(fileStr);//Important. Make CspFile structuralization.
                if (structLine == null)
                {
                    errors.Add(lineNum,
                        ($"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{fileStr}`.",
                            $"意外的标记`{fileStr}`导致的语法错误。"));
                }
                else
                    translatedLine.Add(structLine.FileSignToCS());

                lineNum++;
            }

            if (errors.Count == 0)
                Success = true;

        }

        public void WriteToCSFile()
        {
            if (errors.Count == 0)
            {
                foreach (var i in translatedLine)
                {
                    Console.WriteLine(i);
                }

                using (StreamWriter csFile = new StreamWriter(cspFilePath + ".cs"))
                    foreach (var i in translatedLine)
                    {
                        csFile.Write(i);
                    }
            }
            else
            {
                foreach (var error in errors)
                {
                    Error.WriteLine(error.Value.Item1, Error.FontRed);
                    Error.WriteLine(error.Value.Item2, Error.FontWhite);
                }
            }
        }
    }

    class FileSign
    {
<<<<<<< HEAD
        public static Regex Letters = new Regex("[A-Za-z]+");
        public static Regex NotLineFeed = new Regex("[^\\n]*");
        public static Regex Empty = new Regex("\\s*");
        public static Regex NextLine = new Regex("\\s*\n\\s*");
        public static Regex NextOrThisLine = new Regex("\\s*\n?\\s*");
        public static Regex LineEndAndComment = new Regex($"(?<Comment>\\s*(//{NotLineFeed})?)");
=======
        public static Regex Empty = new Regex("\\s*");
        public static Regex NextLine = new Regex("\\s*\n\\s*");
        public static Regex NextOrThisLine = new Regex("\\s*\n?\\s*");
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
    }

    class CspFile
    {
        static Regex Is = GetIs();

<<<<<<< HEAD
        static Regex GetIs(string FileNamespaceUsing = "FileNamespaceUsing",
            string FileNamespaceBlock = "FileNamespaceBlock", string UsingNamespaceName = "UsingNamespaceName")
=======
        static Regex GetIs(String FileNamespaceUsing = "FileNamespaceUsing",
            String FileNamespaceBlock = "FileNamespaceBlock", String UsingNamespaceName = "UsingNamespaceName")
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            return new Regex(
                $"(?<{FileNamespaceUsing}>{NamespaceUsing.GetIs(UsingNamespaceName)}){FileSign.NextLine}(?<{FileNamespaceBlock}>{NamespaceBlock.GetIs()})");
        }
        
<<<<<<< HEAD
        public static CspFile Find(string str)
=======
        public static CspFile Find(String str)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var block = NamespaceBlock.Find(match.Groups["FileNamespaceBlock"].ToString());
            if (block == null)
                return null;

<<<<<<< HEAD
            return new CspFile()
=======
            return new CspFile
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            {
                namespaceUsing = NamespaceUsing.Find(match.Groups["FileNamespaceUsing"].ToString()),
                namespaceBlock = block
            };
<<<<<<< HEAD
        }

        NamespaceUsing namespaceUsing;
        NamespaceBlock namespaceBlock;

        public string FileSignToCS()
        {
            return namespaceUsing.FileSignToCS() + namespaceBlock.FileSignToCS();
        }
    }

    class NamespaceUsing
    {
        static Regex Is = GetIs();

        public static Regex GetIs(string UsingNamespaceName = "UsingNamespaceName")
        {
            return Element.Element.GetTailLoopRegex($"using (?<{UsingNamespaceName}>{MemberName.Is})",
                FileSign.NextLine.ToString());
        }

        public static NamespaceUsing Find(string str)
        {
=======
        }

        NamespaceUsing namespaceUsing;
        NamespaceBlock namespaceBlock;

        public string FileSignToCS()
        {
            return namespaceUsing.FileSignToCS() + namespaceBlock.FileSignToCS();
        }
    }

    class NamespaceUsing
    {
        static Regex Is = GetIs();

        public static Regex GetIs(String UsingNamespaceName = "UsingNamespaceName")
        {
            return Element.Element.GetTailLoopRegex($"using (?<{UsingNamespaceName}>{MemberName.Is})",
                FileSign.NextLine.ToString());
        }

        public static NamespaceUsing Find(String str)
        {
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            bool exist = !match.Groups["UsingNamespaceName"].Captures.IsAllEmpty();

            var namespacesName = new List<MemberName>();
            foreach (Capture capture in match.Groups["UsingNamespaceName"].Captures)
            {
                namespacesName.Add(new MemberName(capture.ToString()));
            }

<<<<<<< HEAD
            return new NamespaceUsing()
            {
                Exist = exist,
                namespacesUsing = namespacesName.ToArray(),
=======
            return new NamespaceUsing
            {
                Exist = exist,
                namespacesUsing = namespacesName.ToArray()
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            };
        }

        MemberName[] namespacesUsing;
        public bool Exist = true;

        public string FileSignToCS()
        {
            if (!Exist)
                return "";
            var str = "";
            foreach (var namespaceUsing in namespacesUsing)
            {
                str += $"using {namespaceUsing.Name};\n";
            }

            return str;
        }

    }

    class NamespaceBlock
    {
        static Regex Is = GetIs();

        public static Regex GetIs()
        {
            var NamespaceStr = $"(namespace (?<NamespaceBlock_namespace>{MemberName.Is}){FileSign.NextLine})?";
            return new Regex(NamespaceStr + $"(?<NamespaceBlock_Main>{MainFunc.Is})");
        }

<<<<<<< HEAD
        public static NamespaceBlock Find(string str)
=======
        public static NamespaceBlock Find(String str)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var main = MainFunc.Find(match.Groups["NamespaceBlock_Main"].ToString());
            if (main == null)
                return null;

<<<<<<< HEAD
            return new NamespaceBlock()
=======
            return new NamespaceBlock
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            {
                main = main,
                thisNamespace = new MemberName(match.Groups["NamespaceBlock_namespace"].ToString())
            };
        }

        MainFunc main;
        MemberName thisNamespace;

        public string FileSignToCS()
        {
            if (thisNamespace.Name == "")
                return main.FileSignToCS();
            var thisNamespaceStr = thisNamespace.Name;
            var lastDot = thisNamespaceStr.LastIndexOf('.');
            var csclass = $"class {thisNamespaceStr.Substring(lastDot + 1)}{{";
            if (lastDot == -1)
                return csclass + main.FileSignToCS() + "}";

            var csnamespace = $"namespace {thisNamespaceStr.Substring(0, lastDot)}{{";
            return csnamespace + csclass + main.FileSignToCS() + "}}";
        }

        private class MainFunc
        {
            
            public static Regex Is
            {
                get
                {
                    var begin =
                        $"main = (\\((?<MainFunc_argsName>{LocalVaribleName.Is})?\\))? ?(: ?(?<MainFunc_returnInt>int))?({FileSign.NextOrThisLine})*{{\\s*";
                    var middle = $"(?<MainFunc_statements>{Statements.Is})";
                    var end = FileSign.NextOrThisLine + "}";
                    return new Regex(begin + middle + end);
                }

            }
<<<<<<< HEAD
            public static MainFunc Find(string str)
=======
            public static MainFunc Find(String str)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                var returnIntOrVoid = new MemberName("int");
                if (match.Groups["MainFunc_returnInt"].ToString() == "")
                    returnIntOrVoid = new MemberName("void");

                var statements = Statements.Find(match.Groups["MainFunc_statements"].ToString());
                if (statements == null)
                    return null;

                return new MainFunc
                {
                    returnIntOrVoid = returnIntOrVoid,
                    statements = statements,
                    argsName = new LocalVaribleName(match.Groups["MainFunc_argsName"].ToString())
                };
            }
            LocalVaribleName argsName;
            MemberName returnIntOrVoid;
            Statements statements;
            public string FileSignToCS()
            {
<<<<<<< HEAD
                string param;
=======
                String param;
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
                if (argsName.Name == "")
                    param = "";
                else
                    param = "String[] " + argsName;
<<<<<<< HEAD
                string begin = $"static {returnIntOrVoid} Main({param}){{";
                string middle = statements.FileSignToCS();
                string end = "}";
=======
                String begin = $"static {returnIntOrVoid} Main({param}){{";
                String middle = statements.FileSignToCS();
                String end = "}";
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
                return begin + " \n\t" + middle + "\n" + end;
            }
            
            /*
            public static Regex Is
            {
                get
                {
                    var begin =
                        $"main = (\\((?<MainFunc_argsName>{LocalVaribleName.Is})?\\))? ?(: ?(?<MainFunc_returnInt>int))?{FileSign.NextOrThisLine}*{{\\s*";
                    var middle = Element.Element.GetTailLoopRegex($"(?<MainFunc_statements>{Statement.Is})",
                        FileSign.NextLine.ToString());
                    var end = FileSign.NextOrThisLine + "}";
                    return new Regex(begin + middle + end);
                }
            }

            public static MainFunc Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                MemberName returnIntOrVoid = new MemberName("int");
                if (match.Groups["MainFunc_returnInt"].ToString() == "")
                    returnIntOrVoid = new MemberName("void");
                if (match.Groups["MainFunc_statements"].Captures.IsAllEmpty()) // No statements. Return a empty statements.
                {
                    return new MainFunc
                    {
                        returnIntOrVoid = returnIntOrVoid,
                        statements = new IStatement[] { },
                        argsName = new LocalVaribleName(match.Groups["MainFunc_argsName"].ToString())
                    };
                }

                var statements = new List<IStatement>();
                foreach (Capture capture in match.Groups["MainFunc_statements"].Captures)
                {
                    var captureNoEmpty = capture.ToString().Trim();
                    if (captureNoEmpty == "")
                        continue;
                    IStatement statement = Statement.Find(captureNoEmpty);
                    if (statement == null)
                        return null;
                    statements.Add(statement);
                }

                return new MainFunc
                {
                    returnIntOrVoid = returnIntOrVoid,
                    statements = statements.ToArray(),
                    argsName = new LocalVaribleName(match.Groups["MainFunc_argsName"].ToString())
                };
            }

            LocalVaribleName argsName;
            MemberName returnIntOrVoid;
            IStatement[] statements;

            public string FileSignToCS()
            {
                String param;
                if (argsName.Name == "")
                    param = "";
                else
                    param = "String[] " + argsName;
                String begin = $"static {returnIntOrVoid} Main({param}){{";
                String end = "}";
                if (statements.Length == 0)
                    return begin + " \n\t" + end;

                String middle;
                middle = statements[0].StatementToCS();
                for (int i = 1; i < statements.Length; i++)
                {
                    middle += "\n\t" + statements[i].StatementToCS();
                }

                return begin + " \n\t" + middle + "\n" + end;
            }*/
        }
    }

    class Statements
    {
        public static Regex Is =>
            Element.Element.GetTailLoopRegex($"(?<_statements>{Statement.Is})", FileSign.NextLine.ToString());

<<<<<<< HEAD
        public static Statements Find(string str)
=======
        public static Statements Find(String str)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            /*
            if (match.Groups["_statements"].Captures.IsAllEmpty())// No statements. Return a empty statements.
            {
                return new Statements
                {
                    statements = new IStatement[] { }
                };
            }
            */

            var statements = new List<IStatement>();
            var captures = match.Groups["_statements"].Captures;// TODO "_" may not be sopported in group name.
            for (int i = 0; i < captures.Count; i++)
            //BUG The empty line may disrupt the order of output Lines. Maybe it's because of the nested parentheses's match pattern of Regex.
            //
            /* input:
            using System;
            namespace Cspnet{class FirstApp{static void Main(){ 
	            var i = 0 + 1;
	            Console.WriteLine(i);
	
            }}}
            */
            /* output:
            using System;
            namespace Cspnet{class FirstApp{static void Main(){ 
	            var i = 0 + 1;
	            Console.WriteLine(i);
	
            }}}
             */
            {
                var statement = Statement.Find(captures[i].ToString());
                if (statement == null)
                {
<<<<<<< HEAD
                    Error.WriteLine($"In main scope: {str}, error {captures[i]}", ConsoleColor.Red);
=======
                    Console.WriteLine($"In main scope: {str}, error {captures[i]}");
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
                    return null;
                }
                statements.Add(statement);
            }

            return new Statements
            {
                statements = statements.ToArray()
            };
        }

        IStatement[] statements;

        public string FileSignToCS()
        {
            var csStatements = statements[0].StatementToCS();
            for (var i = 1; i < statements.Length; i++)
            {
                csStatements += "\n\t" + statements[i].StatementToCS();
            }
            return csStatements;
        }
    }

}