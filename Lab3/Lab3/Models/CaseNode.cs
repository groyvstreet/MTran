using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class CaseNode : ExpressionNode
    {
        public ExpressionNode LiteralNode { get; set; }

        public CaseNode(ExpressionNode literalNode)
        {
            LiteralNode = literalNode;
        }
    }
}
