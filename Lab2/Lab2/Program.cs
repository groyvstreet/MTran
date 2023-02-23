using System.IO;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        static string IsVariableExists(Dictionary<string, Dictionary<string, string>> variablesTables, string word, string environment)
        {
            while (environment != "-1")
            {
                if (variablesTables[environment].ContainsKey(word))
                {
                    return environment;
                }
                else
                {
                    environment = environment.Remove(environment.Length - 2);

                    var temp = environment.Split(':');
                    temp[0] = (int.Parse(temp[0]) - 1).ToString();
                    environment = string.Empty;
                    environment += temp[0];

                    for (var i = 1; i < temp.Length; i++)
                    {
                        environment += $":{temp[i]}";
                    }
                }
            }

            return string.Empty;
        }

        static void Main(string[] args)
        {
            var path = "Program1.cpp";
            
            using var reader = new StreamReader(path!);
            string codeText = reader.ReadToEnd();
            reader.Close();

            var variablesTypes = new List<string> { "int", "double", "char", "string", "void", "bool" };
            var variablesTables = new Dictionary<string, Dictionary<string, string>>
            {
                { "0:0", new Dictionary<string, string>() }
            };
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
            var level = 0;
            var name = 0;
            var environment = "0:0";
            var row = 1;
            var col = 1;
            var info = "";
            
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
                    if (readSpace && (word.Contains('\n') || word.Contains('\r')))
                    {
                        Console.WriteLine($"{path} ({row}, {col - word.Length}): Константа string не имеет закрывающего символа: {word}\n{info}");
                        isError = true;
                        break;
                    }

                    if (isCharReading && word.Length > 3)
                    {
                        Console.WriteLine($"{path} ({row}, {col - word.Length}): Константа char не имеет закрывающего символа: {word}\n{info}");
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
                        if (IsVariableExists(variablesTables, word, environment) == string.Empty && !currentKeyWords.ContainsKey(word))
                        {
                            var type = IsVariable(tokens, variablesTypes);

                            if (type != string.Empty)
                            {
                                variablesTables[environment].Add(word, type);
                                tokens.Add($"{word} {type}");
                            }
                            else
                            {
                                Console.WriteLine($"{path} ({row}, {col - word.Length}): Неизвестный идентификатор: {word}\n{info}");
                                isError = true;
                                break;
                            }
                        }
                        else
                        {
                            var temp = IsVariableExists(variablesTables, word, environment);

                            if (temp != string.Empty)
                            {
                                tokens.Add($"{word} {variablesTables[temp][word]}");
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
                                Console.WriteLine($"{path} ({row}, {col - word.Length}): Константа char неверно задана: {word}\n{info}");
                                isError = true;
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{path} ({row}, {col - word.Length}): Константа char неверно задана: {word}\n{info}");
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
                            Console.WriteLine($"{path} ({row}, {col - word.Length}): Ожидается имя идентификатора: {word}\n{info}");
                            isError = true;
                            break;
                        }
                        else
                        {
                            if (!readSpace && !isCharReading && symbol != '>')
                            {
                                Console.WriteLine($"{path} ({row}, {col - word.Length}): Неизвестный идентификатор: {word}\n{info}");
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

                                variablesTables[environment].Remove(temp);

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

                        if (symbol == '<' || symbol == '>')
                        {
                            if (tokens[^1].Split()[0] == "#include")
                            {
                                word += symbol;

                                continue;
                            }
                        }

                        if (symbol == '{')
                        {
                            level++;
                            name++;

                            var temp = environment.Split(':');
                            temp[0] = level.ToString();
                            environment = string.Empty;
                            environment += temp[0];

                            for (var i = 1; i < temp.Length; i++)
                            {
                                environment += $":{temp[i]}";
                            }

                            environment += $":{name}";

                            variablesTables.Add(environment, new Dictionary<string, string>());
                        }

                        if (symbol == '}')
                        {
                            level--;
                            environment = environment.Remove(environment.Length - 2);

                            var temp = environment.Split(':');
                            temp[0] = (int.Parse(temp[0]) - 1).ToString();
                            environment = string.Empty;
                            environment += temp[0];

                            for (var i = 1; i < temp.Length; i++)
                            {
                                environment += $":{temp[i]}";
                            }
                        }
                    }

                    if (!readSpace && !isCharReading)
                    {
                        if (keySymbols.ContainsKey($"{symbol}"))
                        {
                            if (!currentKeySymbols.ContainsKey($"{symbol}"))
                            {
                                currentKeySymbols.Add($"{symbol}", "key symbol");
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
                                else
                                {
                                    var temp = int.Parse(currentOperations[$"{symbol}"].Split()[^1]);
                                    currentOperations[$"{symbol}"] = currentOperations[$"{symbol}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in currentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            currentOperations.Remove($"{symbol}");
                                        }
                                    }
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
                                else
                                {
                                    var temp = int.Parse(currentOperations[$"{tokens[^1].Split()[0]}"].Split()[^1]);
                                    currentOperations[$"{tokens[^1].Split()[0]}"] = currentOperations[$"{tokens[^1].Split()[0]}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in currentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            currentOperations.Remove($"{symbol}");
                                        }
                                    }
                                }

                                tokens.RemoveAt(tokens.Count - 1);
                                tokens.Add($"{tokens[^1].Split()[0]}{symbol} operation");
                            }
                            else
                            {
                                if (!currentOperations.ContainsKey($"{symbol}"))
                                {
                                    currentOperations.Add($"{symbol}", "operation 1");
                                }
                                else
                                {
                                    var temp = int.Parse(currentOperations[$"{symbol}"].Split()[^1]);
                                    currentOperations[$"{symbol}"] = currentOperations[$"{symbol}"].Replace($"{temp}", $"{++temp}");
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

                if (symbol == '\n')
                {
                    row++;
                    col = 1;
                    info = "";
                }
                else
                {
                    col++;
                    info += symbol;
                }
            }

            if (!isError)
            {
                Console.WriteLine("Variables:");

                foreach (var elem in variablesTables)
                {
                    Console.WriteLine($"\t{elem.Key}");

                    foreach (var elem2 in elem.Value)
                    {
                        Console.WriteLine($"\t\t{elem2.Key} : {elem2.Value}");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("\nLiterals:");

                foreach (var elem in literals)
                {
                    Console.WriteLine($"\t{elem.Key} : {elem.Value}");
                }

                Console.WriteLine("\nKey words:");

                foreach (var elem in currentKeyWords)
                {
                    Console.WriteLine($"\t{elem.Key} : {elem.Value}");
                }

                Console.WriteLine("\nKey symbols:");

                foreach (var elem in currentKeySymbols)
                {
                    Console.WriteLine($"\t{elem.Key} : {elem.Value}");
                }

                Console.WriteLine("\nOperations:");

                foreach (var elem in currentOperations)
                {
                    Console.WriteLine($"\t{elem.Key} : {elem.Value}");
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
