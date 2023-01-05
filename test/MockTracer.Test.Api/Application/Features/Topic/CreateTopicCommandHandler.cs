using MediatR;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Application.Features.Topic;

public class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, int>
{
    private readonly IBlogDbContext _context;
    public CreateTopicCommandHandler(IBlogDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var entity = _context.Topics.Add(new Domain.Topic()
        {
            Content = request.Content,
            Created = DateTime.Now,
            Title = request.Title,
            AuthorId = request.AuthorId
        });
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Entity.Id;
    }
}
