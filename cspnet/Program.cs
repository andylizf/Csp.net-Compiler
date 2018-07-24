using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Translation;

namespace System
{
    static class StringExt
    {
        public static T[] SubArray<T>(this T[] arr, int begin) => arr.SubArray(begin, arr.Length);
        public static T[] SubArray<T>(this T[] arr, int begin, int len)
        {
            var subArr = new T[len];
            Array.ConstrainedCopy(arr, len, subArr, begin, len);
            return subArr;
        }
    }
}
namespace Cspnet
{
    public class Program
    {
        static Process CallDotnet(String argument = "")
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                Arguments = argument,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var p = new Process
            {
                StartInfo = startInfo
            };
            p.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            p.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            p.Start();
            return p;
        }
        static Process CallDotnet(String[] arguments)
        {
            var argumentsStr = "";
            if (arguments.Length > 0)
            {
                argumentsStr = arguments[0];
                for (var i = 1; i < arguments.Length; i++)
                {
                    argumentsStr += " " + arguments[i];
                }
            }
            return CallDotnet(argumentsStr);
        }
        static void InvalidInput(string[] strs)
        {
            Console.WriteLine(help.help_txt);
            Error.Write("无效的输入:", Error.FontRed);
            foreach (var str in strs)
            {
                Console.WriteLine();
                Error.Write("  " + str);
            }
        }
        public static void Main(string[] args)
        {
            bool otherwise = false;
            if (args.Length >= 1)
            {
                switch (args[0])
                {
                    case "new":
                        if (args.Length == 2)
                        {
                            using (var newP = CallDotnet(new[] { "new console -o", args[1] }))
                            {
                                newP.BeginOutputReadLine();
                                newP.BeginErrorReadLine();
                                newP.WaitForExit();

                                Thread.Sleep(10);// For safety reason.
                            }
                            File.Delete(Path.Combine(args[1], "Program.cs"));
                        }
                        else
                            InvalidInput(args.SubArray(2));

                        otherwise = true;
                        break;
                    case "run":
                        foreach (FileInfo file in new DirectoryInfo(Environment.CurrentDirectory).GetFiles())
                        {
                            if (file.Extension != ".csp") continue;
                            var t = new Translater(file.FullName);
                            t.Output();
                            if (!t.Success) continue;
                            using (var runP = CallDotnet("run"))
                            {
                                runP.BeginOutputReadLine();
                                runP.BeginErrorReadLine();
                                runP.WaitForExit();

                                Thread.Sleep(10);// For safety reason.
                            }
                        }

                        otherwise = true;
                        break;
                    case "--help":
                        Console.WriteLine(help.help_txt);

                        otherwise = true;
                        break;
                    case "--version":
                        Console.WriteLine("Csp.net 版本" + Assembly.GetExecutingAssembly().GetName().Version);
                        Console.Write(".NET Core SDK 版本");
                        CallDotnet("--version");

                        otherwise = true;
                        break;
                    default:
                        CallDotnet(args);

                        otherwise = true;
                        break;
                }
            }
            if (!otherwise)
                Console.WriteLine(help.help_txt);
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
