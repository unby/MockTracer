using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage.Configuration;

public class OutputConfig : IEntityTypeConfiguration<Output>
{
  public void Configure(EntityTypeBuilder<Output> builder)
  {
    builder.ToTable("Outputs");
    builder.HasKey(ci => ci.Id);
  }
}
