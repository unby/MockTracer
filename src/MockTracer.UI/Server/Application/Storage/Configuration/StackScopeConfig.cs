using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage.Configuration;

public class StackScopeConfig : IEntityTypeConfiguration<StackScope>
{
  public void Configure(EntityTypeBuilder<StackScope> builder)
  {
    builder.ToTable("StackScopes");
    builder.HasKey(ci => ci.Id);

    builder.HasMany(x => x.Stack).WithOne(x => x.Scope)
           .HasForeignKey(x => x.ScopeId);

  }
}
