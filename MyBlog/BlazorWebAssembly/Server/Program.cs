using Auth0.AspNetCore.Authentication;
using BlazorWebAssembly.Server.Endpoints;
using Components.RazorComponents;
using Data;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddOptions<BlogApiJsonDirectAccessSetting>()
    .Configure(options =>
    {
        options.DataPath = @"..\..\..\Data\";
        options.BlogPostsFolder = "BlogPosts";
        options.TagsFolder = "Tags";
        options.CategoriesFolder = "Categories";
    });
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Authority"] ?? "";
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "";
});

builder.Services.AddTransient<ILoginStatus, LoginStatus>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlogPostApi();
app.MapCategoryApi();
app.MapTagApi();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapGet("authentication/login", async (string redirectUri, 
    HttpContext context) =>
{
    var authenticationProperties = 
    new LoginAuthenticationPropertiesBuilder()
    .WithRedirectUri(redirectUri)
    .Build();
    await context.ChallengeAsync(Auth0Constants.AuthenticationScheme, 
        authenticationProperties);
});

app.MapGet("authentication/logout", async (HttpContext context) =>
{
    var authenticationProperties = 
    new LogoutAuthenticationPropertiesBuilder()
    .WithRedirectUri("/")
    .Build();
    await context.SignOutAsync(Auth0Constants.AuthenticationScheme,
        authenticationProperties);
    await context.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);
});


app.Run();
