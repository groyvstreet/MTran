using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class FunctionNode : ExpressionNode
    {
        public Token Function { get; set; }
        public List<Token> Parameters { get; set; }
        public ExpressionNode Body { get; set; }

        public FunctionNode(Token function, List<Token> parameters, ExpressionNode body)
        {
            Function = function;
            Parameters = parameters;
            Body = body;
        }
    }
}
