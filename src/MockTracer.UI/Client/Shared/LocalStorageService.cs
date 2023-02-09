using Microsoft.JSInterop;
using System.Text.Json;

namespace MockTracer.UI.Client.Shared;

/// <inheritdoc/>
public class LocalStorageService : ILocalStorageService
{
  private IJSRuntime _jsRuntime;

  /// <summary>
  /// LocalStorageService
  /// </summary>
  /// <param name="jsRuntime"><see cref="IJSRuntime"/></param>
  public LocalStorageService(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime;
  }

  /// <inheritdoc/>
  public async Task<T> GetItem<T>(string key)
  {
    var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

    if (json == null)
      return default;

    return JsonSerializer.Deserialize<T>(json);
  }

  /// <inheritdoc/>
  public async Task SetItem<T>(string key, T value)
  {
    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
  }

  /// <inheritdoc/>
  public async Task RemoveItem(string key)
  {
    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
  }
}
