using Lab2.Models;
using Lab3.Models;

namespace Lab4.Models
{
    internal class Semantic
    {
        private ExpressionNode Root { get; set; }
        private Dictionary<string, List<Token>> Functions { get; set; } = new();

        public Semantic(ExpressionNode root)
        {
            Root = root;
        }

        public void CheckCode()
        {
            CheckNode(Root);
        }

        private void CheckNode(ExpressionNode? expressionNode)
        {
            if (expressionNode == null)
            {
                return;
            }

            if (expressionNode is StatementsNode node)
            {
                foreach (var elem in node.Nodes)
                {
                    CheckNode(elem);
                }
            }

            if (expressionNode is FunctionNode functionNode)
            {
                Functions.Add(functionNode.Function.Identifier, functionNode.Parameters);
                CheckNode(functionNode.Body);
            }

            if (expressionNode is WhileNode whileNode)
            {
                CheckNode(whileNode.Condition);
                CheckNode(whileNode.Body);
            }

            if (expressionNode is IfNode ifNode)
            {
                CheckNode(ifNode.Condition);
                CheckNode(ifNode.Body);
                CheckNode(ifNode.ElseBody);
            }

            if (expressionNode is CoutNode coutNode)
            {
                var parameters = coutNode.Parameters;

                foreach (var parameter in parameters)
                {
                    CheckNode(parameter);
                }
            }

            if (expressionNode is CinNode cinNode)
            {
                var parameters = cinNode.Parameters;

                foreach (var parameter in parameters)
                {
                    if (!(parameter is BinaryOperationNode operation && operation.Operator.Identifier == "[]") &&
                        parameter is not VariableNode)
                    {
                        throw new Exception("В качестве параметра для 'cin' ожидается переменная");
                    }
                }
            }

            if (expressionNode is ForNode forNode)
            {
                CheckNode(forNode.First);
                CheckNode(forNode.Second);
                CheckNode(forNode.Third);
                CheckNode(forNode.Body);
            }

            if (expressionNode is FunctionExecutionNode functionExecutionNode)
            {
                var parameters = Functions[functionExecutionNode.Function.Identifier];
                var length1 = parameters.Count;
                var executionParameters = functionExecutionNode.Parameters;
                var length2 = executionParameters.Count;

                if (length1 != length2)
                {
                    throw new Exception($"Ожидается {length1} параметров, получено {length2}");
                }

                for (var i = 0; i < length1; i++)
                {
                    var returnType = GetReturnType(executionParameters[i]);

                    if (parameters[i].Type != returnType)
                    {
                        throw new Exception($"Ожидается {parameters[i].Type}, получено {returnType}");
                    }
                }
            }

            if (expressionNode is SwitchNode switchNode)
            {
       
                if (switchNode.Variable.Type != "int" && switchNode.Variable.Type != "char" &&
                    switchNode.Variable.Type != "bool")
                {
                    throw new Exception("Выражение в 'switch' должно быть типа int, char или bool");
                }

                CheckNode(switchNode.Body);
            }

            if (expressionNode is CaseNode caseNode)
            {
                if (caseNode.Literal.Type != "int literal" && caseNode.Literal.Type != "char literal" &&
                    caseNode.Literal.Type != "bool literal")
                {
                    throw new Exception("После 'case' ожидается int literal, char literal или bool literal");
                }
            }

            if (expressionNode is KeyWordNode)
            {
                return;
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                var returnType1 = GetReturnType(binaryOperationNode.LeftNode);
                var returnType2 = GetReturnType(binaryOperationNode.RightNode);

                if (returnType1 != returnType2)
                {
                    if ((returnType1 != "int" || returnType2 != "double") &&
                        (returnType1 != "int" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "double"))
                    {
                        if (binaryOperationNode.Operator.Identifier != "new" && binaryOperationNode.Operator.Identifier != "[]")
                        {
                            throw new Exception($"Невозможно выполнить операцию {binaryOperationNode.Operator.Identifier} для {returnType1} и {returnType2}");
                        }
                        else
                        {
                            if (returnType2 != "int" && returnType2 != "char")
                            {
                                throw new Exception($"Невозможно выполнить операцию {binaryOperationNode.Operator.Identifier} для {returnType1} и {returnType2}");
                            }
                        }
                    }
                }
                else
                {
                    if (binaryOperationNode.Operator.Identifier == "new" || binaryOperationNode.Operator.Identifier == "[]")
                    {
                        if (returnType1 != "int" && returnType2 != "char")
                        {
                            throw new Exception($"Невозможно выполнить операцию {binaryOperationNode.Operator.Identifier} для {returnType1} и {returnType2}");
                        }
                    }
                }
            }

            if (expressionNode is UnaryOperationNode unaryOperationNode)
            {
                var returnType = GetReturnType(unaryOperationNode.Operand);

                if (returnType == "string" || returnType == "bool")
                {
                    throw new Exception($"Невозможно выполнить операцию {unaryOperationNode.Operator.Identifier} для {returnType}");
                }
            }

            if (expressionNode is LiteralNode)
            {
                return;
            }

            if (expressionNode is VariableNode)
            {
                return;
            }

            if (expressionNode is VariableTypeNode)
            {
                return;
            }
        }

