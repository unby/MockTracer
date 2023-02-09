namespace MockTracer.UI.Client.Shared;

/// <inheritdoc/>
public class AlertService : IAlertService
{
  private const string _defaultId = "default-alert";
  public event Action<AlertModel> OnAlert;

  /// <inheritdoc/>
  public void Success(string message, bool keepAfterRouteChange = false, bool autoClose = true)
  {
    Alert(new AlertModel
    {
      Type = AlertType.Success,
      Message = message,
      KeepAfterRouteChange = keepAfterRouteChange,
      AutoClose = autoClose
    });
  }

  /// <inheritdoc/>
  public void Error(string message, bool keepAfterRouteChange = false, bool autoClose = true)
  {
    Alert(new AlertModel
    {
      Type = AlertType.Error,
      Message = message,
      KeepAfterRouteChange = keepAfterRouteChange,
      AutoClose = autoClose
    });
  }

  /// <inheritdoc/>
  public void Info(string message, bool keepAfterRouteChange = false, bool autoClose = true)
  {
    Alert(new AlertModel
    {
      Type = AlertType.Info,
      Message = message,
      KeepAfterRouteChange = keepAfterRouteChange,
      AutoClose = autoClose
    });
  }

  /// <inheritdoc/>
  public void Warn(string message, bool keepAfterRouteChange = false, bool autoClose = true)
  {
    Alert(new AlertModel
    {
      Type = AlertType.Warning,
      Message = message,
      KeepAfterRouteChange = keepAfterRouteChange,
      AutoClose = autoClose
    });
  }

  /// <inheritdoc/>
  public void Alert(AlertModel alert)
  {
    alert.Id = alert.Id ?? _defaultId;
    OnAlert?.Invoke(alert);
  }

  /// <inheritdoc/>
  public void Clear(string id = _defaultId)
  {
    OnAlert?.Invoke(new AlertModel { Id = id });
  }
}
