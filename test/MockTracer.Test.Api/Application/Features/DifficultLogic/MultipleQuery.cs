using MediatR;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class MultipleQuery : IRequest
{
  public int Arg { get; set; }

  public bool IsRecursive { get; set; }
}
