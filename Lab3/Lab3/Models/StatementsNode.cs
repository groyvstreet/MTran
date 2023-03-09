namespace Lab3.Models
{
    internal class StatementsNode : ExpressionNode
    {
        public List<ExpressionNode> Nodes { get; set; } = new();

        public void AddNode(ExpressionNode node)
        {
            Nodes.Add(node);
        }
    }
}
