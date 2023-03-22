using Lab2.Models;

namespace Lab3.Models
{
    public class VariableNode : ExpressionNode
    {
        public Token Variable { get; set; }

        public VariableNode(Token variable)
        {
            Variable = variable;
        }
    }
}
