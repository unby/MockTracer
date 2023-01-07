using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Watcher;
public class ScopeWathcer : IDisposable
{
  private bool _disposedValue;

  public Guid ScopeId { get; } = VariableMaster.Next();

  private readonly Stack<StackRow> _stack = new Stack<StackRow>();
  private int _counter = 1;
  private StackScope _scope;
  private readonly DumpOptions _sharpOptions = new DumpOptions()
  {
    TrimTrailingColonName = true,
    TrimInitialVariableName = true,
    DumpStyle = DumpStyle.CSharp,
    IgnoreDefaultValues = true,
    IgnoreIndexers = true
  };

  internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions() { WriteIndented = true };
  private readonly MockTracerDbContext _context;

  public ScopeWathcer(IServiceProvider serviceProvider)
  {
    _context = serviceProvider.GetRequiredService<MockTracerDbContext>();
  }

  public Task AddInputAsync(TraceInfo trace, params ServiceData[]? serviceData)
  {
    if (trace is null)
    {
      throw new ArgumentNullException(nameof(trace));
    }

    var time = VariableMaster.CurrentTime();
    StackRow? stackRow = _stack.Any() ? _stack.Peek() : null;

    if (stackRow == null)
    {
      _scope = new StackScope()
      {
        Id = ScopeId,
        FirstId = trace.TraceId,
        FirstType = trace.TracerType,
        Time = time,
        Title = trace.Title
      };
      stackRow = new StackRow()
      {
        Id = trace.TraceId,
        Order = _counter++,
        Title = trace.Title,
        Time = time,
        ScopeId = _scope.Id,
        StackTrace = JsonSerializer.Serialize(trace.StackTrace, JsonOptions),
        TracerType = trace.TracerType,
        IsEntry = true,
        DeepLevel = 1
      };
      _context.StackScopes.Add(_scope);
    }
    else
    {
      stackRow = new StackRow()
      {
        Id = trace.TraceId,
        ParentId = stackRow.Id,
        Title = trace.Title,
        Order = _counter++,
        Time = time,
        ScopeId = _scope.Id,
        StackTrace = JsonSerializer.Serialize(trace.StackTrace, JsonOptions),
        TracerType = trace.TracerType,
        IsEntry = false,
        DeepLevel = 1 + stackRow.DeepLevel
      };
    }

    if (serviceData != null && serviceData.Length > 0)
    {
      stackRow.Input = new List<Input>(serviceData.Length);
      foreach (var item in serviceData)
      {
        stackRow.Input.Add(InputArgs(item));
      }
    }
    _stack.Push(stackRow);
    _scope.Stack.Add(stackRow);
    return Task.CompletedTask;
  }

  private Input InputArgs(ServiceData item)
  {
    return new Input()
    {
      Id = VariableMaster.Next(),
      Name = item.ArgumentName,
      ClassName = item.ClassName,
      Namespace = item.Namespace,
      AddInfo = item.AdvancedInfo != null ? JsonSerializer.Serialize(item.AdvancedInfo, JsonOptions) : null,
      Json = JsonSerializer.Serialize(item.OriginalObject, JsonOptions),
      ShortView = ObjectDumper.Dump(item.OriginalObject),
      SharpCode = ObjectDumper.Dump(item.OriginalObject, _sharpOptions)
    };
  }

  public Task AddOutputAsync(TraceInfo trace, ServiceData? serviceData)
  {
    if (trace is null)
    {
      throw new ArgumentNullException(nameof(trace));
    }

    var row = _stack.Pop();
    if (serviceData != null)
    {
      row.Output = OutputArgs(serviceData);
    }

    return Task.CompletedTask;
  }


  private Output OutputArgs(ServiceData item)
  {
    if (item.OriginalObject == null)
    {
      return new Output()
      {
        Id = VariableMaster.Next(),
        Name = item.ArgumentName,
        ClassName = item.ClassName,
        Namespace = item.Namespace,
        Json = string.Empty,
        AddInfo = item.AdvancedInfo != null ? JsonSerializer.Serialize(item.AdvancedInfo, JsonOptions) : null,
        ShortView = "null",
        SharpCode = "null;"
      };
    }
    return new Output()
    {
      Id = VariableMaster.Next(),
      Name = item.ArgumentName,
      ClassName = item.ClassName,
      Namespace = item.Namespace,
      AddInfo = item.AdvancedInfo != null ? JsonSerializer.Serialize(item.AdvancedInfo, JsonOptions) : null,
      Json = JsonSerializer.Serialize(item.OriginalObject, JsonOptions),
      ShortView = ObjectDumper.Dump(item.OriginalObject),
      SharpCode = ObjectDumper.Dump(item.OriginalObject, _sharpOptions)
    };
  }

  private ExceptionType Exception(Exception ex)
  {
    return new ExceptionType()
    {
      Id = VariableMaster.Next(),
      Name = "ex",
      ClassName = ex.GetType().GetRealTypeName(),
      Namespace = ex.GetType().Namespace,
      Json = JsonSerializer.Serialize(ex, JsonOptions),
      ShortView = ex.ToString(),
      SharpCode = $@"new {ex.GetType().GetRealTypeName()}(""{ex.Message}"");"
    };
  }

  public Task Catch(TraceInfo trace, Exception ex)
  {
    if (trace is null)
    {
      throw new ArgumentNullException(nameof(trace));
    }

    if (ex is null)
    {
      throw new ArgumentNullException(nameof(ex));
    }

    var row = _stack.Pop();

    row.Exception = Exception(ex);


    return Task.CompletedTask;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        if (_scope != null)
        {
          try
          {
            _context.SaveChanges();
          }
          catch (Exception)
          {
            throw;
          }
        }
        _context.Dispose();
      }
      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}