        private string GetReturnType(ExpressionNode expressionNode)
        {
            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                var returnType1 = GetReturnType(binaryOperationNode.LeftNode);
                var returnType2 = GetReturnType(binaryOperationNode.RightNode);

                if (returnType1 != returnType2)
                {
                    if ((binaryOperationNode.Operator.Identifier == "new" || binaryOperationNode.Operator.Identifier == "[]")
                        && returnType2 == "int")
                    {
                        if (binaryOperationNode.Operator.Identifier == "new")
                        {
                            return GetReturnType(binaryOperationNode.LeftNode) + "*";
                        }

                        var returnType3 = GetReturnType(binaryOperationNode.LeftNode);

                        if (returnType3.EndsWith('*'))
                        {
                            return returnType3.Remove(returnType3.Length - 1);
                        }

                        if (returnType3 == "string")
                        {
                            return "char";
                        }

                        throw new Exception($"Невозможно выполнить операцию [] для {returnType3}");
                    }

                    if ((returnType1 != "int" || returnType2 != "double") &&
                        (returnType1 != "int" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "double"))
                    {
                        throw new Exception($"Невозможно выполнить операцию {binaryOperationNode.Operator.Identifier} для {returnType1} и {returnType2}");
                    }

                    if (binaryOperationNode.Operator.Identifier == "+" || binaryOperationNode.Operator.Identifier == "-" ||
                        binaryOperationNode.Operator.Identifier == "*" || binaryOperationNode.Operator.Identifier == "/")
                    {
                        if (returnType1 == "double" || returnType2 == "double")
                        {
                            return "double";
                        }

                        return "int";
                    }

                    if (binaryOperationNode.Operator.Identifier == "==" || binaryOperationNode.Operator.Identifier == "!=" ||
                        binaryOperationNode.Operator.Identifier == "<" || binaryOperationNode.Operator.Identifier == ">")
                    {
                        return "int";
                    }

                    return GetReturnType(binaryOperationNode.LeftNode);
                }

                if (binaryOperationNode.Operator.Identifier == "==" || binaryOperationNode.Operator.Identifier == "!=" ||
                        binaryOperationNode.Operator.Identifier == "<" || binaryOperationNode.Operator.Identifier == ">")
                {
                    return "int";
                }

                if ((binaryOperationNode.Operator.Identifier == "new" || binaryOperationNode.Operator.Identifier == "[]")
                        && returnType2 == "int")
                {
                    if (binaryOperationNode.Operator.Identifier == "new")
                    {
                        return GetReturnType(binaryOperationNode.LeftNode) + "*";
                    }

                    var returnType3 = GetReturnType(binaryOperationNode.LeftNode);

                    if (returnType3.EndsWith('*'))
                    {
                        return returnType3.Remove(returnType3.Length - 1);
                    }

                    if (returnType3 == "string")
                    {
                        return "char";
                    }

                    throw new Exception($"Невозможно выполнить операцию [] для {returnType3}");
                }

                return returnType1;
            }

            if (expressionNode is UnaryOperationNode unaryOperationNode)
            {
                var returnType = GetReturnType(unaryOperationNode.Operand);

                if (returnType == "string" || returnType == "bool")
                {
                    throw new Exception($"Невозможно выполнить операцию {unaryOperationNode.Operator.Identifier} для {returnType}");
                }

                return returnType;
            }

            if (expressionNode is IfNode ifNode)
            {
                CheckNode(ifNode.Condition);
                CheckNode(ifNode.Body);
                CheckNode(ifNode.ElseBody);

                var returnType1 = GetReturnType(ifNode.Body);
                var returnType2 = GetReturnType(ifNode.ElseBody!);

                if (returnType1 != returnType2)
                {
                    if ((returnType1 != "int" || returnType2 != "double") &&
                        (returnType1 != "int" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "char") &&
                        (returnType1 != "double" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "int") &&
                        (returnType1 != "char" || returnType2 != "double"))
                    {
                        throw new Exception($"Несоответствие возвращаемых типов в тернарном операторе сравнения: {returnType1} и {returnType2}");
                    }
                }

                return returnType1;
            }

            if (expressionNode is VariableNode variableNode)
            {
                return variableNode.Variable.Type;
            }

            if (expressionNode is LiteralNode literalNode)
            {
                return literalNode.Literal.Type.Split()[0];
            }

            if (expressionNode is VariableTypeNode variableTypeNode)
            {
                return variableTypeNode.VariableType.Identifier;
            }

            return "none";
        }
    }
}
