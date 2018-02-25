using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using RegexGrammar.Expression.Operation;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Element;

namespace RegexGrammar
{
    namespace Expression
    {
        static class Expression
        {
            public static IEnumerable<Type> GetMethodsFromClass(Type interfaceType)
            {
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    var intef = type.GetInterface(interfaceType.Name);
                    if(intef != null)
                        yield return intef;
                }
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
        interface IClassType
        {
            string TypeToCS();
        }
        static class Statement
        {
            public static IStatement Find(String str)
            {
                var Finds = Expression.GetMethodsFromClass(typeof(IStatement));
                foreach (var find in Finds)
                {
                    var structExp = find.GetMethod("Find", new[] { typeof(String), typeof(Level) }).Invoke(null, new[] { str }) as IStatement;
                    if (structExp != null)
                        return structExp;
                }
                return null;
            }
        }
        static class Value
        {
            public static Regex Is = new Regex(@"\S*");
            public static IValue Find(String str)
            {
                var Finds = Expression.GetMethodsFromClass(typeof(IValue));
                foreach (var find in Finds)
                {
                    var structExp = find.GetMethod("Find", new[] { typeof(String), typeof(Level) }).Invoke(null, new[] { str }) as IValue;
                    if (structExp != null)
                        return structExp;
                }
                return null;
            }
        }
        static class ClassType
        {
            public static Regex Is = new Regex($"({MemberName.Is}| |->|\\(\\))*");
            public static IValue Find(String str)
            {
                var Finds = Expression.GetMethodsFromClass(typeof(IValue));
                foreach (var find in Finds)
                {
                    var structExp = find.GetMethod("Find", new[] { typeof(String), typeof(Level) }).Invoke(null, new[] { str }) as IValue;
                    if (structExp != null)
                        return structExp;
                }
                return null;
            }
        }
        class FuncTypeExpression : IClassType
        {
            public static Regex GetIs(String parameters, String returntype)
            {
                return new Regex($"((?<>{ClassType.Is}) ?-> ?");
            }
            public string TypeToCS()
            {
                throw new NotImplementedException();
            }
        }
        abstract class OperatorExpression
        {
            // a++ a-- ++a --a a = b
        }
        abstract class UnaryOperatorExpression : OperatorExpression
        {
        }
        class IncrementDecrementOperator : UnaryOperatorExpression, IStatement, IValue
        {
            static Level level = new Level(14);
            static Regex Is = GetIs();
            public static Regex GetIs(String Prefix = "Prefix", String Postfix = "Postfix", String Operand = "Operand")
            {
                var inOrDe = "[++|--]";
                var prefix = $"(?<{Prefix}>{inOrDe})";
                var postfix = $"(?<{Postfix}>{inOrDe})";
                return new Regex($"{prefix}?(?<{Operand}>{VaribleName.Is})(?({Prefix}){postfix}|)");
            }
            public static IncrementDecrementOperator Find(String str, Level alrFindLv)
            {
                if (level >= alrFindLv)
                    return null;
                
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);

                return new IncrementDecrementOperator()
                {
                    match = match,
                    str = str,
                    oper = (match.Groups["Prefix"].ToString(), match.Groups["Postfix"].ToString()),
                    operand = new VaribleName(match.Groups["Operand"].ToString())
                };
            }

            string StatementToCS()
            {
                return ValueToCS() + ";";
            }
            string ValueToCS()
            {
                return Is.Replace(str, "${Prefix}" + operand.ToCS() + "${Postfix}");
            }
            Match match;
            String str;

