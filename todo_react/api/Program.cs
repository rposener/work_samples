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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "DevCors",
                          policy =>
                          {
                              policy.WithOrigins("https://localhost:5174");
                          });
    });
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}
app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { }