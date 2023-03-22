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
                CheckNode(whileNode.Body);
            }

            if (expressionNode is IfNode ifNode)
            {
                CheckNode(ifNode.Body);
                CheckNode(ifNode.ElseBody);
            }

            if (expressionNode is CoutNode coutNode)
            {
                return;
            }

            if (expressionNode is CinNode cinNode)
            {
                return;
            }

            if (expressionNode is ForNode forNode)
            {
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
                    throw new Exception("Выражение в 'switch' должно быть типа int или char");
                }

                CheckNode(switchNode.Body);
            }

            if (expressionNode is CaseNode caseNode)
            {
                if (caseNode.Literal.Type != "int literal" && caseNode.Literal.Type != "char literal")
                {
                    throw new Exception("После 'case' ожидается int literal или char literal");
                }
            }

            if (expressionNode is KeyWordNode keyWordNode)
            {
                return;
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                var returnType1 = GetReturnType(binaryOperationNode.LeftNode);
                var returnType2 = GetReturnType(binaryOperationNode.RightNode);

                if (returnType1 != returnType2)
                {
                    if (returnType1 == "string" || returnType1 == "bool" ||
                        returnType2 == "string" || returnType2 == "bool")
                    {
                        throw new Exception($"Невозможно выполнить операцию {binaryOperationNode.Operator.Identifier} для {returnType1} и {returnType2}");
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

            if (expressionNode is LiteralNode literalNode)
            {
                return;
            }

            if (expressionNode is VariableNode variableNode)
            {
                return;
            }

            if (expressionNode is VariableTypeNode variableTypeNode)
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
                        return GetReturnType(binaryOperationNode.LeftNode);
                    }

                    if (returnType1 == "string" || returnType1 == "bool" ||
                        returnType2 == "string" || returnType2 == "bool")
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
