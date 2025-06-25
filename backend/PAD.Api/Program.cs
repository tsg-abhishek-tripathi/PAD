using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PAD.Infrastructure;
using PAD.Infrastructure.Data;
using PAD.Core.Interfaces;
using PAD.Infrastructure.Repositories;
using PAD.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework
builder.Services.AddDbContext<PadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemAdmin", policy => 
        policy.RequireClaim("extension_UserRole", "SystemAdmin"));
    options.AddPolicy("OfficeStaffing", policy => 
        policy.RequireClaim("extension_UserRole", "OfficeStaffing", "SystemAdmin"));
    options.AddPolicy("PPKGlobalLead", policy => 
        policy.RequireClaim("extension_UserRole", "PPKGlobalLead", "SystemAdmin"));
    options.AddPolicy("PPKRegionalLead", policy => 
        policy.RequireClaim("extension_UserRole", "PPKRegionalLead", "SystemAdmin"));
    options.AddPolicy("PPKProgramTeam", policy => 
        policy.RequireClaim("extension_UserRole", "PPKProgramTeam", "SystemAdmin"));
    options.AddPolicy("StaffingSystemSupport", policy => 
        policy.RequireClaim("extension_UserRole", "StaffingSystemSupport", "SystemAdmin"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add custom services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAffiliationRepository, AffiliationRepository>();
builder.Services.AddScoped<IAffiliationService, AffiliationService>();
builder.Services.AddScoped<ITaxonomyRepository, TaxonomyRepository>();
builder.Services.AddScoped<ITaxonomyService, TaxonomyService>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();
builder.Services.AddScoped<IOfficeService, OfficeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "PAD 2.0 API", 
        Version = "v1",
        Description = @"Practice Area Affiliation Database API provides endpoints for managing employee affiliations, roles, and office assignments.

Key Features:
- Employee Management
- Practice Area Affiliations
- Office Management
- Role-based Access Control
- Audit Logging",
        Contact = new OpenApiContact
        {
            Name = "PAD Development Team",
            Email = "pad-support@bain.com"
        },
        License = new OpenApiLicense
        {
            Name = "Internal Use Only",
            Url = new Uri("https://www.bain.com")
        }
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if they exist
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add response types that are common across controllers
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["AzureAd:Instance"] + builder.Configuration["AzureAd:TenantId"] + "/oauth2/v2.0/authorize"),
                TokenUrl = new Uri(builder.Configuration["AzureAd:Instance"] + builder.Configuration["AzureAd:TenantId"] + "/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api://pad-api/access_as_user", "Access PAD API" }
                }
            }
        }
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PadDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAD 2.0 API v1");
    c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger instead of root
    c.DocumentTitle = "PAD 2.0 API Documentation";
    c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    c.DisplayRequestDuration(); // Show request duration
    c.EnableDeepLinking(); // Enable deep linking for requests
    c.EnableFilter(); // Enable header filtering
    c.EnableTryItOutByDefault(); // Enable try it out by default
});

app.UseHttpsRedirection();

app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoints
app.MapHealthChecks("/health");

// Add a default root endpoint that redirects to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();

// Global Exception Handling Middleware
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request.",
                details = exception.Message,
                timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
} 