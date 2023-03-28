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

                    var param = ExecuteNode(parameter) as string;

                    VariablesTables[block][param!] = type switch
                    {
                        "int" => int.Parse(Console.ReadLine()!),
                        "double" => double.Parse(Console.ReadLine()!),
                        "char" => char.Parse(Console.ReadLine()!),
                        _ => Console.ReadLine()!,
                    };

                    if (param is string str)
                    {
                        param = str.Replace("\"", "").Replace("\\n", "\n");
                    }

                    Console.Write(param);
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
    }
}
