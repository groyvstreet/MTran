namespace Lab2.Models
{
    internal class Token
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
