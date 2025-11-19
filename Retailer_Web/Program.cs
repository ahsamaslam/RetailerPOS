using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Retailer.POS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor pages (only once)
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages().AddNToastNotifyNoty(); // you can combine, but avoid duplicate AddRazorPages calls

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenDelegatingHandler>();

// HttpClient used by your ApiClient; TokenDelegatingHandler will add Bearer token from cookie claims
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/");
}).AddHttpMessageHandler<TokenDelegatingHandler>();
builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthModule:Authority"]);
});

// IMPORTANT: set cookie as the default scheme for web pages so unauthorized -> redirect
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // cookie default
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // challenge -> redirect to Login
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.Cookie.Name = "RetailerWebAuth";
    options.SlidingExpiration = true;
})
// Keep JwtBearer available if you ever need it (APIs should configure their own JwtBearer)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["AuthModule:Authority"];
    options.Audience = "RetailerWebAPI";
    options.RequireHttpsMetadata = false;
    // optional: events etc.
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseNToastNotify();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// authentication before authorization and endpoints
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
