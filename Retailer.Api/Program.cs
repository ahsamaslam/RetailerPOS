using AuthModule.API.Auth;
using AuthModule.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Retailer.Api.Data;
using Retailer.Api.Middleware;
using Retailer.Api.Services;
using Retailer.Api.Services.Reports;
using Retailer.Api.Services.Reports.Interface;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Mappings;
using Retailer.POS.Api.Repositories;
using Retailer.POS.Api.Services;
using Retailer.POS.API.UnitOfWork;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddDbContext<RetailerDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

// Repositories & UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>(); 
builder.Services.AddScoped<IPurchaseReturnService, PurchaseReturnService>(); 
builder.Services.AddScoped<IPurchaseReportExportService, PurchaseReportExportService>();
builder.Services.AddScoped<IPurchaseReturnReportExportService, PurchaseReturnReportExportService>();
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddScoped<Retailer.Api.Data.IDbInitializer, Retailer.Api.Data.DbInitializer>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<ISalesReturnService, SalesReturnService>();


// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),

        ValidateAudience = true,
        // Accept multiple audiences configured under Jwt:Audiences (fallback to Jwt:Audience)
        ValidAudiences = jwtSection.GetSection("Audiences").Get<string[]>(),

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSection.GetValue<string>("SigningKeyBase64"))),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),

        // Map the name and role claim types to what the token contains
        // You issue ClaimTypes.NameIdentifier and ClaimTypes.Role in the token,
        // so map NameClaimType -> ClaimTypes.NameIdentifier
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    // Helpful debug logging for token validation failures
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            // Log exception to console (or replace with your logger)
            Console.WriteLine("Jwt authentication failed: " + ctx.Exception?.Message);
            return Task.CompletedTask;
        },
        OnMessageReceived = ctx =>
        {
            // Inspect if Authorization header is present
            var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine("Auth header: " + (auth ?? "[none]"));
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("Token validated for: " + ctx.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddHttpContextAccessor();

// register the handler
builder.Services.AddTransient<TokenDelegationHandler>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("AuthModule", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthModule:Authority"] ?? "http://localhost:7001/");
})
// ensure the handler is applied to this HttpClient
.AddHttpMessageHandler<TokenDelegationHandler>();

// Register MenuService expecting an HttpClientFactory (inject IHttpClientFactory or HttpClient via named client)
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Retailer.POS.API", Version = "v1" });

    // Add JWT auth UI to swagger (optional, but useful)
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put `Bearer {token}`",
    };
    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });

});
builder.Services.AddHttpClient<IFbrClient, FbrClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Fbr:Url"] ?? "https://fbr.example/");
    // set default headers or auth here if required
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RequiresPermissionAttribute.PermissionPolicyName, policy =>
    {
        policy.Requirements.Add(new PermissionRequirement());
    });
});
builder.Services.AddScoped<IAuthorizationHandler, ClaimsPermissionHandler>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    if (app.Environment.IsDevelopment())
    {

        var db = services.GetRequiredService<RetailerDbContext>();
        await db.Database.MigrateAsync();
    }
    var initializer = services.GetRequiredService<Retailer.Api.Data.IDbInitializer>();
    await initializer.InitializeAsync();
}
// middleware order: Routing -> Auth -> AuthZ -> Endpoints
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
