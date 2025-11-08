using Expressions;

namespace ExpressionOptimizer
{
    internal class ExpressionTreeTraverser
    {
        private IExpression? node;
        private Stack<IExpression> stack = new Stack<IExpression>();

        public ExpressionTreeTraverser(IExpression root)
        {
            node = root;
        }     

        public IExpression? next()
        {
            if (node == null)
                return null;

            IExpression result = node;

            if (node.RightExpression != null)
                stack.Push(node.RightExpression);

            if (node.LeftExpression != null)
                node = node.LeftExpression;
            else if (stack.Count > 0)
                node = stack.Pop();            
            else
                node = null;            

            return result;
        }
    }
}
