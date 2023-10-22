using System.Linq.Expressions;
using System.Reflection;
using AgileObjects.ReadableExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MockTracer.Test.Api.Infrastracture.Database;

public class KeyOrderingExpressionInterceptor : IQueryExpressionInterceptor
{
  public Expression QueryCompilationStarting(
      Expression queryExpression,
      QueryExpressionEventData eventData)
       => new KeyOrderingExpressionVisitor().Visit(queryExpression);
   

  private class KeyOrderingExpressionVisitor : ExpressionVisitor
  {
    private static readonly MethodInfo ThenByMethod
        = typeof(Queryable).GetMethods()
            .Single(m => m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);

    protected override Expression VisitMethodCall(MethodCallExpression? methodCallExpression)
    {
      //https://code-maze.com/csharp-deep-copy-of-object/
      Console.WriteLine(methodCallExpression);
      var methodInfo = methodCallExpression!.Method;
      if (methodInfo.DeclaringType == typeof(Queryable)
          && methodInfo.Name == nameof(Queryable.OrderBy)
          && methodInfo.GetParameters().Length == 2)
      {
        Console.WriteLine(methodInfo);
       
      }
      string readable = methodCallExpression.ToReadableString();
      Console.WriteLine(readable);
      return base.VisitMethodCall(methodCallExpression);
    }
  }
}
