#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Expression;

namespace RegexGrammar
{
    class Complier
    {
        static ConsoleColor FontBlack = Console.ForegroundColor;
        static ConsoleColor FontRed = ConsoleColor.Red;
        static void ReadFromConsole()
        {
            string line;
            int lineNum = 0;
            while ((line = Console.ReadLine()) != null)
            {
                try
                {
                    Console.WriteLine(lineNum.ToString() + Statement.Find(line).StatementToCS());
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = FontRed;
                    Console.WriteLine($"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{line}`.");
                    Console.ForegroundColor = FontBlack;
                    Console.WriteLine($"意外的标记`{line}`导致的语法错误。");
                }
                lineNum++;
            }
        }
        static void Main(/*String[] args*/)
        {
            var args = new[] { @"C:\Users\andyl\Desktop\test.csp" };
            StreamReader reader;
            try
            {
                reader = new StreamReader(args[0]);
            }
            catch
            {
                ReadFromConsole();
                return;
            }
            if (Path.GetExtension(args[0]) != ".csp")
            {
                ReadFromConsole();
                return;
            }

            string line;
            int lineNum = 0;
            var translatedFile = new List<string>();
            var errors = new Dictionary<int, (string, string)>();

            var fileStr = reader.ReadToEnd();
            var structLine = FileSign.Find(fileStr);
            if (structLine == null)
            {
                errors.Add(lineNum, ($"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{fileStr}`.", $"意外的标记`{fileStr}`导致的语法错误。"));
            }
            else
                translatedFile.Add(structLine.FileSignToCS());
            lineNum++;

            if(errors.Count == 0)
            {
                StreamWriter CSFile = new StreamWriter(Path.GetFileNameWithoutExtension(args[0]) + ".cs");
                foreach(var i in translatedFile)
                {
                    CSFile.WriteLine(i);
                }
            }
            else
            {
                foreach(var error in errors)
                {
                    Console.ForegroundColor = FontRed;
                    Console.WriteLine(error.Value.Item1);
                    Console.ForegroundColor = FontBlack;
                    Console.WriteLine(error.Value.Item2);
                }
            }
            Console.ReadKey();
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
                    catch (Exception exception)
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
        private class UsingStatement: IFileSign
        {
            static Regex Is = GetIs();
            static Regex GetIs(String NamespaceName = "NamespaceName")
            {
                return new Regex($"using (?<{NamespaceName}>{MemberName.Is})");
            }
            public static IFileSign Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                
                var namespacesName = new List<MemberName>();
                foreach (Capture capture in match.Groups["NamespaceName"].Captures)
                {
                    namespacesName.Add(new MemberName(capture.ToString()));
                }
                return new UsingStatement()
                {
                    namespacesUsing = namespacesName.ToArray(),
                };
            }

            MemberName[] namespacesUsing;
            Match match;

            public string FileSignToCS()
            {
                string str = "using " + namespacesUsing[0].Name;
                for (int i = 1; i < namespacesUsing.Length; i++)
                {
                    str += ";\nusing " + namespacesUsing[i].Name;
                }
                return str;
            }
            
        }
        private class NamespaceStatement : IFileSign
        {
            static Regex Is = GetIs();
            static int count = 0; // 单例模式
            static Regex GetIs(String NamespaceName = "NamespaceName", String ParameterName = "ParameterName")
            {
                var NamespaceStr = $"(namespace (?<{NamespaceName}>{MemberName.Is}){FileSign.NextLine})?";
                return new Regex(NamespaceStr + MainFunc.GetIs(ParameterName));
            }
            public static IFileSign Find(String str)
            {
                if (count != 0)
                    return null;

                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                count++;
                return new NamespaceStatement()
                {
                    thisNamespace = new MemberName(match.Groups["NamespaceName"].ToString())
                };
            }

            MainFunc main;
            MemberName thisNamespace;

            public string FileSignToCS()
            {
                var thisNamespaceStr = thisNamespace.Name;
                var lastDot = thisNamespaceStr.LastIndexOf('.');
                var csnamespace = $"namespace {thisNamespaceStr.Substring(0, lastDot - 1)}{{";
                var csclass = $"class {thisNamespaceStr.Substring(lastDot + 1)}{{";
                return csnamespace + csclass + main.FileSignToCS() + "}}";
            }
        }
        private class MainFunc : IFileSign
        {
            static public Regex Is = GetIs();
            static public Regex GetIs(String ArgsName = "ArgsName", String ArgsType = "ArgsType", String Statements = "Statements")
            {
                var begin = $"main = (\\((?<{ArgsName}>{LocalVaribleName.Is})?\\))? ?(: ?(?<ArgsType>int))?{FileSign.NextOrThisLine}*{{\\s*";
                var middle = Element.Element.GetRegexLikeABA(FileSign.NextLine.ToString(), $"(?<{Statements}>{Statement.Is.ToString()})");
                var end = FileSign.NextOrThisLine + "}";
                return new Regex(begin + middle + end);
            }
            public static IFileSign Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                if (match.Groups["Statements"].Captures.Count == 0)// Count?
                {
                    return new MainFunc()
                    {
                        statements = new IStatement[] {},
                        argsName = new LocalVaribleName(match.Groups["ArgsName"].ToString())
                    };
                }
                var statements = new List<IStatement>();
                foreach (Capture capture in match.Groups["Statements"].Captures)
                {
                    var statement = Statement.Find(capture.ToString());
                    if (statement == null)
                        return null;
                    statements.Add(statement);
                }

                return new MainFunc()
                {
                    statements = statements.ToArray(),
                    argsName = new LocalVaribleName(match.Groups["ArgsName"].ToString())
                };
            }

            LocalVaribleName argsName;
            IStatement[] statements;
            public string FileSignToCS()
            {
                String begin = "int Main(String[] " + argsName + "){";
                String middle;
                String end = "}";
                if (statements.Length == 0)
                    return begin + end;

                middle = statements[0].StatementToCS();
                for (int i = 1; i < statements.Length; i++)
                {
                    middle += "\n" + statements[i].StatementToCS();
                }
                return begin + middle + end;
            }
        }
    }
}

