using BlazorApp;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7195") });

builder.Services.AddScoped<IWebService,WebService>(sp =>
{
    var js = sp.GetRequiredService<IJSRuntime>();
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new WebService(js, httpClient, builder.HostEnvironment.BaseAddress);
});

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<JwtAuthenticationProvider>(options => new JwtAuthenticationProvider(options.GetRequiredService<IJSRuntime>()));
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationProvider>(options => options.GetRequiredService<JwtAuthenticationProvider>());
builder.Services.AddScoped<ILoginService, JwtAuthenticationProvider>();


await builder.Build().RunAsync();
