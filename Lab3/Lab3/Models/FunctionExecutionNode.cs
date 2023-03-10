using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class FunctionExecutionNode : ExpressionNode
    {
        public Token Function { get; set; }
        public List<ExpressionNode> Parameters { get; set; }

        public FunctionExecutionNode(Token function, List<ExpressionNode> parameters)
        {
            Function = function;
            Parameters = parameters;
        }
    }
}
