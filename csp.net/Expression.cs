using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using RegexGrammar.Expression.Operation;
using RegexGrammar.Element.RegexGrammar.Name;
using RegexGrammar.Element;

namespace RegexGrammar
{
    namespace Expression
    {
        abstract class Expression
        {
            internal abstract String ToCS();
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
        interface IValue{}
        interface IStatement{}
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
        abstract class OperatorExpression : Expression, IValue
        {
            // a++ a-- ++a --a a = b
        }
        abstract class UnaryOperatorExpression : OperatorExpression
        {
            protected abstract IValue Operand
            {
                get;
                set;
            }
        }
        class IncrementDecrementOperator : UnaryOperatorExpression, IStatement
        {
            static Level level = new Level(14);

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

                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);

                return new IncrementDecrementOperator()
                {
                    oper = (match.Groups["Prefix"].ToString(), match.Groups["Postfix"].ToString()),
                    Operand = new VaribleName(match.Groups["Operand"].ToString())
                };
            }

            internal override string ToCS()
            {
                throw new NotImplementedException();
            }

            (String Prefix, String Postfix) oper;
            protected override IValue Operand
            {
                get;
                set;
            }
        }
        class FuncCallExpression : UnaryOperatorExpression, IStatement
        {
            static Regex GetIs(String OperandValue = "OperandValue", String FuncValue = "FuncValue",
            String ClassName = "ClassName", String Parameters = "Parameters")
            {
                var classname = MemberName.Is;
                var methodname = LocalVaribleName.Is;
                var value = Value.Is;
                var paras = ParametersCall.GetIs(Parameters);

                //(operandvalue).funcname(parameters)
                //  IValue.funcname(parameters)
                //(funcvalue)(parameters)
                //  IValue(parameters)
                //classname.funcname(parameters)
                //  MemberName.funcname(parameters)
                return new Regex($"[(?<OperandValueOrAllClassName>{value}).(?<FuncName>{methodname})|(?<FuncValue>{value})]\\({paras}\\)");
            }
            public static FuncCallExpression Find(String str)
            {
                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
                var operandValue = Value.Find(match.Groups["OperandValueOrAllClassName"].ToString());
                var funcValue = Value.Find(match.Groups["FuncValue"].ToString());
                if (operandValue == null || funcValue == null)
                    return null;

                return new FuncCallExpression()
                {
                    name = match.Groups["ClassName"],
                    varType = match.Groups["Parameters"],
                    operandValue = operandValue,
                    funcValue = funcValue
                };
            }
            public IValue operandValue;
            public ClassAllName className;
            public IValue OperandValueOrAllClassName;
            public Parameters parameters;
        }
        abstract class BinaryOperatorExpression : OperatorExpression
        {
            public static abstract Regex GetIs(String, String);
            IValue PreOperand;
            IValue PostOperand;
        }
        class AssignmentOperatorExpression : BinaryOperatorExpression, IStatement
        {
            public static Regex GetIs(String AssignVarible = "AssignVarible", String AssignValue = "AssignValue")
            {
                var first = $"(?<{AssignVarible}>{MemberAllName.Is})";
                var end = $"(?<{AssignValue}>{Value.Is})";
                return new Regex(first + " ?= ?" + end);
            }
            public static AssignmentOperatorExpression Find(String str)
            {
                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
                var assignValue = Value.Find(match.Groups["AssignValue"]);
                if (operandValue == null)
                    return null;

                return new AssignmentOperatorExpression()
                {
                    AssignVarible = match.Groups["AssignVarible"],
                    AssignValue = assignValue
                };
            }
            MemberAllName AssignVarible;
            IValue AssignValue;
        }
        class VarExpression : Expression
        {
            //var (?<VaribleName>{strname}(: (?<Type>{nspname}))? (?(Type){assign}?|{assign})
            public static Regex GetIs(String VaribleName = "VaribleName", String Type = "Type")
            {
                var strname = StrongName.Is;
                var nspname = NamespaceAllName.Is;
                var assign = $"( ?= ?(?<ValueExpression>{Value.Is}))";
                return new Regex($"var (?<{VaribleName}>{strname}(( )?:( )?(?<{Type}>{nspname}))? (?({Type}){assign}?|{assign})");
            }
            public static VarExpression Find(String str)
            {
                var Is = GetIs();
                if (!Is.IsMatch(str))
                    return null;

                var match = Is.Match(str);
                var varValue = Value.Find(match.Groups["ValueExpression"]);
                if (varValue == null)
                    return null;

                return new VarExpression()
                {
                    varName = match.Groups["VaribleName"],
                    varType = match.Groups["Type"],
                    varValue = varValue
                };
            }

            VaribleName varName;
            ClassAllName varType;
            IValue varValue;
        }
    }
}

