using FastEndpoints;
using FastEndpoints.Swagger;
using System.Runtime.CompilerServices;
using TodoApi;
using TodoApi.Data;


[assembly: InternalsVisibleTo("TodoApiTests")]

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureTableClient("tables");
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

// Add Services
builder.Services.AddScoped<IIdentityManager, IdentityManager>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<IRepositoryInitializer, TodoRepository>();
builder.Services.AddScoped<IRepositoryInitializer, IdentityManager>();

// Add Initializer
builder.Services.AddHostedService<AppInitialize>();

var app = builder.Build();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { }