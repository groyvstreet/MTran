namespace Lab3.Models
{
    public class WhileNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Body { get; set; }

        public WhileNode(ExpressionNode condition, ExpressionNode body)
        {
            Condition = condition;
            Body = body;
        }
    }
}
