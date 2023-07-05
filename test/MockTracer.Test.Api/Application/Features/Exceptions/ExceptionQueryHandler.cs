using MediatR;
using MockTracer.Test.Api.Domain;
using MockTracer.Test.Api.Infrastracture.External;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class ExceptionQueryHandler : IRequestHandler<ExceptionQuery, CatFact>
{
  private readonly ICatService _service;

  public ExceptionQueryHandler(ICatService service)
  {
    _service = service;
  }

  public async Task<CatFact> Handle(ExceptionQuery request, CancellationToken cancellationToken)
  {
    throw new UnbelievableException("Unbeliveable exception");
  }
}
