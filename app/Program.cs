using ExpressionOptimizer;
using Expressions;

// Create the variable 'x'
IExpression variableX = new VariableExpression("x");

// 2 + x
IExpression twoPlusX = new BinaryExpression(
    new ConstantExpression(2),
    variableX,
    OperatorSign.Plus
);

// 7 * (2 + x)
IExpression sevenTimesTwoPlusX = new BinaryExpression(
    new ConstantExpression(7),
    twoPlusX,
    OperatorSign.Multiply
);

// sin(7 * (2 + x))
IExpression sinExpression = new Function(
    FunctionKind.Sin,
    sevenTimesTwoPlusX
);

// cos(x)
IExpression cosExpression = new Function(
    FunctionKind.Cos,
    variableX
);

// sin(7*(2+x)) - 7*(2+x)
IExpression binaryExpression = new BinaryExpression(
    sinExpression,
    sevenTimesTwoPlusX,
    OperatorSign.Minus
);

// Complete expression: (sin(7*(2+x)) - 7*(2+x)) + cos(x)
IExpression finalExpression = new BinaryExpression(
    binaryExpression,
    cosExpression,
    OperatorSign.Plus
);

//Console.WriteLine(finalExpression);
// Output: ((sin(7 * (2 + x)) - 7 * (2 + x)) + cos(x))

ExpressionTreeTraverser traverser = new ExpressionTreeTraverser(finalExpression);

IExpression? expr;
while ((expr = traverser.next()) != null)
{
    Console.WriteLine(expr);
}