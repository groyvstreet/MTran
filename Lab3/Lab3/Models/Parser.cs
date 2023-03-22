using Lab2.Models;

namespace Lab3.Models
{
    public class Parser
    {
        public Lexer Lexer { get; set; }
        public List<Token> Tokens { get; set; }
        public int Position { get; set; } = 0;
        public bool IsSwitch { get; set; } = false;

        public Parser(Lexer lexer, List<Token> tokens)
        {
            Lexer = lexer;
            Tokens = tokens;
        }

        public Token? Match(List<string> tokenTypes)
        {
            if (Position < Tokens.Count)
            {
                var token = Tokens[Position];

                if (tokenTypes.Contains(token.Identifier))
                {
                    Position++;
                    return token;
                }
            }

            return null;
        }

        public Token Require(List<string> tokenTypes)
        {
            var token = Match(tokenTypes);

            if (token == null)
            {
                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен '{tokenTypes[0]}'");
            }

            return token;
        }

        public List<string> GetVariables()
        {
            var variables = new List<string>();

            foreach (var elem in Lexer.VariablesTables.Values)
            {
                foreach (var elem2 in elem.Keys)
                {
                    variables.Add(elem2);
                }
            }

            return variables;
        }

        ExpressionNode ParseVariableType()
        {
            var type = Match(Lexer.VariablesTypes);

            if (type != null)
            {
                return new VariableTypeNode(type);
            }

            throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной");
        }

        ExpressionNode ParseVariableOrLiteral()
        {
            var number = Match(Lexer.Literals.Keys.ToList());

            if (number != null)
            {
                return new LiteralNode(number);
            }

            var variable = Match(GetVariables());

            if (variable != null)
            {
                var leftNode = new VariableNode(variable) as ExpressionNode;
                var @operator = Match(new List<string> { "[" });

                while (@operator != null)
                {
                    @operator.Identifier = "[]";
                    var rightNode = ParseFormula();
                    leftNode = new BinaryOperationNode(@operator, leftNode, rightNode);
                    Require(new List<string> { "]" });
                    @operator = Match(new List<string> { "[" });
                }

                return leftNode;
            }

            if (Tokens[Position].Identifier == "false" || Tokens[Position].Identifier == "true")
            {
                return new LiteralNode(new Token(Tokens[Position++].Identifier, "bool literal"));
            }

            throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной или литерала");
        }

        public ExpressionNode ParseParentheses()
        {
            if (Match(new List<string> { "(" }) != null)
            {
                var node = ParseFormula();
                Require(new List<string> { ")" });
                return node;
            }
            else
            {
                return ParseVariableOrLiteral();
            }
        }

        public ExpressionNode ParseFormula()
        {
            var leftNode = ParseParentheses();
            var @operator = Match(Lexer.CurrentOperations.Keys.ToList());
            @operator ??= Match(new List<string> { "[" });

            while (@operator != null)
            {
                if (@operator.Identifier == "[")
                {
                    @operator.Identifier = "[]";
                    var rightNode = ParseFormula();
                    leftNode = new BinaryOperationNode(@operator, leftNode, rightNode);
                    Require(new List<string> { "]" });
                    @operator = Match(Lexer.Operations.Keys.ToList());
                }
                else if (@operator.Identifier == "++" || @operator.Identifier == "--")
                {
                    leftNode = new UnaryOperationNode(@operator, leftNode);
                    @operator = Match(Lexer.Operations.Keys.ToList());
                    @operator ??= Match(new List<string> { "[" });
                }
                else if (@operator.Identifier == "?")
                {
                    var ifBody = ParseFormula();
                    Require(new List<string> { ":" });
                    var elseBody = ParseFormula();
                    leftNode = new IfNode(leftNode, ifBody, elseBody);
                    @operator = Match(Lexer.Operations.Keys.ToList());
                    @operator ??= Match(new List<string> { "[" });
                }
                else if (@operator.Identifier == "<<" || @operator.Identifier == ">>")
                {
                    Position--;
                    break;
                }
                else if (@operator.Identifier == "==" || /*@operator.Identifier == "!=" ||*/ @operator.Identifier == "<" ||
                    @operator.Identifier == ">")
                {
                    var rightNode = ParseFormula();
                    leftNode = new BinaryOperationNode(@operator, leftNode, rightNode);
                    @operator = Match(Lexer.Operations.Keys.ToList());
                    @operator ??= Match(new List<string> { "[" });
                }
                else
                {
                    var rightNode = ParseParentheses();

                    /*if (leftNode is BinaryOperationNode binary)
                    {
                        binary.RightNode = new BinaryOperationNode(@operator, binary.RightNode, rightNode);
                        leftNode = binary;
                    }
                    else
                    {
                        leftNode = new BinaryOperationNode(@operator, leftNode, rightNode);
                    }*/

                    leftNode = new BinaryOperationNode(@operator, leftNode, rightNode); //

                    @operator = Match(Lexer.Operations.Keys.ToList());
                    @operator ??= Match(new List<string> { "[" });
                }
            }

            return leftNode;
        }

