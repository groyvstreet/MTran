using System.IO;
using System.Text.RegularExpressions;

namespace Lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = "Program1.cpp";

            using var reader = new StreamReader(path!);
            string codeText = reader.ReadToEnd();
            reader.Close();

            var variablesTypes = new List<string> { "int", "double", "char", "string", "string*", "void" };
            var variables = new Dictionary<string, string>();

            var keyWords1 = new Dictionary<string, string>
            {
                { "while", "key word" },
                { "for", "key word" },
                { "if", "key word" },
                { "switch", "key word" },
            };

            var keyWords2 = new Dictionary<string, string>
            {
                { "do", "key word" },
                { "else", "key word" },
            };

            var keyWords3 = new Dictionary<string, string>
            {
                { "continue", "key word" },
                { "break", "key word" },
            };

            var keyWords4 = new Dictionary<string, string>
            {
                { "case", "key word" },
            };

            var keyWords5 = new Dictionary<string, string>
            {
                { "default", "key word" },
            };

            var keyWords6 = new Dictionary<string, string>
            {
                { "cout", "key word" },
                { "cin", "key word" },
                { "new", "key word" },
                { "endl", "key word" },
            };

            var currentKeyWords = new Dictionary<string, string>();

            var keySymbols = new Dictionary<string, string>
            {
                { "(", "key symbol" },
                { ")", "key symbol" },
                { "{", "key symbol" },
                { "}", "key symbol" },
                { "[", "key symbol" },
                { "]", "key symbol" },
                { ",", "key symbol" },
                { ";", "key symbol" },
            };

            var operations = new Dictionary<string, string>
            {
                { "=", "operation" },
                { "!", "operation" },
                { "<", "operation" },
                { ">", "operation" },
                { "+", "operation" },
                { "-", "operation" },
            };

            var tokens = new List<string>();

            var word = "";
            var read = false;

            foreach (var symbol in codeText)
            {
                if ((symbol != ' ' || read) && symbol != '\n' && symbol != '\t' && symbol != '\r'
                    && !keySymbols.ContainsKey($"{symbol}") && !operations.ContainsKey($"{symbol}"))
                {
                    if (symbol == '\"')
                    {
                        read = !read;
                    }

                    word += symbol;
                }
                else
                {
                    if (keyWords1.ContainsKey(word) || keyWords2.ContainsKey(word) || keyWords3.ContainsKey(word)
                         || keyWords4.ContainsKey(word) || keyWords5.ContainsKey(word) || keyWords6.ContainsKey(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "key word");
                        }

                        tokens.Add($"{word} key word");

                        word = "";
                        continue;
                    }

                    if (variablesTypes.Contains(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "variable type");
                        }

                        tokens.Add($"{word} variable type");

                        word = "";
                    }

                    if (symbol == '(')
                    {
                        if (variablesTypes.Contains(tokens[^1].Split()[0]))
                        {
                            if (Regex.IsMatch(word, @"^[a-z][a-z0-9_]*$", RegexOptions.IgnoreCase))
                            {
                                if (!currentKeyWords.ContainsKey(word))
                                {
                                    currentKeyWords.Add(word, "function");
                                }

                                tokens.Add($"{word} function");

                                word = "";
                            }
                            else if (word == "")
                            {
                                var temp = tokens[^1].Split()[0];

                                variables.Remove(temp);

                                if (!currentKeyWords.ContainsKey(temp))
                                {
                                    currentKeyWords.Add(temp, "function");
                                }

                                tokens.RemoveAt(tokens.Count - 1);

                                tokens.Add($"{temp} function");
                            }
                        }
                        else if (tokens[^1].Split()[0] == ";")
                        {
                            if (currentKeyWords.ContainsKey(word))
                            {
                                tokens.Add($"{word} {currentKeyWords[word]}");

                                word = "";
                            }
                        }

                        tokens.Add($"{symbol} key symbol");

                        continue;
                    }

                    if (Regex.IsMatch(word, @"^[a-z][a-z0-9_]*$", RegexOptions.IgnoreCase))
                    {
                        if (!variables.ContainsKey(word))
                        {
                            variables.Add(word, tokens[^1].Split()[0]);
                            tokens.Add($"{word} {tokens[^1].Split()[0]}");
                        }
                        else
                        {
                            tokens.Add($"{word} {variables[word]}");
                        }

                        word = "";
                    }

                    if (word.StartsWith("\"") && word.EndsWith("\"") && word.Length > 1)
                    {
                        tokens.Add($"{word} string literal");

                        word = "";
                    }

                    if (int.TryParse(word, out var val1))
                    {
                        tokens.Add($"{word} int literal");

                        word = "";
                    }
                    else if (double.TryParse(word, out var val2))
                    {
                        tokens.Add($"{word} double literal");

                        word = "";
                    }

                    if (keySymbols.ContainsKey($"{symbol}"))
                    {
                        tokens.Add($"{symbol} key symbol");
                    }

                    if (operations.ContainsKey($"{symbol}"))
                    {
                        tokens.Add($"{symbol} operation");
                    }
                }
            }

            Console.WriteLine("Variables:");

            foreach (var elem in variables)
            {
                Console.WriteLine($"\t{elem.Key}: {elem.Value}");
            }

            Console.WriteLine("\nKey words:");

            foreach (var elem in currentKeyWords)
            {
                Console.WriteLine($"\t{elem.Key}: {elem.Value}");
            }

            Console.WriteLine("\nTokens:");

            foreach (var elem in tokens)
            {
                Console.WriteLine($"\t{elem}");
            }
        }
    }
}
