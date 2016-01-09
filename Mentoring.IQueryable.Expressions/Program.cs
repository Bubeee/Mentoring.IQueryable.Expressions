using System;
using System.Linq.Expressions;

namespace Mentoring.IQueryable.Expressions
{
    public class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<int, int>> expr = a => a + 1;
            Expression<Func<int, int>> expr2 = a => a - 1;
            Console.WriteLine(expr);

            var treeModifierExpressionVisitor = new TransformingExpressionVisitor();
            var modifiedExpr = treeModifierExpressionVisitor.Modify(expr);
            var modifiedExpr2 = treeModifierExpressionVisitor.Modify(expr2);

            Console.WriteLine(modifiedExpr);
            Console.WriteLine(modifiedExpr2);
            Console.ReadLine();
        }
    }
}