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
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<IRepositoryInitializer, TodoRepository>();

// Add Initializer
builder.Services.AddHostedService<AppInitialize>();

var app = builder.Build();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();
