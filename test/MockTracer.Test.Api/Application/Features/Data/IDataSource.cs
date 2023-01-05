using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Application.Features.Data;

public interface IDataSource
{
  DateTime SystemDate(int integer, string name);

  DataRecord SingleRow(int integer, string name);

  Task<List<DataRecord>> MultupleQueryAsync(int integer, string name);

  int ExecuteNonQuery();

  Task<List<User>> GetUsersAsync();
  
}
