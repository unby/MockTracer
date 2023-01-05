namespace MockTracer.Test.Api.Domain;

public class Comment
{
    public int Id { get; init; }

    public DateTime Created { get; init; }

    public string Text { get; init; }

    public Topic Topic { get; init; }

    public int TopicId { get; init; }

    public User User { get; init; }

    public int UserId { get; init; }
}
