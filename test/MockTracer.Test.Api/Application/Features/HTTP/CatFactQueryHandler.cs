using MediatR;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.External;

namespace MockTracer.Test.Api.Application.Features.HTTP;

public class CatFactQueryHandler : IRequestHandler<CatFactQuery, CatFact>
{
  private readonly ICatService _service;

  public CatFactQueryHandler(ICatService service)
  {
    _service = service;
  }

  public async Task<CatFact> Handle(CatFactQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var x = await _service.GetCatFactAsync();
      return x;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return null;
    }
  }
}
