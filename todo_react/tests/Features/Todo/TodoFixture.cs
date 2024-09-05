using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Data;

namespace TodoApiTests.Features.Todo
{
    public class TodoFixture : AppFixture<Program>
    {
        protected override void ConfigureApp(IWebHostBuilder b)
        {
            b.ConfigureAppConfiguration(
                c =>
                {
                    c.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            { "Aspire:Azure:Data:Tables", "tables"},
                            {"ConnectionStrings__tables", "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=xxx;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;" }
                        });
                });
        }

        protected override void ConfigureServices(IServiceCollection s)
        {
            ITodoRepository mockRepo = Mock.Of<ITodoRepository>();
            Mock.Get(mockRepo).Setup(r => r.AddTodoAsync(It.IsAny<string>(), It.IsAny<TodoEntity>())).ReturnsAsync(true);
            s.AddScoped((_) => mockRepo);
        }

        protected override Task SetupAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task TearDownAsync()
        {
            return Task.CompletedTask;
        }
    }
}
