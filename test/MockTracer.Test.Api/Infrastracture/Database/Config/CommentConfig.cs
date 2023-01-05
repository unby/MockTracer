using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test.Api.Infrastracture.Database.Config;

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(ci => ci.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasOne(x => x.Topic).WithMany(x => x.Comments)
               .HasForeignKey(x => x.TopicId);

        builder.HasData(new Comment
        {
            Id = 1,
            Text = "Best!!",
            Created = DateTime.Now,
            UserId = 2,
            TopicId = 1,

        }, new Comment
        {
            Id = 2,
            Text = "thanks :)",
            Created = DateTime.Now,
            UserId = 1,
            TopicId = 1,
        }); ;
    }
}
