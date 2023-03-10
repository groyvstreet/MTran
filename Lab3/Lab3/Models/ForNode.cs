using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class ForNode : ExpressionNode
    {
        public ExpressionNode First { get; set; }
        public ExpressionNode Second { get; set; }
        public ExpressionNode Third { get; set; }
        public ExpressionNode Body { get; set; }

        public ForNode(ExpressionNode first, ExpressionNode second, ExpressionNode third, ExpressionNode body)
        {
            First = first;
            Second = second;
            Third = third;
            Body = body;
        }
    }
}
