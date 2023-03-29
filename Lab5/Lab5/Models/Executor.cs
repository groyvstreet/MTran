using Lab2.Models;
using Lab3.Models;
using Lab4.Models;

namespace Lab5.Models
{
    internal class Executor
    {
        private ExpressionNode Root { get; set; }
        private Dictionary<string, Dictionary<string, object>> VariablesTables { get; set; } = new();
        private Semantic Semantic { get; set; }
        private int BlockIndex { get; set; }
        private Dictionary<string, string> FunctionsBlocks { get; set; } = new();
        private bool IsInFor { get; set; }
        private string Block { get; set; }
        private int Level { get; set; }
        private int Name { get; set; }

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
            IsInFor = false;
            Block = "-1";
            Level = -1;
            Name = -1;
        }

        public void ExecuteCode()
        {
            ExecuteNode(Root);
        }

        private object? ExecuteNode(ExpressionNode? expressionNode)
        {
            if (expressionNode == null)
            {
                return null;
            }

            if (expressionNode is StatementsNode node)
            {
                if (!IsInFor)
                {
                    Level++;
                    Name++;

                    var temp = Block.Split(':');
                    temp[0] = Level.ToString();
                    Block = string.Empty;
                    Block += temp[0];

                    for (var i = 1; i < temp.Length; i++)
                    {
                        Block += $":{temp[i]}";
                    }

                    Block += $":{Name}";
                }
                else
                {
                    IsInFor = false;
                }

                foreach (var elem in node.Nodes)
                {
                    ExecuteNode(elem);
                }

                Level--;

                Block = Block.Remove(Block.Length - 2);

                var temp2 = Block.Split(':');
                temp2[0] = Level.ToString();
                Block = string.Empty;
                Block += temp2[0];

                for (var i = 1; i < temp2.Length; i++)
                {
                    Block += $":{temp2[i]}";
                }
            }

            if (expressionNode is FunctionNode functionNode)
            {
                if (functionNode.Function.Identifier == "main")
                {
                    ExecuteNode(functionNode.Body);
                }
                else
                {
                    RunNode(functionNode.Body);
                }

                return null;
            }

            if (expressionNode is WhileNode whileNode)
            {
                return null;
            }

            if (expressionNode is IfNode ifNode)
            {
                var condition = ExecuteNode(ifNode.Condition) as bool?;

                if (condition == true)
                {
                    ExecuteNode(ifNode.Body);
                }
                else
                {
                    ExecuteNode(ifNode.ElseBody);
                }

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
                        while (block != "-1")
                        {
                            if (VariablesTables[block].ContainsKey(variable.Variable.Identifier))
                            {
                                VariablesTables[block][variable.Variable.Identifier] = type switch
                                {
                                    "int" => int.Parse(Console.ReadLine()!),
                                    "double" => double.Parse(Console.ReadLine()!),
                                    "char" => char.Parse(Console.ReadLine()!),
                                    "bool" => bool.Parse(Console.ReadLine()!),
                                    _ => Console.ReadLine()!,
                                };
                                break;
                            }
                            else
                            {
                                block = block.Remove(block.Length - 2);

                                var temp = block.Split(':');
                                temp[0] = (int.Parse(temp[0]) - 1).ToString();
                                block = string.Empty;
                                block += temp[0];

                                for (var i = 1; i < temp.Length; i++)
                                {
                                    block += $":{temp[i]}";
                                }
                            }
                        }
                    }

