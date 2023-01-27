using MediatR;
using MockTracer.Test.Api.Application.Features.Data;
using MockTracer.Test.Api.Domain;
using static MockTracer.Test.Api.Application.Features.Topic.SqldDataComandHandler;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class SqldDataComandHandler :
  IRequestHandler<ExecuteNonQuery, int>,
  IRequestHandler<MultupleQueryAsync, List<DataRecord>>,
  IRequestHandler<SingleRow, DataRecord>,
  IRequestHandler<SystemDate, DateTime>,
  IRequestHandler<UserList, List<User>>
{
  private readonly IDataSource _service;

  public SqldDataComandHandler(IDataSource service)
  {
    _service = service;
  }

  public Task<int> Handle(ExecuteNonQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var x = _service.ExecuteNonQuery();
      return Task.FromResult(x);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return Task.FromResult(-1);
    }
  }
  public class ExecuteNonQuery : IRequest<int>
  {
  }

  public class MultupleQueryAsync : IRequest<List<DataRecord>>
  {
  }

  public async Task<List<DataRecord>> Handle(MultupleQueryAsync request, CancellationToken cancellationToken)
  {
    try
    {
      return await _service.MultupleQueryAsync(500, "MultupleQueryAsync");

    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return null;
    }
  }
  public class SingleRow : IRequest<DataRecord>
  {
  }

  public Task<DataRecord> Handle(SingleRow request, CancellationToken cancellationToken)
  {
    try
    {
      return Task.FromResult(_service.SingleRow(500, "MultupleQueryAsync"));
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }

  public class SystemDate : IRequest<DateTime>
  {
  }

  public Task<DateTime> Handle(SystemDate request, CancellationToken cancellationToken)
  {
    try
    {
      return Task.FromResult(_service.SystemDate(500, "MultupleQueryAsync"));
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return Task.FromResult(DateTime.Now);
    }
  }

  public class UserList : IRequest<List<User>>
  {
  }

  public async Task<List<User>> Handle(UserList request, CancellationToken cancellationToken)
  {
    try
    {
      return await _service.GetUsersAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }
}
