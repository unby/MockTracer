using Microsoft.EntityFrameworkCore;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage;

/// <summary>
/// MockTracerDbContext
/// </summary>
public class MockTracerDbContext : DbContext
{
  /// <summary>
  /// MockTracerDbContext
  /// </summary>
  /// <param name="options">options</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public MockTracerDbContext(DbContextOptions<MockTracerDbContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    : base(options)
  { }

  /// <summary>
  /// Stack defenition
  /// </summary>
  public DbSet<StackScope> StackScopes { get; set; }

  /// <summary>
  /// input method arguments
  /// </summary>
  public DbSet<Input> Inputs { get; set; }

  /// <summary>
  /// return declaring type
  /// </summary>
  public DbSet<Output> Outputs { get; set; }

  /// <summary>
  /// caught exceptions
  /// </summary>
  public DbSet<ExceptionType> ExceptionTypes { get; set; }

  /// <summary>
  /// stack
  /// </summary>
  public DbSet<StackRow> StackRows { get; set; }

  /// <inheritdoc/>
  protected override void OnModelCreating(ModelBuilder builder)
  {
    builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    base.OnModelCreating(builder);
  }
}
