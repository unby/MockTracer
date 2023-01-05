using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage.Configuration;

public class StackRowConfig : IEntityTypeConfiguration<StackRow>
{
  public void Configure(EntityTypeBuilder<StackRow> builder)
  {
    builder.ToTable("StackRows");
    builder.HasKey(ci => ci.Id);

    builder.HasOne(x => x.Scope).WithMany(x => x.Stack)
           .HasForeignKey(x => x.ScopeId);

    builder.HasMany(x => x.Input).WithOne()
             .HasForeignKey(x => x.StackRowId);

    builder.HasOne(x => x.Output).WithOne().HasForeignKey<Output>(s => s.StackRowId);

    builder.HasOne(x => x.Exception).WithOne().HasForeignKey<ExceptionType>(s => s.StackRowId);
  }
}
