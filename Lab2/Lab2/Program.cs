using System.IO;
using System.Text.RegularExpressions;

namespace Lab2
{
    internal class Program
    {
        static string IsVariable(List<string> tokens, List<string> variablesTypes)
        {
            for (var i = 1; i <= tokens.Count; i++)
            {
                if (tokens[^i].Split()[0] != "," && !variablesTypes.Contains(tokens[^i].Split()[1]))
                {
                    if (variablesTypes.Contains(tokens[^i].Split()[0]))
                    {
                        return tokens[^i].Split()[0];
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        static int IsVariableExists(List<Dictionary<string, string>> variablesTables, string word)
        {
            for (var i = 1; i <= variablesTables.Count; i++)
            {
                if (variablesTables[^i].ContainsKey(word))
                {
                    return variablesTables.Count - i;
                }
            }

            return -1;
        }

        class Level
        {
            public Dictionary<string, string> Variables { get; set; } = new();
            public List<Level> Levels { get; set; } = new();
        }

        static void Main(string[] args)
        {
            var path = "Program1.cpp";

            using var reader = new StreamReader(path!);
            string codeText = reader.ReadToEnd();
            reader.Close();

            var variablesTypes = new List<string> { "int", "double", "char", "string", "void", "bool" };
            var variablesTables = new List<Dictionary<string, string>> { new Dictionary<string, string>() };
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
            var table = 0;
            
            foreach (var symbol in codeText)
            {
               if ((symbol != ' ' || readSpace) && symbol != '\n' && symbol != '\t' && symbol != '\r'
                    && !keySymbols.ContainsKey($"{symbol}") && !operations.ContainsKey($"{symbol}"))
                {
                    if (symbol == '\"' && !isCharReading)
                    {
                        readSpace = !readSpace;
                    }

                    if (symbol == '\'' && !readSpace && (word == string.Empty || word[^1] != '\\'))
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

                    if (isCharReading && word.Length > 3)
                    {
                        Console.WriteLine($"{path}: Константа char не имеет закрывающего символа: {word}");
                        isError = true;
                        break;
                    }

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
                        if (IsVariableExists(variablesTables, word) == -1 && !currentKeyWords.ContainsKey(word))
                        {
                            var type = IsVariable(tokens, variablesTypes);

                            if (type != string.Empty)
                            {
                                variablesTables[table].Add(word, type);
                                tokens.Add($"{word} {type}");
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
                            var index = IsVariableExists(variablesTables, word);

                            if (index != -1)
                            {
                                tokens.Add($"{word} {variablesTables[index][word]}");
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
                    else if (word.StartsWith("\'") && word.EndsWith("\'") && word.Length >= 2)
                    {
                        if (word.Length >= 3)
                        {
                            if ((word.Length == 3 && word[1] != '\\') || (word.Length == 4 && word[1] == '\\' &&
                                (word[2] == 'r' || word[2] == 'n' || word[2] == 't' || word[2] == 'v' || word[2] == '\"'
                                || word[2] == '\'' || word[2] == '\\' || word[2] == '0')))
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

                    if (!readSpace && !isCharReading)
                    {
                        if (symbol == '(')
                        {
                            if (variablesTypes.Contains(tokens[^1].Split()[1]))
                            {
                                var temp = tokens[^1].Split()[0];

                                variablesTables[table].Remove(temp);

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

                        if (symbol == '{')
                        {
                            table++;
                            variablesTables.Add(new Dictionary<string, string>());
                        }

                        if (symbol == '}')
                        {
                            table--;
                        }
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

                for (var i = 0; i < variablesTables.Count; i++)
                {
                    Console.WriteLine($"\t{i}");

                    foreach (var elem in variablesTables[i])
                    {
                        Console.WriteLine($"\t\t{elem.Key}: {elem.Value}");
                    }

                    Console.WriteLine();
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
