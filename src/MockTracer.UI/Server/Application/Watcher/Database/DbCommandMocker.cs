using System.Data;
using System.Data.Common;
using MockTracer.UI.Server.Application.Common;
using MockTracer.UI.Server.Application.Generation;

namespace MockTracer.UI.Server.Application.Watcher.Database;

public sealed class DbCommandMocker : DbCommand, IDbCommand, ITracer, IDataFlusher
{
  private TraceInfo _traceInfo = null;
  public TraceInfo CreateInfo(string title)
  {
    if (_traceInfo == null)
    {
      _traceInfo = new TraceInfo()
      {
        TraceId = VariableMaster.Next(),
        Title = title,
        TracerType = Constants.DbConnection
      };
    }

    return _traceInfo;
  }
  private List<DataSet> result;
  public void FlushResult(List<DataSet> result)
  {
    this.result = result;
  }

  private readonly DbCommand _dbCommand;
  private ScopeWatcher _traceStore;
  private (string? nameSpace, string name)? _dbProviderType;
  private bool _isRunAsync;

  public DbCommandMocker(DbCommand dbCommand, ScopeWatcher traceStore, Type? dbProviderType)
  {
    _dbCommand = dbCommand ?? throw new ArgumentNullException(nameof(dbCommand));
    _traceStore = traceStore ?? throw new ArgumentNullException(nameof(traceStore));
    _dbProviderType = dbProviderType?.GetRealFullTypeName();
  }

  public override string CommandText { get => _dbCommand.CommandText; set => _dbCommand.CommandText = value; }

  public override int CommandTimeout { get => _dbCommand.CommandTimeout; set => _dbCommand.CommandTimeout = value; }

  public override CommandType CommandType { get => _dbCommand.CommandType; set => _dbCommand.CommandType = value; }

  public IDbConnection Connection { get => _dbCommand.Connection; }

  public IDataParameterCollection Parameters => _dbCommand.Parameters;

  public IDbTransaction? Transaction { get => _dbCommand.Transaction; }

  public override UpdateRowSource UpdatedRowSource { get => _dbCommand.UpdatedRowSource; set => _dbCommand.UpdatedRowSource = value; }

  public override bool DesignTimeVisible { get => _dbCommand.DesignTimeVisible; set => _dbCommand.DesignTimeVisible = value; }

  protected override DbConnection? DbConnection { get => _dbCommand.Connection; set => _dbCommand.Connection = value; }

  protected override DbParameterCollection DbParameterCollection => _dbCommand.Parameters;

  protected override DbTransaction? DbTransaction { get => _dbCommand.Transaction; set => _dbCommand.Transaction = value; }

  public override void Cancel()
  {
    _dbCommand.Cancel();
  }

  public IDbDataParameter CreateParameter()
  {
    return _dbCommand.CreateParameter();
  }

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
        ClassName = _dbProviderType?.name,
        Namespace = _dbProviderType?.nameSpace,
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

  public IDataReader ExecuteReader()
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
      ClassName = _dbProviderType?.name ?? string.Empty,
      Namespace = _dbProviderType?.nameSpace,
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

  public IDataReader ExecuteReader(CommandBehavior behavior)
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
      ClassName = _dbProviderType?.name ?? string.Empty,
      Namespace = _dbProviderType?.nameSpace,
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
        ClassName = _dbProviderType?.name,
        Namespace = _dbProviderType?.nameSpace,
        OriginalObject = new DbCommandInput()
        {
          CommandType = CommandType,
          CommandText = CommandText,
          Parameters = pars.ToList()
        }
      });
      object result = _dbCommand.ExecuteScalar();
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

  public override void Prepare()
  {
    _dbCommand.Prepare();
  }

  protected override DbParameter CreateDbParameter()
  {
    return _dbCommand.CreateParameter();
  }

  protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
  {
    var tarceInfo = CreateInfo(Connection.GetType().GetRealTypeName() + " " + nameof(ExecuteReader));
    var pars = _dbCommand.Parameters.Cast<DbParameter>().Select(s => new MockParameter() { Name = s.ParameterName, Type = s.DbType.ToString(), Value = s.Value, ParameterDirection = s.Direction });
    _traceStore.AddInputAsync(tarceInfo, new ArgumentObjectInfo()
    {
      ArgumentName = "input",
      ClassName = _dbProviderType?.name,
      Namespace = _dbProviderType?.nameSpace,
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
