using Microsoft.EntityFrameworkCore;

namespace MockTracer.Test.Api.Domain;

public interface IBlogDbContext
{
    public DbSet<Topic> Topics { get; }
    public DbSet<Comment> Comments { get; }
    public DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
