using Lab2.Models;

namespace Lab3.Models
{
    public class UnaryOperationNode : ExpressionNode
    {
        public Token Operator { get; set; }
        public ExpressionNode Operand { get; set; }

        public UnaryOperationNode(Token @operator, ExpressionNode operand)
        {
            Operator = @operator;
            Operand = operand;
        }
    }
}
