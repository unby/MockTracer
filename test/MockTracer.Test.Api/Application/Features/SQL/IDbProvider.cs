using System.Data;

namespace MockTracer.Test.Api.Application.Features.SQL;

public interface IDbProvider
{
  IDbConnection GetDbConnection();
}
