namespace Expressions
{
    interface IExpression { IExpression? LeftExpression { get; } IExpression? RightExpression { get; } };
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

        public IExpression? LeftExpression => null;

        public IExpression? RightExpression => null;
    }

    class VariableExpression : IVariableExpression
    {
        public string Name { get; }

        public VariableExpression(string name = "x")
        {
            Name = name;
        }

        public override string ToString() => Name;

        public IExpression? LeftExpression => null;

        public IExpression? RightExpression => null;
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

        private string ToString(OperatorSign sign)
        {
            switch(sign)
            {
                case OperatorSign.Plus: return "+";

                case OperatorSign.Minus: return "-";

                case OperatorSign.Multiply: return "*";

                case OperatorSign.Divide: return "/";

                default: throw new Exception();
            }
        }

        public IExpression? LeftExpression => Left;

        public IExpression? RightExpression => Right;
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

        private string ToString(FunctionKind kind)
        {
            switch (kind)
            {
                case FunctionKind.Sin: return "sin";

                case FunctionKind.Cos: return "cos";

                case FunctionKind.Max: return "max";

                default: throw new Exception();
            }
        }

        public IExpression? LeftExpression => Argument;

        public IExpression? RightExpression => null;
    }
}