using AuthModule.API.Auth;
using AuthModule.API.Data;
using AuthModule.API.Models;
using AuthModule.API.Repositories;
using AuthModule.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Logging
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
var val = builder.Configuration.GetConnectionString("DefaultConnection");
// Db
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(val));
// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequireDigit = false; // Don't require a number
    options.Password.RequiredLength = 6;   // Minimum length
    options.Password.RequireNonAlphanumeric = false; // Don't require special character
    options.Password.RequireUppercase = false; // Don't require uppercase
    options.Password.RequireLowercase = false; // Don't require lowercase
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Token config (base64 key)
var signingBase64 = builder.Configuration["Jwt:SigningKeyBase64"]!;
var keyBytes = Convert.FromBase64String(signingBase64);
var myKey = new SymmetricSecurityKey(keyBytes) { KeyId = builder.Configuration["Jwt:KeyId"] };

var jwtSection = builder.Configuration.GetSection("Jwt");

// read audiences: allow either single Audience or Audiences array in config
var audiences = jwtSection.GetSection("Audiences").Get<string[]>();

// Authentication - JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false; // do not map to Microsoft-specific claim types
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudiences = audiences,
        ValidateIssuerSigningKey = true,
        NameClaimType = ClaimTypes.NameIdentifier,    // optional: set which claim maps to Name
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        // Fallback resolver: give middleware the keys to try (useful when kid missing)
        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
        {
            // return all known keys (here a single symmetric key)
            return new List<SecurityKey> { myKey };
        },
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
    // Helpful debug logging for token validation failures
    options.Events = new JwtBearerEvents
    {
        OnForbidden = ctx =>
        {
            // Log exception to console (or replace with your logger)
            Console.WriteLine("Jwt authentication failed: " + ctx);
            return Task.CompletedTask;
        },
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

// Authorization — register before adding custom provider/handlers
builder.Services.AddAuthorization(options =>
{
    // Optionally add static policies here, e.g.:
    // options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    // register a single policy that uses the PermissionRequirement.
    options.AddPolicy(RequiresPermissionAttribute.PermissionPolicyName, policy =>
    {
        policy.Requirements.Add(new PermissionRequirement());
    });
});

// DI registrations
builder.Services.AddSingleton(new TokenHasher(builder.Configuration["Security:HmacKeyBase64"]!));
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddMemoryCache();

// Permission service (scoped)
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Authorization infrastructure:
// Keep IAuthorizationPolicyProvider as singleton, but DO NOT inject scoped services into it.
// The provider should only create policies; PermissionHandler (scoped) will use IPermissionService.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IClaimsTransformation, PermissionClaimsTransformer>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// MVC / Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthModule API",
        Version = "v1",
        Description = "Authentication & Authorization API"
    });

    // Include XML comments if you enabled XML doc generation in csproj
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // JWT Bearer in Swagger UI
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization", // The header name where the token should be sent
        Type = SecuritySchemeType.Http, // HTTP type security (as opposed to API key, OAuth2, etc.)
        Scheme = "bearer", // Indicates it’s a Bearer token
        BearerFormat = "JWT", // Optional, specifies the format of the token (JWT in this case)
        In = ParameterLocation.Header, // The token is sent in the request header
        Description = "Enter 'Bearer {token}' (without quotes). Example: 'Bearer eyJhbGciOi...'", // Helpful description in Swagger UI
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme, // Marks this as a reusable security scheme reference
            Id = "Bearer" // Identifier used to reference this scheme elsewhere
        }
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Swagger available in Development — change as you wish
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthModule API v1");
        c.RoutePrefix = "swagger"; // UI at /swagger
    });
}


using (var scope = app.Services.CreateScope())
{
    var permService = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await permService.InitializeAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
