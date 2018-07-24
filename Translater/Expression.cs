using Translation.RegexExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Translation.Name;
using Translation.Expression.Operation;
using Capture = System.Text.RegularExpressions.Capture;
using CaptureCollection = System.Text.RegularExpressions.CaptureCollection;

namespace Translation.Expression
{
    namespace Operation
    {
        public class Level
        {
            int nLevel;

            public Level(int level)
            {
                nLevel = level;
            }

            public static Level None = new Level(0),
                Min = new Level(int.MinValue),
                Max = new Level(int.MaxValue);

            public static bool operator >(Level a, Level b) => a.nLevel > b.nLevel;
            public static bool operator >=(Level a, Level b) => a.nLevel >= b.nLevel;
            public static bool operator <(Level a, Level b) => a.nLevel < b.nLevel;
            public static bool operator <=(Level a, Level b) => a.nLevel <= b.nLevel;
        }
    }

    public static class Expression
    {
        public static IEnumerable<Type> GetMethodsFromClass(Type interfaceType)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                //var interf = type.GetInterface(interfaceType.Name);
                if (interfaceType.IsAssignableFrom(type) && type != interfaceType)
                    yield return type;
            }
        }
    }

    public interface IValue
    {
        string ValueToCS();
    }

    public interface IStatement
    {

        string StatementToCS();
        //static Regex Is;
    }

    public static class Statement
    {
        public static Regex Is
        {
            get
            //Regex Expansion:
            //The Is field of the class that implements the IStatement interface is spliced "or" to a Regex only as the core Regex contained in the Is field of Statements.
            {
                var sb = new StringBuilder("(");
                foreach (var classType in classesImplIStatement)
                {
                    var groupName = classType.FullName.Split('.').Last();
                    var groupRegex = classType.GetProperty("Is", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Regex;
                    sb.Append($"(?<{groupName}>{groupRegex})|");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                return new Regex(sb.ToString());
                //return new Regex(".*");
            }
        }
        static readonly IEnumerable<Type> classesImplIStatement = Expression.GetMethodsFromClass(typeof(IStatement));

        public static IStatement Find(string str)
        {
            /*
            if (str == string.Empty)
            {
                return new EmptyStatement();
            }
            */
            foreach (var find in classesImplIStatement)
            {
                object structExp;
                try
                {
                    // Trim to let str like "\r\nConsole.WriteLine()\r\n" can be recognition.
                    structExp = find.GetMethod("Find", new[] { typeof(string) })
                        ?.Invoke(null, new[] { str.Trim() }); //TODO Unreserved original format(indentation, etc.)
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
        public class Comment
        {
            public static Dictionary<String, Regex> Map = new Dictionary<String, Regex>();

            //TestingFunctionality
            static Comment()// Init to add Is field in every class implementing IStatement to the LineEndAndComment
                            // because Is field doesn't above LineEndAndComment and it's too difficult to add it in every class by hand.
            {
                Debug.WriteLine("Initing...");
                foreach (var classImplIStatement in classesImplIStatement)
                {
                    Exception initException = null;
                    try
                    {
                        var memberIs = classImplIStatement.GetProperty("Is", BindingFlags.Static | BindingFlags.NonPublic);
                        Map.Add(classImplIStatement.FullName, new Regex($"{memberIs.GetValue(null) as Regex}{FileSign.LineEndAndComment}"));
                    }
                    catch (Exception e)
                    {
                        initException = e;
                    }

                    if (initException != null)
                    {
                        Debug.Write(
                            $"Can't add to Is field in class {classImplIStatement} because of the {initException} excption.");
                    }
                }
            }
        }
        //TestingFunctionality
        static Statement() // Init to add Is field in every class implementing IStatement to the LineEndAndComment
                           // because Is field doesn't above LineEndAndComment and it's too difficult to add it in every class by hand.
        {

        }
    }
    public static class Value
    {
        public static Regex Is => new Regex(@".*");

        public static IValue Find(string str)
        {
            var finds = Expression.GetMethodsFromClass(typeof(IValue));
            foreach (var find in finds)
            {
                object structExp;
                Level findLevel = null;
                try
                {
                    if (findLevel == null)
                        structExp = find.GetMethod("Find", new[] { typeof(string) /*, typeof(Level) */})
                            .Invoke(null, new[] { str });
                    else
                        structExp = find.GetMethod("Find", new[] { typeof(string), typeof(Level) })
                            .Invoke(null, new object[] { str, findLevel });
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
    //BUG Do not implement IStatement becuase implementing the IStatement interface means participating in a Regex Expansion of statement for all classes that implement the IStatement interface, and it causes a bug, for unknown reason.
    //BUG The empty line may disrupt the order of output Lines. Maybe it's because of the nested parentheses's match pattern of Regex.        
    /* input:
    using System
    namespace Cspnet.FirstApp
    main = (){
        var i = 0 + 1

        Console.WriteLine(i)
    }
    */
    /* output:
    using System;
    namespace Cspnet{class FirstApp{static void Main(){ 
        var i = 0 + 1;
        Console.WriteLine(i);

    }}}
    */
    /*
    class EmptyStatement// : IStatement
    {
        static Regex Is => new Regex(@"\s*");

        public static EmptyStatement Find(string str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            return new EmptyStatement
            {
                str = str
            };
        }

        public string StatementToCS()
        {
            return str;
        }

        string str;
    }
    */
    /*
    class FuncLiteral : IValue
    {
        class FuncDeclare
        {
            //f = (a : int) -> int{
            //    return a + 1
            //}
            // (int) -> int
            //(a : str, b : str) -> 

            public static Regex Is
            {
                get
                {
                    var para = $"(?<FuncLiteral_FuncDeclare_ParameterName>{LocalVaribleName.Is}) ?: ?(?<FuncLiteral_FuncDeclare_ParameterType>{MemberName.Is})";
                    var paras = Element.Element.GetTailLoopRegex(para, ",");
                    return new Regex($@"\({paras}\)");
                }
            }

            public static FuncDeclare Find(String str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                var nameCaptures = match.Groups["FuncLiteral_FuncDeclare_ParameterName"].Captures;
                var typeCaptures = match.Groups["FuncLiteral_FuncDeclare_ParameterType"].Captures;
                var parameters = new List<(LocalVaribleName, MemberName)>();
                for (var i = 0; i < nameCaptures.Count; i++)
                                  //typeCaptures.Count
                {
                    parameters.Add((new LocalVaribleName(nameCaptures[i].ToString()), new MemberName(typeCaptures[i].ToString())));
                }

                Action a = () => { };
                return new FuncDeclare
                {
                    _parameters = parameters.ToArray()
                };

            }
            /*
            Lambda:
                Csp.net     f({})
                C#          f(() => {})
            (Local) Function:
                Csp.net     f = {}
                C#          void f(){}

            Local Function is more special than Lambda becuase
            the value of AssignStatemennt, FuncCallStatement and ReturnStatement can be Lambda and only
            VarStatement can be Local Function.
            *//*
    public string ValueToCSInVarStatement() //Translate to Local Function
            {

            }
            public string ValueToCS() //Translate to Lambda
            {
                if (_parameters.Length == 0)
                    return "()";
                //Lambda () => {}
                if (_parameters.Length == 1)
                    return _parameters[0].name.ToString();
                //Lambda a => {}
                string replace_str = _parametersValue[0].ValueToCS();
                for (int i = 1; i < _parametersValue.Length; i++)
                {
                    replace_str += "," + _parametersValue[i].ValueToCS();
                }
                return $"({replace_str})";
            }

            (LocalVaribleName name, MemberName type)[] _parameters;
            /*
            LocalVaribleName[] _parametersVarible;
            MemberName[] _parametersType;*//*
        }
        public static Level level = new Level(15);
        static Regex Is
        {
            get
            {
                var classname = MemberName.Is;
                var methodname = LocalVaribleName.Is;
                var value = Value.Is;

                var paras = FuncDeclare.Is;
                var operandOrClassname = $"(((?<FuncCallStatement_OperandValue>{value})|(?<FuncCallStatement_ClassName>{classname}))\\.(?<FuncCallStatement_FuncName>{methodname}))";
                //(operandvalue).funcname(parameters)
                //      IValue.funcname(parameters)
                //classname.funcname(parameters)
                //      MemberName.funcname(parameters)
                //NOTE If operandvalue is a VaribleName, VaribleName.Is == MemberName.Is. The Regex Engine may choose the first one that is operandvalue.
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
            var parameters = FuncDeclare.Find(match.Groups["FuncCallStatement_ActualParameters"].ToString());
            if (operandValue == null && funcValue == null || parameters == null)
                return null;

            return new FuncDeclare
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
        FuncDeclare parameters;
    }
    */

    class ReturnStatement : IStatement
    {
        static Regex Is => GetIs();

        public static Regex GetIs(string ReturnValue = "ReturnValue")
        {
            return new Regex($"return (?<{ReturnValue}>{Value.Is})?");
        }

        public static ReturnStatement Find(string str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var returnValueStr = match.Groups["ReturnValue"].ToString();
            if (returnValueStr == "")
                return new ReturnStatement
                {
                    valueStr = ""
                };
            var returnValue = Value.Find(returnValueStr);

            if (returnValue == null)
                return null;

            return new ReturnStatement
            {
                valueStr = returnValue.ValueToCS()
            };
        }

        public string StatementToCS()
        {
            return "return " + valueStr + ";";
        }

        string valueStr;
    }

    class IncrementDecrementOperator : IStatement, IValue
    {
        public static Level level = new Level(12);
        static Regex Is => GetIs();

        public static Regex GetIs(string Prefix = "Prefix", string Postfix = "Postfix", string Operand = "Operand")
        {
            var inOrDe = @"(\+\+|\-\-)";
            var prefix = $"(?<{Prefix}>{inOrDe})";
            var postfix = $"(?<{Postfix}>{inOrDe})";
            return new Regex($"{prefix}? ?(?<{Operand}>{VaribleName.Is}) ?(?({Prefix})|{postfix})");
        }

        public static IncrementDecrementOperator Find(string str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }

        public static IncrementDecrementOperator Find(string str)
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

        string str;

        (string Prefix, string Postfix) oper;
        VaribleName operand;
    }

    class PlusMinusOperator : IValue
    {
        public static Level level = new Level(14);
        static Regex Is => GetIs();

        public static Regex GetIs(string PreOperand = "PreOperand", string PostOperand = "PostOperand",
            string Operator = "Operator")
        {
            var PlOrMi = @"(\+|\-)";
            return new Regex($"(?<{PreOperand}>{Value.Is}) ?(?<{Operator}>{PlOrMi}) ?(?<{PostOperand}>{Value.Is})");
        }

        public static PlusMinusOperator Find(string str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }

        public static PlusMinusOperator Find(string str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;
            var operand = (Value.Find(match.Groups["PreOperand"].ToString().Trim()),
                Value.Find(match.Groups["PostOperand"].ToString().Trim()));
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

        string str;
        string oper;
    }

    class TimesDivOperator : IValue
    {
        public static Level level = new Level(13);
        static Regex Is => GetIs();

        public static Regex GetIs(string PreOperand = "PreOperand", string PostOperand = "PostOperand",
            string Operator = "Operator")
        {
            var PlOrMi = @"(\*|/)";
            return new Regex($"(?<{PreOperand}>{Value.Is}) ?(?<{Operator}>{PlOrMi}) ?(?<{PostOperand}>{Value.Is})");
        }

        public static TimesDivOperator Find(string str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }

        public static TimesDivOperator Find(string str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;
            var operand = (Value.Find(match.Groups["PreOperand"].ToString().Trim()),
                Value.Find(match.Groups["PostOperand"].ToString().Trim()));
            if (operand.Item1 == null || operand.Item2 == null)
                return null;

            return new TimesDivOperator()
            {
                str = str,
                oper = match.Groups["Operator"].ToString(),
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

        string str;
        string oper;
    }

    class FuncCallStatement : IStatement, IValue
    {
        class ActualParameters
        {
            public static Regex Is =>
                new Regex(
                    $"({Regex.GetTailLoopRegex($"(?<FuncCallStatement_ActualParameters_Value>{Value.Is})", ",")})?");

            public static ActualParameters Find(string str)
            {
                var match = Is.MatchesAll(str);
                if (match == null)
                    return null;

                if (match.Groups["FuncCallStatement_ActualParameters_Value"].Captures.IsAllEmpty()) // Count?
                {
                    return new ActualParameters
                    {
                        _parametersValue = new IValue[] { }
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

                return new ActualParameters
                {
                    _parametersValue = parametersValue.ToArray()
                };
            }

            public string ValueToCS()
            {
                if (_parametersValue.Length == 0)
                    return "()";
                string replace_str = _parametersValue[0].ValueToCS();
                for (int i = 1; i < _parametersValue.Length; i++)
                {
                    replace_str += "," + _parametersValue[i].ValueToCS();
                }

                return $"({replace_str})";
            }

            IValue[] _parametersValue;
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
                var operandOrClassname =
                    $"(((?<FuncCallStatement_OperandValue>{value})|(?<FuncCallStatement_ClassName>{classname}))\\.(?<FuncCallStatement_FuncName>{methodname}))";
                //(operandvalue).funcname(parameters)
                //      IValue.funcname(parameters)
                //classname.funcname(parameters)
                //      MemberName.funcname(parameters)
                //NOTE If operandvalue is a VaribleName, VaribleName.Is == MemberName.Is. The Regex Engine may choose the first one that is operandvalue.
                var funcValue = $"(?<FuncCallStatement_FuncValue>{value})";

                //(funcvalue)(parameters)
                //      IValue(parameters)
                return new Regex(
                    $"({operandOrClassname}|{funcValue})\\((?<FuncCallStatement_ActualParameters>{paras})\\)");
            }
        }

        public static FuncCallStatement Find(string str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }

        public static FuncCallStatement Find(string str)
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
                replaceStr = operandValue?.ValueToCS() +
                             "${FuncCallStatement_ClassName}.${FuncCallStatement_FuncName}";
            else
                replaceStr = funcValue?.ValueToCS();
            return Is.Replace(str, replaceStr + parameters.ValueToCS());
        }

        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }

        string str;

        IValue operandValue;
        MemberName className;
        LocalVaribleName funcName;
        IValue funcValue;
        ActualParameters parameters;
    }

    public class AssignmentStatement : IStatement, IValue
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

        public static AssignmentStatement Find(string str, Level alrFindLv)
        {
            if (level <= alrFindLv)
                return null;

            return Find(str);
        }

        public static AssignmentStatement Find(string str)
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

        string str;

        VaribleName AssignVarible;
        IValue AssignValue;
    }

    public class VarStatement : IStatement
    {
        static Regex Is
        {
            get
            {
                var varname = LocalVaribleName.Is;
                var vartypename = MemberName.Is;
                var assign = $"( ?= ?(?<VarStatement_Value>{Value.Is}))";
                return new Regex(
                    $"var (?<VarStatement_VarName>{varname})( ?: ?(?<VarStatement_Type>{vartypename}))?(?(VarStatement_Type){assign}?|{assign})");
            }
        }

        //var (?<LocalVaribleName>{strname})(: (?<Type>{nspname}))? (?(Type){assign}?|{assign})
        public static VarStatement Find(string str)
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
            string isValue;
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