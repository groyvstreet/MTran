using System.Text.RegularExpressions;

namespace Lab2.Models
{
    public class Lexer
    {
        public string Path { get; set; }
        public string Code { get; set; }
        public List<Token> Tokens { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> VariablesTypes { get; set; }
        public Dictionary<string, Dictionary<string, string>> VariablesTables { get; set; }
        public Dictionary<string, string> Literals { get; set; }
        public Dictionary<string, string> KeyWords { get; set; }
        public Dictionary<string, string> CurrentKeyWords { get; set; }
        public Dictionary<string, string> KeySymbols { get; set; }
        public Dictionary<string, string> CurrentKeySymbols { get; set; }
        public Dictionary<string, string> Operations { get; set; }
        public Dictionary<string, string> CurrentOperations { get; set; }

        public Lexer(string path, string code)
        {
            Path = path;
            Code = code;
            Tokens = new();
            ErrorMessage = string.Empty;
            Literals = new();
            CurrentKeyWords = new();
            CurrentKeySymbols = new();
            CurrentOperations = new();
            VariablesTables = new()
            {
                { "0:0", new Dictionary<string, string>() }
            };
            VariablesTypes = new() { "int", "double", "char", "string", "void", "bool" };
            KeyWords = new()
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
            KeySymbols = new()
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
            Operations = new()
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
        }

        private string IsVariableExists(string word, string environment, bool skip = false)
        {
            while (environment != "-1")
            {
                if (VariablesTables[environment].ContainsKey(word))
                {
                    return environment;
                }
                else if (!skip)
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
                else
                {
                    break;
                }
            }

            return string.Empty;
        }

        private string IsVariable()
        {
            for (var i = 1; i <= Tokens.Count; i++)
            {
                if (Tokens[^i].Identifier != "," && !VariablesTypes.Contains(Tokens[^i].Type))
                {
                    if (VariablesTypes.Contains(Tokens[^i].Identifier))
                    {
                        return Tokens[^i].Identifier;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        public void GetTokens()
        {
            IsError = false;
            var word = "";
            var readSpace = false;
            var isCharReading = false;
            var level = 0;
            var name = 0;
            var environment = "0:0";
            var row = 1;
            var col = 1;
            var info = "";
            var isBlock = false;

            foreach (var symbol in Code)
            {
                if ((symbol != ' ' || readSpace) && symbol != '\n' && symbol != '\t' && symbol != '\r'
                    && !KeySymbols.ContainsKey($"{symbol}") && !Operations.ContainsKey($"{symbol}"))
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
                        ErrorMessage = $"{Path} ({row}, {col - word.Length}): Константа string не имеет закрывающего символа: {word}\n{info}";
                        IsError = true;
                        break;
                    }

                    if (isCharReading && word.Length > 3)
                    {
                        ErrorMessage = $"{Path} ({row}, {col - word.Length}): Константа char не имеет закрывающего символа: {word}\n{info}";
                        IsError = true;
                        break;
                    }

                    if (KeyWords.ContainsKey(word))
                    {
                        if (!CurrentKeyWords.ContainsKey(word))
                        {
                            CurrentKeyWords.Add(word, KeyWords[word]);
                        }

                        Tokens.Add(new Token(word, "key word"));

                        if (word == "for")
                        {
                            isBlock = true;

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

                            VariablesTables.Add(environment, new Dictionary<string, string>());
                        }

                        word = string.Empty;
                    }
                    else if (VariablesTypes.Contains(word))
                    {
                        if (!CurrentKeyWords.ContainsKey(word))
                        {
                            CurrentKeyWords.Add(word, "variable type");
                        }

                        Tokens.Add(new Token(word, "variable type"));

                        word = string.Empty;
                    }
                    else if (Regex.IsMatch(word, @"^[a-z_][a-z0-9_]*$", RegexOptions.IgnoreCase))
                    {
                        if (IsVariableExists(word, environment, true) == string.Empty && !CurrentKeyWords.ContainsKey(word))
                        {
                            var type = IsVariable();

                            if (type != string.Empty)
                            {
                                VariablesTables[environment].Add(word, type);
                                Tokens.Add(new Token(word, type));
                            }
                            else
                            {
                                var temp = IsVariableExists(word, environment);

                                if (temp != string.Empty)
                                {
                                    Tokens.Add(new Token(word, VariablesTables[temp][word]));
                                }
                                else
                                {
                                    ErrorMessage = $"{Path} ({row}, {col - word.Length}): Неизвестный идентификатор: {word}\n{info}";
                                    IsError = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            var type = IsVariable();

                            if (type == string.Empty)
                            {
                                var temp = IsVariableExists(word, environment);

                                if (temp != string.Empty)
                                {
                                    Tokens.Add(new Token(word, VariablesTables[temp][word]));
                                }
                                else
                                {
                                    Tokens.Add(new Token(word, CurrentKeyWords[word]));
                                }
                            }
                            else
                            {
                                ErrorMessage = $"{Path} ({row}, {col - word.Length}): Переменная уже определена в текущей области: {word}\n{info}";
                                IsError = true;
                                break;
                            }
                        }

                        word = string.Empty;
                    }
                    else if (word.StartsWith("\"") && word.EndsWith("\"") && word.Length >= 2)
                    {
                        if (!Literals.ContainsKey(word))
                        {
                            Literals.Add(word, "string literal");
                        }

                        Tokens.Add(new Token(word, "string literal"));

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
                                if (!Literals.ContainsKey(word))
                                {
                                    Literals.Add(word, "char literal");
                                }

                                Tokens.Add(new Token(word, "char literal"));

                                word = string.Empty;
                            }
                            else
                            {
                                ErrorMessage = $"{Path} ({row}, {col - word.Length}): Константа char неверно задана: {word}\n{info}";
                                IsError = true;
                                break;
                            }
                        }
                        else
                        {
                            ErrorMessage = $"{Path} ({row}, {col - word.Length}): Константа char неверно задана: {word}\n{info}";
                            IsError = true;
                            break;
                        }
                    }
                    else if (int.TryParse(word, out var val1))
                    {
                        if (!Literals.ContainsKey(word))
                        {
                            Literals.Add(word, "int literal");
                        }

                        Tokens.Add(new Token(word, "int literal"));

                        word = string.Empty;
                    }
                    else if (double.TryParse(word.Replace('.', ','), out var val2))
                    {
                        if (!Literals.ContainsKey(word))
                        {
                            Literals.Add(word, "double literal");
                        }

                        Tokens.Add(new Token(word, "double literal"));

                        word = string.Empty;
                    }
                    else if (word != string.Empty)
                    {
                        if (VariablesTypes.Contains(Tokens[^1].Identifier))
                        {
                            ErrorMessage = $"{Path} ({row}, {col - word.Length}): Ожидается имя идентификатора: {word}\n{info}";
                            IsError = true;
                            break;
                        }
                        else
                        {
                            if (!readSpace && !isCharReading && symbol != '>')
                            {
                                ErrorMessage = $"{Path} ({row}, {col - word.Length}): Неизвестный идентификатор: {word}\n{info}";
                                IsError = true;
                                break;
                            }
                        }
                    }

                    if (!readSpace && !isCharReading)
                    {
                        if (symbol == '(')
                        {
                            if (VariablesTypes.Contains(Tokens[^1].Type))
                            {
                                var temp = Tokens[^1].Identifier;

                                VariablesTables[environment].Remove(temp);

                                if (!CurrentKeyWords.ContainsKey(temp))
                                {
                                    CurrentKeyWords.Add(temp, "function");
                                }

                                Tokens.RemoveAt(Tokens.Count - 1);

                                Tokens.Add(new Token(temp, "function"));

                                isBlock = true;

                                level++;
                                name++;

                                var temp2 = environment.Split(':');
                                temp2[0] = level.ToString();
                                environment = string.Empty;
                                environment += temp2[0];

                                for (var i = 1; i < temp2.Length; i++)
                                {
                                    environment += $":{temp2[i]}";
                                }

                                environment += $":{name}";

                                VariablesTables.Add(environment, new Dictionary<string, string>());
                            }
                        }

                        if (symbol == '*')
                        {
                            if (VariablesTypes.Contains(Tokens[^1].Identifier))
                            {
                                var temp = Tokens[^1].Identifier;

                                if (!VariablesTypes.Contains($"{temp}*"))
                                {
                                    VariablesTypes.Add($"{temp}*");
                                }

                                if (!CurrentKeyWords.ContainsKey($"{temp}*"))
                                {
                                    CurrentKeyWords.Add($"{temp}*", "variable type");
                                }

                                Tokens.RemoveAt(Tokens.Count - 1);

                                Tokens.Add(new Token(temp, "variable type"));

                                continue;
                            }
                        }

                        if (symbol == '<' || symbol == '>')
                        {
                            if (Tokens[^1].Identifier == "#include")
                            {
                                word += symbol;

                                continue;
                            }
                        }

                        if (symbol == '{' && !isBlock)
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

                            VariablesTables.Add(environment, new Dictionary<string, string>());
                        }
                        else if (symbol == '{')
                        {
                            isBlock = false;
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
                        if (KeySymbols.ContainsKey($"{symbol}"))
                        {
                            if (!CurrentKeySymbols.ContainsKey($"{symbol}"))
                            {
                                CurrentKeySymbols.Add($"{symbol}", "key symbol");
                            }

                            Tokens.Add(new Token(symbol.ToString(), "key symbol"));
                        }

                        if (Operations.ContainsKey($"{symbol}"))
                        {
                            if ((symbol == '+' || symbol == '-' || symbol == '<' || symbol == '>' || symbol == '=')
                                && $"{symbol}" == Tokens[^1].Identifier)
                            {
                                if (!CurrentOperations.ContainsKey($"{symbol}{symbol}"))
                                {
                                    var temp = int.Parse(CurrentOperations[$"{symbol}"].Split()[^1]);
                                    CurrentOperations[$"{symbol}"] = CurrentOperations[$"{symbol}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in CurrentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            CurrentOperations.Remove($"{symbol}");
                                        }
                                    }

                                    CurrentOperations.Add($"{symbol}{symbol}", "operation");
                                }
                                else
                                {
                                    var temp = int.Parse(CurrentOperations[$"{symbol}"].Split()[^1]);
                                    CurrentOperations[$"{symbol}"] = CurrentOperations[$"{symbol}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in CurrentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            CurrentOperations.Remove($"{symbol}");
                                        }
                                    }
                                }

                                Tokens.RemoveAt(Tokens.Count - 1);
                                Tokens.Add(new Token($"{symbol}{symbol}", "operation"));
                            }
                            else if (symbol == '=' && (Tokens[^1].Identifier == "+" || Tokens[^1].Identifier == "-"
                                || Tokens[^1].Identifier == "*" || Tokens[^1].Identifier == "/" || Tokens[^1].Identifier == "!"))
                            {
                                if (!CurrentOperations.ContainsKey($"{Tokens[^1].Identifier}{symbol}"))
                                {
                                    var temp = int.Parse(CurrentOperations[$"{Tokens[^1].Identifier}"].Split()[^1]);
                                    CurrentOperations[$"{Tokens[^1].Identifier}"] = CurrentOperations[$"{Tokens[^1].Identifier}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in CurrentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            CurrentOperations.Remove($"{Tokens[^1].Identifier}");
                                        }
                                    }

                                    CurrentOperations.Add($"{Tokens[^1].Identifier}{symbol}", "operation");
                                }
                                else
                                {
                                    var temp = int.Parse(CurrentOperations[$"{Tokens[^1].Identifier}"].Split()[^1]);
                                    CurrentOperations[$"{Tokens[^1].Identifier}"] = CurrentOperations[$"{Tokens[^1].Identifier}"].Replace($"{temp}", $"{--temp}");

                                    foreach (var elem in CurrentOperations)
                                    {
                                        if (elem.Value.Split()[^1] == "0")
                                        {
                                            CurrentOperations.Remove($"{symbol}");
                                        }
                                    }
                                }

                                Tokens.Add(new Token($"{Tokens[^1].Identifier}{symbol}", "operation"));
                                Tokens.RemoveAt(Tokens.Count - 2);
                            }
                            else
                            {
                                if (!CurrentOperations.ContainsKey($"{symbol}"))
                                {
                                    CurrentOperations.Add($"{symbol}", "operation 1");
                                }
                                else
                                {
                                    var temp = int.Parse(CurrentOperations[$"{symbol}"].Split()[^1]);
                                    CurrentOperations[$"{symbol}"] = CurrentOperations[$"{symbol}"].Replace($"{temp}", $"{++temp}");
                                }

                                Tokens.Add(new Token(symbol.ToString(), "operation"));
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
        }
    }
}
