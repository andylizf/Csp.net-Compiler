using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Translation
{
    namespace RegexExt
    {
        public class SafetyGroupsHelper
        {
            protected string[] _groupsname;
            public SafetyGroupsHelper(string[] groupsname)
            {
                _groupsname = groupsname;
            }
        }
        public class Regex
        {
            System.Text.RegularExpressions.Regex _regex;
            public Regex(string pattern): this(new System.Text.RegularExpressions.Regex(pattern))
            {
            }
            public Regex(System.Text.RegularExpressions.Regex regex)
            {
                _regex = regex;
            }

            public string[] GetGroupNames()
            {
                return _regex.GetGroupNames();
            }
            public bool IsMatch(string input) => _regex.IsMatch(input);
            public Match Match(string input)
            {
                return new Match(_regex.Match(input), _regex.GetGroupNames());
            }

            public string Replace(string input, string replacement)
            {
                return _regex.Replace(input, replacement);
            }
            public MatchCollection Matches(string input)
            {
                return new MatchCollection(_regex.Matches(input), _regex.GetGroupNames());
            }

            public override string ToString()
            {
                return _regex.ToString();
            }
        }
        public class MatchCollection: SafetyGroupsHelper
        {
            System.Text.RegularExpressions.MatchCollection _matchCollection;
            public MatchCollection(System.Text.RegularExpressions.MatchCollection matchCollection, string[] groupsname):
                base(groupsname)
            {
                _matchCollection = matchCollection;
            }
            public virtual Match this[int i] => new Match(_matchCollection[i], _groupsname);
        }
        public class Match: SafetyGroupsHelper
        {
            System.Text.RegularExpressions.Match _match;
            public Match(System.Text.RegularExpressions.Match match, string[] groupsname):
                base(groupsname) => _match = match;

            public override string ToString() => _match.ToString();
            public int Length => _match.Length;
            public int Index => _match.Index;
            public bool Success => _match.Success;
            public virtual GroupCollection Groups => new GroupCollection(_match.Groups, _groupsname);
        }
        public class GroupCollection: SafetyGroupsHelper
        {
            readonly System.Text.RegularExpressions.GroupCollection _groupCollection;
            public GroupCollection(System.Text.RegularExpressions.GroupCollection groupCollection, string[] groupsname):
                base(groupsname)
            {
                _groupCollection = groupCollection;
            }

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
