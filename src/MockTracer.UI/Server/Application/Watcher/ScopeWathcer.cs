using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;
using MockTracer.UI.Server.Application.Storage;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Watcher;

/// <summary>
/// scope store
/// </summary>
public class ScopeWatcher : IDisposable, IScopeWatcher
{
  private bool _disposedValue;

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

  private readonly MockTracerDbContext _context;

  private readonly Guid ScopeId = VariableMaster.Next();

  internal static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions() { WriteIndented = true, PropertyNameCaseInsensitive = true };

  /// <summary>
  /// 
  /// </summary>
  /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
  public ScopeWatcher(IServiceProvider serviceProvider)
  {
    _context = serviceProvider.GetRequiredService<MockTracerDbContext>();
  }

  /// <summary>
  /// Add input data
  /// </summary>
  /// <param name="trace"><see cref="TraceInfo"/></param>
  /// <param name="serviceData"><see cref="ArgumentObjectInfo"/></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void AddInputAsync(TraceInfo trace, params ArgumentObjectInfo[]? serviceData)
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
  }

  private Input InputArgs(ArgumentObjectInfo item)
  {
    try
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
    catch (Exception ex)
    {
      return new Input()
      {
        Id = VariableMaster.Next(),
        Name = item.ArgumentName,
        ClassName = item.ClassName,
        Namespace = item.Namespace,
        Json = string.Empty,
        ShortView = "don't serialised object: " + ex.ToString(),
        SharpCode = $"/* don't serialised object  {ex.Message}*/",
        IsFilled = false,
      };
    }
  }

  /// <summary>
  /// add output info
  /// </summary>
  /// <param name="trace"><see cref="TraceInfo"/></param>
  /// <param name="serviceData"><see cref="ArgumentObjectInfo"/></param>
  /// <exception cref="ArgumentNullException"></exception>
  public void AddOutputAsync(TraceInfo trace, ArgumentObjectInfo? serviceData)
  {
    var row = _stack.Pop();
    if (serviceData != null)
    {
      row.Output = OutputArgs(serviceData);
    }
  }


  private Output OutputArgs(ArgumentObjectInfo item)
  {
    try
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
    catch (Exception ex)
    {
      return new Output()
      {
        Id = VariableMaster.Next(),
        Name = item.ArgumentName,
        ClassName = item.ClassName,
        Namespace = item.Namespace,
        Json = string.Empty,
        ShortView = "don't serialised object: " + ex.ToString(),
        SharpCode = $"/* don't serialised object  {ex.Message} */",
        IsFilled = false,
      };
    }
  }

  private ExceptionType Exception(Exception ex)
  {
    try
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
    catch (Exception)
    {
      return new ExceptionType()
      {
        Id = VariableMaster.Next(),
        Name = "ex",
        ClassName = ex.GetType().GetRealTypeName(),
        Namespace = ex.GetType().Namespace,
        Json = string.Empty,
        ShortView = ex.ToString(),
        SharpCode = $@"new {ex.GetType().GetRealTypeName()}(""{ex.Message}"");",
        IsFilled = false,
      };
    }
  }

  /// <summary>
  /// save exception info
  /// </summary>
  /// <param name="trace"><see cref="TraceInfo"/></param>
  /// <param name="ex">catched exception</param>
  /// <exception cref="ArgumentNullException"></exception>
  public void Catch(TraceInfo trace, Exception ex)
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
  }

  /// <inheritdoc/>
  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      Console.WriteLine("dispose");
      if (disposing)
      {
        if (_scope != null)
        {
          try
          {
            _context.SaveChanges();
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex);
          }
        }
        _context.Dispose();
      }
      _disposedValue = true;
    }
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}


