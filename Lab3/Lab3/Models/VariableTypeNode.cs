using Lab2.Models;

namespace Lab3.Models
{
    public class VariableTypeNode : ExpressionNode
    {
        public Token VariableType { get; set; }

        public VariableTypeNode(Token variableType)
        {
            VariableType = variableType;
        }
    }
}
