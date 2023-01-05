namespace MockTracer.UI.Server.Application.Generation;

public struct LineFragment
{
  public LineFragment(string code, string line)
  {
    Code = code;
    Line = line;
  }

  public string Code { get; init; }

  public string Line { get; init; }
}
