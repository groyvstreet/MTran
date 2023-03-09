namespace Lab2.Models
{
    public class Token
    {
        public string Identifier { get; set; }
        public string Type { get; set; }

        public Token(string identifier, string type)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}
