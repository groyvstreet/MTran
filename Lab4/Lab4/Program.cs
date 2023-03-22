using Lab2.Models;
using Lab3.Models;
using Lab4.Models;

namespace Lab4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = "Program1.cpp";

            using var reader = new StreamReader(path!);
            string codeText = reader.ReadToEnd();
            reader.Close();

            var lexer = new Lexer(path, codeText);

            lexer.GetTokens();

            if (lexer.IsError)
            {
                Console.WriteLine(lexer.ErrorMessage);
                return;
            }

            var parser = new Parser(lexer, lexer.Tokens);

            var root = parser.ParseCode();

            Lab3.Program.PrintNode(root);

            var semantic = new Semantic(root);

            semantic.CheckCode();
        }
    }
}
