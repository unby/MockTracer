namespace MockTracer.UI.Server.Application.Watcher.Database;

/// <summary>
/// data flusher
/// </summary>
public interface IDataFlusher
{
  /// <summary>
  /// Flush of accumulated data 
  /// </summary>
  /// <param name="result"></param>
  void FlushResult(List<DataSet> result);
}
