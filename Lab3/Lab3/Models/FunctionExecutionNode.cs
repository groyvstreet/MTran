using Lab2.Models;

namespace Lab3.Models
{
    public class FunctionExecutionNode : ExpressionNode
    {
        public Token Function { get; set; }
        public List<ExpressionNode> Parameters { get; set; }

        public FunctionExecutionNode(Token function, List<ExpressionNode> parameters)
        {
            Function = function;
            Parameters = parameters;
        }
    }
}
