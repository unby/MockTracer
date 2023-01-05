using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Infrastracture.Database.Config;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(ci => ci.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasData(new User
        {
            Id = 1,
            Nick = "Don",
            Email = "don@local.local"

        }, new User
        {
            Id = 2,
            Nick = "Bob",
            Email = "Bob@local.local"

        });

    }
}
