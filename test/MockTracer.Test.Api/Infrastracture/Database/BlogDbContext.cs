using Microsoft.EntityFrameworkCore;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Infrastracture.Database;

public class BlogDbContext : DbContext, IBlogDbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
  }
  public DbSet<User> Users { get; set; }

  public DbSet<Topic> Topics { get; set; }

  public DbSet<Comment> Comments { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    base.OnModelCreating(builder);

  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return base.SaveChangesAsync(cancellationToken);
  }
}
