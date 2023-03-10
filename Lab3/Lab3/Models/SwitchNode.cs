using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class SwitchNode : ExpressionNode
    {
        public ExpressionNode VariableNode { get; set; }
        public ExpressionNode Body { get; set; }

        public SwitchNode(ExpressionNode variableNode, ExpressionNode body)
        {
            VariableNode = variableNode;
            Body = body;
        }
    }
}