        public List<Token> ParseFunctionDefinition()
        {
            var parameters = new List<Token>();

            if (Match(Lexer.VariablesTypes) == null)
            {
                if (Match(new List<string> { ")" }) != null)
                {
                    Position--;
                    return parameters;
                }

                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен типа переменной или ')'");
            }

            var parameter = Match(GetVariables());

            if (parameter == null)
            {
                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной");
            }

            parameters.Add(parameter);

            var keySymbol = Match(new List<string> { "," });

            while (keySymbol != null)
            {
                if (Match(Lexer.VariablesTypes) == null)
                {
                    throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен типа переменной");
                }

                parameter = Match(GetVariables());
                parameters.Add(parameter!);
                keySymbol = Match(new List<string> { "," });
            }

            return parameters;
        }

        public List<ExpressionNode> ParseCout()
        {
            var parameters = new List<ExpressionNode>();

            var @operator = Match(new List<string> { "<<" });

            while(@operator != null)
            {
                if (Match(new List<string> { "endl" }) != null)
                {
                    Position--;
                    var temp = new KeyWordNode(Match(new List<string> { "endl" })!);
                    parameters.Add(temp);
                    @operator = Match(new List<string> { "<<" });
                    continue;
                }

                var parameter = ParseFormula();
                parameters.Add(parameter);
                @operator = Match(new List<string> { "<<" });
            }

            if (parameters.Count == 0)
            {
                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен '<<'");
            }

            return parameters;
        }

        public List<ExpressionNode> ParseCin()
        {
            var parameters = new List<ExpressionNode>();

            var @operator = Match(new List<string> { ">>" });

            while (@operator != null)
            {
                var parameter = ParseFormula();
                parameters.Add(parameter);
                @operator = Match(new List<string> { ">>" });
            }

            if (parameters.Count == 0)
            {
                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен '>>'");
            }

            return parameters;
        }

        public List<ExpressionNode> ParseFunctionParameters()
        {
            var parameters = new List<ExpressionNode>();

            var parameter = ParseFormula();
            parameters.Add(parameter);
            var @operator = Match(new List<string> { "," });

            while (@operator != null)
            {
                parameter = ParseFormula();
                parameters.Add(parameter);
                @operator = Match(new List<string> { "," });
            }

            return parameters;
        }

        public ExpressionNode ParseIfElse()
        {
            Require(new List<string> { "(" });
            var ifCondition = ParseFormula();
            Require(new List<string> { ")" });
            Require(new List<string> { "{" });
            var ifBody = ParseCode();
            Position--;
            Require(new List<string> { "}" });

            ExpressionNode? elseBody = null;

            if (Match(new List<string> { "else" }) != null)
            {
                if (Match(new List<string> { "if" }) != null)
                {
                    elseBody = ParseIfElse();
                }
                else
                {
                    Require(new List<string> { "{" });
                    elseBody = ParseCode();
                    Position--;
                    Require(new List<string> { "}" });
                    return new IfNode(ifCondition, ifBody, elseBody);
                }
            }

            return new IfNode(ifCondition, ifBody, elseBody);
        }

