using Expressions;

static void PrintExpression(IExpression? expr, HashSet<int> differentExpressions, string indent = "")
{
    if (expr == null) return;

    Console.WriteLine($"{indent}{expr} (Hash: {expr.GetHashCode()})");

    differentExpressions.Add(expr.GetHashCode());

    if (expr is IBinaryExpression bin)
    {
        PrintExpression(bin.Left, differentExpressions, indent + "   ");
        PrintExpression(bin.Right, differentExpressions, indent + "   ");
    }
    else if (expr is IFunction func)
    {
        PrintExpression(func.Argument, differentExpressions, indent + "   ");
    }    
}

// (2 + x)
IExpression twoPlusX1 = new BinaryExpression(
    new ConstantExpression(2),
    new VariableExpression("x"),
    OperatorSign.Plus
);

IExpression twoPlusX2 = new BinaryExpression(
    new ConstantExpression(2),
    new VariableExpression("x"),
    OperatorSign.Plus
);

// 7 * (2 + x)
IExpression sevenTimesTwoPlusX1 = new BinaryExpression(
    new ConstantExpression(7),
    twoPlusX1,
    OperatorSign.Multiply
);

IExpression sevenTimesTwoPlusX2 = new BinaryExpression(
    new ConstantExpression(7),
    twoPlusX2,
    OperatorSign.Multiply
);

// sin(7 * (2 + x))
IExpression sinExpression = new Function(
    FunctionKind.Sin,
    sevenTimesTwoPlusX1
);

// cos(x)
IExpression cosExpression = new Function(
    FunctionKind.Cos,
    new VariableExpression("x")
);

// sin(7*(2+x)) - 7*(2+x)
IExpression binaryExpression = new BinaryExpression(
    sinExpression,
    sevenTimesTwoPlusX2, // separate instance
    OperatorSign.Minus
);

// Complete expression: (sin(7*(2+x)) - 7*(2+x)) + cos(x)
IExpression original = new BinaryExpression(
    binaryExpression,
    cosExpression,
    OperatorSign.Plus
);

HashSet<int> differentExpressions = new();

Console.WriteLine("Original expression:");
PrintExpression(original, differentExpressions);

int numberOfExpressionsOriginal = differentExpressions.Count;
Console.WriteLine($"Number of different expressions: {numberOfExpressionsOriginal}");

// Optimize
IExpression? optimized = ExpressionOptimizer.Optimize(original);

differentExpressions.Clear();

Console.WriteLine("\nOptimized expression:");
PrintExpression(optimized, differentExpressions);

int numberOfExpressionsOptimized = differentExpressions.Count;
Console.WriteLine($"Number of different expressions: {numberOfExpressionsOptimized}");

Console.WriteLine($"\nNumber of duplicates removed: {numberOfExpressionsOriginal - numberOfExpressionsOptimized}");