            (String Prefix, String Postfix) oper;
            VaribleName operand;
        }
        class FuncCallExpression : OperatorExpression, IStatement
        {
            static Regex GetIs(String OperandValue = "OperandValue", String ClassName = "ClassName",
            String FuncName = "FuncName", String FuncValue = "FuncValue", String Parameters = "Parameters")
            {
                var classname = MemberName.Is;
                var methodname = LocalVaribleName.Is;
                var value = Value.Is;

                var paras = ParametersCall.GetIs(Parameters);
                var operandOrClassname = $"([(?<{OperandValue}>{value})|(?<{ClassName}>{classname})].(?<{FuncName}>{methodname}))";
                //(operandvalue).funcname(parameters)
                //      IValue.funcname(parameters)
                //classname.funcname(parameters)
                //      MemberName.funcname(parameters)
                // TODO: If operandvalue is a VaribleName, VaribleName.Is == MemberName.Is. The Regex Engine may choose the first one that is operandvalue.
                var funcValue = $"(?<{FuncValue}>{value})";
                
                //(funcvalue)(parameters)
                //      IValue(parameters)
                return new Regex($"[{operandOrClassname}|{funcValue}]\\({paras}\\)");
            }
            public static FuncCallExpression Find(String str)
            {
                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
                var operandValue = Value.Find(match.Groups["OperandValueOrAllClassName"].ToString());
                var funcValue = Value.Find(match.Groups["FuncValue"].ToString());
                if (operandValue == null && funcValue == null)
                    return null;

                return new FuncCallExpression()
                {
                    match = match,
                    str = str,
                    operandValue = operandValue,
                    className = new MemberName(match.Groups["ClassName"].ToString()),
                    funcName = new LocalVaribleName(match.Groups["FuncName"].ToString()),
                    funcValue = funcValue,
                    parameters = new ParametersCall(match.Groups["Parameters"].ToString())
                };
            }

            string IValue.ToCS()
            {
                return GetIs().Replace(str, "(?(FuncName)" + operandValue?.ValueToCS() + "${ClassName}.${FuncName}|" + funcValue.ValueToCS() + ")\\(" + parameters.ToCS() + "\\)");
            }
            string IStatement.StatementToCS()
            {
                return GetIs().Replace(str, "(?(FuncName)" + operandValue?.ValueToCS() + "${ClassName}.${FuncName}|" + funcValue.ValueToCS() + ")\\(" + parameters.ToCS() + "\\)");
            }

            Match match;
            String str;
            IValue operandValue;
            MemberName className;
            LocalVaribleName funcName;
            IValue funcValue;
            ParametersCall parameters;
        }
        abstract class BinaryOperatorExpression : OperatorExpression
        {
            IValue PreOperand;
            IValue PostOperand;
        }
        class AssignmentOperatorExpression : BinaryOperatorExpression, IStatement
        {
            public static Regex GetIs(String AssignVarible = "AssignVarible", String AssignValue = "AssignValue")
            {
                var first = $"(?<{AssignVarible}>{VaribleName.Is})";
                var end = $"(?<{AssignValue}>{Value.Is})";
                return new Regex(first + " ?= ?" + end);
            }
            public static AssignmentOperatorExpression Find(String str)
            {
                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
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

            public override string StatementToCS()
            {
                var Is = GetIs();
                Is.Replace(str, "${AssignVarible} = " + AssignValue.ValueToCS() + ";");
                throw new NotImplementedException();
            }
            Match match;
            String str;

            VaribleName AssignVarible;
            IValue AssignValue;
        }
        class VarExpression : IStatement
        {
            static Regex Is = GetIs();
            //var (?<LocalVaribleName>{strname}(: (?<Type>{nspname}))? (?(Type){assign}?|{assign})
            public static Regex GetIs(String VarVaribleName = "LocalVaribleName", String Type = "Type")
            {
                var varname = LocalVaribleName.Is;
                var vartypename = MemberName.Is;
                var assign = $"( ?= ?(?<ValueExpression>{Value.Is}))";
                return new Regex($"var (?<{VarVaribleName}>{varname}(( )?:( )?(?<{Type}>{vartypename}))? (?({Type}){assign}?|{assign})");
            }
            public static VarExpression Find(String str)
            {
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
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
                
            }
            String str;
            Match match;

            LocalVaribleName varName;
            MemberName varType;
            IValue varValue;
        }
    }
}

