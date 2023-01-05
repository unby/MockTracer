using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MockTracer.UI.Client;
using MockTracer.UI.Client.Services;
using MockTracer.UI.Client.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<IAlertService, AlertService>();

builder.Services.AddScoped<ITraceService, TraceService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
/*builder.Services.AddScoped(x => {
  var apiUrl = new Uri("http://localhost:5001");
  return new HttpClient() { BaseAddress = apiUrl };
});*/
builder.Services.AddSingleton<PageHistoryState>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
