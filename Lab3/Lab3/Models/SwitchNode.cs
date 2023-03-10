using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class SwitchNode : ExpressionNode
    {
        public Token Variable { get; set; }
        public ExpressionNode Body { get; set; }

        public SwitchNode(Token variable, ExpressionNode body)
        {
            Variable = variable;
            Body = body;
        }
    }
}
