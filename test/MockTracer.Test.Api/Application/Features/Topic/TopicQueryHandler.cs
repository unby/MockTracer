using MediatR;
using Microsoft.EntityFrameworkCore;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class TopicQueryHandler : IRequestHandler<TopicQuery, List<TopicDto>>
{
    private readonly IBlogDbContext _context;
    public TopicQueryHandler(IBlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<TopicDto>> Handle(TopicQuery request, CancellationToken cancellationToken)
    {
        return await (from t in _context.Topics
                      join u in _context.Users on t.AuthorId equals u.Id
                      select new TopicDto { Id = t.Id, Title = t.Title, AuthorName = u.Nick }).ToListAsync(cancellationToken);
    }
}
