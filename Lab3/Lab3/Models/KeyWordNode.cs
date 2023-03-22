using Lab2.Models;

namespace Lab3.Models
{
    public class KeyWordNode : ExpressionNode
    {
        public Token KeyWord { get; set; }

        public KeyWordNode(Token keyWord)
        {
            KeyWord = keyWord;
        }
    }
}
