using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mentoring.IQueryable.Expressions
{
    public class TransformingExpressionVisitor : ExpressionVisitor
    {
        private Dictionary<ParameterExpression, ConstantExpression> _parameterValues;

        public void SetParameterValues(Dictionary<ParameterExpression, ConstantExpression> parameterValues)
        {
            this._parameterValues = parameterValues;
        }

        public Expression Modify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = this.Visit(node.Left);
            var right = this.Visit(node.Right);

            if (right.NodeType == ExpressionType.Constant && left.NodeType == ExpressionType.Parameter && (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract))
            {
                var value = this.EvaluateExpression(right);
                if (value.GetValueOrDefault() == 1)
                {
                    var operation = node.NodeType == ExpressionType.Add
                                       ? ExpressionType.Increment
                                       : ExpressionType.Decrement;

                    return Expression.MakeUnary(operation, left, typeof(int));
                }

            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            var body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }

            return lambda;
        }

        protected int? EvaluateExpression(Expression expression)
        {
            var lambda = Expression.Lambda(expression);

            var compiled = lambda.Compile();

            var value = compiled.DynamicInvoke(null);
            return (int?)value;
        }
        
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (this._parameterValues == null || this._parameterValues.Count == 0)
            {
                return base.VisitParameter(node);
            }

            ConstantExpression replacement;

            if (this._parameterValues.TryGetValue(node, out replacement))
            {
                return replacement;
            }

            return base.VisitParameter(node);
        }
    }
}