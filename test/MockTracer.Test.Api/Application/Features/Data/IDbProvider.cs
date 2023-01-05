using System.Data;

namespace MockTracer.Test.Api.Application.Features.Data;

public interface IDbProvider
{
  IDbConnection GetDbConnection();
}
