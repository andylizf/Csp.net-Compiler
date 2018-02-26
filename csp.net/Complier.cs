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
        static void Main(String[] args)
        {
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
            var errors = new Dictionary<int, string>();
            
            var finds = Expression.Expression.GetMethodsFromClass(typeof(IFileBegin));
            while ((line = reader.ReadLine()) != null)
            {
                bool isBegin = false;
                try
                {
                    foreach (var find in finds)
                    {
                        Object structExp;
                        try
                        {
                            structExp = find.GetMethod("Find", new[] { typeof(String) }).Invoke(null, new[] { line.Trim() });
                        }
                        catch(Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            structExp = null;
                        }
                        if (structExp != null)
                        {
#if DEBUG
                            Console.WriteLine($"IFileBegin {structExp}");
#endif
                            translatedFile.Add((structExp as IFileBegin).StatementToCS());
                            if (find == typeof(MainFunc))
                                isBegin = true;
                            break;
                        }
                    }
                    if(isBegin == true)
                        translatedFile.Add(Statement.Find(line.Trim()).StatementToCS());
                }
                catch (Exception exception)
                {
                    errors.Add(lineNum, $"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{line}`.");
                    errors.Add(lineNum, $"意外的标记`{line}`导致的语法错误。");
                }
                lineNum++;
            }
            if (line == "}")
            {
                errors.Remove(lineNum - 1);
                errors.Remove(lineNum - 2);
            }
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
                for(int i = 0; i < errors.Count; i += 2)
                {
                    Console.ForegroundColor = FontRed;
                    Console.WriteLine(errors[i]);
                    Console.ForegroundColor = FontBlack;
                    Console.WriteLine(errors[i + 1]);
                }
            }
            Console.ReadKey();
        }
        private interface IFileBegin: IStatement { }
        private class UsingStatement: IFileBegin
        {
            static Regex Is = GetIs();
            MemberName usingNamespaceName;
            Match match;
            static Regex GetIs(String NamespaceName = "NamespaceName")
            {
                return new Regex($"using (?<{NamespaceName}>{MemberName.Is})");
            }
            static IStatement Find(String str)
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
                    namespaces = namespacesName.ToArray(),
                    usingNamespaceName = new MemberName(match.Groups["NamespaceName"].ToString())
                };
            }

            public string StatementToCS()
            {
                string replace_str = namespaces[0].Name;
                for (int i = 1; i < namespaces.Length; i++)
                {
                    replace_str += "\n" + namespaces[i].Name;
                }
                return replace_str;
            }
            MemberName[] namespaces;
        }
        private class ThisNamespaceStatement : IFileBegin
        {
            static Regex Is = GetIs();
            MemberName thisNamespaceName;
            static Regex GetIs(String NamespaceName = "NamespaceName")
            {
                return new Regex($"namespace (?<{NamespaceName}>{MemberName.Is})");
            }
            static IStatement Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                return new ThisNamespaceStatement()
                {
                    thisNamespaceName = new MemberName(match.Groups["NamespaceName"].ToString())
                };
            }

            public string StatementToCS()
            {
                return $"using {thisNamespaceName};";
            }
        }
        private class MainFunc : IFileBegin
        {
            static Regex Is = GetIs();
            LocalVaribleName parameterName;
            static Regex GetIs(String ParameterName = "ParameterName")
            {
                return new Regex($"main = \\(((?<{ParameterName}>{LocalVaribleName.Is})\\): Array\\<String\\>\\))?(: int)?{{");
            }
            static IFileBegin Find(String str)
            {
                Match match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                return new MainFunc()
                {
                    parameterName = new LocalVaribleName(match.Groups["ParameterName"].ToString())
                };
            }

            public string StatementToCS()
            {
                return "class Program{\nint Main(String[] " + parameterName + "){";
            }
        }
    }
}

