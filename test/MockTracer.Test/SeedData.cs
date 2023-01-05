using Microsoft.EntityFrameworkCore;
using MockTracer.Test.Api.Domain;

namespace MockTracer.Test;

public static class SeedData
{
  public static Topic TestTopic
  {
    get
    {
      return new Topic()
      {
        Created = DateTime.Now,
        Title = "test 1",
        Content = "test content 3",
        Author = new User()
        {
          Nick = "tu",
          Email = "tu@test.local",
          Id = 3
        },
        Comments = new List<Comment>()
    {
      new Comment()
      {
        Created = DateTime.Now,
        Text = "Text 1",
        UserId = 3
      },
      new Comment()
      {
        Created = DateTime.Now,
        Text = "Text 2",
        UserId = 2
      },
      new Comment()
      {
        Created = DateTime.Now,
        Text = "Text 3",
        UserId = 1
      }

    }
      };
    }
  }

  public static void AddDataToContext(DbContext context)
  {
    context.Add(TestTopic);
    context.SaveChanges();
  }
}
