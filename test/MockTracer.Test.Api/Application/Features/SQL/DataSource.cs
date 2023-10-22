using Dapper;
using Microsoft.Data.Sqlite;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Application.Features.SQL;

public class DataSource : IDataSource
{
  private readonly IDbProvider _dbProvider;

  public DataSource(IDbProvider dbProvider)
  {
    _dbProvider = dbProvider;
  }

  public int ExecuteNonQuery()
  {
    using var con = _dbProvider.GetDbConnection();

    return con.Execute("select @number2", new { date1 = DateTime.Now, number2 = 666, strCode = DateTime.Now.ToString("MMM") });
  }

  public async Task<List<User>> GetUsersAsync()
  {
    using var con = _dbProvider.GetDbConnection();

    return (await con.QueryAsync<User>("select * from users")).ToList();
  }

  public async Task<List<DataRecord>> MultupleQueryAsync(int integer, string name)
  {
    using var con = _dbProvider.GetDbConnection();
    con.Open();
    var reader = await con.QueryMultipleAsync("select @integer as SomeNumber, @name Name; select 123 as SomeNumber, 'supername' Name", new { integer, name });
    var result = new List<DataRecord>();
    result.AddRange(reader.Read<DataRecord>());
    result.AddRange(reader.Read<DataRecord>());
    return result;
  }

  public Task SetUsersAsync(UserDTO userDTO, CancellationToken cancellationToken)
  {
    using var con = _dbProvider.GetDbConnection();

    return con.ExecuteAsync("INSERT INTO users (Nick, Email, RegistrationDate, Type) VALUES (@Nick, @Email, @RegistrationDate, @Type); ", new { userDTO.Email, userDTO.RegistrationDate, userDTO.Nick, userDTO.Type });
  }

  public Task InvalidStatementAsync(UserDTO userDTO, CancellationToken cancellationToken)
  {
    using var con = _dbProvider.GetDbConnection();

    return con.ExecuteAsync("INSERT INTO users (Name666, Email, RegistrationDate, Type) VALUES (@Name, @Email, @RegistrationDate, @Type); ", userDTO);
  }

  public DataRecord SingleRow(int integer, string name)
  {
    using var con = _dbProvider.GetDbConnection();

    return con.QuerySingle<DataRecord>("select @integer as SomeNumber, @name Name", new { integer, name });
  }

  public DateTime SystemDate(int integer, string name)
  {
    using var con = _dbProvider.GetDbConnection();

    return con.ExecuteScalar<DateTime>("select date()", new { date1 = DateTime.Now.AddDays(-1), number2 = 666, strCode = DateTime.Now.ToString("MMM") });
  }
}
