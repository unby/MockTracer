namespace MockTracer.Test.Api.Application.Features.Data;

public class DataRecord
{
  public string Name { get; set; }

  public int SomeNumber { get; set; }

  public RecordType Type { get; set; } = RecordType.Second;
}
