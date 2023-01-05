using Microsoft.EntityFrameworkCore;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Server.Application.Generation.IntenalTest;

/// <summary>
/// Internal service usage
/// </summary>
public class InternalTestClassGenerator
{
  private MockTracerDbContext _context;
  private readonly DumpOptions _sharpOptions = new DumpOptions() { TrimTrailingColonName = true, TrimInitialVariableName = true, DumpStyle = DumpStyle.CSharp, IgnoreDefaultValues = true, IgnoreIndexers = true };
  public InternalTestClassGenerator(MockTracerDbContext context)
  {
    _context = context;
  }

  /// <summary>
  /// Generate tool's test
  /// </summary>
  /// <param name="params"></param>
  /// <returns></returns>
  internal async Task<TestFile> MakeTestAsync(GenerationAttributes @params)
  {
    var scope = await _context.StackScopes.Where(w => w.Id == @params.ScopeId).Include(w => w.Stack).ThenInclude(w => w.Exception).Include(w => w.Stack).ThenInclude(w => w.Input).Include(w => w.Stack).ThenInclude(w => w.Output).FirstAsync();
    scope.Stack = scope.Stack.Where(w => @params.InputId == w.Id || @params.OutputId.Contains(w.Id)).ToList();
    var entities = _context.ChangeTracker.Entries().Select(s => s.Entity).ToArray();
    foreach (TracedObject item in entities.Where(w => w is TracedObject))
    {
      item.ShortView = string.Empty;
    }
    var className = $"{scope.FirstType}_{scope.Title}_test".ClearFileName();

    return new TestFile() { SourceCode = BuildFile(ObjectDumper.Dump(scope, _sharpOptions), ObjectDumper.Dump(@params, _sharpOptions), className, scope.FirstType), FileName = className + ".cs" };
  }

  private string BuildFile(string data, string @params, string className, string method)
  {
    return $@"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.Tool.Test;

public class {className} : ToolTestBase
{{
  public {className}(Xunit.Abstractions.ITestOutputHelper output)
    : base(output)
  {{
  }}

  [Fact]
  public async Task Should_create_{method}_template_Async()
  {{
    var generator = NewServer().Services.GetService<TestClassGenerator>();
    var newClass = await generator.CreateAsync(Attributes);
    
    Assert.NotNull(newClass.SourceCode);
  }}

  protected GenerationAttributes Attributes => {@params};

  protected override void AfterBuildConfiguration(IHost host)
  {{
    base.AfterBuildConfiguration(host);
    using var scope = host.Services.CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<MockTracerDbContext>();
    context.AddRange({data});
    context.SaveChanges();
  }}
}}
";
  }
}
