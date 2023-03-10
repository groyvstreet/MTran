using Lab2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Models
{
    internal class KeyWordNode : ExpressionNode
    {
        public Token KeyWord { get; set; }

        public KeyWordNode(Token keyWord)
        {
            KeyWord = keyWord;
        }
    }
}
