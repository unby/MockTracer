using System.Data;
using Microsoft.Data.Sqlite;
using MockTracer.UI.Server.Application.Watcher;
using MockTracer.UI.Server.Application.Watcher.Database;

namespace MockTracer.Test.Api.Application.Features.Data;

public class DbProvider : IDbProvider
{
  private readonly string _conString;

  public DbProvider(string conString)
  {
    _conString = conString;
  }

  public IDbConnection GetDbConnection()
  {
    return new SqliteConnection(_conString);
  }
}

public class DbProvider2 : IDbProvider
{
  private readonly IDbProvider _dbProvider;
  private readonly ScopeWathcer _scopeWathcer;

  public DbProvider2(IDbProvider dbProvider, ScopeWathcer scopeWathcer)
  {
    _dbProvider = dbProvider;
    _scopeWathcer = scopeWathcer;
  }

  public IDbConnection GetDbConnection()
  {
    return new DBConnectionTracer(_dbProvider.GetDbConnection(), _scopeWathcer);
  }
}
