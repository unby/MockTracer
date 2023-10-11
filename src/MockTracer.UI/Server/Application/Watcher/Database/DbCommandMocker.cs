using System.Data;
using System.Data.Common;
using System.Reflection;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

namespace MockTracer.UI.Server.Application.Watcher.Database;

/// <inheritdoc/>
public sealed class DbCommandMocker : DbCommand, IDbCommand, ITracer, IDataFlusher
{
  private TraceInfo? _traceInfo = null;
  private List<DataSet> result;
  private readonly DbCommand _dbCommand;
  private ScopeWatcher _traceStore;
  private (string? nameSpace, string name) _dbProviderType;
  private bool _isRunAsync;


  /// <summary>
  /// DbCommandMocker
  /// </summary>
  /// <param name="dbCommand"><see cref="DbCommand"/></param>
  /// <param name="traceStore"><see cref="ScopeWatcher"/></param>
  /// <param name="dbProviderType"><see cref="Type"/></param>
  public DbCommandMocker(DbCommand dbCommand, ScopeWatcher traceStore, Type dbProviderType)
  {
    _dbCommand = dbCommand;
    _traceStore = traceStore;
    _dbProviderType = dbProviderType.GetRealFullTypeName();
  }

  /// <inheritdoc/>
  public TraceInfo CreateInfo(string title, Type? type = null, MethodInfo? methodInfo = null, Type? outputType = null)
  {
    _traceInfo ??= new TraceInfo()
    {
      TraceId = VariableMaster.Next(),
      Title = title,
      TracerType = Constants.DbConnection
    };

    return _traceInfo;
  }

  /// <inheritdoc/>
  public void FlushResult(List<DataSet> result)
  {
    this.result = result;
  }

  /// <inheritdoc/>
  public override string CommandText { get => _dbCommand.CommandText; set => _dbCommand.CommandText = value; }

  /// <inheritdoc/>
  public override int CommandTimeout { get => _dbCommand.CommandTimeout; set => _dbCommand.CommandTimeout = value; }

  /// <inheritdoc/>
  public override CommandType CommandType { get => _dbCommand.CommandType; set => _dbCommand.CommandType = value; }

  /// <inheritdoc/>
  public new IDbConnection Connection { get => _dbCommand.Connection; }

  /// <inheritdoc/>
  public new IDataParameterCollection Parameters => _dbCommand.Parameters;

  /// <inheritdoc/>
  public new IDbTransaction? Transaction { get => _dbCommand.Transaction; }

  /// <inheritdoc/>
  public override UpdateRowSource UpdatedRowSource { get => _dbCommand.UpdatedRowSource; set => _dbCommand.UpdatedRowSource = value; }

  /// <inheritdoc/>
  public override bool DesignTimeVisible { get => _dbCommand.DesignTimeVisible; set => _dbCommand.DesignTimeVisible = value; }

  /// <inheritdoc/>
  protected override DbConnection? DbConnection { get => _dbCommand.Connection; set => _dbCommand.Connection = value; }

  /// <inheritdoc/>
  protected override DbParameterCollection DbParameterCollection => _dbCommand.Parameters;

  /// <inheritdoc/>
  protected override DbTransaction? DbTransaction { get => _dbCommand.Transaction; set => _dbCommand.Transaction = value; }

  /// <inheritdoc/>
  public override void Cancel()
  {
    _dbCommand.Cancel();
  }

