namespace Lab3.Models
{
    public class IfNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Body { get; set; }
        public ExpressionNode? ElseBody { get; set; }

        public IfNode(ExpressionNode condition, ExpressionNode body, ExpressionNode? elseBody)
        {
            Condition = condition;
            Body = body;
            ElseBody = elseBody;
        }
    }
}
