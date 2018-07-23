using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Translation.Element.RegexGrammar.Name;
using Translation.Expression;

namespace Translation
{
    public class Translater
    {
        Dictionary<int, (string, string)> errors = new Dictionary<int, (string, string)>();
        List<String> translatedLine = new List<string>();
        String cspFilePath = null;
        public Translater(String path) {
            if (Path.GetExtension(path) != ".csp")
            {
                throw new Exception("参数path的文件非.csp文件");
            }
            using (var reader = new StreamReader(path))
            {
                cspFilePath = path;
                var lineNum = 0;
                var fileStr = reader.ReadToEnd();
                var structLine = CspFile.Find(fileStr);
                if (structLine == null)
                {
                    errors.Add(lineNum, ($"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{fileStr}`.", $"意外的标记`{fileStr}`导致的语法错误。"));
                }
                else
                    translatedLine.Add(structLine.FileSignToCS());
                lineNum++;
            }
        }
        public void WriteToCSFile()
        {
            if (errors.Count == 0)
            {
                foreach (var i in translatedLine)
                {
                    Console.WriteLine(i);
                }
                using (StreamWriter CSFile = new StreamWriter(cspFilePath + ".cs"))
                    foreach (var i in translatedLine)
                    {
                        CSFile.Write(i);
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
        private interface IFileSign
        {
            String FileSignToCS();
        }
        static class FileSign
        {
            public static Regex NextLine = new Regex("(\\s*\n\\s*)");
            public static Regex NextOrThisLine = new Regex("(\\s*\n?\\s*)");
            public static IFileSign Find(String str)
            {
                var finds = Expression.Expression.GetMethodsFromClass(typeof(IFileSign));
                foreach (var find in finds)
                {
                    Object structExp;
                    try
                    {
                        structExp = find.GetMethod("Find", new[] { typeof(String) }).Invoke(null, new[] { str.Trim() });
                    }
                    catch (Exception)
                    {
                        structExp = null;
                    }
                    if (structExp != null)
                    {
#if DEBUG
                        Console.WriteLine($"IFileSign {structExp}");
#endif
                        return structExp as IFileSign;
                    }
                }
                return null;
            }
        }

        class CspFile : IFileSign
        {
            static Regex Is = GetIs();
            static Regex GetIs(String FileNamespaceUsing = "FileNamespaceUsing", String FileNamespaceBlock = "FileNamespaceBlock", String UsingNamespaceName = "UsingNamespaceName", String ThisNamespaceName = "ThisNamespaceName", String ArgsName = "ArgsName", String ReturnInt = "ReturnInt", String Statements = "Statements", String Main = "Main")
            {
                return new Regex($"(?<{FileNamespaceUsing}>{NamespaceUsing.GetIs(UsingNamespaceName).ToString()}){FileSign.NextLine}(?<{FileNamespaceBlock}>{NamespaceBlock.GetIs(ThisNamespaceName, ArgsName, ReturnInt, Statements, Main).ToString()})");
            }
            public static CspFile Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                var main = NamespaceBlock.Find(match.Groups["Main"].ToString());
                if (main == null)
                    return null;

                return new CspFile()
                {
                    namespaceUsing = NamespaceUsing.Find(match.Groups["FileNamespaceUsing"].ToString()),
                    namespaceBlock = NamespaceBlock.Find(match.Groups["FileNamespaceBlock"].ToString())
                };
            }
            NamespaceUsing namespaceUsing;
            NamespaceBlock namespaceBlock;
            public string FileSignToCS()
            {
                return namespaceUsing.FileSignToCS() + namespaceBlock.FileSignToCS();
            }
        }
        private class NamespaceUsing : IFileSign
        {
            static Regex Is = GetIs();
            public static Regex GetIs(String UsingNamespaceName = "UsingNamespaceName")
            {
                return Element.Element.GetRegexLikeABA($"using (?<{UsingNamespaceName}>{MemberName.Is})", FileSign.NextLine.ToString());
            }
            public static NamespaceUsing Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                bool exist = true;
                if (match.Groups["UsingNamespaceName"].Captures.IsAllEmpty())
                {
                    exist = false;
                }
                var namespacesName = new List<MemberName>();
                foreach (Capture capture in match.Groups["UsingNamespaceName"].Captures)
                {
                    namespacesName.Add(new MemberName(capture.ToString()));
                }
                return new NamespaceUsing()
                {
                    Exist = exist,
                    namespacesUsing = namespacesName.ToArray(),
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
        private class NamespaceBlock : IFileSign
        {
            static Regex Is = GetIs();
            public static Regex GetIs(String ThisNamespaceName = "ThisNamespaceName", String ArgsName = "ArgsName", String ReturnInt = "ReturnInt", String Statements = "Statements", String Main = "Main")
            {
                var NamespaceStr = $"(namespace (?<{ThisNamespaceName}>{MemberName.Is}){FileSign.NextLine})?";
                return new Regex(NamespaceStr + $"(?<{Main}>{MainFunc.GetIs(ArgsName, ReturnInt, Statements)})");
            }
            public static NamespaceBlock Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                var main = MainFunc.Find(match.Groups["Main"].ToString());
                if (main == null)
                    return null;

                return new NamespaceBlock()
                {
                    main = main as MainFunc,
                    thisNamespace = new MemberName(match.Groups["ThisNamespaceName"].ToString())
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
        }
        private class MainFunc : IFileSign
        {
            static public Regex Is = GetIs();
            static public Regex GetIs(String ArgsName = "ArgsName", String ReturnInt = "ReturnInt", String Statements = "Statements")
            {
                var begin = $"main = (\\((?<{ArgsName}>{LocalVaribleName.Is})?\\))? ?(: ?(?<{ReturnInt}>int))?{FileSign.NextOrThisLine}*{{\\s*";
                var middle = Element.Element.GetRegexLikeABA($"(?<{Statements}>{Statement.Is.ToString()})", FileSign.NextLine.ToString());
                var end = FileSign.NextOrThisLine + "}";
                return new Regex(begin + middle + end);
            }
            public static IFileSign Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                MemberName returnIntOrVoid = new MemberName("int");
                if (match.Groups["ReturnInt"].ToString() == "")
                    returnIntOrVoid = new MemberName("void");
                if (match.Groups["Statements"].Captures.IsAllEmpty())// Count?
                {
                    return new MainFunc()
                    {
                        returnIntOrVoid = returnIntOrVoid,
                        statements = new IStatement[] { },
                        argsName = new LocalVaribleName(match.Groups["ArgsName"].ToString())
                    };
                }
                var statements = new List<IStatement>();
                foreach (Capture capture in match.Groups["Statements"].Captures)
                {
                    var captureNoEmpty = capture.ToString().Trim();
                    if (captureNoEmpty == "")
                        continue;
                    IStatement statement = Statement.Find(captureNoEmpty);
                    if (statement == null)
                        return null;
                    statements.Add(statement);
                }

                return new MainFunc()
                {
                    returnIntOrVoid = returnIntOrVoid,
                    statements = statements.ToArray(),
                    argsName = new LocalVaribleName(match.Groups["ArgsName"].ToString())
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
            }
        }
    }
}

