using System;
using System.Collections.Generic;
using Expressions;

class Program
{
    static void Main()
    {
        var tests = new List<(string name, IExpression expr)>
        {
            ("1. Simple constant", new ConstantExpression(5)),

            ("2. Simple variable", new VariableExpression("x")),

            ("3. Duplicate constants",
                new BinaryExpression(
                    new ConstantExpression(5),
                    new ConstantExpression(5),
                    OperatorSign.Plus)),

            ("4. Commutative order swap (2 + x) and (x + 2)",
                new BinaryExpression(
                    new BinaryExpression(new ConstantExpression(2), new VariableExpression("x"), OperatorSign.Plus),
                    new BinaryExpression(new VariableExpression("x"), new ConstantExpression(2), OperatorSign.Plus),
                    OperatorSign.Plus)),

            ("5. Non-commutative order swap (2 - x) and (x - 2)",
                new BinaryExpression(
                    new BinaryExpression(new ConstantExpression(2), new VariableExpression("x"), OperatorSign.Minus),
                    new BinaryExpression(new VariableExpression("x"), new ConstantExpression(2), OperatorSign.Minus),
                    OperatorSign.Plus)),

            ("6. Nested duplicates (x + x) + (x + x)",
                new BinaryExpression(
                    new BinaryExpression(new VariableExpression("x"), new VariableExpression("x"), OperatorSign.Plus),
                    new BinaryExpression(new VariableExpression("x"), new VariableExpression("x"), OperatorSign.Plus),
                    OperatorSign.Plus)),

            ("7. Function duplicates sin(x) + sin(x)",
                new BinaryExpression(
                    new Function(FunctionKind.Sin, new VariableExpression("x")),
                    new Function(FunctionKind.Sin, new VariableExpression("x")),
                    OperatorSign.Plus)),

            ("8. Nested function sin(cos(x)) + sin(cos(x))",
                new BinaryExpression(
                    new Function(FunctionKind.Sin, new Function(FunctionKind.Cos, new VariableExpression("x"))),
                    new Function(FunctionKind.Sin, new Function(FunctionKind.Cos, new VariableExpression("x"))),
                    OperatorSign.Plus)),

            ("9. Deep mixed structure (sin(2*x) + 2*x)",
                new BinaryExpression(
                    new Function(FunctionKind.Sin,
                        new BinaryExpression(new ConstantExpression(2), new VariableExpression("x"), OperatorSign.Multiply)),
                    new BinaryExpression(new ConstantExpression(2), new VariableExpression("x"), OperatorSign.Multiply),
                    OperatorSign.Plus)),

            ("10. Complex expression with shared subtree",
                new BinaryExpression(
                    new BinaryExpression(
                        new Function(FunctionKind.Sin,
                            new BinaryExpression(new ConstantExpression(3), new VariableExpression("y"), OperatorSign.Multiply)),
                        new BinaryExpression(new ConstantExpression(3), new VariableExpression("y"), OperatorSign.Multiply),
                        OperatorSign.Minus),
                    new Function(FunctionKind.Cos, new VariableExpression("y")),
                    OperatorSign.Plus))
        };

        foreach (var (name, expr) in tests)
        {
            RunTest(name, expr);
            Console.WriteLine(new string('-', 70));
        }
    }

    static void RunTest(string name, IExpression expr)
    {
        Console.WriteLine($"\n{name}:");
        HashSet<int> uniqueBefore = new();
        CountUnique(expr, uniqueBefore);
        Console.WriteLine($"Original expression: {expr}");
        Console.WriteLine($"Unique subexpressions (before): {uniqueBefore.Count}");

        var optimized = ExpressionOptimizer.Optimize(expr)!;

        HashSet<int> uniqueAfter = new();
        CountUnique(optimized, uniqueAfter);
        Console.WriteLine($"Optimized expression: {optimized}");
        Console.WriteLine($"Unique subexpressions (after): {uniqueAfter.Count}");
        Console.WriteLine($"Duplicates removed: {uniqueBefore.Count - uniqueAfter.Count}");
    }

    static void CountUnique(IExpression expr, HashSet<int> set)
    {
        int key = expr.GetHashCode()!;
        set.Add(key);

        if (expr is IBinaryExpression bin)
        {
            CountUnique(bin.Left, set);
            CountUnique(bin.Right, set);
        }
        else if (expr is IFunction func)
        {
            CountUnique(func.Argument, set);
        }
    }
}