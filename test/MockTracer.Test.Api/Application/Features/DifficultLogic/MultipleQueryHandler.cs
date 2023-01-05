using MediatR;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class MultipleQueryHandler
  : IRequestHandler<MultipleQuery, Unit>
{
  private readonly IMediator _mediator;

  public MultipleQueryHandler(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Unit> Handle(MultipleQuery request, CancellationToken cancellationToken)
  {

    var result1 = await _mediator.Send(new TopicQuery());
    var result2 = await _mediator.Send(new CatFactQuery());

    // var result1 = _mediator.Send(new TopicQuery());
    if (request.IsRecursive)
    {
      await _mediator.Send(new MultipleQuery() { IsRecursive = false });
    }

    return Unit.Value;
  }
}
