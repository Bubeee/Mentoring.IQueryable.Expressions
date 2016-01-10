using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sample03
{
    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
    {
        StringBuilder resultString;

        public string Translate(Expression exp)
        {
            resultString = new StringBuilder();
            Visit(exp);

            return resultString.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(String)
                && node.Method.Name == "StartsWith")
            {
                var stringToCompare = Expression.Constant(((ConstantExpression)node.Arguments[0]).Value + "*");

                var constantExpression = node.Object;
                if (constantExpression != null)
                {
                    var newExpression = Expression.MakeBinary(ExpressionType.Equal, constantExpression, stringToCompare);
                    return VisitBinary(newExpression);
                }
            }

            if (node.Method.DeclaringType == typeof(String)
                && node.Method.Name == "EndsWith")
            {
                var stringToCompare = Expression.Constant("*" +((ConstantExpression)node.Arguments[0]).Value);

                var constantExpression = node.Object;
                if (constantExpression != null)
                {
                    var newExpression = Expression.MakeBinary(ExpressionType.Equal, constantExpression, stringToCompare);
                    return VisitBinary(newExpression);
                }
            }

            if (node.Method.DeclaringType == typeof(String)
                && node.Method.Name == "Contains")
            {
                var stringToCompare = Expression.Constant("*" +((ConstantExpression)node.Arguments[0]).Value + "*");

                var constantExpression = node.Object;
                if (constantExpression != null)
                {
                    var newExpression = Expression.MakeBinary(ExpressionType.Equal, constantExpression, stringToCompare);
                    return VisitBinary(newExpression);
                }
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (!((node.Left.NodeType == ExpressionType.MemberAccess 
                        && node.Right.NodeType == ExpressionType.Constant) 
                        || (node.Left.NodeType == ExpressionType.Constant 
                        && node.Right.NodeType == ExpressionType.MemberAccess)))
                        throw new NotSupportedException(
                            string.Format("One of operands should be property or field and "
                                          + "the more one of operands should be constant"));

                    Visit(node.Left);
                    resultString.Append("(");
                    Visit(node.Right);
                    resultString.Append(")");
                    break;

                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            resultString.Append(node.Value);

            return node;
        }
    }
}
