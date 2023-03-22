using Lab2.Models;

namespace Lab3.Models
{
    public class CoutNode : ExpressionNode
    {
        public List<ExpressionNode> Parameters { get; set; }

        public CoutNode(List<ExpressionNode> parameters)
        {
            Parameters = parameters;
        }
    }
}
