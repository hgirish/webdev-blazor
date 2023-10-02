using BlazorWebAssembly.Client;
using Components.RazorComponents;
using Data;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<ILoginStatus, LoginStatusWasm>();
string rootpath = System.IO.Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "wwwroot", @"..\..\..\..\JsonData\");
builder.Services.AddOptions<BlogApiJsonDirectAccessSetting>()
    .Configure(options =>
    {
        options.DataPath = rootpath;
        options.BlogPostsFolder = "BlogPosts";
        options.TagsFolder = "Tags";
        options.CategoriesFolder = "Categories";
    });
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();

builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient("Public",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));   
builder.Services.AddHttpClient("Authenticated",
    client => client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience",
        builder.Configuration["Auth0:Audience"]);
})
    .AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();


await builder.Build().RunAsync();
