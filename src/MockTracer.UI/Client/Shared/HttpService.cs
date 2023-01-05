using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MockTracer.UI.Client.Shared
{
  public interface IHttpService
  {
    Task<T> Get<T>(string uri);
    Task Post(string uri, object value);
    Task<T> Post<T>(string uri, object value);
    Task Put(string uri, object value);
    Task<T> Put<T>(string uri, object value);
    Task Delete(string uri);
    Task<T> Delete<T>(string uri);
  }
}
