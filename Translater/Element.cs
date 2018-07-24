<<<<<<< HEAD
<<<<<<< HEAD
﻿using Translation.Expression;
=======
<<<<<<< HEAD
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
﻿using Translation.RegexExt;
=======
﻿using System;
using Translation.RegexExt;
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
using Translation.Expression;
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
using Translation.Expression.Operation;
using Translation.RegexExt;

namespace Translation
{
<<<<<<< HEAD
<<<<<<< HEAD
    public abstract class ElementBase
    {
=======
    public abstract class Element
    {
=======
    public abstract class Element
    {
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
<<<<<<< HEAD
        public static Regex GetTailLoopRegex(string str, string infix)//ABABABABABA......
=======
        public static Regex GetTailLoopRegex(String str, String infix)//ABABABABABA......
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            return new Regex($"{str}({infix}{str})*");
        } // Return like this: str(infixstr)*
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public readonly string Name;
        public override string ToString()
        {
            return Name;
        }
<<<<<<< HEAD
<<<<<<< HEAD
        protected ElementBase(string name)
=======
        public Element(string name)
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
        public Element(string name)
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
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
<<<<<<< HEAD
<<<<<<< HEAD

            public Literal(string name) : base(name)
            {
            }
=======
            public Literal(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            public Literal(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        }

        class IntLiteral : Literal
        {
            public static Level level = Level.Min;
            public static Regex Is = new Regex("[0-9]+");
<<<<<<< HEAD
<<<<<<< HEAD

            IntLiteral(string name) : base(name)
            {
            }

=======
            IntLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            IntLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
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
            public static Level level = Level.Min;
            public static Regex Is = new Regex($"{IntLiteral.Is}.{IntLiteral.Is}");
<<<<<<< HEAD
<<<<<<< HEAD

            public DoubleLiteral(string name) : base(name)
            {
            }

=======
            public DoubleLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            public DoubleLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
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
            public static Level level = Level.Min;
            public static Regex Is = new Regex(@"""[ \S]*""");
<<<<<<< HEAD
<<<<<<< HEAD

            StringLiteral(string name) : base(name)
            {
            }

=======
            StringLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            StringLiteral(string name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            public static IValue Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new StringLiteral(str);
            }
        }
<<<<<<< HEAD

        namespace RegexGrammar.Name
=======
    }

    namespace Name
    {
        public abstract class AllName : ElementBase
        {
<<<<<<< HEAD
<<<<<<< HEAD
            public static Regex Is = Regex.GetTailLoopRegex(LocalVaribleName.Is.ToString(), "\\.");
            public AllName(string name) : base(name) { }
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            public static Regex Is = GetTailLoopRegex(LocalVaribleName.Is.ToString(), "\\.");
            public AllName(String name) : base(name) { }
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        }
        public class MemberName : AllName
        {
            public MemberName(string name) : base(name) { }
        }
<<<<<<< HEAD
<<<<<<< HEAD
        public class VaribleName : AllName, IValue
        {
            public static Level level = Level.Min;
            public VaribleName(string name) : base(name) { }
            public static IValue Find(string str)
=======
        class VaribleName : AllName, IValue
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            public abstract class AllName : Element
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
        class VaribleName : AllName, IValue
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
        {
            public abstract class AllName : Element
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            {
<<<<<<< HEAD
                public static Regex Is = GetTailLoopRegex(LocalVaribleName.Is.ToString(), "\\.");
                public AllName(string name) : base(name) { }
=======
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new VaribleName(str);
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            }
            public class MemberName : AllName
            {
                public MemberName(string name) : base(name) { }
            }
<<<<<<< HEAD
<<<<<<< HEAD
        }
        public class LocalVaribleName : VaribleName
        {
            public new static Regex Is = new Regex("[A-Z|a-z][A-Z|a-z|0-9]*");
            public LocalVaribleName(string name) : base(name) { }
            public new static IValue Find(string str)
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            public class VaribleName : AllName, IValue
            {
<<<<<<< HEAD
                public static Level level = Level.Min;
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
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
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
=======
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;
                return new VaribleName(str);
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            }
        }
    }
}