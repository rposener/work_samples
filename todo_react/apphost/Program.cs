using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");
if (builder.Environment.IsDevelopment())
{
    // Run latest emulator in Development with a data volume
    storage.RunAsEmulator(c =>
    {
        c.WithImageTag("latest");
        c.WithDataVolume();
        // Use Azurite Default Ports to allow browsing
        c.WithTablePort(10002);
    });
}
var table = storage.AddTables("tables");

var todoapi = builder.AddProject<Projects.TodoApi>("todoapi")
    .WithExternalHttpEndpoints()
    .WithReference(table);

builder.AddNpmApp("webapp", "../reactapp", "dev")
    .WithReference(todoapi)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEndpoint(5174,5173,"https", "frontend","PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
