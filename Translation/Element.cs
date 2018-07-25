using Translation.Expression;
using Translation.RegexExt;

namespace Translation
{
    public abstract class ElementBase
    {
        public readonly string Name;
        public override string ToString()
        {
            return Name;
        }
        protected ElementBase(string name)
        {
            Name = name;
        }
    }

    namespace Literal
    {
        abstract class Literal : ElementBase, IValue
        {
            public string ValueToCS()
            {
                return Name;
            }

            public Literal(string name) : base(name)
            {
            }
        }

        class IntLiteral : Literal
        {
            public static Regex Is = new Regex("[0-9]+");

            IntLiteral(string name) : base(name)
            {
            }

            public static IValue Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new IntLiteral(str);
            }
        }

        class DoubleLiteral : Literal
        {
            public static Regex Is = new Regex($"{IntLiteral.Is}.{IntLiteral.Is}");

            public DoubleLiteral(string name) : base(name)
            {
            }

            public static IValue Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new DoubleLiteral(str);
            }
        }

        class StringLiteral : Literal
        {
            public static Regex Is = new Regex(@"""[ \S]*""");

            StringLiteral(string name) : base(name)
            {
            }

            public static IValue Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new StringLiteral(str);
            }
        }
    }

    namespace Name
    {
        public abstract class AllName : ElementBase
        {
            public static Regex Is = Regex.GetTailLoopRegex(LocalVaribleName.Is.ToString(), "\\.");
            public AllName(string name) : base(name) { }
        }
        public class MemberName : AllName
        {
            public MemberName(string name) : base(name) { }
        }
        public class VaribleName : AllName, IValue
        {
            public VaribleName(string name) : base(name) { }
            public static IValue Find(string str)
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
        public class LocalVaribleName : VaribleName
        {
            public new static Regex Is = new Regex("[A-Z|a-z][A-Z|a-z|0-9]*");
            public LocalVaribleName(string name) : base(name) { }
            public new static IValue Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new VaribleName(str);
            }
        }
    }
}