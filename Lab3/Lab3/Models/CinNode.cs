namespace Lab3.Models
{
    public class CinNode : ExpressionNode
    {
        public List<ExpressionNode> Parameters { get; set; }

        public CinNode(List<ExpressionNode> parameters)
        {
            Parameters = parameters;
        }
    }
}
