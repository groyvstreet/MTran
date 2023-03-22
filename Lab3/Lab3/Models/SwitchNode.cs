using Lab2.Models;

namespace Lab3.Models
{
    public class SwitchNode : ExpressionNode
    {
        public Token Variable { get; set; }
        public ExpressionNode Body { get; set; }

        public SwitchNode(Token variable, ExpressionNode body)
        {
            Variable = variable;
            Body = body;
        }
    }
}
