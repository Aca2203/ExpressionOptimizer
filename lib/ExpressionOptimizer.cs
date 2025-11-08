using Expressions;

namespace ExpressionOptimizer
{
    internal class ExpressionOptimizer
    {
        public static IExpression? Optimize(IExpression expression)
        {
            if (expression == null) return null;

            Dictionary<string, IExpression> cache = new Dictionary<string, IExpression>();
            ExpressionTreeTraverser traverser = new ExpressionTreeTraverser(expression);

            IExpression result = expression;
            cache.Clear();

            return result;
        }
    }
}
