using MediatR;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class TopicQuery : IRequest<List<TopicDto>>
{
}
