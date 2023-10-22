using System.Runtime.Intrinsics.X86;
using MockTracer.UI.Shared.Data;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Client.Services;

/// <summary>
/// server API
/// </summary>
public interface ITraceService
{
  /// <summary>
  /// scope list
  /// </summary>
  /// <param name="page"></param>
  /// <returns></returns>
  Task<PagedResult<StackScope>> GetTraceListAsync(int page);
  
  /// <summary>
  /// scope detail info
  /// </summary>
  Task<StackScope> GetExecutionStackAsync(Guid scopeId);

  /// <summary>
  /// Generate test class
  /// </summary>
  /// <param name="attributes"></param>
  /// <returns></returns>
  Task MakeTestAsync(GenerationAttributes attributes);

  /// <summary>
  /// Generate test class and save to directory
  /// </summary>
  /// <param name="attributes"></param>
  /// <returns></returns>
  Task MakeTestAndSaveToProjectAsync(GenerationAttributes attributes);

  /// <summary>
  /// generate internal test
  /// </summary>
  /// <param name="attributes"></param>
  /// <returns></returns>
  Task MakeInternalTestAsync(GenerationAttributes attributes);

  /// <summary>
  /// settings
  /// </summary>
  /// <returns><see cref="ClassGenerationSetting"/></returns>
  Task<ClassGenerationSetting> GetClassGenerationSettingAsync();
}
