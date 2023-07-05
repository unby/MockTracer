namespace MockTracer.Test.Api.Application.Features.SQL;

public class DataRecord
{
  public string Name { get; set; }

  public int SomeNumber { get; set; }

  public RecordType Type { get; set; } = RecordType.Second;
}
