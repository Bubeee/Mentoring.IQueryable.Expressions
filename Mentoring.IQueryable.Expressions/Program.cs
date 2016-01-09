using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            Console.WriteLine(expr2);

            var treeModifierExpressionVisitor = new TransformingExpressionVisitor();
            
            Expression<Func<int, int, int>> expr3 = (a, b) => a * a - b * 5 - 1;
            Console.WriteLine(expr3);

            var listWithValues = new List<int> { 5, 4 };
            var parameterValuesDictionary = expr3.Parameters.Select((parameter, index) => new { parameter, index })
                .ToDictionary(arg => arg.parameter, arg => Expression.Constant(listWithValues[arg.index]));

            treeModifierExpressionVisitor.SetParameterValues(parameterValuesDictionary);

            var modifiedExpr = (Expression<Func<int, int>>)treeModifierExpressionVisitor.Modify(expr);
            var modifiedExpr2 = (Expression<Func<int, int>>)treeModifierExpressionVisitor.Modify(expr2);
            var modifiedExpr3 = (Expression<Func<int, int, int>>)treeModifierExpressionVisitor.Modify(expr3);

            Console.WriteLine(modifiedExpr);
            Console.WriteLine(modifiedExpr2);
            Console.WriteLine(modifiedExpr3);

            Console.WriteLine(modifiedExpr.Compile().Invoke(0));
            Console.WriteLine(modifiedExpr2.Compile().Invoke(0));
            Console.WriteLine(modifiedExpr3.Compile().Invoke(0, 0));

            Console.ReadLine();
        }
    }
}