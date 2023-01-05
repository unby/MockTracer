using System.ComponentModel.DataAnnotations;

namespace MockTracer.Test.Api.Domain;

public class CatFact
{
  [Required]
  public string Fact { get; set; }
  public int Length { get; set; }

  public override bool Equals(object? obj)
  {
    return obj is CatFact fact &&
           Fact == fact.Fact &&
           Length == fact.Length;
  }
}
