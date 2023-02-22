using System.IO;
using System.Text.RegularExpressions;

namespace Lab2
{
    internal class Program
    {
        enum CodeContext
        {
            Default,
            Definition,
            Calling,
            Condition,
            Cycle,
            Switch
        }

        static void Main(string[] args)
        {
            var path = "Program1.cpp";

            using var reader = new StreamReader(path!);
            string codeText = reader.ReadToEnd();
            reader.Close();

            var variablesTypes = new List<string> { "int", "double", "char", "string", "string*", "void" };
            var variables = new Dictionary<string, string>();

            var keyWords = new Dictionary<string, string>
            {
                { "while", "key word" },
                { "for", "key word" },
                { "if", "key word" },
                { "switch", "key word" },
                { "do", "key word" },
                { "else", "key word" },
                { "continue", "key word" },
                { "break", "key word" },
                { "case", "key word" },
                { "default", "key word" },
                { "cout", "key word" },
                { "cin", "key word" },
                { "new", "key word" },
                { "endl", "key word" },
                { "#include", "key word" },
                { "<iostream>", "library" },
                { "<string>", "library" },
                { "using", "key word" },
                { "namespace", "key word" },
                { "std", "namespace" },
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
            var readSpace = false;
            var isError = false;
            var context = CodeContext.Default;
            
            foreach (var symbol in codeText)
            {
                if ((symbol != ' ' || readSpace) && symbol != '\n' && symbol != '\t' && symbol != '\r'
                    && !keySymbols.ContainsKey($"{symbol}") && !operations.ContainsKey($"{symbol}"))
                {
                    if (symbol == '\"')
                    {
                        readSpace = !readSpace;
                    }

                    word += symbol;
                }
                else
                {
                    if (keyWords.ContainsKey(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "key word");
                        }

                        tokens.Add($"{word} key word");

                        word = string.Empty;

                        if (word == "if" || word == "while")
                        {
                            context = CodeContext.Condition;
                        }
                        else if (word == "switch")
                        {
                            context = CodeContext.Switch;
                        }
                        else if (word == "for")
                        {
                            context = CodeContext.Cycle;
                        }
                        else
                        {
                            context = CodeContext.Default;
                        }
                    }
                    else if (variablesTypes.Contains(word))
                    {
                        if (!currentKeyWords.ContainsKey(word))
                        {
                            currentKeyWords.Add(word, "variable type");
                        }

                        tokens.Add($"{word} variable type");

                        word = string.Empty;

                        context = CodeContext.Default;
                    }
                    else if (Regex.IsMatch(word, @"^[a-z][a-z0-9_]*$", RegexOptions.IgnoreCase))
                    {
                        if (!variables.ContainsKey(word) && !currentKeyWords.ContainsKey(word))
                        {
                            if (variablesTypes.Contains(tokens[^1].Split()[0]))
                            {
                                variables.Add(word, tokens[^1].Split()[0]);
                                tokens.Add($"{word} {tokens[^1].Split()[0]}");

                                context = CodeContext.Default;
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

                                context = CodeContext.Default;
                            }
                            else
                            {
                                tokens.Add($"{word} {currentKeyWords[word]}");

                                context = CodeContext.Calling;
                            }
                        }

                        word = string.Empty;
                    }
                    else if (word.StartsWith("\"") && word.EndsWith("\"") && word.Length >= 2)
                    {
                        tokens.Add($"{word} string literal");

                        word = string.Empty;

                        context = CodeContext.Default;
                    }
                    else if (int.TryParse(word, out var val1))
                    {
                        tokens.Add($"{word} int literal");

                        word = string.Empty;

                        context = CodeContext.Default;
                    }
                    else if (double.TryParse(word, out var val2))
                    {
                        tokens.Add($"{word} double literal");

                        word = string.Empty;

                        context = CodeContext.Default;
                    }
                    else if (word != string.Empty)
                    {
                        if (variablesTypes.Contains(tokens[^1].Split()[0]))
                        {
                            Console.WriteLine($"{path}: Ожидается имя идентификатора: {word}");
                            isError = true;
                            break;
                        }
                        else if (variables.ContainsKey(tokens[^1].Split()[0]))
                        {
                            if (context == CodeContext.Definition)
                            {
                                Console.WriteLine($"{path}: Ожидается ',': {word}");
                            }
                            
                            if (context == CodeContext.Calling)
                            {
                                Console.WriteLine($"{path}: Ожидается ',' или выражение, которое возвращает значение: {word}");
                            }

                            if (context == CodeContext.Condition || context == CodeContext.Cycle)
                            {
                                Console.WriteLine($"{path}: Ожидается операция: {word}");
                            }

                            if (context == CodeContext.Switch)
                            {
                                Console.WriteLine($"{path}: Ожидается ')': {word}");
                            }

                            if (context == CodeContext.Default)
                            {
                                Console.WriteLine($"{path}: Ожидается ';' или операция: {word}");
                            }

                            isError = true;
                            break;
                        }
                        else if (tokens[^1].Split()[0] == "{" || tokens[^1].Split()[0] == "}" || tokens[^1].Split()[0] == "(")
                        {
                            Console.WriteLine($"{path}: Ожидается ключевое слово или идентификатор: {word}");
                            isError = true;
                            break;
                        }
                        else if (tokens[^1].Split()[0] == ")")
                        {
                            Console.WriteLine($"{path}: Ожидается '}}' или ';': {word}");
                            isError = true;
                            break;
                        }
                        else if (tokens[^1].Split()[0] == "[")
                        {
                            Console.WriteLine($"{path}: Ожидается идентификатор или литерал int: {word}");
                            isError = true;
                            break;
                        }
                        else if (tokens[^1].Split()[0] == "]")
                        {
                            Console.WriteLine($"{path}: Ожидается '+', '-' или ';': {word}");
                            isError = true;
                            break;
                        }
                        else if ((tokens[^1].Split()[0].StartsWith("<") && tokens[^1].Split()[0].EndsWith(">")) || tokens[^1].Split()[0] == ";")
                        {
                            Console.WriteLine($"{path}: Ожидается ключевое слово: {word}");
                            isError = true;
                            break;
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

                            context = CodeContext.Definition;
                        }
                    }

                    if ((symbol == '<' || symbol == '>') && tokens[^1].Split()[0] == "#include")
                    {
                        word += symbol;

                        if (word.StartsWith('<') && word.EndsWith('>') && word.Length >= 3)
                        {
                            if (!currentKeyWords.ContainsKey(word))
                            {
                                currentKeyWords.Add(word, "library");
                            }

                            tokens.Add(word);

                            word = string.Empty;
                        }

                        continue;
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

            if (!isError)
            {
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
}
