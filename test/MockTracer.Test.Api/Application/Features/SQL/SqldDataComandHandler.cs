using MediatR;
using MockTracer.Test.Api.Domain;
using static MockTracer.Test.Api.Application.Features.SQL.SqldDataComandHandler;

namespace MockTracer.Test.Api.Application.Features.SQL;

public class SqldDataComandHandler :
  IRequestHandler<ExecuteNonQuery, int>,
  IRequestHandler<MultupleQueryAsync, List<DataRecord>>,
  IRequestHandler<SingleRow, DataRecord>,
  IRequestHandler<SystemDate, DateTime>,
  IRequestHandler<UserList, List<User>>,
  IRequestHandler<NewUserCommand>

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

  public class NewUserCommand : IRequest
  {
  }

  public async Task<Unit> Handle(NewUserCommand request, CancellationToken cancellationToken)
  {
    try
    {
      DateTime date = DateTime.Now.AddDays(-200);
      await _service.SetUsersAsync(new UserDTO() { Email=$"e{date.ToString("ddMMyyyy")}@test.test", RegistrationDate = date, Type = 2, Nick = date.ToString("Name_ddMMyyyy") }, cancellationToken);
      return Unit.Value;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }

  public class InvalidSQLStatementCommand : IRequest
  {
  }

  public async Task<Unit> Handle(InvalidSQLStatementCommand request, CancellationToken cancellationToken)
  {
    try
    {
      DateTime date = DateTime.Now.AddDays(-200);
      await _service.InvalidStatementAsync(new UserDTO() { Email = $"e{date.ToString("ddMMyyyy")}@test.test", RegistrationDate = date, Type = 2, Nick = date.ToString("Name_ddMMyyyy") }, cancellationToken);
      return Unit.Value;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      throw;
    }
  }
}
