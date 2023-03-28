using Lab2.Models;
using Lab3.Models;

namespace Lab5.Models
{
    internal class Executor
    {
        private ExpressionNode Root { get; set; }
        private Dictionary<string, Dictionary<string, string>> VariablesTables { get; set; }
        private int BlockIndex { get; set; }
        //private Dictionary<string, List<Token>> Functions { get; set; } = new();

        public Executor(ExpressionNode root, Dictionary<string, Dictionary<string, string>> variablesTables)
        {
            Root = root;
            VariablesTables = variablesTables;
            BlockIndex = -1;
        }

        public void RunCode()
        {
            RunNode(Root);
        }

        private object? RunNode(ExpressionNode expressionNode)
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
                    RunNode(elem);
                }

                BlockIndex--;
            }

            if (expressionNode is FunctionNode functionNode)
            {
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
                return null;
            }

            if (expressionNode is CinNode cinNode)
            {
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

            if (expressionNode is KeyWordNode)
            {
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

            if (expressionNode is LiteralNode)
            {
                return null;
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
    }
}
