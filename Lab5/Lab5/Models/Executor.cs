using Lab2.Models;
using Lab3.Models;
using Lab4.Models;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace Lab5.Models
{
    internal class Executor
    {
        private ExpressionNode Root { get; set; }
        private Dictionary<string, Dictionary<string, object>> VariablesTables { get; set; } = new();
        private Semantic Semantic { get; set; }
        private int BlockIndex { get; set; }
        private Dictionary<string, string> FunctionsBlocks { get; set; } = new();

        public Executor(ExpressionNode root, Dictionary<string, Dictionary<string, string>> variablesTables, Semantic semantic)
        {
            Root = root;

            foreach (var block in variablesTables.Keys)
            {
                VariablesTables.Add(block, new());

                foreach (var variable in variablesTables[block].Keys)
                {
                    VariablesTables[block].Add(variable, new());
                }
            }

            BlockIndex = -1;
            Semantic = semantic;
        }

        public void ExecuteCode()
        {
            ExecuteNode(Root);
        }

        private object? ExecuteNode(ExpressionNode expressionNode)
        {
            if (expressionNode == null)
            {
                return null;
            }

            if (expressionNode is StatementsNode node)
            {
                BlockIndex++;

                foreach (var elem in node.Nodes)
                {
                    ExecuteNode(elem);
                }
            }

            if (expressionNode is FunctionNode functionNode)
            {
                RunNode(functionNode.Body);

                if (functionNode.Function.Identifier == "main")
                {
                    ExecuteNode(functionNode.Body);
                }

                return null;
            }

            if (expressionNode is WhileNode whileNode)
            {
                return null;
            }

            if (expressionNode is IfNode ifNode)
            {
                return null;
            }

            if (expressionNode is CoutNode coutNode)
            {
                foreach (var parameter in coutNode.Parameters)
                {
                    var param = ExecuteNode(parameter);

                    if (param is string str)
                    {
                        param = str.Replace("\"", "").Replace("\\n", "\n");
                    }

                    Console.Write(param);
                }

                return null;
            }

            if (expressionNode is CinNode cinNode)
            {
                foreach (var parameter in cinNode.Parameters)
                {
                    var block = GetBlock();

                    var type = Semantic.GetReturnType(parameter);

                    if (parameter is VariableNode variable)
                    {
                        VariablesTables[block][variable.Variable.Identifier] = type switch
                        {
                            "int" => int.Parse(Console.ReadLine()!),
                            "double" => double.Parse(Console.ReadLine()!),
                            "char" => char.Parse(Console.ReadLine()!),
                            "bool" => bool.Parse(Console.ReadLine()!),
                            _ => Console.ReadLine()!,
                        };
                    }

                    if (parameter is BinaryOperationNode binaryOperation)
                    {
                        var left = binaryOperation.LeftNode as VariableNode;
                        var index = ExecuteNode(binaryOperation.RightNode) as int?;

                        if (type == "int")
                        {
                            (VariablesTables[block][left!.Variable.Identifier] as List<int>)![int.Parse(index.ToString()!)] = int.Parse(Console.ReadLine()!);
                        }
                        else if (type == "double")
                        {
                            (VariablesTables[block][left!.Variable.Identifier] as List<double>)![int.Parse(index.ToString()!)] = double.Parse(Console.ReadLine()!);
                        }
                        else if (type == "char")
                        {
                            (VariablesTables[block][left!.Variable.Identifier] as List<char>)![int.Parse(index.ToString()!)] = char.Parse(Console.ReadLine()!);
                        }
                        else if (type == "bool")
                        {
                            (VariablesTables[block][left!.Variable.Identifier] as List<bool>)![int.Parse(index.ToString()!)] = bool.Parse(Console.ReadLine()!);
                        }
                        else
                        {
                            (VariablesTables[block][left!.Variable.Identifier] as List<string>)![int.Parse(index.ToString()!)] = Console.ReadLine()!;
                        }
                    }
                }

                return null;
            }

            if (expressionNode is ForNode forNode)
            {
                return null;
            }

            if (expressionNode is FunctionExecutionNode functionExecutionNode)
            {
                return null;
            }

            if (expressionNode is SwitchNode switchNode)
            {

                return null;
            }

            if (expressionNode is CaseNode caseNode)
            {
                return null;
            }

            if (expressionNode is KeyWordNode keyWordNode)
            {
                switch (keyWordNode.KeyWord.Identifier)
                {
                    case "endl":
                        return "\n";
                }

                return null;
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                switch (binaryOperationNode.Operator.Identifier)
                {
                    case "=":
                        var block = GetBlock();

                        if (binaryOperationNode.LeftNode is VariableNode variable)
                        {
                            VariablesTables[block][variable.Variable.Identifier] = ExecuteNode(binaryOperationNode.RightNode);
                        }

                        if (binaryOperationNode.LeftNode is BinaryOperationNode binaryOperation)
                        {
                            var left = binaryOperation.LeftNode as VariableNode;
                            var index = ExecuteNode(binaryOperation.RightNode) as int?;

                            (VariablesTables[block][left.Variable.Identifier] as List<object>)[int.Parse(index.ToString())] = ExecuteNode(binaryOperationNode.RightNode);
                        }
                        break;
                    case "==":
                    case "!=":
                    case "<":
                    case ">":
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        var leftType = Semantic.GetReturnType(binaryOperationNode.LeftNode);
                        var rightType = Semantic.GetReturnType(binaryOperationNode.RightNode);

                        switch (leftType)
                        {
                            case "int":
                                var intLeft = ExecuteNode(binaryOperationNode.LeftNode) as int?;
                                switch (rightType)
                                {
                                    case "int":
                                        var intRight = ExecuteNode(binaryOperationNode.LeftNode) as int?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => intLeft == intRight,
                                            "!=" => intLeft != intRight,
                                            "<" => intLeft < intRight,
                                            ">" => intLeft > intRight,
                                            "+" => intLeft + intRight,
                                            "-" => intLeft - intRight,
                                            "*" => intLeft * intRight,
                                            "/" => intLeft / intRight,
                                        };
                                    case "double":
                                        var doubleRight = ExecuteNode(binaryOperationNode.LeftNode) as double?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => intLeft == doubleRight,
                                            "!=" => intLeft != doubleRight,
                                            "<" => intLeft < doubleRight,
                                            ">" => intLeft > doubleRight,
                                            "+" => intLeft + doubleRight,
                                            "-" => intLeft - doubleRight,
                                            "*" => intLeft * doubleRight,
                                            "/" => intLeft / doubleRight,
                                        };
                                    case "char":
                                        var charRight = ExecuteNode(binaryOperationNode.LeftNode) as char?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => intLeft == charRight,
                                            "!=" => intLeft != charRight,
                                            "<" => intLeft < charRight,
                                            ">" => intLeft > charRight,
                                            "+" => intLeft + charRight,
                                            "-" => intLeft - charRight,
                                            "*" => intLeft * charRight,
                                            "/" => intLeft / charRight,
                                        };
                                }
                                break;
                            case "double":
                                var doubleLeft = ExecuteNode(binaryOperationNode.LeftNode) as double?;
                                switch (rightType)
                                {
                                    case "int":
                                        var intRight = ExecuteNode(binaryOperationNode.LeftNode) as int?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => doubleLeft == intRight,
                                            "!=" => doubleLeft != intRight,
                                            "<" => doubleLeft < intRight,
                                            ">" => doubleLeft > intRight,
                                            "+" => doubleLeft + intRight,
                                            "-" => doubleLeft - intRight,
                                            "*" => doubleLeft * intRight,
                                            "/" => doubleLeft / intRight,
                                        };
                                    case "double":
                                        var doubleRight = ExecuteNode(binaryOperationNode.LeftNode) as double?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => doubleLeft == doubleRight,
                                            "!=" => doubleLeft != doubleRight,
                                            "<" => doubleLeft < doubleRight,
                                            ">" => doubleLeft > doubleRight,
                                            "+" => doubleLeft + doubleRight,
                                            "-" => doubleLeft - doubleRight,
                                            "*" => doubleLeft * doubleRight,
                                            "/" => doubleLeft / doubleRight,
                                        };
                                    case "char":
                                        var charRight = ExecuteNode(binaryOperationNode.LeftNode) as char?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => doubleLeft == charRight,
                                            "!=" => doubleLeft != charRight,
                                            "<" => doubleLeft < charRight,
                                            ">" => doubleLeft > charRight,
                                            "+" => doubleLeft + charRight,
                                            "-" => doubleLeft - charRight,
                                            "*" => doubleLeft * charRight,
                                            "/" => doubleLeft / charRight,
                                        };
                                }
                                break;
                            case "char":
                                var charLeft = ExecuteNode(binaryOperationNode.LeftNode) as char?;
                                switch (rightType)
                                {
                                    case "int":
                                        var intRight = ExecuteNode(binaryOperationNode.LeftNode) as int?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => charLeft == intRight,
                                            "!=" => charLeft != intRight,
                                            "<" => charLeft < intRight,
                                            ">" => charLeft > intRight,
                                            "+" => charLeft + intRight,
                                            "-" => charLeft - intRight,
                                            "*" => charLeft * intRight,
                                            "/" => charLeft / intRight,
                                        };
                                    case "double":
                                        var doubleRight = ExecuteNode(binaryOperationNode.LeftNode) as double?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => charLeft == doubleRight,
                                            "!=" => charLeft != doubleRight,
                                            "<" => charLeft < doubleRight,
                                            ">" => charLeft > doubleRight,
                                            "+" => charLeft + doubleRight,
                                            "-" => charLeft - doubleRight,
                                            "*" => charLeft * doubleRight,
                                            "/" => charLeft / doubleRight,
                                        };
                                    case "char":
                                        var charRight = ExecuteNode(binaryOperationNode.LeftNode) as char?;
                                        return binaryOperationNode.Operator.Identifier switch
                                        {
                                            "==" => charLeft == charRight,
                                            "!=" => charLeft != charRight,
                                            "<" => charLeft < charRight,
                                            ">" => charLeft > charRight,
                                            "+" => charLeft + charRight,
                                            "-" => charLeft - charRight,
                                            "*" => charLeft * charRight,
                                            "/" => charLeft / charRight,
                                        };
                                }
                                break;
                            case "bool":
                                var boolLeft = ExecuteNode(binaryOperationNode.LeftNode) as bool?;
                                var boolToIntLeft = 0;

                                if (boolLeft == true)
                                {
                                    boolToIntLeft = 1;
                                }

                                var boolRight = ExecuteNode(binaryOperationNode.LeftNode) as bool?;
                                var boolToIntRight = 0;

                                if (boolRight == true)
                                {
                                    boolToIntRight = 1;
                                }

                                return binaryOperationNode.Operator.Identifier switch
                                {
                                    "==" => boolToIntLeft == boolToIntRight,
                                    "!=" => boolToIntLeft != boolToIntRight,
                                    "<" => boolToIntLeft < boolToIntRight,
                                    ">" => boolToIntLeft > boolToIntRight,
                                    "+" => boolToIntLeft + boolToIntRight,
                                    "-" => boolToIntLeft - boolToIntRight,
                                    "*" => boolToIntLeft * boolToIntRight,
                                    "/" => boolToIntLeft / boolToIntRight,
                                };
                            case "string":
                                var stringLeft = ExecuteNode(binaryOperationNode.LeftNode) as string;
                                var stringRight = ExecuteNode(binaryOperationNode.LeftNode) as string;

                                return binaryOperationNode.Operator.Identifier switch
                                {
                                    "==" => stringLeft == stringRight,
                                    "!=" => stringLeft != stringRight,
                                    "<" => throw new Exception("Невозможно применить оператор '<' для string и string"),
                                    ">" => throw new Exception("Невозможно применить оператор '>' для string и string"),
                                    "+" => stringLeft + stringRight,
                                    "-" => throw new Exception("Невозможно применить оператор '-' для string и string"),
                                    "*" => throw new Exception("Невозможно применить оператор '*' для string и string"),
                                    "/" => throw new Exception("Невозможно применить оператор '/' для string и string"),
                                };
                        }
                        break;
                    case "new":
                        var type = Semantic.GetReturnType(binaryOperationNode.LeftNode).Replace("*", "");
                        
                        switch (type)
                        {
                            case "int":
                                var intCapacity = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                var intList = new List<int>(int.Parse(intCapacity.ToString()!));

                                for (var i = 0; i < intCapacity; i++)
                                {
                                    intList.Add(0);
                                }

                                return intList;
                            case "double":
                                var doubleCapacity = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                var doubleList = new List<double>(int.Parse(doubleCapacity.ToString()!));

                                for (var i = 0; i < doubleCapacity; i++)
                                {
                                    doubleList.Add(0);
                                }

                                return doubleList;
                            case "char":
                                var charCapacity = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                var charList = new List<char>(int.Parse(charCapacity.ToString()!));

                                for (var i = 0; i < charCapacity; i++)
                                {
                                    charList.Add('0');
                                }

                                return charList;
                            case "string":
                                var stringCapacity = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                var stringList = new List<string>(int.Parse(stringCapacity.ToString()!));

                                for (var i = 0; i < stringCapacity; i++)
                                {
                                    stringList.Add("");
                                }

                                return stringList;
                        }
                        break;
                    case "[]":
                        var type2 = Semantic.GetReturnType(binaryOperationNode.LeftNode).Replace("*", "");
                        
                        switch (type2)
                        {
                            case "int":
                                var intList = ExecuteNode(binaryOperationNode.LeftNode) as List<int>;
                                var intIndex = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                return intList![int.Parse(intIndex.ToString()!)];
                            case "double":
                                var doubleList = ExecuteNode(binaryOperationNode.LeftNode) as List<double>;
                                var doubleIndex = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                return doubleList![int.Parse(doubleIndex.ToString()!)];
                            case "char":
                                var charList = ExecuteNode(binaryOperationNode.LeftNode) as List<char>;
                                var charIndex = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                return charList![int.Parse(charIndex.ToString()!)];
                            case "string":
                                var stringList = ExecuteNode(binaryOperationNode.LeftNode) as List<string>;
                                var stringIndex = ExecuteNode(binaryOperationNode.RightNode) as int?;
                                return stringList![int.Parse(stringIndex.ToString()!)];
                        }
                        break;
                }

                return null;
            }

            if (expressionNode is UnaryOperationNode unaryOperationNode)
            {
                return null;
            }

            if (expressionNode is LiteralNode literalNode)
            {
                return literalNode.Literal.Type switch
                {
                    "int literal" => int.Parse(literalNode.Literal.Identifier),
                    "double literal" => double.Parse(literalNode.Literal.Identifier),
                    "char literal" => char.Parse(literalNode.Literal.Identifier),
                    _ => literalNode.Literal.Identifier,
                };
            }

            if (expressionNode is VariableNode variableNode)
            {
                var block = GetBlock();

                return VariablesTables[block][variableNode.Variable.Identifier];
            }

            if (expressionNode is VariableTypeNode)
            {
                return null;
            }

            return null;
        }

        private string GetBlock()
        {
            var index = 0;

            foreach (var key in VariablesTables.Keys)
            {
                if (index == BlockIndex)
                {
                    return key;
                }

                index++;
            }

            return string.Empty;
        }

        private void RunNode(ExpressionNode expressionNode)
        {
            if (expressionNode == null)
            {
                return;
            }

            if (expressionNode is StatementsNode node)
            {
                BlockIndex++;

                foreach (var elem in node.Nodes)
                {
                    RunNode(elem);
                }
            }
        }

        private object Assign(ExpressionNode expressionNode)
        {
            var block = GetBlock();

            if (expressionNode is VariableNode variableNode)
            {
                return VariablesTables[block][variableNode.Variable.Identifier];
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                var type = Semantic.GetReturnType(binaryOperationNode.LeftNode);

                if (type == "int")
                {
                    var variable = Assign(binaryOperationNode.LeftNode) as List<object>;
                    var index = ExecuteNode(binaryOperationNode.RightNode) as int?;
                    return variable![int.Parse(index.ToString()!)];
                }
            }

            return new();
        }
    }
}
