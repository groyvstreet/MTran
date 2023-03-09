using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class IndexerNode : ExpressionNode
    {
        public ExpressionNode Variable { get; set; }
        public ExpressionNode Index { get; set; }

        public IndexerNode(ExpressionNode variable, ExpressionNode index)
        {
            Variable = variable;
            Index = index;
        }
    }
}
