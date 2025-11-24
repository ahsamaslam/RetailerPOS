using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Retailer.Api.Services;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Mappings;
using Retailer.POS.Api.Repositories;
using Retailer.POS.Api.Services;
using Retailer.POS.API.UnitOfWork;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<RetailerDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

// Repositories & UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
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
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        ValidateIssuerSigningKey = true,
        NameClaimType = JwtRegisteredClaimNames.Sub,    // optional: set which claim maps to Name
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSection.GetValue<string>("SigningKeyBase64"))),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
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
    client.BaseAddress = new Uri(builder.Configuration["AuthModule:Authority"] ?? "https://localhost:7001/");
})
// ensure the handler is applied to this HttpClient
.AddHttpMessageHandler<TokenDelegationHandler>();

builder.Services.AddTransient<TokenDelegationHandler>();
// Register MenuService expecting an HttpClientFactory (inject IHttpClientFactory or HttpClient via named client)
builder.Services.AddScoped<IMenuService, MenuService>();

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

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// middleware order: Routing -> Auth -> AuthZ -> Endpoints
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
