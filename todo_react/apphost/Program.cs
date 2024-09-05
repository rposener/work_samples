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

builder.AddProject<Projects.TodoApi>("todoapi")
    .WithExternalHttpEndpoints()
    .WithReference(table);

builder.Build().Run();
