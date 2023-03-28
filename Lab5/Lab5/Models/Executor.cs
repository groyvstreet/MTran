using Lab2.Models;
using Lab3.Models;

namespace Lab5.Models
{
    internal class Executor
    {
        private ExpressionNode Root { get; set; }
        //private Dictionary<string, List<Token>> Functions { get; set; } = new();

        public Executor(ExpressionNode root)
        {
            Root = root;
        }

        public void RunCode()
        {
            RunNode(Root);
        }

        private void RunNode(ExpressionNode expressionNode)
        {
            if (expressionNode == null)
            {
                return;
            }

            if (expressionNode is StatementsNode node)
            {
                foreach (var elem in node.Nodes)
                {
                    RunNode(elem);
                }
            }

            if (expressionNode is FunctionNode functionNode)
            {
                return;
            }

            if (expressionNode is WhileNode whileNode)
            {
                return;
            }

            if (expressionNode is IfNode ifNode)
            {
                return;
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
                return;
            }

            if (expressionNode is FunctionExecutionNode functionExecutionNode)
            {
                return;
            }

            if (expressionNode is SwitchNode switchNode)
            {

                return;
            }

            if (expressionNode is CaseNode caseNode)
            {
                return;
            }

            if (expressionNode is KeyWordNode)
            {
                return;
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                return;
            }

            if (expressionNode is UnaryOperationNode unaryOperationNode)
            {
                return;
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
    }
}
