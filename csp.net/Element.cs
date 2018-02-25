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
    class ParametersCall : Element
    {
        public static Regex Is = new Regex($"({GetRegexLikeABA(",", Value.Is.ToString())})?");
        public static Regex GetIs(String Parameters = "Parameters")
        {
            return new Regex($"({GetRegexLikeABA(",", $"(?<{Parameters}>{Value.Is})")})?");
        }
        public ParametersCall(String name)
        {
            if (Is.IsMatch(name))
                Name = name;
            else
                Name = null;
        }
        IValue[] parasValue;//TODO
    }
    namespace Literal
    {
        abstract class Literal : Element, IValue
        {
            ///- TODO: 0.0F······
        }
        class IntLiteral: Literal
        {
            public static Regex baseIs = new Regex("[0-9]*");
            public IntLiteral(String name)
            {
                if (baseIs.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
        }
        class DoubleLiteral: Literal
        {
            public static Regex baseIs = new Regex($"{IntLiteral.baseIs}.{IntLiteral.baseIs}");
            public DoubleLiteral(String name)
            {
                if (baseIs.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
        }
        class StringLiteral: Literal
        {
            public static Regex GetIs(String StringLiteral = "StringLiteral")
            {
                return new Regex($"\"(?<{StringLiteral}>[ \\S]*)\"");
            }
            public StringLiteral(String name)
            {
                var baseIs = new Regex(@"""([ \S]*)""");
                if (baseIs.IsMatch(name))
                    Name = name;
                else
                    Name = null;
            }
        }
        }
    namespace RegexGrammar.Name
    {
        abstract class AllName : Element
        {
            public static Regex Is = GetRegexLikeABA(".", LocalVaribleName.Is.ToString());
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
        class NotFuncTypeName : AllName, IClassType
        {
            public NotFuncTypeName(String name): base(name) { }
        }
        class FuncTypeName : AllName, IClassType
        {
            public static Regex Is = GetRegexLikeABA(".", LocalVaribleName.Is.ToString());
            public FuncTypeName(String name): 
        }
        class VaribleName : AllName, IValue
        {
            public VaribleName(String name): base(name) { }
            protected VaribleName()
            {
                Name = null;
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
        }
    }
}