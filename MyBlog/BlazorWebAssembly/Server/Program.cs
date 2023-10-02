using Auth0.AspNetCore.Authentication;
using BlazorWebAssembly.Server.Endpoints;
using Components.RazorComponents;
using Data;
using Data.Models.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
string rootpath = System.IO.Path.Combine(
    System.IO.Directory.GetCurrentDirectory(),
    "wwwroot");

var dataPath = Path.GetFullPath(@"..\..\..\Data\");
builder.Services.AddOptions<BlogApiJsonDirectAccessSetting>()
    .Configure(options =>
    {
        options.DataPath = rootpath;
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
    {
        c.Authority = builder.Configuration["Auth0:Authority"];
        c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = builder.Configuration["Auth0:Audience"],
            ValidIssuer = builder.Configuration["Auth0:Authority"]
        };
    });
builder.Services.AddAuthorization();

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
