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
        public Token Literal { get; set; }

        public CaseNode(Token literal)
        {
            Literal = literal;
        }
    }
}
