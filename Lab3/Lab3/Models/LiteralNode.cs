using Lab2.Models;

namespace Lab3.Models
{
    internal class LiteralNode : ExpressionNode
    {
        public Token Literal { get; set; }

        public LiteralNode(Token literal)
        {
            Literal = literal;
        }
    }
}
