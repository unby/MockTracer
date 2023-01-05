namespace MockTracer.Test.Api.Domain;

public class Topic
{
    public int Id { get; init; }

    public string Title { get; init; }

    public string Content { get; init; }

    public DateTime Created { get; init; }

    public User Author { get; init; }

    public int AuthorId { get; init; }

    public IList<Comment> Comments { get; init; } = new List<Comment>();

}
