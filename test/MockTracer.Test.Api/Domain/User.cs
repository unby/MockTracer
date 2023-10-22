namespace MockTracer.Test.Api.Domain;

public class User
{
    public int Id { get; init; }

    public string Nick { get; init; }

    public string Email { get; init; }

    public DateTime RegistrationDate { get; init; } = DateTime.UtcNow;

    public int Type { get; init; } = 0;
}
