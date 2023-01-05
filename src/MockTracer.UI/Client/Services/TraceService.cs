using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MockTracer.UI.Client.Shared;
using MockTracer.UI.Shared.Data;
using MockTracer.UI.Shared.Entity;
using MockTracer.UI.Shared.Generation;

namespace MockTracer.UI.Client.Services;

public class TraceService : ITraceService
{
  private readonly HttpClient _httpClient;
  private readonly IConfiguration _configuration;
  private NavigationManager _navigationManager;
  private readonly IJSRuntime _jsruntime;

  public TraceService(HttpClient httpClient, IConfiguration configuration, NavigationManager navigationManager, IJSRuntime jsruntime)
  {
    _httpClient = httpClient;
    _configuration = configuration;
    _navigationManager = navigationManager;
    _jsruntime = jsruntime;
  }
  public async Task<PagedResult<StackScope>> GetTraceListAsync(int page)
  {
    return await Get<PagedResult<StackScope>>($"data/trace-list?page={page}");
  }

  public async Task<StackRow[]> GetTraceRowsAsync(Guid scopeId)
  {
    return await Get<StackRow[]>($"data/trace-rows/{scopeId}");
  }

  public async Task MakeTestAsync(GenerationAttributes attributes)
  {
    var request =  createRequest(HttpMethod.Post, $"data/generate", attributes);
    using var response = await _httpClient.SendAsync(request);
    using var streamRef = new DotNetStreamReference(stream: response.Content.ReadAsStream());

    await _jsruntime.InvokeVoidAsync(
      "downloadFileFromStream",
      new ContentDisposition(response.Content.Headers.ContentDisposition.ToString()).FileName,
      streamRef);
  }

  public async Task MakeInternalTestAsync(GenerationAttributes attributes)
  {
    var request = createRequest(HttpMethod.Post, $"data/internal-test-generate", attributes);
    using var response = await _httpClient.SendAsync(request);
    using var streamRef = new DotNetStreamReference(stream: response.Content.ReadAsStream());

    await _jsruntime.InvokeVoidAsync(
      "downloadFileFromStream",
      new ContentDisposition(response.Content.Headers.ContentDisposition.ToString()).FileName,
      streamRef);
  }

  protected async Task<T> Get<T>(string uri)
  {
    var request = new HttpRequestMessage(HttpMethod.Get, uri);
    return await sendRequest<T>(request);
  }

  protected async Task Post(string uri, object value)
  {
    var request = createRequest(HttpMethod.Post, uri, value);
    await sendRequest(request);
  }

  protected async Task<T> Post<T>(string uri, object value)
  {
    var request = createRequest(HttpMethod.Post, uri, value);
    return await sendRequest<T>(request);
  }

  protected async Task Put(string uri, object value)
  {
    var request = createRequest(HttpMethod.Put, uri, value);
    await sendRequest(request);
  }

  protected async Task<T> Put<T>(string uri, object value)
  {
    var request = createRequest(HttpMethod.Put, uri, value);
    return await sendRequest<T>(request);
  }

  protected async Task Delete(string uri)
  {
    var request = createRequest(HttpMethod.Delete, uri);
    await sendRequest(request);
  }

  private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
  {
    var request = new HttpRequestMessage(method, uri);
    if (value != null)
      request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
    return request;
  }

  private async Task sendRequest(HttpRequestMessage request)
  {
    // send request
    using var response = await _httpClient.SendAsync(request);

    // auto logout on 401 response
    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
      _navigationManager.NavigateTo("user/logout");
      return;
    }

    await handleErrors(response);
  }

  private async Task<T> sendRequest<T>(HttpRequestMessage request)
  {
    // send request
    using var response = await _httpClient.SendAsync(request);

    // auto logout on 401 response
    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
      _navigationManager.NavigateTo("user/logout");
      return default;
    }

    await handleErrors(response);

    var options = new JsonSerializerOptions();
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new StringConverter());
    return await response.Content.ReadFromJsonAsync<T>(options);
  }

  private async Task handleErrors(HttpResponseMessage response)
  {
    // throw exception on error response
    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
      throw new Exception(error["message"]);
    }
  }
}
