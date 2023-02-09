namespace MockTracer.UI.Shared.Generation;

/// <summary>
/// class attributes
/// </summary>
public class ClassGenerationSetting
{
  /// <summary>
  /// ClassGenerationSetting
  /// </summary>
  public ClassGenerationSetting()
  {
  }

  /// <summary>
  /// namespace
  /// </summary>
  public string DefaultNameSpace { get; set; } = "DefaultNameSpace";

  /// <summary>
  /// class name
  /// </summary>
  public string DefaultClassName { get; set; } = "DefaultClassName";

  /// <summary>
  /// method name
  /// </summary>
  public string DefaultMethodName { get; set; } = "DefaultMethodName";

  /// <summary>
  /// namespace's brakets
  /// </summary>
  public bool IsWriteNameSpaceBracket { get; set; } = false;

  /// <summary>
  /// parent class
  /// </summary>
  public string TestBase { get; set; } = "SampleTestBase";
}
