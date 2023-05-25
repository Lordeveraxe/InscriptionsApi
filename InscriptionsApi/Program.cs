using InscriptionsApi.Controllers;
using InscriptionsApi.Models;
using InscriptionsApiLocal.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlServer<InscriptionsUniversityContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
var jwtOptions = new JwtData
{
    Key = Environment.GetEnvironmentVariable("Jwt_Key"),
    Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
    Audience = Environment.GetEnvironmentVariable("Jwt_Audience"),
    Subject = Environment.GetEnvironmentVariable("Jwt_Subject")
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(optionss =>
{
    optionss.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes((jwtOptions.Key)))

    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Inserte Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
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

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddControllers();



var connectionString = "mi-redis-cache.redis.cache.windows.net:6380,password=qWSX4Yv1FMBJLgj0LDk4HwlIXQAwcnpLIAzCaMw6aVc=,ssl=True,abortConnect=False";
var configurationOptions = ConfigurationOptions.Parse(connectionString);

// Crea el objeto ConnectionMultiplexer para conectarse a Redis
var redisConnection = ConnectionMultiplexer.Connect(configurationOptions);

// Agrega el objeto ConnectionMultiplexer al servicio de la aplicaci�n
builder.Services.AddSingleton(redisConnection);



var app = builder.Build();
app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("x-my-custom-header", "tamanio", "tamanio-subjects", "tamanio-inscriptions"));

app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<JwtAuthorizeAttribute>();


app.UseAuthorization();

app.MapControllers();

app.Run();
