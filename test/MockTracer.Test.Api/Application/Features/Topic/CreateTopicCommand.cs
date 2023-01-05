using MediatR;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class CreateTopicCommand : IRequest<int>
{
    public string Title { get; init; }
    public string Content { get; init; }
    public int AuthorId { get; init; }
}
