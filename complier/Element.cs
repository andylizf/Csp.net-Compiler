using System;
using System.Text.RegularExpressions;
using RegexGrammar.Expression;
using RegexGrammar.Expression.Operation;

namespace RegexGrammar.Element
{
    abstract class Element
    {
        public static Regex GetRegexLikeABA(String infix, String str)
        {
            return new Regex($"{str}({infix}{str})*");
        } // Return like this: str(infixstr)*
        public readonly String Name;
        public override String ToString()
        {
            return Name;
        }
        public Element(String name)
        {
            Name = name;
        }
    }
    namespace Literal
    {
        abstract class Literal : Element, IValue
        {
            ///- TODO: 0.0F······
            public string ValueToCS()
            {
                return Name;
            }
            public Literal(String name) : base(name) { }
        }
        class IntLiteral : Literal
        {
            public static Level level = Level.Min;
            public static Regex Is = new Regex("[0-9]+");
            IntLiteral(String name) : base(name) { }
            public static IValue Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                else return new IntLiteral(str);
            }
        }
        class DoubleLiteral : Literal
        {
            public static Level level = Level.Min;
            public static Regex Is = new Regex($"{IntLiteral.Is}.{IntLiteral.Is}");
            public DoubleLiteral(String name) : base(name) { }
            public static IValue Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                else return new DoubleLiteral(str);
            }
        }
        class StringLiteral : Literal
        {
            public static Level level = Level.Min;
            public static Regex Is = new Regex(@"""[ \S]*""");
            StringLiteral(String name) : base(name) { }
            public static IValue Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                else return new StringLiteral(str);
            }
        }
    }
    namespace RegexGrammar.Name
    {
        abstract class AllName : Element
        {
            public static Regex Is = GetRegexLikeABA("\\.", LocalVaribleName.Is.ToString());
            public AllName(String name) : base(name) { }
        }
        class MemberName : AllName
        {
            public MemberName(String name) : base(name) { }
        }
        class VaribleName : AllName, IValue
        {
            public static Level level = Level.Min;
            public VaribleName(String name) : base(name) { }
            public static IValue Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                else return new VaribleName(str);
            }
            public string ValueToCS()
            {
                return Name;
            }
        }
        class LocalVaribleName : VaribleName
        {
            public new static Regex Is = new Regex("[A-Z|a-z][A-Z|a-z|0-9]*");
            public LocalVaribleName(String name) : base(name) { }
            public new static IValue Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                else return new VaribleName(str);
            }
        }
    }
}