        public ExpressionNode? ParseExpression()
        {
            if (Match(Lexer.VariablesTypes) != null)
            {
                var functionToken = Match(Lexer.CurrentKeyWords.Keys.ToList());

                if (functionToken != null)
                {
                    if (functionToken.Type == "function")
                    {
                        Require(new List<string> { "(" });
                        var parameters = ParseFunctionDefinition();
                        Require(new List<string> { ")" });
                        Require(new List<string> { "{" });
                        var body = ParseCode();
                        Position--;
                        Require(new List<string> { "}" });
                        return new FunctionNode(functionToken, parameters, body);
                    }
                    else
                    {
                        throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной или функции");
                    }
                }

                var variableToken = Match(GetVariables());

                if (variableToken != null)
                {
                    var leftNode = new VariableNode(variableToken) as ExpressionNode;
                    var @operator = Match(new List<string> { "," });

                    while (@operator != null)
                    {
                        variableToken = Match(GetVariables());

                        if (variableToken == null)
                        {
                            throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной");
                        }

                        var rightNode = new VariableNode(variableToken);
                        leftNode = new BinaryOperationNode(new Token("=", "operation"), leftNode, rightNode);
                        @operator = Match(new List<string> { "," });
                    }

                    @operator = Match(new List<string> { "=" });

                    if (@operator != null)
                    {
                        if (Match(new List<string> { "new" }) != null)
                        {
                            var type = Match(Lexer.VariablesTypes);

                            if (type != null)
                            {
                                Position--;
                                var typeNode = ParseVariableType();
                                Require(new List<string> { "[" });
                                var index = ParseFormula();
                                Require(new List<string> { "]" });
                                Require(new List<string> { ";" });
                                var rightNode = new BinaryOperationNode(new Token("new", "key word"), typeNode, index);
                                return new BinaryOperationNode(@operator, leftNode, rightNode);
                            }

                            throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен типа переменной");
                        }

                        var value = ParseFormula();
                        Require(new List<string> { ";" });
                        return new BinaryOperationNode(new Token("=", "operation"), leftNode, value);
                    }
                    else
                    {
                        Require(new List<string> { ";" });
                        return null;
                    }
                }
            }
            
            if (Match(GetVariables()) != null)
            {
                Position--;
                var variableNode = ParseVariableOrLiteral();
                var list = Lexer.CurrentOperations.Keys.ToList();
                list.Add("[");
                var @operator = Match(list);

                if (@operator != null)
                {
                    if (@operator.Identifier == "++" || @operator.Identifier == "--")
                    {
                        var unaryNode = new UnaryOperationNode(@operator, variableNode);
                        Require(new List<string> { ";" });
                        return unaryNode;
                    }

                    if (@operator.Identifier == "[")
                    {
                        @operator.Identifier = "[]";
                        var rightNode = ParseFormula();
                        variableNode = new BinaryOperationNode(@operator, variableNode, rightNode);
                        Require(new List<string> { "]" });
                        @operator = Match(Lexer.Operations.Keys.ToList());
                    }

                    if (Match(new List<string> { "new" }) != null)
                    {
                        var type = Match(Lexer.VariablesTypes);

                        if (type != null)
                        {
                            Position--;
                            var typeNode = ParseVariableType();
                            Require(new List<string> { "[" });
                            var value = ParseFormula();
                            Require(new List<string> { "]" });
                            Require(new List<string> { ";" });
                            return new BinaryOperationNode(new Token("new", "key word"), typeNode, value);
                        }

                        throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен типа переменной");
                    }

                    var rightFormulaNode = ParseFormula();
                    var binaryNode = new BinaryOperationNode(@operator, variableNode, rightFormulaNode);
                    Require(new List<string> { ";" });
                    return binaryNode;
                }
                else
                {
                    Require(new List<string> { ";" });
                    return null;
                }

                throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен оператора");
            }

            if (Match(Lexer.CurrentKeyWords.Keys.ToList()) != null)
            {
                Position--;
                var token = Match(Lexer.CurrentKeyWords.Keys.ToList());

                switch (token!.Identifier)
                {
                    case "while":
                        Require(new List<string> { "(" });
                        var condition = ParseFormula();
                        Require(new List<string> { ")" });
                        Require(new List<string> { "{" });
                        var body = ParseCode();
                        Position--;
                        Require(new List<string> { "}" });
                        return new WhileNode(condition, body);
                    case "cout":
                        var parameters = ParseCout();
                        Require(new List<string> { ";" });
                        return new CoutNode(parameters);
                    case "cin":
                        var cin_parameters = ParseCin();
                        Require(new List<string> { ";" });
                        return new CinNode(cin_parameters);
                    case "for":
                        Require(new List<string> { "(" });
                        Match(Lexer.VariablesTypes);
                        var first = ParseFormula();
                        Require(new List<string> { ";" });
                        var second = ParseFormula();
                        Require(new List<string> { ";" });
                        var third = ParseFormula();
                        Require(new List<string> { ")" });
                        Require(new List<string> { "{" });
                        var forBody = ParseCode();
                        Position--;
                        Require(new List<string> { "}" });
                        return new ForNode(first, second, third, forBody);
                    case "if":
                        return ParseIfElse();
                    case "switch":
                        Require(new List<string> { "(" });
                        var variable = Match(GetVariables());

                        if(variable == null)
                        {
                            throw new Exception($"После токена '{Tokens[Position - 1].Identifier}' ожидается токен переменной");
                        }

                        Require(new List<string> { ")" });
                        Require(new List<string> { "{" });
                        IsSwitch = true;
                        var switchBody = ParseCode();
                        IsSwitch = false;
                        Position--;
                        Require(new List<string> { "}" });
                        return new SwitchNode(variable, switchBody);
                    case "case":
                        if (IsSwitch)
                        {
                            var literalNode = ParseVariableOrLiteral() as LiteralNode;

                            if(literalNode == null)
                            {
                                throw new Exception($"После токена {Tokens[Position - 1].Identifier} ожидается токен литерала");
                            }

                            Require(new List<string> { ":" });
                            return new CaseNode(literalNode.Literal);
                        }

                        throw new Exception("Неожидаемый токен: 'case' вне конструкции 'switch'");
                    case "default":
                        if (IsSwitch)
                        {
                            Require(new List<string> { ":" });
                            return new KeyWordNode(token);
                        }

                        throw new Exception("Неожидаемый токен: 'default' вне конструкции 'switch'");
                    case "break":
                        Require(new List<string> { ";" });
                        return new KeyWordNode(token);
                    case "continue":
                        Require(new List<string> { ";" });
                        return new KeyWordNode(token);
                    default:
                        Require(new List<string> { "(" });
                        var functionParameters = ParseFunctionParameters();
                        Require(new List<string> { ")" });
                        Require(new List<string> { ";" });
                        return new FunctionExecutionNode(token, functionParameters);
                }
            }

            throw new Exception($"Ожидается тип переменной, переменная, литерал или ключевое слово {Position}");
        }

        public ExpressionNode ParseCode()
        {
            var root = new StatementsNode();

            while(Position < Tokens.Count)
            {
                if (Match(new List<string> { "}" }) != null)
                {
                    return root;
                }

                var statementNode = ParseExpression();

                if (statementNode != null)
                {
                    root.AddNode(statementNode);
                }
            }

            return root;
        }
    }
}
