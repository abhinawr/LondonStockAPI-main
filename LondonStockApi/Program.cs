
using LondonStockApi.Data;
using LondonStockApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Access configuration

// Add services to the container.

// Configure DbContext:
// Option 1: In-Memory Database (Default for this setup)
builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseInMemoryDatabase("LondonStockApiInMemoryDb"));

// Option 2: SQL Server (Uncomment to use, ensure connection string is set and migrations run)
// var connectionString = configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<StockDbContext>(options =>
//    options.UseSqlServer(connectionString));


// Register custom services
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IStockValuationService, StockValuationService>();

// Configure JWT Authentication
var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured or is too short."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction(); // True in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Set to false
        // ValidIssuer = jwtSettings["Issuer"], // (optional: can remove or comment out)
        ValidateAudience = false, // Set to false
        // ValidAudience = jwtSettings["Audience"], // (optional: can remove or comment out)
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Or a small tolerance
    };
});

builder.Services.AddAuthorization(); // Needed for [Authorize] attribute

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "London Stock API",
        Version = "v1",
        Description = @"<b>Demo Users:</b><br>
        <ul>
            <li><b>Username:</b> broker1<br><b>Password:</b> Password123!</li>
            <li><b>Username:</b> broker2<br><b>Password:</b> SecurePassword!</li>
        </ul>
        <br>Use these credentials to obtain a JWT token."
    });
    // Add JWT Bearer Token security definition to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer", 
        BearerFormat = "JWT"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "London Stock API v1");
        c.RoutePrefix = string.Empty;
    });

    // Ensure database is created if In-Memory, or migrations applied if SQL Server
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<StockDbContext>();
            if (context.Database.IsInMemory())
            {
                context.Database.EnsureCreated(); // For In-Memory
                // SeedData.Initialize(services); // Optional
            }
            // else if (context.Database.IsSqlServer())
            // {
            //     context.Database.Migrate(); // For SQL Server
            // }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();