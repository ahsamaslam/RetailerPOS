using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Retailer.POS.Web.Services;
using Retailer.Web.Filters;
using Retailer.Web.Helpers;
using Retailer.Web.Services.Layout;
using System.Security.Claims;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<ApiUnauthorizedRedirectFilter>();

        // Razor pages (only once)
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/");
            options.Conventions.AllowAnonymousToPage("/Login");
        })
        .AddMvcOptions(options =>
        {
            options.Filters.Add<ApiUnauthorizedRedirectFilter>();
        })
        .AddNToastNotifyNoty();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<TokenDelegatingHandler>();
        builder.Services.AddScoped<RdlcDataTableHelper>();
        builder.Services.AddScoped<ILayoutContext, LayoutContext>();
        // HttpClient used by your ApiClient; TokenDelegatingHandler will add Bearer token from cookie claims
        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001/");
        }).AddHttpMessageHandler<TokenDelegatingHandler>();
        builder.Services.AddHttpClient("AuthApi", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["AuthModule:Authority"]);
        }).AddHttpMessageHandler<TokenDelegatingHandler>();

        // IMPORTANT: set cookie as the default scheme for web pages so unauthorized -> redirect
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Login";
            options.AccessDeniedPath = "/Login";
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.Cookie.Name = "Retailer.Web";
            options.SlidingExpiration = true;

        })
        // Keep JwtBearer available if you ever need it (APIs should configure their own JwtBearer)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = builder.Configuration["AuthModule:Authority"];
            options.Audience = builder.Configuration["AuthModule:Audience"];
            options.RequireHttpsMetadata = false;
            // optional: events etc.
        });

        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy(RequiresPermissionAttribute.PermissionPolicyName, policy =>
        //    {
        //        policy.Requirements.Add(new PermissionRequirement());
        //    });
        //});
        // near other builder.Services calls
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(8);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
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

        app.UseSession();

        // authentication before authorization and endpoints
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.Run();
    }
}