                    if (parameter is BinaryOperationNode binaryOperation)
                    {
                        var left = binaryOperation.LeftNode as VariableNode;
                        var index = ExecuteNode(binaryOperation.RightNode) as int?;

                        while (block != "-1")
                        {
                            if (VariablesTables[block].ContainsKey(left!.Variable.Identifier))
                            {
                                break;
                            }
                            else
                            {
                                block = block.Remove(block.Length - 2);

                                var temp = block.Split(':');
                                temp[0] = (int.Parse(temp[0]) - 1).ToString();
                                block = string.Empty;
                                block += temp[0];

                                for (var i = 1; i < temp.Length; i++)
                                {
                                    block += $":{temp[i]}";
                                }
                            }
                        }

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
                Level++;
                Name++;

                var temp = Block.Split(':');
                temp[0] = Level.ToString();
                Block = string.Empty;
                Block += temp[0];

                for (var i = 1; i < temp.Length; i++)
                {
                    Block += $":{temp[i]}";
                }

                Block += $":{Name}";

                ExecuteNode(forNode.First);

                while (true)
                {
                    var condition = ExecuteNode(forNode.Second) as bool?;

                    if (condition != null)
                    {
                        if (condition == false)
                        {
                            Level--;

                            Block = Block.Remove(Block.Length - 2);

                            var temp2 = Block.Split(':');
                            temp2[0] = Level.ToString();
                            Block = string.Empty;
                            Block += temp2[0];

                            for (var i = 1; i < temp2.Length; i++)
                            {
                                Block += $":{temp2[i]}";
                            }

                            break;
                        }
                    }

                    IsInFor = true;
                    var level = Level;
                    var name = Name;
                    var block = Block;
                    ExecuteNode(forNode.Body);

                    Level = level;
                    Name = name;
                    Block = block;

                    ExecuteNode(forNode.Third);
                }

                Name--;

                RunNode(forNode);

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
                            while (block != "-1")
                            {
                                if (VariablesTables[block].ContainsKey(variable.Variable.Identifier))
                                {
                                    break;
                                }
                                else
                                {
                                    block = block.Remove(block.Length - 2);

                                    var temp = block.Split(':');
                                    temp[0] = (int.Parse(temp[0]) - 1).ToString();
                                    block = string.Empty;
                                    block += temp[0];

                                    for (var i = 1; i < temp.Length; i++)
                                    {
                                        block += $":{temp[i]}";
                                    }
                                }
                            }

                            VariablesTables[block][variable.Variable.Identifier] = ExecuteNode(binaryOperationNode.RightNode);
                        }

                        if (binaryOperationNode.LeftNode is BinaryOperationNode binaryOperation)
                        {
                            var left = binaryOperation.LeftNode as VariableNode;
                            var index = ExecuteNode(binaryOperation.RightNode) as int?;

                            while (block != "-1")
                            {
                                if (VariablesTables[block].ContainsKey(left.Variable.Identifier))
                                {
                                    break;
                                }
                                else
                                {
                                    block = block.Remove(block.Length - 2);

                                    var temp = block.Split(':');
                                    temp[0] = (int.Parse(temp[0]) - 1).ToString();
                                    block = string.Empty;
                                    block += temp[0];

                                    for (var i = 1; i < temp.Length; i++)
                                    {
                                        block += $":{temp[i]}";
                                    }
                                }
                            }

                            var type3 = Semantic.GetReturnType(binaryOperation.LeftNode);

                            if (type3 == "int")
                            {
                                (VariablesTables[block][left.Variable.Identifier] as List<int>)[int.Parse(index.ToString())] = int.Parse((ExecuteNode(binaryOperationNode.RightNode) as int?).ToString());
                            }
                            else if (type3 == "double")
                            {
                                (VariablesTables[block][left.Variable.Identifier] as List<double>)[int.Parse(index.ToString())] = double.Parse((ExecuteNode(binaryOperationNode.RightNode) as double?).ToString());
                            }
                            else if (type3 == "char")
                            {
                                (VariablesTables[block][left.Variable.Identifier] as List<char>)[int.Parse(index.ToString())] = char.Parse((ExecuteNode(binaryOperationNode.RightNode) as char?).ToString());
                            }
                            else if (type3 == "bool")
                            {
                                (VariablesTables[block][left.Variable.Identifier] as List<bool>)[int.Parse(index.ToString())] = bool.Parse((ExecuteNode(binaryOperationNode.RightNode) as bool?).ToString());
                            }
                            else
                            {
                                (VariablesTables[block][left.Variable.Identifier] as List<string>)[int.Parse(index.ToString())] = ExecuteNode(binaryOperationNode.RightNode) as string;
                            }
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
                                        var intRight = ExecuteNode(binaryOperationNode.RightNode) as int?;
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
                                        var doubleRight = ExecuteNode(binaryOperationNode.RightNode) as double?;
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
                                        var charRight = ExecuteNode(binaryOperationNode.RightNode) as char?;
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
                                        var intRight = ExecuteNode(binaryOperationNode.RightNode) as int?;
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
                                        var doubleRight = ExecuteNode(binaryOperationNode.RightNode) as double?;
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
                                        var charRight = ExecuteNode(binaryOperationNode.RightNode) as char?;
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
                                        var intRight = ExecuteNode(binaryOperationNode.RightNode) as int?;
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
                                        var doubleRight = ExecuteNode(binaryOperationNode.RightNode) as double?;
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
                                        var charRight = ExecuteNode(binaryOperationNode.RightNode) as char?;
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

                                var boolRight = ExecuteNode(binaryOperationNode.RightNode) as bool?;
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
                                var stringRight = ExecuteNode(binaryOperationNode.RightNode) as string;

                                return binaryOperationNode.Operator.Identifier switch
                                {
                                    "==" => stringLeft == stringRight,
                                    "!=" => stringLeft != stringRight,
                                    "<" => stringLeft.CompareTo(stringRight) < 0 ? true : false,
                                    ">" => stringLeft.CompareTo(stringRight) > 0 ? true : false,
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
                if (unaryOperationNode.Operator.Identifier == "++")
                {
                    var variable = unaryOperationNode.Operand as VariableNode;
                    var block = GetBlock();

                    if (variable.Variable.Type == "int")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as int?)! + 1;
                    }

                    if (variable.Variable.Type == "double")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as double?)! + 1;
                    }

