// Packaged interface
/*
 * Project: https://github.com/Anti-Li/Csp.net-Complier
 * Author: https://github.com/Anti-Li
 */

using System;
using System.Collections.Generic;
using System.IO;
using Translation.Name;
using Translation.RegexExt;
using Translation.Expression;
using Capture = System.Text.RegularExpressions.Capture;

namespace Translation
{
    public class Translater
    {
        Dictionary<int, (string, string)> errors = new Dictionary<int, (string, string)>();
        List<String> translatedLine = new List<string>();
        String cspFilePath;
        /// <summary>
        /// 指示是否分析成功。
        /// </summary>
        public bool Success;

        public Translater(String path)
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
                    translatedLine.Add(structLine.ToCS());

                lineNum++;
            }

            if (errors.Count == 0)
                Success = true;

        }

        public void Output()
        {
            if (Success)
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

    /// <summary>
    /// 帮助类。标识一些可能在语句块等文件标识正则表达式中可能遇到的字段。
    /// </summary>
    static class FileSign
    {
        public static Regex Letters = new Regex("[A-Za-z]+");
        public static Regex NotLineFeed = new Regex("[^\\n]*");
        public static Regex Empty = new Regex("\\s*");
        public static Regex NextLine = new Regex("\\s*\n\\s*");
        public static Regex NextOrThisLine = new Regex("\\s*\n?\\s*");
        public static Regex LineEndAndComment = new Regex($"(?<Comment>\\s*(//{NotLineFeed})?)");
    }

    public class CspFile
    {
        /// <summary>
        /// namespaceBlock与namespaceUsing与一些空格相加的结果。成功匹配是具有Csp.net基本文件标识，即是一个Csp.net代码文件的充要条件。
        /// </summary>
        static Regex Is = GetIs();

        static Regex GetIs(string FileNamespaceUsing = "FileNamespaceUsing",
            string FileNamespaceBlock = "FileNamespaceBlock", string UsingNamespaceName = "UsingNamespaceName")
        {
            return new Regex(
                $"(?<{FileNamespaceUsing}>{NamespaceUsing.GetIs(UsingNamespaceName)}){FileSign.NextLine}(?<{FileNamespaceBlock}>{NamespaceBlock.GetIs()})");
        }
        
        public static CspFile Find(string str)
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var block = NamespaceBlock.Find(match.Groups["FileNamespaceBlock"].ToString());
            if (block == null)
                return null;

            return new CspFile()
            {
                namespaceUsing = NamespaceUsing.Find(match.Groups["FileNamespaceUsing"].ToString()),
                namespaceBlock = block
            };
        }

        NamespaceUsing namespaceUsing;
        NamespaceBlock namespaceBlock;

        public string ToCS()
        {
            return namespaceUsing.ToCS() + namespaceBlock.ToCS();
        }
    }

    class NamespaceUsing
    {
        static Regex Is = GetIs();

        public static Regex GetIs(string UsingNamespaceName = "UsingNamespaceName")
        {
            return Regex.GetTailLoopRegex($"using (?<{UsingNamespaceName}>{MemberName.Is})",
                FileSign.NextLine.ToString());
        }

        public static NamespaceUsing Find(string str)
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            bool exist = !match.Groups["UsingNamespaceName"].Captures.IsAllEmpty();

            var namespacesName = new List<MemberName>();
            foreach (Capture capture in match.Groups["UsingNamespaceName"].Captures)
            {
                namespacesName.Add(new MemberName(capture.ToString()));
            }

            return new NamespaceUsing
            {
                Exist = exist,
                namespacesUsing = namespacesName.ToArray(),
            };
        }

        MemberName[] namespacesUsing;
        public bool Exist = true;

        public string ToCS()
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

    public class NamespaceBlock
    {
        static Regex Is = GetIs();

        public static Regex GetIs()
        {
            var NamespaceStr = $"(namespace (?<NamespaceBlock_namespace>{MemberName.Is}){FileSign.NextLine})?";
            return new Regex(NamespaceStr + $"(?<NamespaceBlock_Main>{MainFunc.Is})");
        }

        public static NamespaceBlock Find(string str)
        {
            Match match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var main = MainFunc.Find(match.Groups["NamespaceBlock_Main"].ToString());
            if (main == null)
                return null;

            return new NamespaceBlock()
            {
                main = main,
                thisNamespace = new MemberName(match.Groups["NamespaceBlock_namespace"].ToString())
            };
        }

        MainFunc main;
        MemberName thisNamespace;

        public string ToCS()
        {
            if (thisNamespace.Name == "")
                return main.ToCS();
            var thisNamespaceStr = thisNamespace.Name;
            var lastDot = thisNamespaceStr.LastIndexOf('.');
            var csclass = $"class {thisNamespaceStr.Substring(lastDot + 1)}{{";
            if (lastDot == -1)
                return csclass + main.ToCS() + "}";

            var csnamespace = $"namespace {thisNamespaceStr.Substring(0, lastDot)}{{";
            return csnamespace + csclass + main.ToCS() + "}}";
        }

        public class MainFunc
        {
            
            public static Regex Is
            {
                get
                {
                    var begin =
                        $"main = (\\((?<MainFunc_argsName>{LocalVaribleName.Is})?\\))? ?(: ?(?<MainFunc_returnInt>int))?({FileSign.NextOrThisLine})*{{\\s*";
                    var middle = $"(?<MainFunc_statements>{Statements.Is})";
                    var end = FileSign.NextOrThisLine + @"\}";
                    return new Regex(begin + middle + end);
                }

            }
            public static MainFunc Find(string str)
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
                    _statements = statements,
                    argsName = new LocalVaribleName(match.Groups["MainFunc_argsName"].ToString())
                };
            }
            LocalVaribleName argsName;
            MemberName returnIntOrVoid;
            Statements _statements;
            public string ToCS()
            {
                string param;
                if (argsName.Name == "")
                    param = "";
                else
                    param = "String[] " + argsName;
                string begin = $"static {returnIntOrVoid} Main({param}){{";
                string middle = _statements.ToCS();
                string end = "}";
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

    public class Statements
    {
        //To match some statements (a block of statements).
        //Contains Statement.Is.
        /// <summary>
        /// Is字段是Statement.Is的尾循环。成功匹配是是一个语句块的充要条件。
        /// </summary>
        public static Regex Is =>
                    Regex.GetTailLoopRegex($"(?<_statements>{Statement.Is})", FileSign.NextLine.ToString());

        public static Statements Find(string str)
        {
            Match match = Is.MatchesAll(str.Trim());
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
            for (var i = 0; i < captures.Count; i++)
            {
                var statement = Statement.Find(captures[i].ToString());
                if (statement == null)
                {
                    Error.WriteLine($"In main scope: {str}, error {captures[i]}", ConsoleColor.Red);
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

        public string ToCS()
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