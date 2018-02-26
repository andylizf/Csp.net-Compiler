using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using RegexGrammar.Expression.Operation;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Element;

namespace RegexGrammar.Expression
{ 
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
                    Console.WriteLine($"IExpression {structExp.GetType()} {structExp}");
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
                    Console.WriteLine($"IValue {structExp.GetType()} {structExp}");
#endif
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
    class IncrementDecrementOperator : OperatorExpression, IStatement, IValue
    {
        static Level level = new Level(14);
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
            if (level >= alrFindLv)
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
                match = match,
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
        Match match;
        String str;

        (String Prefix, String Postfix) oper;
        VaribleName operand;
    }
    class FuncCallExpression : OperatorExpression, IStatement, IValue
    {
        static Regex Is = GetIs();
        static Regex GetIs(String OperandValue = "OperandValue", String ClassName = "ClassName",
        String FuncName = "FuncName", String FuncValue = "FuncValue", String Parameters = "Parameters")
        {
            var classname = MemberName.Is;
            var methodname = LocalVaribleName.Is;
            var value = Value.Is;

            var paras = ParametersCall.GetIs(Parameters);
            var operandOrClassname = $"(((?<{OperandValue}>{value})|(?<{ClassName}>{classname}))\\.(?<{FuncName}>{methodname}))";
            //(operandvalue).funcname(parameters)
            //      IValue.funcname(parameters)
            //classname.funcname(parameters)
            //      MemberName.funcname(parameters)
            // TODO: If operandvalue is a VaribleName, VaribleName.Is == MemberName.Is. The Regex Engine may choose the first one that is operandvalue.
            var funcValue = $"(?<{FuncValue}>{value})";
                
            //(funcvalue)(parameters)
            //      IValue(parameters)
            return new Regex($"({operandOrClassname}|{funcValue})\\({paras}\\)"/*, RegexOptions.RightToLeft*/);
        }
        public static FuncCallExpression Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var operandValue = Value.Find(match.Groups["OperandValue"].ToString());
            var funcValue = Value.Find(match.Groups["FuncValue"].ToString());
            var parameters = ParametersCall.Find(match.Groups["Parameters"].ToString());
            if ((operandValue == null && funcValue == null) || parameters == null)
                return null;

            return new FuncCallExpression()
            {
                match = match,
                str = str,
                operandValue = operandValue,
                className = new MemberName(match.Groups["ClassName"].ToString()),
                funcName = new LocalVaribleName(match.Groups["FuncName"].ToString()),
                funcValue = funcValue,
                parameters = parameters
            };
        }

        public string ValueToCS()
        {
            return Is.Replace(str, "(?(FuncName)" + operandValue?.ValueToCS() + "${ClassName}.${FuncName}|" + funcValue.ValueToCS() + ")\\(" + parameters.ValueToCS() + "\\)");
        }
        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }

        Match match;
        String str;

        IValue operandValue;
        MemberName className;
        LocalVaribleName funcName;
        IValue funcValue;
        ParametersCall parameters;
    }
    
    class ParametersCall
    {
        static Regex Is = GetIs();
        public static Regex GetIs(String ParametersValue = "ParametersValue")
        {
            return new Regex($"({Element.Element.GetRegexLikeABA(",", $"(?<{ParametersValue}>{Value.Is})")})?");
        }
        public static ParametersCall Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var parametersValue = new List<IValue>();
            foreach (Capture capture in match.Groups["ParametersValue"].Captures)
            {
                var parameterValue = Value.Find(capture.ToString());
                if (parameterValue == null)
                    return null;
                parametersValue.Add(parameterValue);
            }

            return new ParametersCall()
            {
                match = match,
                str = str,
                parametersValue = parametersValue.ToArray()
            };
        }
        public string ValueToCS()
        {
            if (parametersValue.Length == 0)
                return "()";
            string replace_str = parametersValue[0].ValueToCS();
            foreach(var value in parametersValue)
            {
                replace_str += "," + value.ValueToCS();
            }
            return $"({replace_str})";
        }

        Match match;
        String str;

        IValue[] parametersValue;
    }

    class AssignmentOperatorExpression : OperatorExpression, IStatement, IValue
    {
        static Regex Is = GetIs();
        public static Regex GetIs(String AssignVarible = "AssignVarible", String AssignValue = "AssignValue")
        {
            var first = $"(?<{AssignVarible}>{VaribleName.Is})";
            var end = $"(?<{AssignValue}>{Value.Is})";
            return new Regex(first + " ?= ?" + end);
        }
        public static AssignmentOperatorExpression Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var assignValue = Value.Find(match.Groups["AssignValue"].ToString());
            if (assignValue == null)
                return null;

            return new AssignmentOperatorExpression()
            {
                match = match,
                str = str,
                AssignVarible = new VaribleName(match.Groups["AssignVarible"].ToString()),
                AssignValue = assignValue
            };
        }

        public string ValueToCS()
        {
            Is.Replace(str, "${AssignVarible} = " + AssignValue.ValueToCS());
            return str;
        }
        public string StatementToCS()
        {
            return ValueToCS() + ";";
        }
        Match match;
        String str;

        VaribleName AssignVarible;
        IValue AssignValue;
    }
    class VarExpression : IStatement
    {
        static Regex Is = GetIs();
        //var (?<LocalVaribleName>{strname})(: (?<Type>{nspname}))? (?(Type){assign}?|{assign})
        public static Regex GetIs(String VarVaribleName = "LocalVaribleName", String Type = "Type", String ValueExpression = "ValueExpression")
        {
            var varname = LocalVaribleName.Is;
            var vartypename = MemberName.Is;
            var assign = $"( ?= ?(?<{ValueExpression}>{Value.Is}))";
            return new Regex($"var (?<{VarVaribleName}>{varname})(( )?:( )?(?<{Type}>{vartypename}))? (?({Type}){assign}?|{assign})");
        }
        public static VarExpression Find(String str)
        {
            var match = Is.MatchesAll(str);
            if (match == null)
                return null;

            var varValue = Value.Find(match.Groups["ValueExpression"].ToString());
            if (varValue == null)
                return null;

            return new VarExpression()
            {
                str = str,
                match = match,
                varName = new LocalVaribleName(match.Groups["LocalVaribleName"].ToString()),
                varType = new MemberName(match.Groups["Type"].ToString()),
                varValue = varValue
            };
        }

        public string StatementToCS()
        {
            String TypeOrVar;
            if (varType.Name != "")
                TypeOrVar = varType.Name;
            else TypeOrVar = "var";

            if (varValue != null)
                Is.Replace(str, TypeOrVar + " ${LocalVaribleName} = " + varValue.ValueToCS() + ";");
            return str;
        }
        String str;
        Match match;

        LocalVaribleName varName;
        MemberName varType;
        IValue varValue;
    }
}