                    if (variable.Variable.Type == "char")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as char?)! + 1;
                    }
                }

                if (unaryOperationNode.Operator.Identifier == "--")
                {
                    var variable = unaryOperationNode.Operand as VariableNode;
                    var block = GetBlock();

                    if (variable.Variable.Type == "int")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as int?)! - 1;
                    }

                    if (variable.Variable.Type == "double")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as double?)! - 1;
                    }

                    if (variable.Variable.Type == "char")
                    {
                        VariablesTables[block][variable.Variable.Identifier] = (VariablesTables[block][variable.Variable.Identifier] as char?)! - 1;
                    }
                }

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

                while (block != "-1")
                {
                    if (VariablesTables[block].ContainsKey(variableNode.Variable.Identifier))
                    {
                        return VariablesTables[block][variableNode.Variable.Identifier];
                    }
                    else
                    {
                        block = block.Remove(block.Length - 2);

                        var temp = block.Split(':');
                        temp[0] = (int.Parse(temp[0]) - 1).ToString();
                        block = string.Empty;
                        block += temp[0];

                        for (var i = 1; i < temp.Length; i++)
                        {
                            block += $":{temp[i]}";
                        }
                    }
                }
            }

            if (expressionNode is VariableTypeNode)
            {
                return null;
            }

            return null;
        }

        private string GetBlock()
        {
            return Block;

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
                Level++;
                Name++;

                var temp = Block.Split(':');
                temp[0] = (int.Parse(temp[0]) + 1).ToString();
                Block = string.Empty;
                Block += temp[0];

                for (var i = 1; i < temp.Length; i++)
                {
                    Block += $":{temp[i]}";
                }

                Block += $":{Name}";

                foreach (var elem in node.Nodes)
                {
                    RunNode(elem);
                }

                Level--;

                Block = Block.Remove(Block.Length - 2);

                var temp2 = Block.Split(':');
                temp2[0] = Level.ToString();
                Block = string.Empty;
                Block += temp2[0];

                for (var i = 1; i < temp2.Length; i++)
                {
                    Block += $":{temp2[i]}";
                }
            }

            if (expressionNode is ForNode forNode)
            {
                RunNode(forNode.Body);
            }

            if (expressionNode is IfNode ifNode)
            {
                RunNode(ifNode.Body);
            }

            if (expressionNode is WhileNode whileNode)
            {
                RunNode(whileNode.Body);
            }
        }
    }
}
