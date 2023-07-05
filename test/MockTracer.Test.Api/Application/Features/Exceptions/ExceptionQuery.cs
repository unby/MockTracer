using MediatR;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class ExceptionQuery : IRequest<CatFact>
{
  public int? FactId { get; set; }
}
