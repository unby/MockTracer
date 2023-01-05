using Microsoft.EntityFrameworkCore;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage;

public class MockTracerDbContext : DbContext
{
  public MockTracerDbContext(DbContextOptions<MockTracerDbContext> options)
    : base(options)
  {
  }
  public DbSet<StackScope> StackScopes { get; set; }

  public DbSet<Input> Inputs { get; set; }

  public DbSet<Output> Outputs { get; set; }

  public DbSet<ExceptionType> ExceptionTypes { get; set; }

  public DbSet<StackRow> StackRows { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    base.OnModelCreating(builder);
  }
}
