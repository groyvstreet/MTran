using Lab2.Models;

namespace Lab3.Models
{
    public class FunctionNode : ExpressionNode
    {
        public Token Function { get; set; }
        public List<Token> Parameters { get; set; }
        public ExpressionNode Body { get; set; }

        public FunctionNode(Token function, List<Token> parameters, ExpressionNode body)
        {
            Function = function;
            Parameters = parameters;
            Body = body;
        }
    }
}
