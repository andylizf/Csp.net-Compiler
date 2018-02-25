using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using RegexGrammar.Expression.Operation;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Element;
using RegexGrammar.Expression;

namespace RegexGrammar
{
    class Complier
    {
        static void Main(String[] args)
        {
            string line;
            while((line = Console.ReadLine()) != null)
            {
                Console.WriteLine(Statement.Find(line).StatementToCS());
            }
        }
    }
}

