using System;
<<<<<<< HEAD:Translater/Element.cs
using Translation.RegexExt;
using Translation.Expression;
using Translation.Expression.Operation;
=======
using System.Text.RegularExpressions;
using RegexGrammar.Expression;
using RegexGrammar.Expression.Operation;
>>>>>>> 1aa31ac0c4f0eae3c1274d967fa8b4ebe38ff158:complier/Element.cs

namespace RegexGrammar.Element
{
    abstract class Element
    {
<<<<<<< HEAD:Translater/Element.cs
        public static Regex GetTailLoopRegex(String str, String infix)//ABABABABABA......
=======
        public static Regex GetRegexLikeABA(String infix, String str)
>>>>>>> 1aa31ac0c4f0eae3c1274d967fa8b4ebe38ff158:complier/Element.cs
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
                return new IntLiteral(str);
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
                return new DoubleLiteral(str);
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
                return new StringLiteral(str);
            }
        }
    }
    namespace RegexGrammar.Name
    {
        abstract class AllName : Element
        {
<<<<<<< HEAD:Translater/Element.cs
            public static Regex Is = GetTailLoopRegex(LocalVaribleName.Is.ToString(), "\\.");
=======
            public static Regex Is = GetRegexLikeABA("\\.", LocalVaribleName.Is.ToString());
>>>>>>> 1aa31ac0c4f0eae3c1274d967fa8b4ebe38ff158:complier/Element.cs
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
                return new VaribleName(str);
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
                return new VaribleName(str);
            }
        }
    }
}