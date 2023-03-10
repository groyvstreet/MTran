using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class IfNode : ExpressionNode
    {
        public ExpressionNode Condition { get; set; }
        public ExpressionNode Body { get; set; }

        public IfNode(ExpressionNode condition, ExpressionNode body)
        {
            Condition = condition;
            Body = body;
        }
    }
}
