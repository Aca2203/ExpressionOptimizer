namespace Expressions
{
    interface IExpression;
    interface IConstantExpression : IExpression { int Value { get; } }
    interface IVariableExpression : IExpression { string Name { get; } }
    interface IBinaryExpression : IExpression { IExpression Left { get; } IExpression Right { get; } OperatorSign Sign { get; } }
    interface IFunction : IExpression { FunctionKind Kind { get; } IExpression Argument { get; } }

    enum FunctionKind { Sin, Cos, Max }
    enum OperatorSign { Plus, Minus, Multiply, Divide }

    static class OperatorSignExtensions
    {
        public static bool IsCommutative(this OperatorSign sign)
        {
            return sign == OperatorSign.Plus || sign == OperatorSign.Multiply;
        }

        public static string AsSymbol(this OperatorSign sign) => sign switch
        {
            OperatorSign.Plus => "+",
            OperatorSign.Minus => "-",
            OperatorSign.Multiply => "*",
            OperatorSign.Divide => "/",
            _ => throw new Exception("Invalid operator sign")
        };
    }

    static class FunctionKindExtensions
    {
        public static string AsName(this FunctionKind kind) => kind.ToString().ToLower();
    }

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
            Left = leftExpression ?? throw new ArgumentNullException(nameof(leftExpression));
            Right = rightExpression ?? throw new ArgumentNullException(nameof(rightExpression));
            Sign = sign;
        }

        public override string ToString() {
            string left = Left.ToString()!;
            string right = Right.ToString()!;

            if (Sign.IsCommutative())
            {
                if (string.Compare(left, right) > 0)
                {
                    var temp = left;
                    left = right;
                    right = temp;
                }
            }

            return $"({left} {Sign.AsSymbol()} {right})";
        }        
    }

    class Function : IFunction
    {
        public FunctionKind Kind { get; }
        public IExpression Argument { get; }

        public Function(FunctionKind kind, IExpression argument)
        {
            Kind = kind;
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        }

        public override string ToString() => $"{Kind.AsName()}({Argument})";        
    }

    class ExpressionOptimizer
    {
        public static IExpression? Optimize(IExpression? expression)
        {
            if (expression == null) return null;

            Dictionary<string, IExpression> cache = new();
            return RemoveDuplicates(expression, cache);
        }

        protected static IExpression RemoveDuplicates(IExpression expression, Dictionary<string, IExpression> cache)
        {
            string key = expression.ToString()!;
            if (cache.TryGetValue(key, out IExpression? existingExpression)) return existingExpression;

            IExpression newExpression = CloneExpression(expression, cache);

            cache[key] = newExpression;
            return newExpression;
        }     

        protected static IExpression CloneExpression(IExpression expression, Dictionary<string, IExpression> cache) => expression switch
        {
            ConstantExpression c => new ConstantExpression(c.Value),
            VariableExpression v => new VariableExpression(v.Name),
            BinaryExpression b => new BinaryExpression(RemoveDuplicates(b.Left, cache), RemoveDuplicates(b.Right, cache), b.Sign),
            Function f => new Function(f.Kind, RemoveDuplicates(f.Argument, cache)),
            _ => throw new Exception("Invalid expression type")
        };
    }
}