using Lab2.Models;

namespace Lab3.Models
{
    internal class BinaryOperationNode : ExpressionNode
    {
        public Token Operator { get; set; }
        public ExpressionNode LeftNode { get; set; }
        public ExpressionNode RightNode { get; set; }

        public BinaryOperationNode(Token @operator, ExpressionNode leftNode, ExpressionNode rightNode)
        {
            Operator = @operator;
            LeftNode = leftNode;
            RightNode = rightNode;
        }
    }
}
