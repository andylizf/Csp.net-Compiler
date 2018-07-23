using System;
using Translation.Expression;

namespace GrammarHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            var lineNum = 1;
            while ((line = Console.ReadLine()) != null)
            {
                string csline;
                try
                {
                    csline = Statement.Find(line.Trim()).StatementToCS();
                }
                catch (Exception)
                {
                    Error.WriteLine($"ERROR: Can't translate! Uncaught SyntaxError: Unexpected token `{line}`.", Error.FontRed);
                    Error.WriteLine($"意外的标记`{line}`导致的语法错误。", Error.FontWhite);
                    continue;
                }

                Error.WriteLine($"Line {lineNum}: ", Error.FontBlue);
                Error.WriteLine(csline, Error.FontWhite);
                lineNum++;
            }
        }
    }
}
