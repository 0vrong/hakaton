using Microsoft.EntityFrameworkCore;
using WebService.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Schedule API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "¬ведите API Key в заголовок X-API-Key",
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
         options.JsonSerializerOptions.WriteIndented = true; // ƒл€ удобного чтени€ JSON
     });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Schedule.db"));


builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedule API v1"));
app.UseRouting();
app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/Auth", StringComparison.OrdinalIgnoreCase) ||
        context.Request.Path.Value.EndsWith("/api/auth/register", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Skipping API Key check for Auth endpoint");
        await next(context);
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKeyValues))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key is missing");
        return;
    }

    var apiKey = apiKeyValues.ToString();
    if (string.IsNullOrEmpty(apiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key is empty");
        return;
    }

    using var scope = context.RequestServices.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.ApiKey == apiKey);

    if (user == null)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid API Key");
        return;
    }

    var claims = new[]
    {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, "ApiKey");
    context.User = new System.Security.Claims.ClaimsPrincipal(identity);

    await next(context);
});

app.UseAuthorization();
app.MapControllers();

app.Run();