using System;
<<<<<<< HEAD
<<<<<<< HEAD
using System.Linq;
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Translation
{
    namespace RegexExt
    {
<<<<<<< HEAD
<<<<<<< HEAD
        public static class CapturesExt
        {
            public static bool IsAllEmpty(this CaptureCollection captures)
            {
                foreach (Capture capture in captures)
                {
                    if (capture.Length != 0)
                        return false;
                }

                return true;
            }
        }
        public class SafetyGroupsHelper
        {
            protected string[] _groupsname;
            public SafetyGroupsHelper(string[] groupsname)
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public class SafetyGroupsHelper
        {
<<<<<<< HEAD
            protected string[] _groupsname;
            public SafetyGroupsHelper(string[] groupsname)
=======
            protected String[] _groupsname;
            public SafetyGroupsHelper(String[] groupsname)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            {
                _groupsname = groupsname;
            }
        }
<<<<<<< HEAD
<<<<<<< HEAD
        /*
        public class RegexBuilder
        {
            public Regex NamedGroup(string name, Regex regex)
            {
                return new Regex($"(?<{name}>{regex})");
            }

            public Regex RawRegex(string raw)
            {
                char[] symbols = { '{', '}', '(', ')', '<', '>', '[', ']', '-', '+', '*', '?', ','};
                foreach (var c in raw)
                {
                    if (symbols.Count(c1 => c == c1) != 0)
                        raw.Ins
                }
            }
        }*/
        
        public class Regex
        {
            System.Text.RegularExpressions.Regex _regex;
            public Regex(string pattern) : this(new System.Text.RegularExpressions.Regex(pattern))
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public class Regex
        {
            System.Text.RegularExpressions.Regex _regex;
<<<<<<< HEAD
            public Regex(string pattern): this(new System.Text.RegularExpressions.Regex(pattern))
=======
            public Regex(String pattern): this(new System.Text.RegularExpressions.Regex(pattern))
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            {
            }
            public Regex(System.Text.RegularExpressions.Regex regex)
            {
                _regex = regex;
            }
<<<<<<< HEAD
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c

            public string[] GetGroupNames()
            {
                return _regex.GetGroupNames();
            }
            public bool IsMatch(string input) => _regex.IsMatch(input);
            public Match Match(string input)
<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
            public Match Match(String input)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
=======
            public Match Match(String input)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            {
                return new Match(_regex.Match(input), _regex.GetGroupNames());
            }

            public string Replace(string input, string replacement)
            {
                return _regex.Replace(input, replacement);
            }
<<<<<<< HEAD
            public MatchCollection Matches(string input)
=======
<<<<<<< HEAD
<<<<<<< HEAD
            public MatchCollection Matches(string input)
=======
            public MatchCollection Matches(String input)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            public MatchCollection Matches(String input)
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            {
                return new MatchCollection(_regex.Matches(input), _regex.GetGroupNames());
            }

<<<<<<< HEAD
            public override string ToString()
<<<<<<< HEAD
            {
                return _regex.ToString();
            }

            // Return like this: str(infixstr)*
            public static Regex GetTailLoopRegex(string str, string infix)//ABABABABABA......
            {
                return new Regex($"{str}({infix}{str})*");
            }

            public Match MatchesAll(string str)
            {
                var match = Match(str);
                if (match.Index != 0 || match.Length != str.Length || !match.Success)
                    return null;
                return match;
            }

            public static bool IsAllEmpty(CaptureCollection captures)
            {
                foreach (Capture capture in captures)
                {
                    if (capture.Length != 0)
                        return false;
                }

                return true;
            }
        }
        public class MatchCollection : SafetyGroupsHelper
        {
            System.Text.RegularExpressions.MatchCollection _matchCollection;
            public MatchCollection(System.Text.RegularExpressions.MatchCollection matchCollection, string[] groupsname) :
=======
<<<<<<< HEAD
            public override string ToString()
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
            public override String ToString()
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
            {
                return _regex.ToString();
            }
        }
        public class MatchCollection: SafetyGroupsHelper
        {
            System.Text.RegularExpressions.MatchCollection _matchCollection;
<<<<<<< HEAD
            public MatchCollection(System.Text.RegularExpressions.MatchCollection matchCollection, string[] groupsname):
=======
            public MatchCollection(System.Text.RegularExpressions.MatchCollection matchCollection, String[] groupsname):
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
                base(groupsname)
            {
                _matchCollection = matchCollection;
            }
            public virtual Match this[int i] => new Match(_matchCollection[i], _groupsname);
        }
<<<<<<< HEAD
<<<<<<< HEAD
        public class Match : SafetyGroupsHelper
        {
            System.Text.RegularExpressions.Match _match;
            public Match(System.Text.RegularExpressions.Match match, string[] groupsname) :
                base(groupsname) => _match = match;

            public override string ToString() => _match.ToString();
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public class Match: SafetyGroupsHelper
        {
            System.Text.RegularExpressions.Match _match;
<<<<<<< HEAD
            public Match(System.Text.RegularExpressions.Match match, string[] groupsname):
                base(groupsname) => _match = match;

            public override string ToString() => _match.ToString();
=======
            public Match(System.Text.RegularExpressions.Match match, String[] groupsname):
                base(groupsname)
            {
                _match = match;
            }

>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            public int Length => _match.Length;
            public int Index => _match.Index;
            public bool Success => _match.Success;
            public virtual GroupCollection Groups => new GroupCollection(_match.Groups, _groupsname);
        }
<<<<<<< HEAD
<<<<<<< HEAD
        public class GroupCollection : SafetyGroupsHelper
        {
            readonly System.Text.RegularExpressions.GroupCollection _groupCollection;
            public GroupCollection(System.Text.RegularExpressions.GroupCollection groupCollection, string[] groupsname) :
=======
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public class GroupCollection: SafetyGroupsHelper
        {
            readonly System.Text.RegularExpressions.GroupCollection _groupCollection;
<<<<<<< HEAD
            public GroupCollection(System.Text.RegularExpressions.GroupCollection groupCollection, string[] groupsname):
=======
            public GroupCollection(System.Text.RegularExpressions.GroupCollection groupCollection, String[] groupsname):
>>>>>>> bb4d2dafdc5a556eb7c5fd073272132e9f41f930
<<<<<<< HEAD
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
                base(groupsname)
            {
                _groupCollection = groupCollection;
            }

<<<<<<< HEAD
<<<<<<< HEAD
            /// <summary>
            /// 当不存在此Regex不存在此Group时抛出NoGroupException。
            /// </summary>
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
            public Group this[string groupname]
            {
                get
                {
                    if (Array.IndexOf(_groupsname, groupname) == -1)
                    {
                        throw new NoGroupException($"There is no Group named {groupname}.");
                    }

                    return _groupCollection[groupname];
                }
            }
        }

<<<<<<< HEAD
<<<<<<< HEAD
        [Serializable]
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
=======
>>>>>>> 15c06afe46adb851d5e50169b817698f089b622c
        public class NoGroupException : Exception
        {
            public NoGroupException()
            {
            }

            public NoGroupException(string message) : base(message)
            {
            }

            public NoGroupException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected NoGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
