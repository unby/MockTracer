using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;
using DelegateDecompiler;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MockTracer.Test.Api.Application.Features.Topic;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.Database;
using Xunit.Abstractions;
using static MockTracer.Test.Api.Application.Features.Topic.TopicQueryHandler;

namespace MockTracer.Test.Manual;
public class EfInterceptorTest : SampleTestBase
{
  public EfInterceptorTest(ITestOutputHelper output) : base(output)
  {
  }

  [Fact]
  public async Task RunAsync()
  {
    var host = NewServer();

    var context = host.GetInstance<BlogDbContext>();
    var result = await (from t in context.Topics.Where(w => w.Id > 0).Select(s => (Topic)new Topic2
    {
      Id = s.Id,
      Title = s.Title,
      Content = s.Content,
      Created = s.Created,
      Author = s.Author,
      AuthorId = s.AuthorId,
      Comments = s.Comments,
    })
                        join u in context.Users on t.AuthorId equals u.Id
                        join c in context.Comments on u.Id equals c.UserId
                        select new TopicDto { Id = t.Id, Title = t.Title, AuthorName = u.Nick }).ToListAsync();

    // todo: assert
    
    Assert.NotEmpty(result);
    Assert.NotEmpty(context.ChangeTracker.Entries().Select(s => s.Entity));
  }

  protected override Action<IServiceCollection> ConfigureHost(string testName)
  {
    return (s) =>
    {
      s.RemoveAll<DbContextOptions<BlogDbContext>>();

      s.AddScoped<IBlogDbContext, BlogDbContext>();
      s.AddDbContext<BlogDbContext>(options =>
            options.AddInterceptors(new MockProjectionExpressionInterceptor(Log)).UseInMemoryDatabase("BlogDbContext" + testName)/*.UseSqlite("Filename=Blog.db")*/);
      s.AddScoped<IBlogDbContext, BlogDbContext>();
    };
  }

  public class MockProjectionExpressionInterceptor : IQueryExpressionInterceptor
  {
    private ITestOutputHelper _log;

    public MockProjectionExpressionInterceptor(ITestOutputHelper log)
    {
      _log = log;
    }

    public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
    {
      var r = new MockProjectionExpressionVisitor(_log);
      var expression = r.Visit(queryExpression);
      foreach (var item in r.Types)
      {
        foreach (var generic in item.GenericTypeArguments)
        {
          _log.WriteLine(generic.Name);
        }
      
      }

      return expression;
    }

    private class MockProjectionExpressionVisitor : DecompileExpressionVisitor
    {
      private ITestOutputHelper _log;
      private int count = 0;
      public MockProjectionExpressionVisitor(ITestOutputHelper log)
      {
        _log = log;
      }

      public List<Type> Types { get; } = new List<Type>();

      protected override Expression VisitMethodCall(MethodCallExpression? methodCallExpression)
      {
        //https://code-maze.com/csharp-deep-copy-of-object/
        // _log.WriteLine("+" + nameof(VisitMethodCall) + methodCallExpression.ToString());
       
        Types.Add(methodCallExpression.Type);
        CheckNeedChanges(methodCallExpression);
        return base.VisitMethodCall(methodCallExpression);
      }
      protected override Expression VisitNew(NewExpression node)
      {
        CheckNeedChanges(node);
        return base.VisitNew(node);
      }

      protected override Expression VisitBlock(BlockExpression node)
      {
        string readable = node.ToReadableString();
        _log.WriteLine(nameof(VisitBlock) + readable);
        return base.VisitBlock(node);
      }

      protected override Expression VisitConditional(ConditionalExpression node)
      {
        string readable = node.ToReadableString();
        _log.WriteLine(nameof(VisitConditional) + readable);
        return base.VisitConditional(node);
      }

      protected override Expression VisitExtension(Expression node)
      {
        // string readable = node.ToReadableString();
        // _log.WriteLine(nameof(VisitExtension) + readable);
        // Types.Add(node.Type);
        CheckNeedChanges(node);
        return base.VisitExtension(node);
      }

      /* [return: NotNullIfNotNull("node")]
       public override Expression? Visit(Expression? node)
       {
         string readable = node.ToReadableString();
         _log.WriteLine(nameof(Visit) + readable);
         return base.Visit(node);
       }*/

      protected override Expression VisitDynamic(DynamicExpression node)
      {
        string readable = node.ToReadableString();
        _log.WriteLine(nameof(DynamicExpression) + readable);
        return base.VisitDynamic(node);
      }

      private bool CheckNeedChanges(Expression expression, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
      {
        if (expression.Type.IsGenericType)
        {
          var genericType = expression.Type.GenericTypeArguments.FirstOrDefault();
          if (genericType != null)
          {
            string readable = expression.ToReadableString();
            _log.WriteLine($"{count++} {callerName} out:{genericType.Name} {readable}");
            return true;
          }
        }

        return false;
      }

      public static bool IsGenericTypeOf(Type t, Type genericDefinition, out Type[] genericParameters)
      {
        genericParameters = new Type[] { };
        if (!genericDefinition.IsGenericType)
        {
          return false;
        }

        var isMatch = t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
        if (!isMatch && t.BaseType != null)
        {
          isMatch = IsGenericTypeOf(t.BaseType, genericDefinition, out genericParameters);
        }
        if (!isMatch && genericDefinition.IsInterface && t.GetInterfaces().Any())
        {
          foreach (var i in t.GetInterfaces())
          {
            if (IsGenericTypeOf(i, genericDefinition, out genericParameters))
            {
              isMatch = true;
              break;
            }
          }
        }

        if (isMatch && !genericParameters.Any())
        {
          genericParameters = t.GetGenericArguments();
        }
        return isMatch;
      }
    }
  }
}

