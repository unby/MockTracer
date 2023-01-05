using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage.Configuration;

public class InputConfig : IEntityTypeConfiguration<Input>
{
  public void Configure(EntityTypeBuilder<Input> builder)
  {
    builder.ToTable("Inputs");
    builder.HasKey(ci => ci.Id);
  }
}
