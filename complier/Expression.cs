using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Expression.Operation;

namespace RegexGrammar.Expression
{
    namespace Operation
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
    static class Expression
    {
        public static IEnumerable<Type> GetMethodsFromClass(Type interfaceType)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                //var interf = type.GetInterface(interfaceType.Name);
                if(interfaceType.IsAssignableFrom(type) && type != interfaceType)
                    yield return type;
            }
        }
        public static Match MatchesAll(this Regex regex, String str)
        {
            Match match = regex.Match(str);
            if (match.Index != 0 || match.Length != str.Length || !match.Success)
                return null;
            return match;
        }
        public static bool IsAllEmpty(this CaptureCollection captures)
        {
            foreach(Capture capture in captures)
            {
                if (capture.Length != 0)
                    return false;
            }
            return true;
        }
    }
    interface IValue
    {
        string ValueToCS();
    }
    interface IStatement
    {
        string StatementToCS();
    }
    static class Statement
    {
        public static Regex Is = new Regex(@".*");
        public static IStatement Find(String str)
        {
            var finds = Expression.GetMethodsFromClass(typeof(IStatement));
            foreach (var find in finds)
            {
                Object structExp;
                try
                {
                    structExp = find.GetMethod("Find", new[] { typeof(String)/*, typeof(Level) */}).Invoke(null, new[] { str });
                }
                catch
                {
                    structExp = null;
                }
                if (structExp != null)
                {
#if DEBUG
                    Console.WriteLine($"IStatement {structExp}");
#endif
                    return structExp as IStatement;
                }
            }
            return null;
        }
    }
    static class Value
    {
        public static Regex Is = new Regex(@".*");
        public static IValue Find(String str)
        {
            var finds = Expression.GetMethodsFromClass(typeof(IValue));
            Level level = null;
            foreach (var find in finds)
            {
                Object structExp;
                Level findLevel = null;
                try
                {
                    if(level == null)
                        structExp = find.GetMethod("Find", new[] { typeof(String)/*, typeof(Level) */}).Invoke(null, new[] { str });
                    else
                        structExp = find.GetMethod("Find", new[] { typeof(String), typeof(Level) }).Invoke(null, new object[] { str, findLevel });
                }
                catch
                {
                    structExp = null;
                }
                if (structExp != null)
                {
#if DEBUG
                    Console.WriteLine($"IValue {structExp}");
#endif
                    findLevel = find.GetField("level").GetValue(null) as Level;
                    return structExp as IValue;
                }
            }
            return null;
        }
    }
    abstract class OperatorExpression
    {
        // a++ a-- ++a --a a = b
    }
    class ReturnStatement : IStatement
    {
        static Regex Is = GetIs();
        public static Regex GetIs(String ReturnValue = "ReturnValue")
        {
            return new Regex($"return (?<{ReturnValue}>{Value.Is})");
        }
        public static ReturnStatement Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var returnValue = Value.Find(match.Groups["ReturnValue"].ToString());
            if (returnValue == null)
                return null;

            return new ReturnStatement()
            {
                returnValue = returnValue
            };
        }

        public string StatementToCS()
        {
            return "return " + returnValue.ValueToCS() + ";";
        }
        IValue returnValue;
    }
    class IncrementDecrementOperator : OperatorExpression, IStatement, IValue
    {
        public static Level level = new Level(12);
        static Regex Is = GetIs();
        public static Regex GetIs(String Prefix = "Prefix", String Postfix = "Postfix", String Operand = "Operand")
        {
            var inOrDe = @"(\+\+|\-\-)";
            var prefix = $"(?<{Prefix}>{inOrDe})";
            var postfix = $"(?<{Postfix}>{inOrDe})";
            return new Regex($"{prefix}? ?(?<{Operand}>{VaribleName.Is}) ?(?({Prefix})|{postfix})");
        }
        public static IncrementDecrementOperator Find(String str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }
        public static IncrementDecrementOperator Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            return new IncrementDecrementOperator()
            {
                str = str,
                oper = (match.Groups["Prefix"].ToString(), match.Groups["Postfix"].ToString()),
                operand = new VaribleName(match.Groups["Operand"].ToString())
            };
        }

        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        public string ValueToCS()
        {
            return Is.Replace(str, "${Prefix}" + operand.ValueToCS() + "${Postfix}");
        }
        String str;

        (String Prefix, String Postfix) oper;
        VaribleName operand;
    }
    class PlusMinusOperator : OperatorExpression, IValue
    {
        public static Level level = new Level(14);
        static Regex Is = GetIs();
        public static Regex GetIs(String PreOperand = "PreOperand", String PostOperand = "PostOperand", String Operator = "Operator")
        {
            var PlOrMi = @"(\+|\-)";
            return new Regex($"(?<{PreOperand}>{Value.Is}) ?(?<{Operator}>{PlOrMi}) ?(?<{PostOperand}>{Value.Is})");
        }
        public static PlusMinusOperator Find(String str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }
        public static PlusMinusOperator Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;
            var operand = (Value.Find(match.Groups["PreOperand"].ToString().Trim()), Value.Find(match.Groups["PostOperand"].ToString().Trim()));
            if (operand.Item1 == null || operand.Item2 == null)
                return null;

            return new PlusMinusOperator()
            {
                str = str,
                oper = match.Groups["Operator"].ToString()
            };
        }

        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        public string ValueToCS()
        {
            return Is.Replace(str, "${PreOperand}" + oper + " ${PostOperand}");
        }
        String str;

        String oper;
    }

    class TimesDivOperator : OperatorExpression, IValue
    {
        public static Level level = new Level(13);
        static Regex Is = GetIs();
        public static Regex GetIs(String PreOperand = "PreOperand", String PostOperand = "PostOperand", String Operator = "Operator")
        {
            var PlOrMi = @"(\*|/)";
            return new Regex($"(?<{PreOperand}>{Value.Is}) ?(?<{Operator}>{PlOrMi}) ?(?<{PostOperand}>{Value.Is})");
        }
        public static TimesDivOperator Find(String str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }
        public static TimesDivOperator Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;
            var operand = (Value.Find(match.Groups["PreOperand"].ToString().Trim()), Value.Find(match.Groups["PostOperand"].ToString().Trim()));
            if (operand.Item1 == null || operand.Item2 == null)
                return null;

            return new TimesDivOperator()
            {
                str = str,
                oper = match.Groups["Operator"].ToString(),
                operand = operand
            };
        }

        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        public string ValueToCS()
        {
            return Is.Replace(str, "${PreOperand}" + oper + " ${PostOperand}");
        }
        String str;

        String oper;
        (IValue pre, IValue post) operand;
    }
    class FuncCallStatement : OperatorExpression, IStatement, IValue
    {
        class ActualParameters
        {
            public static Regex Is => new Regex($"({Element.Element.GetRegexLikeABA($"(?<FuncCallStatement_ActualParameters_Value>{Value.Is})", ",")})?");

            public static ActualParameters Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                if (match.Groups["FuncCallStatement_ActualParameters_Value"].Captures.IsAllEmpty())// Count?
                {
                    return new ActualParameters()
                    {
                        parametersValue = new IValue[] { }
                    };
                }
                var parametersValue = new List<IValue>();
                foreach (Capture capture in match.Groups["FuncCallStatement_ActualParameters_Value"].Captures)
                {
                    var parameterValue = Value.Find(capture.ToString().Trim());
                    if (parameterValue == null)
                        return null;
                    parametersValue.Add(parameterValue);
                }

                return new ActualParameters()
                {
                    parametersValue = parametersValue.ToArray()
                };
            }
            public string ValueToCS()
            {
                if (parametersValue.Length == 0)
                    return "()";
                string replace_str = parametersValue[0].ValueToCS();
                for (int i = 1; i < parametersValue.Length; i++)
                {
                    replace_str += "," + parametersValue[i].ValueToCS();
                }
                return $"({replace_str})";
            }
            
            IValue[] parametersValue;
        }
        public static Level level = new Level(15);
        static Regex Is
        {
            get
            {
                var classname = MemberName.Is;
                var methodname = LocalVaribleName.Is;
                var value = Value.Is;

                var paras = ActualParameters.Is;
                var operandOrClassname = $"(((?<FuncCallStatement_OperandValue>{value})|(?<FuncCallStatement_ClassName>{classname}))\\.(?<FuncCallStatement_FuncName>{methodname}))";
                //(operandvalue).funcname(parameters)
                //      IValue.funcname(parameters)
                //classname.funcname(parameters)
                //      MemberName.funcname(parameters)
                // TODO: If operandvalue is a VaribleName, VaribleName.Is == MemberName.Is. The Regex Engine may choose the first one that is operandvalue.
                var funcValue = $"(?<FuncCallStatement_FuncValue>{value})";

                //(funcvalue)(parameters)
                //      IValue(parameters)
                return new Regex($"({operandOrClassname}|{funcValue})\\((?<FuncCallStatement_ActualParameters>{paras})\\)");
            }
        }

        public static FuncCallStatement Find(String str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }
        public static FuncCallStatement Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var operandValue = Value.Find(match.Groups["FuncCallStatement_OperandValue"].ToString().Trim());
            var funcValue = Value.Find(match.Groups["FuncCallStatement_FuncValue"].ToString().Trim());
            var parameters = ActualParameters.Find(match.Groups["FuncCallStatement_ActualParameters"].ToString());
            if ((operandValue == null && funcValue == null) || parameters == null)
                return null;

            return new FuncCallStatement()
            {
                str = str,
                operandValue = operandValue,
                className = new MemberName(match.Groups["FuncCallStatement_ClassName"].ToString()),
                funcName = new LocalVaribleName(match.Groups["FuncCallStatement_FuncName"].ToString()),
                funcValue = funcValue,
                parameters = parameters
            };
        }

        public string ValueToCS()
        {
            string replaceStr;
            if (funcName != null)
                replaceStr = operandValue?.ValueToCS() + "${FuncCallStatement_ClassName}.${FuncCallStatement_FuncName}";
            else
                replaceStr = funcValue?.ValueToCS();
            return Is.Replace(str, replaceStr + parameters.ValueToCS());
        }
        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        
        String str;

        IValue operandValue;
        MemberName className;
        LocalVaribleName funcName;
        IValue funcValue;
        ActualParameters parameters;
    }
    

    class AssignmentStatement : OperatorExpression, IStatement, IValue
    {
        public static Level level = new Level(16);
        static Regex Is
        {
            get
            {
                var first = $"(?<AssignmentStatement_Varible>{VaribleName.Is})";
                var end = $"(?<AssignmentStatement_Value>{Value.Is})";
                return new Regex(first + " ?= ?" + end);
            }
        }
        public static AssignmentStatement Find(String str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }
        public static AssignmentStatement Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var assignValue = Value.Find(match.Groups["AssignmentStatement_Value"].ToString().Trim());
            if (assignValue == null)
                return null;

            return new AssignmentStatement()
            {
                str = str,
                AssignVarible = new VaribleName(match.Groups["AssignmentStatement_Varible"].ToString()),
                AssignValue = assignValue
            };
        }

        public string ValueToCS()
        {
            return Is.Replace(str, "${AssignmentStatement_Varible} = " + AssignValue.ValueToCS());
        }
        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        String str;

        VaribleName AssignVarible;
        IValue AssignValue;
    }
    class VarStatement : IStatement
    {
        static Regex Is
        {
            get
            {
                var varname = LocalVaribleName.Is;
                var vartypename = MemberName.Is;
                var assign = $"( ?= ?(?<VarStatement_Value>{Value.Is}))";
                return new Regex($"var (?<VarStatement_VarName>{varname})( ?: ?(?<VarStatement_Type>{vartypename}))?(?(VarStatement_Type){assign}?|{assign})");
            }
        }
        //var (?<LocalVaribleName>{strname})(: (?<Type>{nspname}))? (?(Type){assign}?|{assign})
        public static VarStatement Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var varValue = Value.Find(match.Groups["VarStatement_Value"].ToString().Trim());
            if (varValue == null && match.Groups["VarStatement_Type"].ToString() == "")
                return null;

            return new VarStatement()
            {
                _varName = new LocalVaribleName(match.Groups["VarStatement_VarName"].ToString()),
                _varType = new MemberName(match.Groups["VarStatement_Type"].ToString()),
                _varValue = varValue
            };
        }

        public string StatementToCS()
        {
            var typeOrVar = _varType.Name != "" ? _varType.Name : "var";
            String isValue;
            if (_varValue != null)
                isValue = " = " + _varValue.ValueToCS();
            else
                isValue = "";
            return typeOrVar + $" {_varName}{isValue}" + ";";
        }
        LocalVaribleName _varName;
        MemberName _varType;
        IValue _varValue;
    }
}
