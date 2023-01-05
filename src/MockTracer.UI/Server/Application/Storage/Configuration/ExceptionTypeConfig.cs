using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.UI.Shared.Entity;

namespace MockTracer.UI.Server.Application.Storage.Configuration;

public class ExceptionTypeConfig : IEntityTypeConfiguration<ExceptionType>
{
  public void Configure(EntityTypeBuilder<ExceptionType> builder)
  {
    builder.ToTable("ExceptionTypes");
    builder.HasKey(ci => ci.Id);
  }
}