  /// <inheritdoc/>
  public new IDbDataParameter CreateParameter()
  {
    return _dbCommand.CreateParameter();
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    if (_isRunAsync)
    {
      var tarceInfo = _traceInfo;
      _traceStore.AddOutputAsync(tarceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "dataset",
        ClassName = result.GetType().GetRealTypeName(),
        OriginalObject = result,
        AdvancedInfo = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
        {
          Name = s.ParameterName,
          Type = s.DbType.ToString(),
          Value = s.Value,
          ParameterDirection = s.Direction,
        }).Where(w => w.ParameterDirection != ParameterDirection.Input).ToList(),
        Namespace = result.GetType().Namespace,
      });
    }
    _dbCommand.Dispose();
    _traceInfo = null;
  }

  /// <inheritdoc/>
  public override int ExecuteNonQuery()
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteNonQuery));
    try
    {
      var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
      {
        Name = s.ParameterName,
        Type = s.DbType.ToString(),
        Value = s.Value,
        ParameterDirection = s.Direction
      });
      _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "input",
        ClassName = _dbProviderType.name,
        Namespace = _dbProviderType.nameSpace,
        OriginalObject = new DbCommandInput()
        {
          CommandType = CommandType,
          CommandText = CommandText,
          Parameters = pars.ToList()
        }
      });
      var nonQuery = _dbCommand.ExecuteNonQuery();
      _traceStore.AddOutputAsync(tarceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "executeCode",
        ClassName = nonQuery.GetType().GetRealTypeName(),
        OriginalObject = nonQuery,
        AdvancedInfo = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
        {
          Name = s.ParameterName,
          Type = s.DbType.ToString(),
          Value = s.Value,
          ParameterDirection = s.Direction
        }).Where(w => w.ParameterDirection != ParameterDirection.Input).ToList(),
        Namespace = nonQuery.GetType().Namespace,
      });

      return nonQuery;
    }
    catch (Exception ex)
    {
      _traceStore.Catch(tarceInfo, ex);
      throw;
    }
  }

  /// <inheritdoc/>
  public new IDataReader ExecuteReader()
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteReader));
    var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
    {
      Name = s.ParameterName,
      Type = s.DbType.ToString(),
      Value = s.Value,
      ParameterDirection = s.Direction
    });
    _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
    {
      ArgumentName = "input",
      ClassName = _dbProviderType.name,
      Namespace = _dbProviderType.nameSpace,
      OriginalObject = new DbCommandInput()
      {
        CommandType = CommandType,
        CommandText = CommandText,
        Parameters = pars.ToList()
      }
    });
    var reader = new MockerDataReader(_dbCommand.ExecuteReader(), this);
    _isRunAsync = true;
    return reader;
  }

  /// <inheritdoc/>
  public new IDataReader ExecuteReader(CommandBehavior behavior)
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteReader));
    var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
    {
      Name = s.ParameterName,
      Type = s.DbType.ToString(),
      Value = s.Value,
      ParameterDirection = s.Direction
    });
    _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
    {
      ArgumentName = "input",
      ClassName = _dbProviderType.name ?? string.Empty,
      Namespace = _dbProviderType.nameSpace,
      OriginalObject = new DbCommandInput()
      {
        CommandType = CommandType,
        CommandText = CommandText,
        Parameters = pars.ToList()
      }
    });
    var reader = new MockerDataReader(_dbCommand.ExecuteReader(behavior), this);
    _isRunAsync = true;
    return reader;
  }

  /// <inheritdoc/>
  public override object? ExecuteScalar()
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteScalar));

    try
    {
      var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
      {
        Name = s.ParameterName,
        Type = s.DbType.ToString(),
        Value = s.Value,
        ParameterDirection = s.Direction
      });
      _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "input",
        ClassName = _dbProviderType.name,
        Namespace = _dbProviderType.nameSpace,
        OriginalObject = new DbCommandInput()
        {
          CommandType = CommandType,
          CommandText = CommandText,
          Parameters = pars.ToList()
        }
      });
      object? result = _dbCommand.ExecuteScalar();
      var resultType = result?.GetType() ?? typeof(object);
      _traceStore.AddOutputAsync(tarceInfo, new ArgumentObjectInfo()
      {
        ArgumentName = "firstColumn",
        ClassName = resultType.GetRealTypeName(),
        OriginalObject = result,
        AdvancedInfo = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter()
        {
          Name = s.ParameterName,
          Type = s.DbType.ToString(),
          Value = s.Value,
          ParameterDirection = s.Direction
        }).Where(w => w.ParameterDirection != ParameterDirection.Input).ToList(),
        Namespace = resultType.Namespace,
      });

      return result;
    }
    catch (Exception ex)
    {
      _traceStore.Catch(tarceInfo, ex);
      throw;
    }
  }

  /// <inheritdoc/>
  public override void Prepare()
  {
    _dbCommand.Prepare();
  }

  /// <inheritdoc/>
  protected override DbParameter CreateDbParameter()
  {
    return _dbCommand.CreateParameter();
  }

  /// <inheritdoc/>
  protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteReader));
    var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter() { Name = s.ParameterName, Type = s.DbType.ToString(), Value = s.Value, ParameterDirection = s.Direction });
    _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
    {
      ArgumentName = "input",
      ClassName = _dbProviderType.name,
      Namespace = _dbProviderType.nameSpace,
      OriginalObject = new DbCommandInput()
      {
        CommandType = CommandType,
        CommandText = CommandText,
        Parameters = pars.ToList()
      }
    });
    _isRunAsync = true;
    return new MockerDataReader(_dbCommand.ExecuteReader(), this);
  }
}
