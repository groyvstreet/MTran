using Lab2.Models;

namespace Lab3.Models
{
    internal class VariableNode : ExpressionNode
    {
        public Token Variable { get; set; }

        public VariableNode(Token variable)
        {
            Variable = variable;
        }
    }
}
