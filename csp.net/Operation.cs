using System;

namespace RegexGrammar.Expression.Operation
{
    class Level
    {
        Int32 nLevel;
        public Level(Int32 level)
        {
            nLevel = level;
        }
        public static Level None = new Level(0),
        Min = new Level(Int32.MinValue),
        Max = new Level(Int32.MaxValue);
        public static bool operator >(Level a, Level b) => a.nLevel > b.nLevel;
        public static bool operator >=(Level a, Level b) => a.nLevel >= b.nLevel;
        public static bool operator <(Level a, Level b) => a.nLevel < b.nLevel;
        public static bool operator <=(Level a, Level b) => a.nLevel <= b.nLevel;
    }
}
