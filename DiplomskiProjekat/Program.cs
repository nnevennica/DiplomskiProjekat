using DiplomskiProjekat.Services.Auth;
using DiplomskiProjekat.Services.Meteo;
using DiplomskiProjekat.Services.Pollution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DiplomskiProjekat.Services.Alerts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DiplomskiProjekat API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Unesi: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials()
    );
});

builder.Services.AddSingleton<IUserStore, JsonUserStore>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddSingleton<MeteoDataService>();
builder.Services.AddSingleton<IMeteoStore, InMemoryMeteoStore>();
builder.Services.AddHostedService<MeteoSimulationHostedService>();

builder.Services.AddSingleton<PollutionDataService>();
builder.Services.AddSingleton<IPollutionStore, InMemoryPollutionStore>();
builder.Services.AddHostedService<PollutionSimulationHostedService>();

builder.Services.AddSingleton<AlertsService>();


var jwtKey = builder.Configuration["Jwt:Key"]!;
var issuer = builder.Configuration["Jwt:Issuer"]!;
var audience = builder.Configuration["Jwt:Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

var app = builder.Build();

var meteoCities = new (string City, string File)[]
{
    ("Beograd", "BeogradMeteo.csv"),
    ("NoviSad", "NoviSadMeteo.csv"),
    ("Nis", "NisMeteo.csv"),
    ("Subotica", "SuboticaMeteo.csv"),
    ("Uzice", "UziceMeteo.csv"),
};

var pollutionCities = new (string City, string File)[]
{
    ("Beograd", "BeogradPollution.csv"),
    ("NoviSad", "NoviSadPollution.csv"),
    ("Nis", "NisPollution.csv"),
    ("Subotica", "SuboticaPollution.csv"),
    ("Uzice", "UzicePollution.csv"),
};

{
    var store = app.Services.GetRequiredService<IMeteoStore>();
    var meteoData = app.Services.GetRequiredService<MeteoDataService>();

    foreach (var (city, file) in meteoCities)
    {
        var csvPath = Path.Combine(app.Environment.ContentRootPath, "Data", file);

        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"[WARN] Meteo CSV ne postoji: {csvPath}");
            continue;
        }

        var series = meteoData.LoadHourlyAverages(csvPath, day: 1);
        store.Initialize(city, series);

        Console.WriteLine($"[OK] Meteo init: {city} ({series.Count} tačaka)");
    }
}

{
    var store = app.Services.GetRequiredService<IPollutionStore>();
    var pollutionData = app.Services.GetRequiredService<PollutionDataService>();

    foreach (var (city, file) in pollutionCities)
    {
        var csvPath = Path.Combine(app.Environment.ContentRootPath, "Data", file);

        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"[WARN] Pollution CSV ne postoji: {csvPath}");
            continue;
        }

        var series = pollutionData.LoadHourlyAverages(csvPath, day: 1);
        store.Initialize(city, series);

        Console.WriteLine($"[OK] Pollution init: {city} ({series.Count} tačaka)");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
