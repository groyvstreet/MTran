using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class VariableTypeNode : ExpressionNode
    {
        public Token VariableType { get; set; }

        public VariableTypeNode(Token variableType)
        {
            VariableType = variableType;
        }
    }
}
