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

            var variablesTypes = new List<string> { "int", "double", "char", "string", "void", "bool" };
            var variables = new Dictionary<string, string>();
            var literals = new Dictionary<string, string>();

            var keyWords = new Dictionary<string, string>
            {
                { "do", "key word" },
                { "while", "key word" },
                { "for", "key word" },
                { "if", "key word" },
                { "else", "key word" },
                { "switch", "key word" },
                { "case", "key word" },
                { "default", "key word" },
                { "break", "key word" },
                { "continue", "key word" },
                { "cout", "key word" },
                { "cin", "key word" },
                { "new", "key word" },
                { "endl", "key word" },
                { "true", "key word" },
                { "false", "key word" },
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
                { ":", "key symbol" },
                { ";", "key symbol" },
            };

            var currentKeySymbols = new Dictionary<string, string>();

            var operations = new Dictionary<string, string>
            {
                { "=", "operation" },
                { "!", "operation" },
                { "<", "operation" },
                { ">", "operation" },
                { "+", "operation" },
                { "-", "operation" },
                { "*", "operation" },
                { "/", "operation" },
                { "?", "operation" },
            };

            var currentOperations = new Dictionary<string, string>();

            var tokens = new List<string>();

            var word = "";
            var readSpace = false;
            var isCharReading = false;
            var isError = false;
            
            foreach (var symbol in codeText)
            {
                if ((symbol != ' ' || readSpace) && symbol != '\n' && symbol != '\t' && symbol != '\r'
                    && !keySymbols.ContainsKey($"{symbol}") && !operations.ContainsKey($"{symbol}"))
                {
                    if (symbol == '\"')
                    {
                        readSpace = !readSpace;
                    }

                    if (symbol == '\'' && !readSpace)
                    {
                        isCharReading = !isCharReading;
                    }

                    word += symbol;
                }
                else
                {
                    if (readSpace && word.Contains('\n'))
                    {
                        Console.WriteLine($"{path}: Константа string не имеет закрывающего символа: {word}");
                        isError = true;
                        break;
                    }

                    /*if (isCharReading && word.Length > 3)
                    {
                        Console.WriteLine($"{path}: Константа char неверно задана: {word}");
                        isError = true;
                        break;
                    }*/

                    if (keyWords.ContainsKey(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "key word");
                        }

                        tokens.Add($"{word} key word");

                        word = string.Empty;
                    }
                    else if (variablesTypes.Contains(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "variable type");
                        }

                        tokens.Add($"{word} variable type");

                        word = string.Empty;
                    }
                    else if (Regex.IsMatch(word, @"^[a-z_][a-z0-9_]*$", RegexOptions.IgnoreCase))
                    {
                        if (!variables.ContainsKey(word) && !currentKeyWords.ContainsKey(word))
                        {
                            if (variablesTypes.Contains(tokens[^1].Split()[0]))
                            {
                                variables.Add(word, tokens[^1].Split()[0]);
                                tokens.Add($"{word} {tokens[^1].Split()[0]}");
                            }
                            else
                            {
                                Console.WriteLine($"{path}: Неизвестный идентификатор: {word}");
                                isError = true;
                                break;
                            }
                        }
                        else
                        {
                            if (variables.ContainsKey(word))
                            {
                                tokens.Add($"{word} {variables[word]}");
                            }
                            else
                            {
                                tokens.Add($"{word} {currentKeyWords[word]}");
                            }
                        }

                        word = string.Empty;
                    }
                    else if (word.StartsWith("\"") && word.EndsWith("\"") && word.Length >= 2)
                    {
                        if (!literals.ContainsKey(word))
                        {
                            literals.Add(word, "string literal");
                        }

                        tokens.Add($"{word} string literal");

                        word = string.Empty;
                    }
                    else if (word.StartsWith("\'") && word.EndsWith("\'") && word.Length >= 3)
                    {
                        if (word.Length == 3 || (word[1] == '\\' && (word[2] == 'r' || word[2] == 'n' || word[2] == 't'
                            || word[2] == 'v' || word[2] == '\"' || word[2] == '\'' || word[2] == '\\' || word[2] == '0')))
                        {
                            if (!literals.ContainsKey(word))
                            {
                                literals.Add(word, "char literal");
                            }
                            
                            tokens.Add($"{word} char literal");

                            word = string.Empty;
                        }
                        else
                        {
                            Console.WriteLine($"{path}: Константа char неверно задана: {word}");
                            isError = true;
                            break;
                        }
                    }
                    else if (int.TryParse(word, out var val1))
                    {
                        if (!literals.ContainsKey(word))
                        {
                            literals.Add(word, "int literal");
                        }

                        tokens.Add($"{word} int literal");
                        
                        word = string.Empty;
                    }
                    else if (double.TryParse(word, out var val2))
                    {
                        if (!literals.ContainsKey(word))
                        {
                            literals.Add(word, "double literal");
                        }

                        tokens.Add($"{word} double literal");

                        word = string.Empty;
                    }
                    else if (word != string.Empty)
                    {
                        if (variablesTypes.Contains(tokens[^1].Split()[0]))
                        {
                            Console.WriteLine($"{path}: Ожидается имя идентификатора: {word}");
                            isError = true;
                            break;
                        }
                        else
                        {
                            if (!readSpace && !isCharReading)
                            {
                                Console.WriteLine($"{path}: Неизвестный идентификатор: {word}");
                                isError = true;
                                break;
                            }
                        }
                    }

                    if (symbol == '(')
                    {
                        if (variablesTypes.Contains(tokens[^1].Split()[1]))
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

                    if (symbol == '*')
                    {
                        if (variablesTypes.Contains(tokens[^1].Split()[0]))
                        {
                            var temp = tokens[^1].Split()[0];

                            if (!variablesTypes.Contains($"{temp}*"))
                            {
                                variablesTypes.Add($"{temp}*");
                            }

                            if (!currentKeyWords.ContainsKey($"{temp}*"))
                            {
                                currentKeyWords.Add($"{temp}*", "variable type");
                            }

                            tokens.RemoveAt(tokens.Count - 1);

                            tokens.Add($"{temp}* variable type");
                        }

                        continue;
                    }

                    if (!readSpace && !isCharReading)
                    {
                        if (keySymbols.ContainsKey($"{symbol}"))
                        {
                            if (!currentKeySymbols.ContainsKey($"{symbol}"))
                            {
                                currentKeySymbols.Add($"{symbol}", "function");
                            }

                            tokens.Add($"{symbol} key symbol");
                        }

                        if (operations.ContainsKey($"{symbol}"))
                        {
                            if ((symbol == '+' || symbol == '-' || symbol == '<' || symbol == '>' || symbol == '=')
                                && $"{symbol}" == tokens[^1].Split()[0])
                            {
                                if (!currentOperations.ContainsKey($"{symbol}{symbol}"))
                                {
                                    currentOperations.Add($"{symbol}{symbol}", "operation");
                                }

                                tokens.RemoveAt(tokens.Count - 1);
                                tokens.Add($"{symbol}{symbol} operation");
                            }
                            else if (symbol == '=' && (tokens[^1].Split()[0] == "+" || tokens[^1].Split()[0] == "-"
                                || tokens[^1].Split()[0] == "*" || tokens[^1].Split()[0] == "/" || tokens[^1].Split()[0] == "!"))
                            {
                                if (!currentOperations.ContainsKey($"{tokens[^1].Split()[0]}{symbol}"))
                                {
                                    currentOperations.Add($"{tokens[^1].Split()[0]}{symbol}", "operation");
                                }

                                tokens.RemoveAt(tokens.Count - 1);
                                tokens.Add($"{tokens[^1].Split()[0]}{symbol} operation");
                            }
                            else
                            {
                                if (!currentOperations.ContainsKey($"{symbol}"))
                                {
                                    currentOperations.Add($"{symbol}", "function");
                                }

                                tokens.Add($"{symbol} operation");
                            }
                        }
                    }
                    else
                    {
                        word += symbol;
                    }
                }
            }

            if (!isError)
            {
                Console.WriteLine("Variables:");

                foreach (var elem in variables)
                {
                    Console.WriteLine($"\t{elem.Key}: {elem.Value}");
                }

                Console.WriteLine("\nLiterals:");

                foreach (var elem in literals)
                {
                    Console.WriteLine($"\t{elem.Key}: {elem.Value}");
                }

                Console.WriteLine("\nKey words:");

                foreach (var elem in currentKeyWords)
                {
                    Console.WriteLine($"\t{elem.Key}: {elem.Value}");
                }

                Console.WriteLine("\nKey symbols:");

                foreach (var elem in currentKeySymbols)
                {
                    Console.WriteLine($"\t{elem.Key}: {elem.Value}");
                }

                Console.WriteLine("\nOperations:");

                foreach (var elem in currentOperations)
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
}
