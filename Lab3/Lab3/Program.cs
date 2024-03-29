﻿using Lab2.Models;
using Lab3.Models;

namespace Lab3
{
    public class Program
    {
        static void PrintTab(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                Console.Write("\t");
            }
        }

        static public void PrintNode(ExpressionNode? expressionNode, int level = 0)
        {
            if (expressionNode == null)
            {
                return;
            }

            if (expressionNode is StatementsNode node)
            {
                foreach (var elem in node.Nodes)
                {
                    PrintNode(elem, level);
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

            if (expressionNode is IfNode ifNode)
            {
                PrintTab(level);
                Console.WriteLine("if");
                PrintNode(ifNode.Condition, level + 1);
                PrintNode(ifNode.Body, level + 1);
                PrintNode(ifNode.ElseBody, level + 1);
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

            if (expressionNode is ForNode forNode)
            {
                PrintTab(level);
                Console.WriteLine("for");

                PrintNode(forNode.First, level + 1);
                PrintNode(forNode.Second, level + 1);
                PrintNode(forNode.Third, level + 1);
                PrintNode(forNode.Body, level + 1);
            }

            if (expressionNode is FunctionExecutionNode functionExecutionNode)
            {
                PrintTab(level);
                Console.WriteLine(functionExecutionNode.Function.Identifier);

                foreach (var elem in functionExecutionNode.Parameters)
                {
                    PrintNode(elem, level + 1);
                }
            }

            if (expressionNode is SwitchNode switchNode)
            {
                PrintTab(level);
                Console.WriteLine("switch");
                PrintTab(level);
                Console.WriteLine(switchNode.Variable.Identifier);
                PrintNode(switchNode.Body, level + 1);
            }

            if (expressionNode is CaseNode caseNode)
            {
                PrintTab(level);
                Console.WriteLine("case");
                PrintTab(level);
                Console.WriteLine(caseNode.Literal.Identifier);
            }

            if (expressionNode is KeyWordNode keyWordNode)
            {
                PrintTab(level);
                Console.WriteLine(keyWordNode.KeyWord.Identifier);
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

            Console.WriteLine();
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
