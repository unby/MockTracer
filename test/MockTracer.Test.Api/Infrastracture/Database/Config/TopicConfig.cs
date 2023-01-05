using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Infrastracture.Database.Config;

public class TopicConfig : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("Topics");
        builder.HasKey(ci => ci.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasMany(x => x.Comments).WithOne(x => x.Topic)
               .HasForeignKey(x => x.TopicId);

        builder.HasData(new Topic
        {
            Id = 1,
            Content = "Value Object in Domain Driven Design",
            Title = "Value Object",
            Created = DateTime.Now,
            AuthorId = 1,
        }, new Topic
        {
            Id = 2,
            Content = "Domian Event in Domain Driven Design",
            Title = "Domian Event",
            Created = DateTime.Now,
            AuthorId = 1,
        });
    }
}
