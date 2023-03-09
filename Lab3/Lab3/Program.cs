using Lab2.Models;
using Lab3.Models;
using System.Net.Security;

namespace Lab3
{
    internal class Program
    {
        static void PrintTab(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                Console.Write("\t");
            }
        }

        static void PrintNode(ExpressionNode expressionNode, int level = 0)
        {
            if (expressionNode is StatementsNode node)
            {
                foreach (var elem in node.Nodes)
                {
                    PrintNode(elem, level + 1);
                }
            }

            if (expressionNode is FunctionNode functionNode)
            {
                PrintTab(level);
                Console.WriteLine(functionNode.Function.Identifier);
                PrintTab(level + 1);

                foreach (var elem in functionNode.Parameters)
                {
                    Console.Write(elem.Identifier);
                    Console.Write(" ");
                }

                Console.WriteLine();

                PrintNode(functionNode.Body, level + 1);
            }

            if (expressionNode is WhileNode whileNode)
            {
                PrintTab(level);
                Console.WriteLine("while");
                PrintNode(whileNode.Condition, level + 1);
                PrintNode(whileNode.Body, level + 1);
            }

            if (expressionNode is CoutNode coutNode)
            {
                PrintTab(level);
                Console.WriteLine("cout");

                foreach (var elem in coutNode.Parameters)
                {
                    PrintNode(elem, level + 1);
                }
            }

            if (expressionNode is CinNode cinNode)
            {
                PrintTab(level);
                Console.WriteLine("cin");

                foreach (var elem in cinNode.Parameters)
                {
                    PrintNode(elem, level + 1);
                }
            }

            if (expressionNode is BinaryOperationNode binaryOperationNode)
            {
                PrintNode(binaryOperationNode.LeftNode, level + 1);
                PrintTab(level);
                Console.WriteLine(binaryOperationNode.Operator.Identifier);
                PrintNode(binaryOperationNode.RightNode, level + 1);
            }

            if (expressionNode is UnaryOperationNode unaryOperationNode)
            {
                PrintTab(level);
                Console.WriteLine(unaryOperationNode.Operator.Identifier);
                PrintNode(unaryOperationNode.Operand, level + 1);
            }

            if (expressionNode is LiteralNode literalNode)
            {
                PrintTab(level);
                Console.WriteLine(literalNode.Literal.Identifier);
            }

            if (expressionNode is VariableNode variableNode)
            {
                PrintTab(level);
                Console.WriteLine(variableNode.Variable.Identifier);
            }

            if (expressionNode is VariableTypeNode variableTypeNode)
            {
                PrintTab(level);
                Console.WriteLine(variableTypeNode.VariableType.Identifier);
            }
        }

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

            PrintNode(root);
        }
    }
}
