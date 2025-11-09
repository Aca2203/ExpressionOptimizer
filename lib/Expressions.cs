namespace Expressions
{
    interface IExpression;
    interface IConstantExpression : IExpression { int Value { get; } }
    interface IVariableExpression : IExpression { string Name { get; } }
    interface IBinaryExpression : IExpression { IExpression Left { get; } IExpression Right { get; } OperatorSign Sign { get; } }
    interface IFunction : IExpression { FunctionKind Kind { get; } IExpression Argument { get; } }

    enum FunctionKind { Sin, Cos, Max }
    enum OperatorSign { Plus, Minus, Multiply, Divide }

    class ConstantExpression : IConstantExpression
    {
        public int Value { get; }

        public ConstantExpression(int value = 0)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }

    class VariableExpression : IVariableExpression
    {
        public string Name { get; }

        public VariableExpression(string name = "x")
        {
            Name = name;
        }

        public override string ToString() => Name;
    }

    class BinaryExpression : IBinaryExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }
        public OperatorSign Sign { get; }

        public BinaryExpression(IExpression leftExpression, IExpression rightExpression, OperatorSign sign)
        {
            Left = leftExpression;
            Right = rightExpression;
            Sign = sign;
        }

        public override string ToString() => $"({Left} {ToString(Sign)} {Right})";

        protected string ToString(OperatorSign sign)
        {
            switch(sign)
            {
                case OperatorSign.Plus: return "+";

                case OperatorSign.Minus: return "-";

                case OperatorSign.Multiply: return "*";

                case OperatorSign.Divide: return "/";

                default: throw new Exception("Invalid operator sign");
            }
        }
    }

    class Function : IFunction
    {
        public FunctionKind Kind { get; }

        public IExpression Argument { get; }

        public Function(FunctionKind kind, IExpression argument)
        {
            Kind = kind;
            Argument = argument;
        }

        public override string ToString() => $"{ToString(Kind)}({Argument})";

        protected string ToString(FunctionKind kind) => kind.ToString().ToLower();
    }

    class ExpressionOptimizer
    {
        public static IExpression? Optimize(IExpression? expression)
        {
            if (expression == null) return null;

            Dictionary<string, IExpression> cache = new Dictionary<string, IExpression>();
            IExpression optimizedExpression = RemoveDuplicates(expression, cache);
            return optimizedExpression;
        }

        protected static IExpression RemoveDuplicates(IExpression expression, Dictionary<string, IExpression> cache)
        {
            string key = expression.ToString()!;
            if (cache.TryGetValue(key, out IExpression? existingExpression)) return existingExpression;

            IExpression newExpression = CloneExpression(expression, cache);

            cache[key] = newExpression;
            return newExpression;
        }

        protected static IExpression CloneExpression(IExpression expression, Dictionary<string, IExpression> cache)
        {
            IExpression clonedExpression = expression switch
            {
                ConstantExpression c => new ConstantExpression(c.Value),
                VariableExpression v => new VariableExpression(v.Name),
                BinaryExpression b => new BinaryExpression(RemoveDuplicates(b.Left, cache), RemoveDuplicates(b.Right, cache), b.Sign),
                Function f => new Function(f.Kind, RemoveDuplicates(f.Argument, cache)),
                _ => throw new Exception("Invalid expression type")
            };

            return clonedExpression;
        }
    }
}