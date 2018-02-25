using System;
using System.Text.RegularExpressions;
using RegexGrammar.Expression;

namespace RegexGrammar.Element
{
    abstract class Element
    {
        public static Regex GetRegexLikeABA(String infix, String str)
        {
            return new Regex($"{str}({infix}{str})*");
        } // Return like this: str(infixstr)*
        public String Name;
        public override String ToString()
        {
            return Name;
        }
        public Element NameEmpty()
        {
            if (Name == null)
                return null;
            else
                return this;
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
        }
        class IntLiteral: Literal
        {
            public static Regex Is = new Regex("[0-9]*");
            public IntLiteral(String name)
            {
                if (Is.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
            public static IValue Find(String str)
            {
                if (!Is.IsMatch(str))
                    return null;
                else return new IntLiteral(str);
            }
        }
        class DoubleLiteral: Literal
        {
            public static Regex Is = new Regex($"{IntLiteral.Is}.{IntLiteral.Is}");
            public DoubleLiteral(String name)
            {
                if (Is.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
            public static IValue Find(String str)
            {
                if (!Is.IsMatch(str))
                    return null;
                else return new IntLiteral(str);
            }
        }
        class StringLiteral: Literal
        {
            public static Regex Is = new Regex(@"""([ \S]*)""");
            public StringLiteral(String name)
            {
                if (Is.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
            public static IValue Find(String str)
            {
                if (!Is.IsMatch(str))
                    return null;
                else return new IntLiteral(str);
            }
        }
    }
    namespace RegexGrammar.Name
    {
        abstract class AllName : Element
        {
            public static Regex Is = GetRegexLikeABA("\\.", LocalVaribleName.Is.ToString());
            public AllName(String name)
            {
                if (Is.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
            protected AllName()
            {
                Name = null;
            }
        }
        class MemberName : AllName
        {
            public MemberName(String name): base(name) { }
        }
        class VaribleName : AllName, IValue
        {
            public VaribleName(String name): base(name) { }
            protected VaribleName()
            {
                Name = null;
            }
            public static IValue Find(String str)
            {
                if (!Is.IsMatch(str))
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
            public LocalVaribleName(String name)
            {
                if (Is.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
            public new static IValue Find(String str)
            {
                if (!Is.IsMatch(str))
                    return null;
                else return new VaribleName(str);
            }
        }
    }
}