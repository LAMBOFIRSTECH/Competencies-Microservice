using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Competency.DataBaseContext;
using Competency.Middlewares;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Competency.Interfaces;
using Competency.Services;
using Competency.Repositories;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("1", new OpenApiInfo
    {
        Title = "Competencies Management | Api",
        Description = "An ASP.NET Core Web API for managing Employees competencies Api",
        Version = builder.Configuration["Kestrel:ApiVersion"],
        Contact = new OpenApiContact
        {
            Name = "Artur Lambo",
            Email = "lamboartur94@gmail.com"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false);
var item = builder.Configuration.GetSection("ConnectionStrings");
var conStrings = item["DefaultConnection"];
builder.Services.AddDbContext<CompetenciesMigrationContext>(opt => opt.UseInMemoryDatabase(conStrings!));
builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

ThreadPool.SetMinThreads(100, 100);
var kestrelSectionCertificate = builder.Configuration.GetSection("Kestrel:EndPoints:Https:Certificate");
var certificateFile = kestrelSectionCertificate["File"];
var certificatePassword = kestrelSectionCertificate["Password"];

builder.Services.Configure<KestrelServerOptions>(options =>
{
    if (string.IsNullOrEmpty(certificateFile) || string.IsNullOrEmpty(certificatePassword))
    {
        throw new InvalidOperationException("Certificate path or password not configured");
    }
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxRequestBodySize = 10 * 1024;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.ConfigureHttpsDefaults(opt =>
    {
        opt.ClientCertificateMode = ClientCertificateMode.NoCertificate; // Required Certificate dans les autres services c'est du allowCertificate
    });
});
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("api-competencies"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddJaegerExporter(jaegerOptions =>
            {
                jaegerOptions.AgentHost = builder.Configuration["Jaeger:IpAddress"];
                jaegerOptions.AgentPort = Int16.Parse(builder.Configuration["Jaeger:Port"]!);
            });
    });
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IHashicorpVaultService, HashicorpVaultService>();
builder.Services.AddScoped<ICompetenceService, CompetenceService>();
builder.Services.AddScoped<ICompetenceEmployeService, CompetenceEmployeService>();

builder.Services.AddScoped<CompetenceRepository>();
builder.Services.AddScoped<CompetenceEmployeRepository>();
builder.Services.AddScoped<CompetenceFormationRepository>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddLogging();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication("JwtAuthorization")
    .AddScheme<JwtBearerOptions, JwtBearerAuthenticationMiddleware>("JwtAuthorization", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
 {
     options.AddPolicy("AdminPolicy", policy =>
         policy.RequireRole("Administrateur")
               .RequireAuthenticatedUser()
               .AddAuthenticationSchemes("JwtAuthorization"));
 });
var app = builder.Build();
app.UseMiddleware<ContextPathMiddleware>("/lambo-skills-manager");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(con =>
     {
         con.SwaggerEndpoint("/lambo-skills-manager/swagger/1/swagger.yml", "Competencies Management API");

         con.RoutePrefix = string.Empty;
     });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
 {
     endpoints.MapControllers();
     endpoints.MapHealthChecks("/health");
     endpoints.MapGet("/version", async context => await context.Response.WriteAsync(app.Configuration.GetValue<string>("Kestrel:ApiVersion")!));
 });
app.Run();