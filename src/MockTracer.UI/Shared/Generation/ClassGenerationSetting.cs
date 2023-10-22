namespace MockTracer.UI.Shared.Generation;

/// <summary>
/// class attributes
/// </summary>
public class ClassGenerationSetting : System.ICloneable
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

  /// <summary>
  /// output directory
  /// </summary>
  public string DefaultFolder { get; set; } = string.Empty;

  public string[] NameSpaces { get; set; } = new string[] { "MockTracer.Test" };

  public string FileExtentions { get; set; } = "cs.tmp";

  public ClassGenerationSetting Clone()
  {
    return new ClassGenerationSetting
    {
      DefaultNameSpace = DefaultNameSpace,
      DefaultClassName = DefaultClassName,
      DefaultMethodName = DefaultMethodName,
      IsWriteNameSpaceBracket = IsWriteNameSpaceBracket,
      TestBase = TestBase,
      DefaultFolder = DefaultFolder,
      NameSpaces = NameSpaces.ToArray(),
      FileExtentions = FileExtentions
    };
  }

  object ICloneable.Clone()
  {
    return Clone();
  }
}
