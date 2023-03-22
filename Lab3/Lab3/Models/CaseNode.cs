using Lab2.Models;

namespace Lab3.Models
{
    public class CaseNode : ExpressionNode
    {
        public Token Literal { get; set; }

        public CaseNode(Token literal)
        {
            Literal = literal;
        }
    }
}
