namespace MockTracer.UI.Server.Options;

public class ClassGenerationSetting
{
  public string DefaultNameSpace { get; set; } = "DefaultNameSpace";

  public string DefaultClassName { get; set; } = "DefaultClassName";

  public string DefaultMethodName { get; set; } = "DefaultMethodName";

  public bool IsWriteNameSpaceBracket { get; set; } = false;

  public string DefaultTemplate { get; set; }

  public string TestBase { get; set; } = "SampleTestBase";
}
