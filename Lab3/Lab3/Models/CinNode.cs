using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class CinNode : ExpressionNode
    {
        public List<ExpressionNode> Parameters { get; set; }

        public CinNode(List<ExpressionNode> parameters)
        {
            Parameters = parameters;
        }
    }
}
