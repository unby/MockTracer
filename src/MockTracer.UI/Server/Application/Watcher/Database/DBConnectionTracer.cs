using System.Data;
using System.Data.Common;

namespace MockTracer.UI.Server.Application.Watcher.Database;

/// <summary>
/// db connection for listening sql query
/// </summary>
public class DBConnectionTracer : DbConnection, IDbConnection
{
  private readonly DbConnection _dbConnection;
  private readonly ScopeWathcer _traceStore;
  private readonly Type? _dbProviderType;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="dbConnection">manual connection</param>
  /// <param name="traceStore">traceStore from DI</param>
  /// <param name="dbProviderType">don't set this arguments</param>
  /// <exception cref="ArgumentException"></exception>
  public DBConnectionTracer(IDbConnection dbConnection, ScopeWathcer traceStore, Type? dbProviderType = null)
  {
    _dbConnection = dbConnection as DbConnection ?? throw new ArgumentException(nameof(dbConnection)); ;
    _traceStore = traceStore;
    _dbProviderType = dbProviderType;
  }

  /// <inheritdoc/>
  public override string ConnectionString { get => _dbConnection.ConnectionString; set => _dbConnection.ConnectionString = value; }

  /// <inheritdoc/>
  public int ConnectionTimeout => _dbConnection.ConnectionTimeout;

  /// <inheritdoc/>
  public override string Database => _dbConnection.Database;

  /// <inheritdoc/>
  public override ConnectionState State => _dbConnection.State;

  /// <inheritdoc/>
  public override string DataSource => _dbConnection.DataSource;

  /// <inheritdoc/>
  public override string ServerVersion => _dbConnection.ServerVersion;

  /// <inheritdoc/>
  public IDbTransaction BeginTransaction()
  {
    return _dbConnection.BeginTransaction();
  }

  /// <inheritdoc/>
  public IDbTransaction BeginTransaction(IsolationLevel il)
  {
    return _dbConnection.BeginTransaction(il);
  }

  /// <inheritdoc/>
  public override void ChangeDatabase(string databaseName)
  {
    _dbConnection.ChangeDatabase(databaseName);
  }

  /// <inheritdoc/>
  public override void Close()
  {
    _dbConnection?.Close();
  }

  /// <inheritdoc/>
  public IDbCommand CreateCommand()
  {
    return new DbCommandMocker(_dbConnection.CreateCommand(), _traceStore, _dbProviderType);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    _dbConnection.Dispose();
  }

  /// <inheritdoc/>
  public override void Open()
  {
    _dbConnection.Open();
  }

  /// <inheritdoc/>
  protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
  {
    return _dbConnection.BeginTransaction();
  }

  /// <inheritdoc/>
  protected override DbCommand CreateDbCommand()
  {
    return _dbConnection.CreateCommand();
  }
}
