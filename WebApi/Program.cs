using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Model.Entities.Sql.DataBase;
using Model.Interfaces;
using Model.Managers;
using Shared.Interfaces;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using WebApi.DependencyInjection;
using WebApi.Interfaces;
using WebApi.Security.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment _environment = builder.Environment;

var jwtKey = configuration["Security:SecretKey"];

builder.Logging
     .AddDebug();


builder.Services.AddCors(options =>
{
    var section = configuration.GetSection("Security:Cors");
    var origin = section.Get<string[]>();

    options.AddPolicy("CorsPolicy",
        policy =>
        {
            if (origin != null)
            {
                policy.WithOrigins(origin.Distinct().ToArray());

            }
            else
            {
                policy.AllowAnyOrigin();
            }
            policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

var connection_string = configuration.GetConnectionString("SqlServerConnectionString");

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(connection_string);
    if (!_environment.IsProduction())
    {
        options.EnableSensitiveDataLogging();
        builder.Logging
        .AddConsole();
    }

});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtService, JwtService>((provider) => new JwtService(provider.GetService<IConfiguration>()!));
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<ISupervisorManager, SupervisorManager>();


var secretKey = configuration["Security:SecretKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateIssuerSigningKey = true,
                                ValidateLifetime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                                ClockSkew = TimeSpan.Zero,
                                RequireExpirationTime = true,
                            };
                        });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;

    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
