using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class CoutNode : ExpressionNode
    {
        public List<ExpressionNode> Parameters { get; set; }

        public CoutNode(List<ExpressionNode> parameters)
        {
            Parameters = parameters;
        }
    }
}
