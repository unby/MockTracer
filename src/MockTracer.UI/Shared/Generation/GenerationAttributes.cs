namespace MockTracer.UI.Shared.Generation;

public class GenerationAttributes
{
  public Guid ScopeId { get; init; }

  public Guid InputId { get; init; }

  public Guid[]? OutputId { get; init; }

  public string? TestName { get; init; }

  public string? TemplateCode { get; init; }
}
